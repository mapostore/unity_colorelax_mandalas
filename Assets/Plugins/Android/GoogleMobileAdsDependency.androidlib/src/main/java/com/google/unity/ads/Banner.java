package com.google.unity.ads;

import android.app.Activity;
import android.view.Gravity;
import android.view.View;
import android.widget.FrameLayout;

import com.google.android.gms.ads.AdListener;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdSize;
import com.google.android.gms.ads.AdView;
import com.google.android.gms.ads.LoadAdError;
import com.google.android.gms.ads.ResponseInfo;

public class Banner {
    private final Activity activity;
    private final UnityAdListener listener;
    private AdView adView;
    private FrameLayout.LayoutParams layoutParams;

    public Banner(Activity activity, UnityAdListener listener) {
        this.activity = activity;
        this.listener = listener;
    }

    public void create(final String adUnitId, final AdSize adSize, final int position) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                createInternal(adUnitId, adSize, createLayoutParams(position));
            }
        });
    }

    public void create(final String adUnitId, final AdSize adSize, final int x, final int y) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                FrameLayout.LayoutParams params = new FrameLayout.LayoutParams(
                        FrameLayout.LayoutParams.WRAP_CONTENT,
                        FrameLayout.LayoutParams.WRAP_CONTENT);
                params.leftMargin = x;
                params.topMargin = y;
                createInternal(adUnitId, adSize, params);
            }
        });
    }

    public void loadAd(final AdRequest request) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (adView != null) {
                    adView.loadAd(request);
                }
            }
        });
    }

    public void show() {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (adView != null) {
                    adView.setVisibility(View.VISIBLE);
                }
            }
        });
    }

    public void hide() {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (adView != null) {
                    adView.setVisibility(View.GONE);
                }
            }
        });
    }

    public void destroy() {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (adView == null) {
                    return;
                }

                FrameLayout rootView = getRootView();
                rootView.removeView(adView);
                adView.destroy();
                adView = null;
            }
        });
    }

    public float getHeightInPixels() {
        return adView == null ? 0f : adView.getHeight();
    }

    public float getWidthInPixels() {
        return adView == null ? 0f : adView.getWidth();
    }

    public void setPosition(final int position) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (adView == null) {
                    return;
                }

                layoutParams = createLayoutParams(position);
                adView.setLayoutParams(layoutParams);
            }
        });
    }

    public void setPosition(final int x, final int y) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (adView == null) {
                    return;
                }

                layoutParams = new FrameLayout.LayoutParams(
                        FrameLayout.LayoutParams.WRAP_CONTENT,
                        FrameLayout.LayoutParams.WRAP_CONTENT);
                layoutParams.leftMargin = x;
                layoutParams.topMargin = y;
                adView.setLayoutParams(layoutParams);
            }
        });
    }

    public String getMediationAdapterClassName() {
        if (adView == null || adView.getResponseInfo() == null) {
            return null;
        }

        ResponseInfo responseInfo = adView.getResponseInfo();
        return responseInfo.getMediationAdapterClassName();
    }

    private void createInternal(String adUnitId, AdSize adSize, FrameLayout.LayoutParams params) {
        destroyExistingView();

        adView = new AdView(activity);
        adView.setAdUnitId(adUnitId);
        adView.setAdSize(adSize);
        adView.setAdListener(new AdListener() {
            @Override
            public void onAdLoaded() {
                if (listener != null) {
                    listener.onAdLoaded();
                }
            }

            @Override
            public void onAdFailedToLoad(LoadAdError adError) {
                if (listener != null) {
                    listener.onAdFailedToLoad(adError == null ? "unknown" : adError.toString());
                }
            }

            @Override
            public void onAdOpened() {
                if (listener != null) {
                    listener.onAdOpened();
                }
            }

            @Override
            public void onAdClosed() {
                if (listener != null) {
                    listener.onAdClosed();
                }
            }
        });

        layoutParams = params;
        getRootView().addView(adView, layoutParams);
    }

    private void destroyExistingView() {
        if (adView == null) {
            return;
        }

        getRootView().removeView(adView);
        adView.destroy();
        adView = null;
    }

    private FrameLayout getRootView() {
        return (FrameLayout) activity.findViewById(android.R.id.content);
    }

    private FrameLayout.LayoutParams createLayoutParams(int position) {
        FrameLayout.LayoutParams params = new FrameLayout.LayoutParams(
                FrameLayout.LayoutParams.WRAP_CONTENT,
                FrameLayout.LayoutParams.WRAP_CONTENT);
        params.gravity = getGravity(position);
        return params;
    }

    private int getGravity(int position) {
        switch (position) {
            case 0:
                return Gravity.TOP | Gravity.CENTER_HORIZONTAL;
            case 1:
                return Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL;
            case 2:
                return Gravity.TOP | Gravity.LEFT;
            case 3:
                return Gravity.TOP | Gravity.RIGHT;
            case 4:
                return Gravity.BOTTOM | Gravity.LEFT;
            case 5:
                return Gravity.BOTTOM | Gravity.RIGHT;
            case 6:
                return Gravity.CENTER;
            default:
                return Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL;
        }
    }
}
