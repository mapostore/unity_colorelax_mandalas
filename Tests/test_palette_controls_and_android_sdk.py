"""Repository invariants for palette controls and Android SDK configuration."""

from pathlib import Path
import re
import struct
import unittest


ROOT = Path(__file__).resolve().parents[1]
COLORING = ROOT / "Assets/Scripts/Coloring/Coloring.cs"
ANDROID_HELPER = ROOT / "Assets/Editor/Unity6AndroidBuildStability.cs"
GRADLE_PROPERTIES = ROOT / "Assets/Plugins/Android/gradleTemplate.properties"
ADMOB_GRADLE = (
    ROOT
    / "Assets/Plugins/Android/GoogleMobileAdsDependency.androidlib/build.gradle"
)
PROJECT_SETTINGS = ROOT / "ProjectSettings/ProjectSettings.asset"


def read_android_sdk_levels(settings_path: Path) -> tuple[int, int]:
    """Read Android SDK levels through Unity's embedded serialization tree."""
    blob = settings_path.read_bytes()
    if struct.unpack_from(">I", blob, 8)[0] != 22:
        raise AssertionError("Unsupported Unity serialized-file version")

    node_count, string_size = struct.unpack_from("<ii", blob, 91)
    nodes_offset = 99
    strings_offset = nodes_offset + node_count * 32
    strings = blob[strings_offset : strings_offset + string_size]

    def decode_name(offset: int) -> str:
        if offset & 0x80000000:
            return f"#{offset & 0x7FFFFFFF}"
        return strings[offset : strings.index(0, offset)].decode()

    nodes = []
    for index in range(node_count):
        version, level, flags, type_name, name, size, child_index, meta, reference = (
            struct.unpack_from("<hBBIIiiiQ", blob, nodes_offset + index * 32)
        )
        nodes.append(
            {
                "level": level,
                "flags": flags,
                "name": decode_name(name),
                "size": size,
                "meta": meta,
            }
        )

    subtree_end = []
    for index, node in enumerate(nodes):
        end = index + 1
        while end < node_count and nodes[end]["level"] > node["level"]:
            end += 1
        subtree_end.append(end)

    position = struct.unpack_from(">Q", blob, 32)[0]
    values = {}

    def parse(index: int) -> None:
        nonlocal position
        node = nodes[index]
        start = position

        if node["flags"] & 1:
            count = struct.unpack_from("<i", blob, position)[0]
            position += 4
            if count < 0 or count > 100000:
                raise AssertionError(f"Unexpected serialized array count: {count}")
            for _ in range(count):
                parse(index + 2)
        elif subtree_end[index] > index + 1:
            child = index + 1
            while child < subtree_end[index]:
                parse(child)
                child = subtree_end[child]
        else:
            if node["size"] < 0:
                raise AssertionError(f"Unsupported serialized leaf: {node['name']}")
            position += node["size"]

        if node["name"] in ("AndroidMinSdkVersion", "AndroidTargetSdkVersion"):
            values[node["name"]] = struct.unpack_from("<i", blob, start)[0]

        if node["meta"] & 0x4000:
            position = (position + 3) & ~3

    parse(0)
    if position != len(blob):
        raise AssertionError("Unity PlayerSettings parser did not consume the file")

    return values["AndroidMinSdkVersion"], values["AndroidTargetSdkVersion"]


class PaletteControlsTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls) -> None:
        cls.source = COLORING.read_text(encoding="utf-8-sig")

    def test_carousel_reserves_four_visible_palette_slots(self) -> None:
        self.assertRegex(
            self.source,
            r"const int FeaturedVisiblePaletteCount\s*=\s*4\s*;",
        )
        self.assertEqual(
            3,
            self.source.count("featuredToggleSize * FeaturedVisiblePaletteCount"),
        )
        self.assertEqual(
            6,
            self.source.count("(FeaturedVisiblePaletteCount - 1)"),
        )

    def test_carousel_uses_larger_filled_directional_arrows(self) -> None:
        self.assertRegex(
            self.source,
            r"const float FeaturedArrowSizePixels\s*=\s*51f\s*;",
        )
        self.assertIn(
            "Texture2D CreateFilledTriangleTexture(bool pointLeft)",
            self.source,
        )
        self.assertIn("featuredLeftArrowTexture", self.source)
        self.assertIn("featuredRightArrowTexture", self.source)

        draw_method = re.search(
            r"void DrawFeaturedThemeScrollArrow\(.*?\n    }\n\n",
            self.source,
            re.DOTALL,
        )
        self.assertIsNotNone(draw_method)
        self.assertNotIn("backChevronIcon", draw_method.group(0))


class GradientControlsTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls) -> None:
        cls.source = COLORING.read_text(encoding="utf-8-sig")

    def test_gradient_controls_have_more_spacing_and_a_larger_slider(self) -> None:
        self.assertRegex(
            self.source,
            r"const float GradientControlGapPixels\s*=\s*28f\s*;",
        )
        self.assertRegex(
            self.source,
            r"const float GradientBoostSliderWidthPixels\s*=\s*220f\s*;",
        )
        self.assertRegex(
            self.source,
            r"const float GradientBoostSliderHeightPixels\s*=\s*44f\s*;",
        )
        self.assertIn("GradientControlGapPixels * scale_x", self.source)
        self.assertIn("GradientBoostSliderWidthPixels * scale_x", self.source)
        self.assertIn("GradientBoostSliderHeightPixels * scale_y", self.source)


class AndroidSdkTests(unittest.TestCase):
    def test_android_sdk_pins_agree_on_min_24_and_target_36(self) -> None:
        helper = ANDROID_HELPER.read_text(encoding="utf-8-sig")
        properties = GRADLE_PROPERTIES.read_text(encoding="utf-8-sig")
        admob_gradle = ADMOB_GRADLE.read_text(encoding="utf-8-sig")

        self.assertIn(
            'SetAndroidSdkLevelProperty("targetSdkVersion", "AndroidApiLevel36")',
            helper,
        )
        self.assertIn(
            'SetAndroidSdkLevelProperty("minSdkVersion", "AndroidApiLevel24")',
            helper,
        )
        self.assertIn(
            'Upsert(ref lines, "android.suppressUnsupportedCompileSdk", "36")',
            helper,
        )
        self.assertIn("android.suppressUnsupportedCompileSdk=36", properties)
        self.assertRegex(admob_gradle, r"(?m)^\s*compileSdk 36\s*$")
        self.assertRegex(admob_gradle, r"(?m)^\s*minSdk 24\s*$")
        self.assertRegex(admob_gradle, r"(?m)^\s*targetSdk 36\s*$")

    def test_active_unity_player_settings_pin_android_24_and_36(self) -> None:
        android_min_sdk, android_target_sdk = read_android_sdk_levels(
            PROJECT_SETTINGS
        )

        self.assertEqual(24, android_min_sdk)
        self.assertEqual(36, android_target_sdk)


if __name__ == "__main__":
    unittest.main()
