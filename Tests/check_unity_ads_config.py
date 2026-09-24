#!/usr/bin/env python3
"""Regression checks for Unity Ads production configuration."""

from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
BANNER = ROOT / "Assets/Scripts/Admob/GoogleMobileAdsScript.cs"
INTERSTITIAL = ROOT / "Assets/Scripts/Others/AdManager.cs"
PRIVACY = ROOT / "Assets/Scripts/Ads/UnityAdsPrivacy.cs"


def require(condition: bool, message: str) -> None:
    if not condition:
        raise AssertionError(message)


def main() -> None:
    banner = BANNER.read_text(encoding="utf-8-sig")
    interstitial = INTERSTITIAL.read_text(encoding="utf-8-sig")

    for name, source in (("banner", banner), ("interstitial", interstitial)):
        require('private const string AndroidGameId = "1724257";' in source,
                f"{name} must use the production Android game ID")
        require('private const string IosGameId = "1724256";' in source,
                f"{name} must use the production iOS game ID")
        require("[SerializeField] private string gameId" not in source,
                f"{name} game ID must not be overridable by stale scene data")
        require("UnityAdsPrivacy.CanInitializeAds()" in source,
                f"{name} must gate initialization on resolved consent")
        require("UnityAdsPrivacy.ApplyConsentMetadata();" in source,
                f"{name} must set privacy metadata before initialization")

    require(PRIVACY.exists(), "shared Unity Ads privacy policy helper is missing")
    privacy = PRIVACY.read_text(encoding="utf-8-sig")
    require('PlayerPrefs.GetString("personalized_ok", "")' in privacy,
            "privacy helper must read the persisted consent choice")
    require('new MetaData("gdpr")' in privacy,
            "privacy helper must create Unity Ads GDPR metadata")
    require('metadata.Set("consent", consentGranted);' in privacy,
            "privacy helper must pass the resolved consent value")

    print("Unity Ads production configuration checks passed")


if __name__ == "__main__":
    main()
