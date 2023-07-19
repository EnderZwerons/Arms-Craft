using System;
using System.Collections;
using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

public class MainManager : MonoBehaviour
{
	public UISprite loadSprite;

	public UISprite energySprite;

	public UILabel energyLabel;

	public Transform stageRoot;

	public GameObject loadingScreen;

	public GameObject stageSelector;

	public GameObject fadeOut;

	private Transform curStage;

	private bool isPlay;

	private bool isMessage;

	public GameObject panelMenu;

	public GameObject panelShop;

	public GameObject panelLab;

	public GameObject panelBody;

	public GameObject layerDropInfo;

	public GameObject player;

	public GameObject rootShip;

	public Camera mainCam;

	public GameObject[] weapons = new GameObject[3];

	public Transform[] itemRoot = new Transform[3];

	public Transform itemPos;

	public Transform itemGO;

	private int weaponNumber;

	private int curWeaponNbr;

	private int difficultLevel;

	public UILabel difficultLabel;

	public UILabel percentLabel;

	public UILabel goldLabel;

	public UILabel cashLabel;

	public int moneyShopState;

	public GameObject message;

	public Transform[] stage = new Transform[4];

	public Transform[] userProfiles = new Transform[4];

	public Transform[] dropInfo = new Transform[6];

	public UILabel noAdsLabel;

	public GameObject[] inGameTutorial;

	public GameObject freeGemObject;

	private void Awake()
	{
		Application.targetFrameRate = 240;
		GC.Collect();
	}

	private void Start()
	{
		Time.timeScale = 1f;
		isPlay = false;
		weaponNumber = 1;
		curWeaponNbr = 0;
		difficultLevel = 0;
		moneyShopState = 0;
		InitAudioManager();
		InitPlayerWeapon();
		InitUserProfile();
		UpdateDifficultLabel(difficultLevel);
		UpdateWalletLabel();
		difficultLevel = PlayerPrefs.GetInt("difficultLevel");
		UpdateDifficultLabel(difficultLevel);
		if (!DataManager.Instance.gamePrefs.isNoAds)
		{
			//if (!AdMobAndroid.isInterstitalReady())
			//{
			//	AdMobAndroid.requestInterstital("a15305641d6ea40");
			//}
		}
		else
		{
			noAdsLabel.enabled = false;
		}
	}

	private void FixedUpdate()
	{
		if (Application.loadedLevelName.Equals("GameMain"))
		{
			//if (DWUnityAdsManager.Instance.CanShowAds())
			//{
			//	freeGemObject.SetActive(true);
			//}
			//else
			//{
				freeGemObject.SetActive(false);
			//}
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			//EtceteraAndroid.showAlert("Attention", "Do you really want to exit?", "Cancel!", "Ok!");
		}
	}

	private void UpdateWalletLabel()
	{
		goldLabel.text = DataManager.Instance.gamePrefs.gold.ToString();
		cashLabel.text = DataManager.Instance.gamePrefs.cash.ToString();
	}

	private void InitAudioManager()
	{
		if (AudioManager.Instance.soundBgm.clip.name != AudioManager.Instance.BGM_Menu.name)
		{
			AudioManager.Instance.OnPlayBgm(SOUND.BGM);
		}
	}

	private void InitPlayerWeapon()
	{
		for (int i = 0; i < 3; i++)
		{
			List<BlockInfo> weaponList = GetWeaponList(curWeaponNbr);
			BlockInfo blockInfo = new BlockInfo();
			for (int j = 0; j < weaponList.Count; j++)
			{
				blockInfo = weaponList[j];
				int num = int.Parse(blockInfo.blockName.Substring(0, 2));
				GameObject gameObject = UnityEngine.Object.Instantiate(position: new Vector3(blockInfo.blockPosX, blockInfo.blockPosY, blockInfo.blockPosZ), original: BlockObjects.Instance.blockObject[num], rotation: Quaternion.identity) as GameObject;
				UnityEngine.Object.Destroy(gameObject.GetComponent<MeshCollider>());
				gameObject.transform.parent = itemRoot[i];
			}
			weapons[i].SetActive(false);
			curWeaponNbr++;
		}
		itemGO.transform.parent = itemPos;
		itemGO.localPosition = Vector3.zero;
		itemGO.localEulerAngles = Vector3.zero;
		itemGO.localScale *= 0.15f;
		weapons[1].transform.localScale *= 1.5f;
		weapons[2].transform.localScale *= 1.5f;
		weapons[weaponNumber].SetActive(true);
	}

	private List<BlockInfo> GetWeaponList(int wNum)
	{
		List<BlockInfo> result = new List<BlockInfo>();
		switch (wNum)
		{
		case 0:
			result = DataManager.Instance.gamePrefs.weapon0;
			break;
		case 1:
			result = DataManager.Instance.gamePrefs.weapon1;
			break;
		case 2:
			result = DataManager.Instance.gamePrefs.weapon2;
			break;
		}
		return result;
	}

	private void InitUserProfile()
	{
		userProfiles[0].Find("Label").GetComponent<UILabel>().text = DataManager.Instance.gamePrefs.killCnt.ToString();
		userProfiles[1].Find("Label").GetComponent<UILabel>().text = DataManager.Instance.gamePrefs.headCnt.ToString();
		userProfiles[2].Find("Label").GetComponent<UILabel>().text = DataManager.Instance.gamePrefs.meleeCnt.ToString();
		userProfiles[3].Find("Label").GetComponent<UILabel>().text = ConvertTime(DataManager.Instance.gamePrefs.playTime);
	}

	private void WeaponSwipe()
	{
		float x = UICamera.currentTouch.totalDelta.x;
		float num = (float)Screen.width / 1024f * 10f;
		if (x - num > 0f)
		{
			weaponNumber--;
			if (weaponNumber < 0)
			{
				weaponNumber = 2;
			}
		}
		else if (x + num < 0f)
		{
			weaponNumber++;
			if (weaponNumber > 2)
			{
				weaponNumber = 0;
			}
		}
		GameObject[] array = weapons;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		weapons[weaponNumber].SetActive(true);
	}

	private void UpdateStageLock()
	{
		GameObject gameObject = stage[1].Find("Sprite_Unlock").gameObject;
		GameObject gameObject2 = stage[2].Find("Sprite_Unlock").gameObject;
		GameObject gameObject3 = stage[3].Find("Sprite_Unlock").gameObject;
		Debug.Log(gameObject);
		int killCnt = DataManager.Instance.gamePrefs.killCnt;
		if (killCnt >= 200)
		{
			gameObject.SetActive(false);
		}
		if (killCnt >= 500)
		{
			gameObject2.SetActive(false);
		}
		if (killCnt >= 1000)
		{
			gameObject3.SetActive(false);
		}
	}

	private IEnumerator UnlockMessage(int num)
	{
		isMessage = true;
		UIPlayTween playTween = new UIPlayTween();
		playTween.resetOnPlay = true;
		playTween.tweenTarget = message;
		playTween.ifDisabledOnPlay = EnableCondition.EnableThenPlay;
		playTween.disableWhenFinished = DisableCondition.DisableAfterReverse;
		playTween.playDirection = Direction.Toggle;
		playTween.Play(true);
		switch (num)
		{
		case 2:
		{
			UISprite spriteStage3 = message.transform.Find("Sprite_Stage").GetComponent<UISprite>();
			UILabel labelQuest3 = message.transform.Find("Label_Quest").GetComponent<UILabel>();
			spriteStage3.spriteName = "planet_" + num;
			spriteStage3.MakePixelPerfect();
			spriteStage3.transform.localScale = new Vector3(3f, 3f, 1f);
			int killCnt3 = DataManager.Instance.gamePrefs.killCnt;
			labelQuest3.text = killCnt3 + "/200";
			break;
		}
		case 3:
		{
			UISprite spriteStage2 = message.transform.Find("Sprite_Stage").GetComponent<UISprite>();
			UILabel labelQuest2 = message.transform.Find("Label_Quest").GetComponent<UILabel>();
			spriteStage2.spriteName = "planet_" + num;
			spriteStage2.MakePixelPerfect();
			spriteStage2.transform.localScale = new Vector3(3f, 3f, 1f);
			int killCnt2 = DataManager.Instance.gamePrefs.killCnt;
			labelQuest2.text = killCnt2 + "/500";
			break;
		}
		case 4:
		{
			UISprite spriteStage = message.transform.Find("Sprite_Stage").GetComponent<UISprite>();
			UILabel labelQuest = message.transform.Find("Label_Quest").GetComponent<UILabel>();
			spriteStage.spriteName = "planet_" + num;
			spriteStage.MakePixelPerfect();
			spriteStage.transform.localScale = new Vector3(3f, 3f, 1f);
			int killCnt = DataManager.Instance.gamePrefs.killCnt;
			labelQuest.text = killCnt + "/1000";
			break;
		}
		}
		yield return new WaitForSeconds(2f);
		playTween.Play(false);
		isMessage = false;
	}

	private void SelectStage(Transform stageObject)
	{
		if (curStage == stageObject || isPlay || isMessage)
		{
			return;
		}
		if (stageObject.Find("Sprite_Unlock").gameObject.activeSelf)
		{
			int num = int.Parse(stageObject.name.Substring(5, 1));
			StartCoroutine(UnlockMessage(num));
			return;
		}
		Debug.Log(stageObject);
		curStage = stageObject;
		stageSelector.SetActive(true);
		stageSelector.transform.position = stageObject.position;
		stageSelector.GetComponent<Animation>().Play();
		if (inGameTutorial[0].activeSelf)
		{
			inGameTutorial[0].SetActive(false);
			inGameTutorial[1].SetActive(true);
		}
		UpdateDropInfo();
	}

	private void UpdateDropInfo()
	{
		if (!(curStage == null))
		{
			int index = int.Parse(curStage.name.Substring(5, 1)) - 1;
			int num = difficultLevel * 2 + 2;
			int num2 = 0;
			if (difficultLevel == 0)
			{
				num2 = -32;
			}
			else if (difficultLevel == 1)
			{
				num2 = -96;
			}
			else if (difficultLevel == 2)
			{
				num2 = -160;
			}
			for (int i = 0; i < 6; i++)
			{
				dropInfo[i].gameObject.SetActive(false);
			}
			for (int j = 0; j < num; j++)
			{
				dropInfo[j].gameObject.SetActive(true);
				dropInfo[j].transform.Find("Sprite").GetComponent<UISprite>().spriteName = LevelDesign.Instance.dropItem[index].dropItemInfo[j].itenName;
				dropInfo[j].localPosition = new Vector3(num2 + j * 64, 0f, 0f);
			}
			UIPlayTween uIPlayTween = new UIPlayTween();
			uIPlayTween.tweenTarget = layerDropInfo;
			uIPlayTween.resetOnPlay = true;
			uIPlayTween.ifDisabledOnPlay = EnableCondition.EnableThenPlay;
			uIPlayTween.disableWhenFinished = DisableCondition.DisableAfterReverse;
			uIPlayTween.playDirection = Direction.Forward;
			uIPlayTween.Play(true);
			uIPlayTween = null;
		}
	}

	private void GameStart()
	{
		if (!(curStage == null) && !isPlay && DataManager.Instance.gamePrefs.energy.energyAmt > 0)
		{
			isPlay = true;
			GameObject.Find("StartSprite").GetComponent<BoxCollider>().enabled = true;
			stageSelector.SetActive(false);
			Debug.Log("Stage Name : " + curStage.name);
			Transform child = stageRoot.GetChild(0);
			float x = curStage.localPosition.x * -1f;
			float y = curStage.localPosition.y * -1f;
			StartCoroutine(TweenEvent(child.GetComponent<TweenPosition>(), new Vector3(x, y, 0f)));
			if (inGameTutorial[1].activeSelf)
			{
				inGameTutorial[1].SetActive(false);
			}
		}
	}

	private IEnumerator TweenEvent(TweenPosition tweenTarget, Vector3 tweenPos)
	{
		tweenTarget.to = tweenPos;
		UIPlayTween playTween = new UIPlayTween();
		playTween.tweenTarget = tweenTarget.gameObject;
		playTween.Play(true);
		yield return new WaitForSeconds(3f);
		playTween.tweenTarget = stageRoot.gameObject;
		playTween.Play(true);
		yield return new WaitForSeconds(1.5f);
		playTween.tweenTarget = fadeOut;
		playTween.Play(true);
		yield return new WaitForSeconds(2f);
		ShowObject(loadingScreen);
		StartCoroutine(LoadScene("GamePlay" + curStage.name));
	}

	private IEnumerator LoadScene(string name)
	{
		AsyncOperation async = Application.LoadLevelAsync(name);
		async.allowSceneActivation = false;
		float tTime = 0f;
		int num = 1;
		while (!async.isDone)
		{
			yield return new WaitForSeconds(0.05f);
			loadSprite.spriteName = "b00" + num;
			num++;
			if (num > 9)
			{
				num = 1;
			}
			tTime += 0.05f;
			//if (!DataManager.Instance.gamePrefs.isNoAds && AdMobAndroid.isInterstitalReady() && tTime >= 2f)
			//{
			//	AdMobAndroid.displayInterstital();
			//}
			if (async.progress >= 0.9f && tTime >= 7f)
			{
				Debug.Log("Load Complete!");
				async.allowSceneActivation = true;
			}
		}
	}

	private void GoScene(string name)
	{
		Debug.Log("Go Scene Name : " + name);
		Application.LoadLevel(name);
	}

	private void ShowObject(GameObject target)
	{
		target.SetActive(true);
		switch (target.name)
		{
		case "Panel_Shop":
		{
			panelMenu.SetActive(false);
			player.SetActive(false);
			rootShip.SetActive(false);
			ShopManager component = GameObject.Find("_ShopManager").GetComponent<ShopManager>();
			component.UpdateInvenLabel();
			break;
		}
		case "Panel_Lab":
			panelMenu.SetActive(false);
			player.SetActive(false);
			rootShip.SetActive(false);
			LabManager.Instance.InitLabManager(true);
			break;
		case "Panel_Upgrade":
			panelMenu.SetActive(false);
			rootShip.SetActive(false);
			mainCam.rect = new Rect(-0.45f, 0f, 1f, 1f);
			mainCam.depth = 3f;
			break;
		case "Panel_Stage":
			panelMenu.SetActive(false);
			player.SetActive(false);
			rootShip.SetActive(false);
			UpdateStageLock();
			if (!DataManager.Instance.gamePrefs.stageViewTutorial)
			{
				DataManager.Instance.gamePrefs.stageViewTutorial = true;
				inGameTutorial[0].SetActive(true);
			}
			break;
		case "Panel_Option":
			panelMenu.SetActive(false);
			player.SetActive(false);
			rootShip.SetActive(false);
			GetComponent<OptionManager>().InitOption();
			break;
		case "Panel_MoneyShop":
			if (panelMenu.activeSelf)
			{
				panelMenu.SetActive(false);
				moneyShopState = 0;
			}
			else if (panelShop.activeSelf)
			{
				panelShop.SetActive(false);
				moneyShopState = 1;
			}
			else if (panelLab.activeSelf)
			{
				panelLab.SetActive(false);
				moneyShopState = 2;
			}
			else if (panelBody.activeSelf)
			{
				panelBody.SetActive(false);
				player.SetActive(false);
				moneyShopState = 3;
			}
			else
			{
				moneyShopState = -1;
			}
			break;
		case "Panel_Profile":
			panelMenu.SetActive(false);
			rootShip.SetActive(false);
			mainCam.rect = new Rect(0.55f, 0f, 1f, 1f);
			mainCam.depth = 3f;
			break;
		case "Panel_StuffInventory":
			LabManager.Instance.UpdateStuffLabel();
			break;
		}
	}

	private void HideObject(GameObject target)
	{
		target.SetActive(false);
		switch (target.name)
		{
		case "Panel_Shop":
			panelMenu.SetActive(true);
			player.SetActive(true);
			rootShip.SetActive(true);
			break;
		case "Panel_Lab":
			panelMenu.SetActive(true);
			player.SetActive(true);
			rootShip.SetActive(true);
			LabManager.Instance.InitLabManager(false);
			break;
		case "Panel_Upgrade":
			panelMenu.SetActive(true);
			rootShip.SetActive(true);
			mainCam.rect = new Rect(0f, 0f, 1f, 1f);
			mainCam.depth = 0f;
			break;
		case "Panel_Stage":
			panelMenu.SetActive(true);
			player.SetActive(true);
			rootShip.SetActive(true);
			stageSelector.SetActive(false);
			layerDropInfo.SetActive(false);
			curStage = null;
			break;
		case "Panel_Option":
			panelMenu.SetActive(true);
			player.SetActive(true);
			rootShip.SetActive(true);
			DataManager.Instance.SaveData();
			break;
		case "Panel_MoneyShop":
			if (moneyShopState == 0)
			{
				panelMenu.SetActive(true);
			}
			else if (moneyShopState == 1)
			{
				panelShop.SetActive(true);
			}
			else if (moneyShopState == 2)
			{
				panelLab.SetActive(true);
			}
			else if (moneyShopState == 3)
			{
				panelBody.SetActive(true);
				player.SetActive(true);
			}
			break;
		case "Panel_Profile":
			panelMenu.SetActive(true);
			rootShip.SetActive(true);
			mainCam.rect = new Rect(0f, 0f, 1f, 1f);
			mainCam.depth = 0f;
			break;
		}
	}

	private void BuyGold(int num)
	{
		//InAppManager.Instance.PurchaseGold(num);
	}

	private void BuyCash(int num)
	{
		//InAppManager.Instance.PurchaseCash(num);
	}

	private void DifficultyAdjusting()
	{
		Debug.Log("Dificulty Adjusting");
		difficultLevel++;
		if (difficultLevel > 2)
		{
			difficultLevel = 0;
		}
		PlayerPrefs.SetInt("difficultLevel", difficultLevel);
		UpdateDifficultLabel(difficultLevel);
		UpdateDropInfo();
	}

	private void UpdateDifficultLabel(int num)
	{
		switch (num)
		{
		case 0:
			difficultLabel.text = "EASY";
			percentLabel.text = "x 1";
			LevelDesign.Instance.LoadLevelDesignVal(num);
			break;
		case 1:
			difficultLabel.text = "NORMAL";
			percentLabel.text = "x 1.5";
			LevelDesign.Instance.LoadLevelDesignVal(num);
			break;
		case 2:
			difficultLabel.text = "HARD";
			percentLabel.text = "x 2";
			LevelDesign.Instance.LoadLevelDesignVal(num);
			break;
		}
	}

	public void FreeGem()
	{
		//DWUnityAdsManager.Instance.ShowAds();
	}

	private string ConvertTime(long time)
	{
		long num = 0L;
		long num2 = 0L;
		long num3 = 0L;
		num = time / 3600;
		num2 = time % 3600 / 60;
		num3 = time % 3600 % 60;
		return num.ToString("D2") + ":" + num2.ToString("D2") + ":" + num3.ToString("D2");
	}
}
