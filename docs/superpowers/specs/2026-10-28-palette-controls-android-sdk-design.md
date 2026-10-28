# Palette Controls and Android SDK Design

## Intent

Improve the coloring screen’s central palette navigation and gradient controls so they are visually clear and easier to touch, while aligning the Android release configuration with min SDK 24 and target SDK 36.

## Goals

- Replace the current downward chevrons with larger filled left/right triangles.
- Show four central circular palette choices at once instead of six.
- Reserve more horizontal space around the navigation arrows.
- Increase spacing between gradient controls.
- Make the gradient boost slider longer, taller, and easier to touch.
- Set Android min SDK to 24 and target SDK to 36 wherever this repository pins those values.

## Non-goals

- Redesign the surrounding toolbar, palette content, or gradient behavior.
- Change the number of available palettes; only the visible viewport count changes.
- Modify user-specific Unity layout files.

## Constraints

- Preserve responsive scaling and all existing palette scrolling/touch behavior.
- Keep the user’s existing changes in `ProjectSettings/ProjectSettings.asset` and avoid overwriting unrelated settings.
- Generated arrows must not depend on the custom font or the existing downward chevron asset.

## Approaches

- **Selected:** generate filled triangle textures at runtime and draw them in the existing arrow hit rectangles. This is orientation-safe and independent of font glyphs or imported icon direction.
- Rotate the existing chevron texture by ±90 degrees. Rejected because the result remains outlined and depends on the source asset.
- Draw Unicode triangle glyphs. Rejected because the custom font can substitute or misrender glyphs.

## Implementation Shape

- Add cached left/right filled-triangle textures and draw them at approximately 1.5 times the current visual arrow size, while retaining suitable hit rectangles.
- Replace the hard-coded six-visible-palette layout calculation with a four-visible-palette calculation in every responsive layout branch.
- Increase gradient-control gaps and allocate additional width and height to the boost slider without changing its 0–1 value semantics.
- Update the Unity Android stability helper and Gradle compatibility property to target SDK 36 and min SDK 24; preserve the existing Android library’s target 36 and reconcile its minimum SDK to 24.
- Add regression checks for the UI constants/layout invariants and Android SDK pins, then run Unity batch compilation.
