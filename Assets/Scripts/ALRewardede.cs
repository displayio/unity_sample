using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ALRewardede : MonoBehaviour
{
#if UNITY_IOS
    string adUnitId = "140a474097abb735";
#else
    string adUnitId = "f31c504d046cfc08";
#endif

    public Button rvButton;
    private TMP_Text buttonText;

    private bool isSubscribed = false;

    void Start()
    {
        MaxSdk.ShowMediationDebugger();

        if (rvButton == null)
        {
            rvButton = GameObject.Find("ALRewardedButton")?.GetComponent<Button>();
        }

        if (rvButton != null)
        {
            buttonText = rvButton.GetComponentInChildren<TMP_Text>();
        }
    }

    public void InitializRewardedAds()
    {
        if (MaxSdk.IsRewardedAdReady(adUnitId))
        {
            UpdateButtonText("AL Rewarded Video");
            MaxSdk.ShowRewardedAd(adUnitId);
            return;
        }


        if (!isSubscribed)
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

            isSubscribed = true;
        }

        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(adUnitId);
        Debug.Log("Loading Rewarded Ad");
        UpdateButtonText("Loading Ad");
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded Ad Loaded");
        UpdateButtonText("Ad is Loaded");
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        Debug.LogError("Rewarded Ad Failed to Load: " + errorInfo.Message);
        UpdateButtonText("Failed to Load");
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        Debug.LogError("Rewarded Ad Failed to Display");
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded Ad Closed");
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("OnRewardedAdReceivedRewardEvent");
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void UpdateButtonText(string newText)
    {
        if (buttonText != null)
        {
            buttonText.text = newText;
        }

    }
}
