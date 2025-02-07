using UnityEngine;
using UnityEngine.UI;
using TMPro;  

public class ALInterstitial : MonoBehaviour
{
    #if UNITY_IOS
    string adUnitId = "88a2c8359162b418";
    #else 
    string adUnitId = "09d80a95e7f64732";
    #endif

    public Button interstitialButton;
    private TMP_Text buttonText;  

    private bool isSubscribed = false;

    void Start()
    {
        MaxSdk.ShowMediationDebugger();

        if (interstitialButton == null)
        {
            interstitialButton = GameObject.Find("ALInterstitialButton")?.GetComponent<Button>();
        }

        if (interstitialButton != null)
        {
            buttonText = interstitialButton.GetComponentInChildren<TMP_Text>();  
        }
    }

    public void InitializeInterstitialAds()
    {
        if (MaxSdk.IsInterstitialReady(adUnitId))
        {
            UpdateButtonText("AL Interstitial");
            MaxSdk.ShowInterstitial(adUnitId);
            return;
        }

        if (!isSubscribed)
        {
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        isSubscribed = true;
        }

        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(adUnitId);
        Debug.Log("Loading Interstitial Ad");
        UpdateButtonText("Loading Ad");
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Interstitial Ad Loaded");
        UpdateButtonText("Ad is Loaded");
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        Debug.LogError("Interstitial Ad Failed to Load: " + errorInfo.Message);
        UpdateButtonText("Failed to Load");
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {}

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        Debug.LogError("Interstitial Ad Failed to Display");
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {}

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Interstitial Ad Closed");
    }

    private void UpdateButtonText(string newText)
    {
        if (buttonText != null)
        {
            buttonText.text = newText;
        }
       
    }
}
