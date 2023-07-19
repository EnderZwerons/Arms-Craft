using System.Collections;
using UnityEngine;

public class BootManager : MonoBehaviour
{
	public enum STATE
	{
		GAMELOAD,
		DATALOAD,
		INIT,
		WAIT
	}

	private BaasIO baasIO;

	//private InAppManager inAppManager;

	private LevelDesign levelDesign;

	private DataManager dataManager;

	public STATE state = STATE.WAIT;

	private IEnumerator Start()
	{
		baasIO = GameObject.Find("_BaasIO").GetComponent<BaasIO>();
		//inAppManager = GameObject.Find("_InAppManager").GetComponent<InAppManager>();
		levelDesign = GameObject.Find("_LevelDesign").GetComponent<LevelDesign>();
		dataManager = GameObject.Find("_DataManager").GetComponent<DataManager>();
		state = STATE.GAMELOAD;
		yield return null;
	}

	private void FixedUpdate()
	{
		switch (state)
		{
		case STATE.GAMELOAD:
			LoadLevelDesign();
			break;
		case STATE.DATALOAD:
			LoadUserData();
			break;
		case STATE.INIT:
			StartCoroutine(LoadGame());
			break;
		}
	}

	private void ConnectProductsList()
	{
		state = STATE.WAIT;
		//inAppManager.RequestProducts();
	}

	private void ConnectAdMob()
	{
		state = STATE.WAIT;
		if (!DataManager.Instance.gamePrefs.isNoAds)
		{
		//	AdMobAndroid.init("a15305641d6ea40");
		}
		StartCoroutine("RequestConnectionSuccessful");
	}

	private IEnumerator RequestConnectionSuccessful()
	{
		yield return null;
		state = STATE.GAMELOAD;
	}

	private void LoadLevelDesign()
	{
		state = STATE.WAIT;
		levelDesign.LoadLevelDesign();
		state = STATE.DATALOAD;
	}

	private void LoadUserData()
	{
		state = STATE.WAIT;
		Debug.Log("IN LOAD");
		dataManager.LoadData();
		AudioManager.Instance.InitAudio();
		AudioManager.Instance.OnPlayBgm(SOUND.BGM);
		Debug.Log("LOAD DONE");
		state = STATE.INIT;
	}

	private IEnumerator LoadGame()
	{
		state = STATE.WAIT;
		Debug.Log("LOAD GAME");
		AsyncOperation async2 = new AsyncOperation();
		async2 = ((!dataManager.isStarter) ? Application.LoadLevelAsync("GameMain") : Application.LoadLevelAsync("GameTutorial"));
		baasIO.isConnect = true;
		baasIO.isSuccess = true;
		yield return async2;
	}

	public void GetTimeStamp(long time)
	{
		dataManager.serverTimeStamp = time / 10000000;
		dataManager.intervalTime = Time.realtimeSinceStartup;
	}
}
