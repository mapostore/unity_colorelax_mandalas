package com.google.unity.ads;

public interface UnityAdListener {
    void onAdLoaded();
    void onAdFailedToLoad(String errorReason);
    void onAdOpened();
    void onAdClosed();
    void onAdLeftApplication();
}
