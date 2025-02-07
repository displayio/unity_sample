using UnityEngine;

public class DIOInterstitialManager : MonoBehaviour
{
    private string placementId = "5133"; 

    public void LoadAd()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    AndroidJavaObject controller = new AndroidJavaClass("com.brandio.ads.Controller")
                                                    .CallStatic<AndroidJavaObject>("getInstance");

                    AndroidJavaObject placement;
                    try
                    {
                        placement = controller.Call<AndroidJavaObject>("getPlacement", placementId);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("Failed to get placement: " + e.Message);
                        DIOInitManager.ShowToast(activity, "Failed to get placement");
                        return;
                    }

                    AndroidJavaObject adRequest = placement.Call<AndroidJavaObject>("newAdRequest");

                    AndroidJavaProxy adRequestListener = new AdRequestListenerProxy(activity);
                    adRequest.Call("setAdRequestListener", adRequestListener);
                    adRequest.Call("requestAd");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Ad Request Failed: " + e.Message);
            }
        }
        else
        {
            Debug.Log("Ad request ignored: Not running on Android");
        }
    }

    private class AdRequestListenerProxy : AndroidJavaProxy
    {
        private AndroidJavaObject activity;

        public AdRequestListenerProxy(AndroidJavaObject activity) 
            : base("com.brandio.ads.listeners.AdRequestListener")
        {
            this.activity = activity;
        }

        public void onAdReceived(AndroidJavaObject ad)
        {
            Debug.Log("Ad received");

            // Set event listener for the ad
            AndroidJavaProxy adEventListener = new AdEventListenerProxy();
            ad.Call("setEventListener", adEventListener);

            // Show the ad
            ad.Call("showAd", activity);
        }

        public void onNoAds(AndroidJavaObject error)
        {
            Debug.LogError("No ads available for placement");
            DIOInitManager.ShowToast(activity, "No ads available for placement");
        }

        public void onFailedToLoad(AndroidJavaObject dioError)
        {
            string errorMsg = dioError.Call<string>("getMessage");
            if (string.IsNullOrEmpty(errorMsg))
                errorMsg = "Failed to load ad";

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
            Debug.Log("Ad Shown");
        }

        public void onFailedToShow(AndroidJavaObject ad)
        {
            Debug.LogError("Ad Failed to Show");
        }

        public void onClicked(AndroidJavaObject ad)
        {
            Debug.Log("Ad Clicked");
        }

        public void onClosed(AndroidJavaObject ad)
        {
            Debug.Log("Ad Closed");
        }

        public void onAdCompleted(AndroidJavaObject ad)
        {
            Debug.Log("Ad Completed");
        }
    }
}
