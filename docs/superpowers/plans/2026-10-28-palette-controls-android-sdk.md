# Palette Controls and Android SDK Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Improve palette and gradient control usability and pin Android builds to min SDK 24 and target SDK 36.

**Architecture:** Keep the existing immediate-mode Unity GUI and responsive layout branches, but centralize the new dimensions as constants so all branches stay consistent. Generate and cache orientation-specific filled triangle textures in `Coloring`, and verify the UI invariants and Android pins with a lightweight repository regression test before compiling in Unity batch mode.

**Tech Stack:** Unity 6.2, C#, Unity IMGUI, Android Gradle templates, Python `unittest`.

**Spec:** `docs/superpowers/specs/2026-10-28-palette-controls-android-sdk-design.md`

## Global Constraints

- Display exactly four central circular palette choices at once.
- Use cached runtime-generated filled left/right triangles, independent of fonts and `backChevronIcon`.
- Make arrow visuals approximately 1.5 times the existing 34-pixel size while preserving responsive scaling and hit behavior.
- Increase gradient-control separation and make the boost slider longer, taller, and easier to touch without changing its 0–1 semantics.
- Set Android min SDK to 24 and target SDK to 36 everywhere this repository pins them.
- Do not modify `UserSettings/Layouts/default-6000.dwlt` or overwrite unrelated content in `ProjectSettings/ProjectSettings.asset`.

## Review Focus

- Narrow screens: the four-slot viewport must remain non-negative and must not overlap either arrow.
- Both layout paths: every responsive branch must use the same four-visible-palette calculation.
- Missing generated texture: arrow rendering must safely regenerate the cached texture and keep a text fallback.
- Repeated GUI draws: triangle textures must be cached rather than allocated every frame.
- Android configuration drift: helper code, Gradle compatibility property, and dependency library manifest must agree on SDK 24/36.

---

### Task 1: Palette carousel arrows and four-item viewport

**Files:**
- Modify: `Assets/Scripts/Coloring/Coloring.cs:40-80,950-1100,1280-1330,4633-4675`
- Create: `Tests/test_palette_controls_and_android_sdk.py`

**Interfaces:**
- Consumes: Existing `DrawFeaturedThemeScrollArrow(Rect rect, bool pointLeft, bool isPressed)` call sites and responsive layout state.
- Produces: `CreateFilledTriangleTexture(bool pointLeft) -> Texture2D`, cached left/right textures, and shared carousel layout constants.

- [ ] **Step 1: Write failing carousel regression tests**

Add Python `unittest` assertions that `Coloring.cs` declares a visible-count constant of `4`, uses it in all three viewport-width/slot-size calculations, uses an approximately 51-pixel arrow dimension, and routes arrow drawing through cached generated left/right triangle textures rather than `backChevronIcon`.

- [ ] **Step 2: Run the focused test and verify it fails**

Run: `python3 -m unittest Tests.test_palette_controls_and_android_sdk.PaletteControlsTests -v`

Expected: FAIL because the layout still hard-codes six items and uses the downward chevron asset.

- [ ] **Step 3: Add shared carousel constants and update every responsive layout branch**

In `Coloring.cs`, add constants for `FeaturedVisiblePaletteCount = 4` and `FeaturedArrowSizePixels = 51f`; replace each six-item desired-width and clamped-slot formula with expressions derived from the visible-count constant, including `count - 1` gaps.

- [ ] **Step 4: Add cached runtime filled-triangle generation**

Add `CreateFilledTriangleTexture(bool pointLeft) -> Texture2D` using a small transparent RGBA texture with an opaque white filled triangle, cache one texture per direction, set clamp filtering/wrapping, and update `DrawFeaturedThemeScrollArrow` to draw the matching texture centered in the existing hit rectangle. Retain the existing `<`/`>` label only as a safe fallback if generation fails.

- [ ] **Step 5: Run the focused carousel tests**

Run: `python3 -m unittest Tests.test_palette_controls_and_android_sdk.PaletteControlsTests -v`

Expected: PASS.

- [ ] **Step 6: Commit the carousel deliverable**

```bash
git add Assets/Scripts/Coloring/Coloring.cs Tests/test_palette_controls_and_android_sdk.py
git commit -m "fix: improve palette carousel controls"
```

### Task 2: Gradient-control spacing and slider touch target

**Files:**
- Modify: `Assets/Scripts/Coloring/Coloring.cs:1380-1410`
- Modify: `Tests/test_palette_controls_and_android_sdk.py`

**Interfaces:**
- Consumes: Existing `gradientDirectionRect`, rotation controls, slider, label, reset control, and scale factors.
- Produces: Shared gradient layout constants used by `UpdateGUIElementsRects`.

- [ ] **Step 1: Extend the regression test with gradient layout assertions**

Assert constants pin the control gap to `28f`, boost slider width to `220f`, and boost slider height to `44f`, and assert the slider rectangle uses those constants with `scale_x`/`scale_y`.

- [ ] **Step 2: Run the focused gradient test and verify it fails**

Run: `python3 -m unittest Tests.test_palette_controls_and_android_sdk.GradientControlsTests -v`

Expected: FAIL against the current 20-pixel gap and 152-by-34 slider.

- [ ] **Step 3: Implement the gradient dimensions**

Add `GradientControlGapPixels = 28f`, `GradientBoostSliderWidthPixels = 220f`, and `GradientBoostSliderHeightPixels = 44f` to `Coloring.cs`; use them in the layout while leaving value calculation and input handling unchanged.

- [ ] **Step 4: Run the focused gradient test**

Run: `python3 -m unittest Tests.test_palette_controls_and_android_sdk.GradientControlsTests -v`

Expected: PASS.

- [ ] **Step 5: Commit the gradient deliverable**

```bash
git add Assets/Scripts/Coloring/Coloring.cs Tests/test_palette_controls_and_android_sdk.py
git commit -m "fix: enlarge gradient controls"
```

### Task 3: Android min SDK 24 and target SDK 36

**Files:**
- Modify: `Assets/Editor/Unity6AndroidBuildStability.cs:28-36,95-106`
- Modify: `Assets/Plugins/Android/gradleTemplate.properties:6`
- Modify: `Assets/Plugins/Android/GoogleMobileAdsDependency.androidlib/build.gradle:9-13`
- Modify: `Tests/test_palette_controls_and_android_sdk.py`
- Preserve: `ProjectSettings/ProjectSettings.asset`

**Interfaces:**
- Consumes: Unity `PlayerSettings.Android` reflection helper and Gradle template pins.
- Produces: Consistent min SDK 24, compile/target SDK 36 configuration.

- [ ] **Step 1: Extend the regression test with Android SDK assertions**

Assert the Unity helper uses `AndroidApiLevel24` and `AndroidApiLevel36`, both Gradle compatibility properties equal `36`, and the Google Mobile Ads dependency library uses `compileSdk 36`, `minSdk 24`, and `targetSdk 36`.

- [ ] **Step 2: Run the focused Android test and verify it fails**

Run: `python3 -m unittest Tests.test_palette_controls_and_android_sdk.AndroidSdkTests -v`

Expected: FAIL because the helper/property still pin 35 and the dependency library minimum is 26.

- [ ] **Step 3: Update repository SDK pins**

Change the stability helper target enum to `AndroidApiLevel36`, update its comment and Gradle normalization value to `36`, set `android.suppressUnsupportedCompileSdk=36`, and lower the dependency library `minSdk` from 26 to 24. Do not directly rewrite the already-modified binary `ProjectSettings/ProjectSettings.asset`.

- [ ] **Step 4: Run the focused Android test**

Run: `python3 -m unittest Tests.test_palette_controls_and_android_sdk.AndroidSdkTests -v`

Expected: PASS.

- [ ] **Step 5: Commit the Android configuration deliverable**

```bash
git add Assets/Editor/Unity6AndroidBuildStability.cs Assets/Plugins/Android/gradleTemplate.properties Assets/Plugins/Android/GoogleMobileAdsDependency.androidlib/build.gradle Tests/test_palette_controls_and_android_sdk.py
git commit -m "build: target Android SDK 36"
```

### Task 4: Integrated verification

**Files:**
- Verify: all files modified by Tasks 1–3
- Preserve: `ProjectSettings/ProjectSettings.asset`, `UserSettings/Layouts/default-6000.dwlt`

**Interfaces:**
- Consumes: Completed UI and Android configuration changes.
- Produces: Evidence that static regressions pass and Unity compiles the project.

- [ ] **Step 1: Run the complete repository regression suite**

Run: `python3 -m unittest discover -s Tests -p 'test_*.py' -v`

Expected: PASS for the existing ads checks and the new palette/SDK checks.

- [ ] **Step 2: Run Unity batch compilation**

Run the installed Unity 6000.2.4f1 editor in batch mode with `-projectPath`, `-quit`, and a temporary log path outside the repository.

Expected: exit code 0 and no C# compiler errors in the log.

- [ ] **Step 3: Inspect the final diff and working tree**

Run: `git diff --check && git status --short && git diff -- Assets/Scripts/Coloring/Coloring.cs Assets/Editor/Unity6AndroidBuildStability.cs Assets/Plugins/Android/gradleTemplate.properties Assets/Plugins/Android/GoogleMobileAdsDependency.androidlib/build.gradle Tests/test_palette_controls_and_android_sdk.py`

Expected: no whitespace errors; only intended task files plus the pre-existing user changes and planning docs are present.

- [ ] **Step 4: Commit planning documents if they remain uncommitted**

```bash
git add docs/superpowers/specs/2026-10-28-palette-controls-android-sdk-design.md docs/superpowers/plans/2026-10-28-palette-controls-android-sdk.md
git commit -m "docs: record palette controls implementation plan"
```
