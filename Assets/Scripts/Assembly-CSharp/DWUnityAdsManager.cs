using UnityEngine;
using UnityEngine.Advertisements;

public class DWUnityAdsManager : MonoBehaviour
{
	private static DWUnityAdsManager instance;

	public string gameID_IOS;

	public string gameID_Android;

	public bool testMode;

	public bool allowPrecache = true;

	public static DWUnityAdsManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.Find("_DevWeek").GetComponent<DWUnityAdsManager>();
			}
			return instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (Advertisement.isSupported)
		{
			Advertisement.allowPrecache = allowPrecache;
			Advertisement.Initialize(gameID_Android, testMode);
		}
	}

	public void ShowAds()
	{
		if (Advertisement.isReady("rewardedVideoZone"))
		{
			Advertisement.Show("rewardedVideoZone", new ShowOptions
			{
				resultCallback = delegate(ShowResult result)
				{
					HandleShowResult(result);
				}
			});
		}
	}

	public bool CanShowAds()
	{
		return Advertisement.isReady("rewardedVideoZone");
	}

	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log("The ad was successfully shown.");
			EtceteraAndroid.showAlert("Reward", "2 Gem", "OK!");
			DataManager.Instance.gamePrefs.cash += 2;
			DataManager.Instance.SaveData();
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}
}
