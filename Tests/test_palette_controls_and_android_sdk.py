"""Repository invariants for palette controls and Android SDK configuration."""

from pathlib import Path
import re
import unittest


ROOT = Path(__file__).resolve().parents[1]
COLORING = ROOT / "Assets/Scripts/Coloring/Coloring.cs"
ANDROID_HELPER = ROOT / "Assets/Editor/Unity6AndroidBuildStability.cs"
GRADLE_PROPERTIES = ROOT / "Assets/Plugins/Android/gradleTemplate.properties"
ADMOB_GRADLE = (
    ROOT
    / "Assets/Plugins/Android/GoogleMobileAdsDependency.androidlib/build.gradle"
)


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


if __name__ == "__main__":
    unittest.main()
