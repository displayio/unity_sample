using UnityEngine;

public class InitAppLovin : MonoBehaviour
{
    public void InitAppLovinSdk()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            Debug.Log("AppLovin SDK is initialized");

            if (Application.platform == RuntimePlatform.Android)
            {
                ShowToast("AppLovin SDK is initialized");
            }
        };

        // MaxSdk.SetSdkKey("Em8gDrZPoITWOz7bpmbygQ4zNdahVTBLK2bKsKzA7ZTgDproADG9WTuqRiMLeof9RTkTqzcTdXR55xGHNuMRDp"); //deprecated
        MaxSdk.InitializeSdk();
    }

    void ShowToast(string message)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>("makeText", context, message, toastClass.GetStatic<int>("LENGTH_LONG"));
                toast.Call("show");
            }));
        }
    }
}

