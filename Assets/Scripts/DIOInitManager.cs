using UnityEngine;

public class DIOInitManager : MonoBehaviour
{
    private const string DIO_APP_ID = "7729";

    public void InitDIO()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                    AndroidJavaClass controller = new AndroidJavaClass("com.brandio.ads.Controller");

                    // Check if the SDK is already initialized
                    bool isInitialized = controller.CallStatic<AndroidJavaObject>("getInstance")
                                                   .Call<bool>("isInitialized");

                    if (isInitialized)
                    {
                        ShowToast(activity, "DIO SDK is already Initialized");
                        Debug.Log("DIO SDK is already Initialized");
                    }
                    else
                    {
                        // Proceed with initialization
                        AndroidJavaProxy sdkInitListener = new SdkInitListenerProxy(activity);
                        controller.CallStatic<AndroidJavaObject>("getInstance")
                                  .Call("init", activity, DIO_APP_ID, sdkInitListener);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("DIO SDK Init Failed: " + e.Message);
            }
        }
        else
        {
            Debug.Log("DIO SDK not initialized: Not running on Android");
        }
    }

    public static void ShowToast(AndroidJavaObject activity, string message)
    {
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>("makeText", context, message, toastClass.GetStatic<int>("LENGTH_LONG"));
            toast.Call("show");
        }));
    }

    private class SdkInitListenerProxy : AndroidJavaProxy
    {
        private AndroidJavaObject activity;

        public SdkInitListenerProxy(AndroidJavaObject activity) : base("com.brandio.ads.listeners.SdkInitListener")
        {
            this.activity = activity;
        }

        public void onInit()
        {
            Debug.Log("DIO SDK init success");
            DIOInitManager.ShowToast(activity, "DIO SDK init success");
        }

        public void onInitError(AndroidJavaObject dioError)
        {
            string errorMsg = dioError.Call<string>("getMessage");
            if (string.IsNullOrEmpty(errorMsg))
                errorMsg = "Error initializing DIO SDK";

            Debug.LogError(errorMsg);
            DIOInitManager.ShowToast(activity, errorMsg);
        }
    }
}
