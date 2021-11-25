using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System.Collections;

public class AdsManeger : MonoBehaviour
{
    public string gameID_Android = "4471015";
    public string placementID = "Banner_Android";

    void Start()
    {
        Advertisement.Initialize(gameID_Android);
        StartCoroutine(ShowBannerWhenInitialized());
    }

    IEnumerator ShowBannerWhenInitialized() 
    {
        while (!Advertisement.isInitialized)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(placementID);
    }
}
