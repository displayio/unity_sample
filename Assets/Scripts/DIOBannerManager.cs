using UnityEngine;

public class DIOBannerManager : MonoBehaviour
{
    private string placementId = "6349"; 
    private AndroidJavaObject bannerView;

   public void LoadBanner()
{
    if (Application.platform == RuntimePlatform.Android)
    {
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                RemoveBanner();
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject controller = new AndroidJavaClass("com.brandio.ads.Controller")
                                                .CallStatic<AndroidJavaObject>("getInstance");

                // Check if DIO SDK is initialized
                bool isInitialized = controller.Call<bool>("isInitialized");
                Debug.Log("DIO SDK Initialized: " + isInitialized);

                if (!isInitialized)
                {
                    Debug.LogError("DIO SDK is NOT initialized! Cannot load banner.");
                    DIOInitManager.ShowToast(activity, "DIO SDK is not initialized!");
                    return;
                }

                //  Proceed with getting placement
                AndroidJavaObject placement = controller.Call<AndroidJavaObject>("getPlacement", placementId);
                if (placement == null)
                {
                    Debug.LogError("DIO Banner Placement is NULL. Check placement ID.");
                    DIOInitManager.ShowToast(activity, "Failed to get banner placement!");
                    return;
                }

                //  Create Ad Request
                AndroidJavaObject adRequest = placement.Call<AndroidJavaObject>("newAdRequest");

                //  Set Ad Request Listener
                AndroidJavaProxy adRequestListener = new AdRequestListenerProxy(this, activity, placement);
                adRequest.Call("setAdRequestListener", adRequestListener);

                //  Request the Ad
                adRequest.Call("requestAd");

                Debug.Log("DIO Banner Ad Request Sent");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Banner Request Failed: " + e.Message);
        }
    }
    else
    {
        Debug.Log("Banner request ignored: Not running on Android");
    }
}

public void RemoveBanner()
{
    if (Application.platform == RuntimePlatform.Android && bannerView != null)
    {
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    try
                    {
                        AndroidJavaObject decorView = activity.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView");
                        AndroidJavaObject frameLayout = decorView.Call<AndroidJavaObject>("findViewById", AndroidResourceId("content"));

                        frameLayout.Call("removeView", bannerView);
                        bannerView.Dispose();
                        bannerView = null;
                        
                        Debug.Log("DIO Banner View Removed");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("Failed to remove banner view: " + e.Message);
                    }
                }));
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in RemoveBanner: " + e.Message);
        }
    }
    else
    {
        Debug.Log("No banner to remove or not running on Android");
    }
}

    private class AdRequestListenerProxy : AndroidJavaProxy
    {
        private DIOBannerManager bannerManager;
        private AndroidJavaObject activity;
        private AndroidJavaObject placement;

        public AdRequestListenerProxy(DIOBannerManager bannerManager, AndroidJavaObject activity, AndroidJavaObject placement)
            : base("com.brandio.ads.listeners.AdRequestListener")
        {
            this.bannerManager = bannerManager;
            this.activity = activity;
            this.placement = placement;
        }

      public void onAdReceived(AndroidJavaObject ad)
{
    Debug.Log("DIO Banner Ad Received");

    // Set event listener for tracking impressions and clicks
    AndroidJavaProxy adEventListener = new AdEventListenerProxy();
    ad.Call("setEventListener", adEventListener);

    activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
    {
        try
        {
            //  Use FrameLayout as container for the banner
            bannerManager.bannerView = new AndroidJavaObject("android.widget.FrameLayout", activity);

            int width = ConvertDpToPx(activity, 320);
            int height = ConvertDpToPx(activity, 50);

            // Set Layout Parameters
            AndroidJavaClass gravityClass = new AndroidJavaClass("android.view.Gravity");
            int gravityCenterBottom = gravityClass.GetStatic<int>("BOTTOM") | gravityClass.GetStatic<int>("CENTER_HORIZONTAL");
            AndroidJavaObject layoutParams = new AndroidJavaObject("android.widget.FrameLayout$LayoutParams", width, height, gravityCenterBottom);

            AndroidJavaObject decorView = activity.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView");
            AndroidJavaObject frameLayout = decorView.Call<AndroidJavaObject>("findViewById", AndroidResourceId("content"));

            frameLayout.Call("addView", bannerManager.bannerView, layoutParams);
            Debug.Log("DIO Banner View Added");

            AndroidJavaObject bannerContainer = placement.Call<AndroidJavaObject>("getContainer", ad.Call<string>("getRequestId"));

            if (bannerContainer == null)
            {
                Debug.LogError("DIO Banner Container is NULL");
                return;
            }

            bannerContainer.Call("bindTo", bannerManager.bannerView);
            Debug.Log("DIO Banner Bound to FrameLayout");

        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to create and bind banner view: " + e.Message);
        }
    }));
}

        public void onNoAds(AndroidJavaObject error)
        {
            Debug.LogError("DIO No Ads Available for Banner");
            DIOInitManager.ShowToast(activity, "No ads available for banner");
        }

        public void onFailedToLoad(AndroidJavaObject dioError)
        {
            string errorMsg = dioError.Call<string>("getMessage");
            if (string.IsNullOrEmpty(errorMsg))
                errorMsg = "Failed to load banner ad";

            Debug.LogError(errorMsg);
            DIOInitManager.ShowToast(activity, errorMsg);
        }
    }

    private class AdEventListenerProxy : AndroidJavaProxy
    {
        public AdEventListenerProxy()
            : base("com.brandio.ads.listeners.AdEventListener")
        {
        }

        public void onShown(AndroidJavaObject ad)
        {
            Debug.Log("DIO Banner Ad Shown");
        }

        public void onFailedToShow(AndroidJavaObject ad)
        {
            Debug.LogError("DIO Banner Ad Failed to Show");
        }

        public void onClicked(AndroidJavaObject ad)
        {
            Debug.Log("DIO Banner Ad Clicked");
        }

        public void onClosed(AndroidJavaObject ad)
        {
            Debug.Log("DIO Banner Ad Closed");
        }

        public void onAdCompleted(AndroidJavaObject ad)
        {
            Debug.Log("DIO Banner Ad Completed");
        }
    }

    private static int ConvertDpToPx(AndroidJavaObject activity, int dp)
    {
        AndroidJavaObject resources = activity.Call<AndroidJavaObject>("getResources");
        AndroidJavaObject displayMetrics = resources.Call<AndroidJavaObject>("getDisplayMetrics");
        return Mathf.RoundToInt(dp * displayMetrics.Get<float>("density"));
    }

    private static int AndroidResourceId(string name)
    {
        using (AndroidJavaClass rClass = new AndroidJavaClass("android.R$id"))
        {
            return rClass.GetStatic<int>(name);
        }
    }
}
