using System.Collections;
using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static GameManager instance;

	public STATE state;

	public Transform hitRoot;

	public UISprite hitScreenFlash;

	public UISprite getScreenFlash;

	public Vector3 hitPos;

	public GameObject[] weapons;

	public Transform[] itemRoot;

	public GameObject pausePanel;

	public GameObject shopPanel;

	public GameObject downPanel;

	public GameObject resultPanel;

	public GameObject orignPanel;

	public GameObject killRewardLayer;

	private float speed;

	public int kill;

	public int headShot;

	public int meleeKill;

	public long playTime;

	public int useBullets;

	public int hitBullets;

	public int hitPlayer;

	public int downPlayer;

	public int revivalPlayer;

	public bool autoFire;

	public Animation HeadshotEffect;

	public UILabel[] resultData = new UILabel[4];

	public bool isPlay;

	public bool isDown;

	public FPSController fpsController;

	private float orignX;

	private int touchCount;

	private int weaponNumber = 1;

	public int WeaponIndex
	{
		get
		{
			return weaponNumber;
		}
	}

	public UISlider killBar;

	public UISlider healthBar;

	public UISlider shieldBar;

	public UILabel killLabel;

	public UILabel bulletLabel;

	public UILabel grenadeLabel;

	public UISprite weaponIcon;

	public UILabel[] goldLabel;

	public UILabel[] cashLabel;

	public UILabel countLabel;

	public UILabel[] rewardLabel = new UILabel[3];

	public static GameManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.Find("_GameManager").GetComponent<GameManager>();
			}
			return instance;
		}
	}

	private void OnApplicationQuit()
	{
		instance = null;
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus && !shopPanel.activeSelf)
		{
			GamePause(false);
		}
	}

	private void Start()
	{
		InitAudioManager();
		InitWeaponData();
		InitRewardLabel();
		InitKillReward();
		//if (PlayerPrefs.GetInt("autoFire") == 0)
		//{
			autoFire = false;
		//}
		//else
		//{
		//	autoFire = true;
		//}
		fpsController.FPSControl.InitControl();
		//if (autoFire)
		//{
		//	GameObject.Find("AutoButton").transform.GetChild(0).GetComponent<UISprite>().spriteName = "auto_on";
		//}
		//else
		//{
		//	GameObject.Find("AutoButton").transform.GetChild(0).GetComponent<UISprite>().spriteName = "auto_off";
		//}
		speed = 12f + (float)(DataManager.Instance.gamePrefs.upgradeLower - 1) * 0.1f;
		UpdateWeaponData(weaponNumber);
		UpdateWalletLabel();
		state = STATE.INIT;
		Time.timeScale = 1f;
	}

	private void InitAudioManager()
	{
		Transform parent = GameObject.FindWithTag("Player").transform;
		AudioManager.Instance.transform.parent = parent;
		AudioManager.Instance.transform.localPosition = Vector3.up;
		AudioManager.Instance.transform.localEulerAngles = Vector3.zero;
		AudioManager.Instance.OnPlayBgm(SOUND.PLAY);
	}

	private void Update()
	{
		switch (state)
		{
		case STATE.INIT:
			GameInit();
			break;
		case STATE.START:
			GameStart();
			break;
		case STATE.DAMAGE:
			HitPlayer();
			break;
		case STATE.DOWN:
			PlayerDown();
			break;
		case STATE.CLEAR:
			GameClear();
			break;
		case STATE.OVER:
			GameOver();
			break;
		case STATE.LOAD:
			LoadData();
			break;
		}
		if (!Input.GetKeyDown(KeyCode.Escape) || shopPanel.activeSelf || downPanel.activeSelf || resultPanel.activeSelf)
		{
			return;
		}
		if (pausePanel.activeSelf)
		{
			if (GameObject.Find("Panel_Option") == null)
			{
				GameResume(false);
			}
		}
		else if (GameObject.Find("Panel_Option") == null)
		{
			GamePause(false);
		}
	}

	private void GameInit()
	{
		fpsController.InitPlayerState();
		UpdateKillCountGUI();
		UpdateKillLabelGUI();
		UpdateHealthGUI();
		UpdateShieldGUI();
		UpdateAmmoGUI();
		UpdateGrenadeGUI();
		state = STATE.LOAD;
	}

	private void GameStart()
	{
		state = STATE.IDLE;
		isPlay = true;
	}

	private void HitPlayer()
	{
		Debug.Log("Hit Player");
		hitPlayer++;
		Vector3 vector = fpsController.transform.InverseTransformPoint(hitPos);
		float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
		string animation = "PlayerHitR";
		if (num < 0f)
		{
			num += 360f;
			animation = "PlayerHitL";
		}
		hitRoot.transform.localEulerAngles = new Vector3(0f, 0f, 0f - num);
		UIPlayTween uIPlayTween = new UIPlayTween();
		uIPlayTween.tweenTarget = hitScreenFlash.gameObject;
		uIPlayTween.resetOnPlay = true;
		uIPlayTween.Play(true);
		fpsController.mainAnim.GetComponent<Animation>()[animation].speed = 1f;
		fpsController.mainAnim.Play(animation);
		state = STATE.IDLE;
	}

	private void PlayerDown()
	{
		state = STATE.WAIT;
		if (!isDown)
		{
			isDown = true;
			fpsController.mainAnim.GetComponent<Animation>()["PlayerDown"].speed = 1f;
			fpsController.mainAnim.Play("PlayerDown");
			fpsController.FPSControl.PauseMotor();
			downPlayer++;
			StartCoroutine("CountDown");
		}
	}

	private void PlayerRevival()
	{
		if (DataManager.Instance.gamePrefs.cash >= 5)
		{
			DataManager.Instance.gamePrefs.cash -= 5;
			isDown = false;
			StopCoroutine("CountDown");
			orignPanel.SetActive(true);
			downPanel.SetActive(false);
			fpsController.mainAnim.GetComponent<Animation>()["PlayerDown"].speed = -1f;
			fpsController.mainAnim.GetComponent<Animation>()["PlayerDown"].time = fpsController.mainAnim.GetComponent<Animation>()["PlayerDown"].length;
			fpsController.mainAnim.Play("PlayerDown");
			revivalPlayer++;
			fpsController.InitPlayerState();
			UpdateHealthGUI();
			UpdateShieldGUI();
			UpdateAmmoGUI();
			UpdateGrenadeGUI();
			UpdateWalletLabel();
			state = STATE.START;
		}
	}

	public void WeaponChangeForPC(bool down)
	{
		if (fpsController.reload)
		{
			return;
		}
		if (down)
		{
			weaponNumber--;
			if (weaponNumber < 0)
			{
				weaponNumber = 2;
			}
		}
		else
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
		UpdateWeaponData(weaponNumber);
	}

	private void WeaponSwipe()
	{
		if (fpsController.reload)
		{
			return;
		}
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
		UpdateWeaponData(weaponNumber);
	}

	public void AutoFire()
	{
		autoFire = !autoFire;
		fpsController.FPSControl.InitControl();
		if (autoFire)
		{
			GameObject.Find("AutoButton").transform.GetChild(0).GetComponent<UISprite>().spriteName = "auto_on";
		}
		else
		{
			GameObject.Find("AutoButton").transform.GetChild(0).GetComponent<UISprite>().spriteName = "auto_off";
		}
		int value = (autoFire ? 1 : 0);
		PlayerPrefs.SetInt("autoFire", value);
	}

	private void UpdateWeaponData(int num)
	{
		weaponIcon.spriteName = "weapon" + num;
		fpsController.CurrentWeapon = fpsController.WeaponList[num];
		bulletLabel.transform.parent.gameObject.SetActive(false);
		if (fpsController.CurrentWeapon.firearms)
		{
			bulletLabel.transform.parent.gameObject.SetActive(true);
			UpdateAmmoGUI();
		}
		fpsController.moveSpeed = speed - LevelDesign.Instance.basicWeaponOption[num].basicWeight;
	}

	private IEnumerator CountDown()
	{
		orignPanel.SetActive(false);
		downPanel.SetActive(true);
		Debug.Log("Begin CountDown Event");
		UIPlayTween playTween = new UIPlayTween
		{
			tweenTarget = countLabel.gameObject,
			resetOnPlay = true
		};
		for (int countTimer = 10; countTimer > 0; countTimer--)
		{
			countLabel.text = countTimer.ToString();
			playTween.Play(true);
			yield return new WaitForSeconds(1f);
		}
		state = STATE.OVER;
	}

	private void GameClear()
	{
		Debug.Log("Game Clear");
	}

	private void GameOver()
	{
		state = STATE.WAIT;
		Debug.Log("Game Over");
		StopCoroutine("CountDown");
		playTime = (long)Time.timeSinceLevelLoad;
		DataManager.Instance.gamePrefs.killCnt += kill;
		DataManager.Instance.gamePrefs.headCnt += headShot;
		DataManager.Instance.gamePrefs.meleeCnt += meleeKill;
		DataManager.Instance.gamePrefs.playTime += playTime;
		DataManager.Instance.gamePrefs.useBullets += useBullets;
		DataManager.Instance.gamePrefs.hitBullets += hitBullets;
		DataManager.Instance.gamePrefs.hitPlayer += hitPlayer;
		DataManager.Instance.gamePrefs.downPlayer += downPlayer;
		DataManager.Instance.gamePrefs.revivalPlayer += revivalPlayer;
		DataManager.Instance.SaveData();
		resultData[0].text = kill.ToString();
		resultData[1].text = headShot.ToString();
		resultData[2].text = meleeKill.ToString();
		resultData[3].text = ConvertTime(playTime);
		downPanel.SetActive(false);
		resultPanel.SetActive(true);
	}

	private void GamePause(bool shop)
	{
		Debug.Log("Game Pause");
		fpsController.FPSControl.PauseMotor();
		UpdateKillCountGUI();
		if (shop)
		{
			shopPanel.SetActive(true);
		}
		else
		{
			pausePanel.SetActive(true);
		}
		state = STATE.PAUSE;
		Time.timeScale = 0f;
	}

	private void GameResume(bool shop)
	{
		Debug.Log("Game Resume");
		if (shop)
		{
			shopPanel.SetActive(false);
		}
		else
		{
			pausePanel.SetActive(false);
		}
		state = STATE.IDLE;
		Time.timeScale = 1f;
	}

	public void ScreenFlashEvent()
	{
		UIPlayTween uIPlayTween = new UIPlayTween();
		uIPlayTween.tweenTarget = getScreenFlash.gameObject;
		uIPlayTween.resetOnPlay = true;
		uIPlayTween.Play(true);
	}

	public void HeadShotEffect()
	{
		AudioManager.Instance.OnPlayEffect();
		HeadshotEffect.Play();
	}

	private void ShowObject(GameObject target)
	{
		target.SetActive(true);
		switch (target.name)
		{
		case "Panel_Option":
			pausePanel.SetActive(false);
			GetComponent<OptionManager>().InitOption();
			break;
		}
	}

	private void HideObject(GameObject target)
	{
		target.SetActive(false);
		switch (target.name)
		{
		case "Panel_Option":
			pausePanel.SetActive(true);
			DataManager.Instance.SaveData();
			fpsController.Sensitivity = DataManager.Instance.gamePrefs.sensitivity;
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

	private void InitRewardLabel()
	{
		switch (LevelDesign.Instance.difficultDesign.difficultLevel)
		{
		case 0:
			rewardLabel[0].text = "1~5";
			rewardLabel[1].text = "100~300";
			rewardLabel[2].text = "1~3";
			break;
		case 1:
			rewardLabel[0].text = "2~10";
			rewardLabel[1].text = "200~500";
			rewardLabel[2].text = "2~5";
			break;
		case 2:
			rewardLabel[0].text = "3~15";
			rewardLabel[1].text = "300~700";
			rewardLabel[2].text = "3~7";
			break;
		}
	}

	private void InitKillReward()
	{
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			LevelDesign.Instance.killReward[i].rewardAmount = RandomCreator(num);
			LevelDesign.Instance.killReward[i].rewardCheck = false;
			num++;
		}
	}

	public void CheckKillReward()
	{
		if (50 <= kill && !LevelDesign.Instance.killReward[0].rewardCheck)
		{
			LevelDesign.Instance.killReward[0].rewardCheck = true;
			StartCoroutine(GetKillReward(0, LevelDesign.Instance.killReward[0].rewardAmount));
		}
		else if (100 <= kill && !LevelDesign.Instance.killReward[1].rewardCheck)
		{
			LevelDesign.Instance.killReward[1].rewardCheck = true;
			StartCoroutine(GetKillReward(1, LevelDesign.Instance.killReward[1].rewardAmount));
		}
		else if (150 <= kill && !LevelDesign.Instance.killReward[2].rewardCheck)
		{
			LevelDesign.Instance.killReward[2].rewardCheck = true;
			StartCoroutine(GetKillReward(2, LevelDesign.Instance.killReward[2].rewardAmount));
		}
	}

	private IEnumerator GetKillReward(int num, int amount)
	{
		UISprite background = killRewardLayer.transform.Find("Background").GetComponent<UISprite>();
		UISprite sprite = killRewardLayer.transform.Find("Sprite").GetComponent<UISprite>();
		UILabel label = killRewardLayer.transform.Find("Label").GetComponent<UILabel>();
		string amt = amount.ToString();
		background.width = 105 + 25 * amt.Length;
		sprite.spriteName = "reward" + num;
		label.text = "+ " + amt;
		switch (num)
		{
		case 0:
			fpsController.Grenade += amount;
			UpdateGrenadeGUI();
			break;
		case 1:
			DataManager.Instance.gamePrefs.gold += amount;
			UpdateWalletLabel();
			break;
		case 2:
			DataManager.Instance.gamePrefs.cash += amount;
			UpdateWalletLabel();
			break;
		}
		UIPlayTween playTween = new UIPlayTween();
		playTween.resetOnPlay = true;
		playTween.tweenTarget = killRewardLayer;
		playTween.ifDisabledOnPlay = EnableCondition.EnableThenPlay;
		playTween.disableWhenFinished = DisableCondition.DisableAfterReverse;
		playTween.playDirection = Direction.Toggle;
		playTween.Play(true);
		yield return new WaitForSeconds(4f);
		playTween.Play(false);
	}

	private int RandomCreator(int reward)
	{
		int num = Random.Range(1, 101);
		int num2 = ((1 > num || num > 50) ? ((51 <= num && num <= 85) ? 1 : ((86 <= num && num <= 95) ? 2 : ((96 > num || num > 99) ? 4 : 3))) : 0);
		int difficultLevel = LevelDesign.Instance.difficultDesign.difficultLevel;
		switch (reward)
		{
		case 0:
			num2 = LevelDesign.Instance.rewardTier[difficultLevel].reward0Amount[num2];
			break;
		case 1:
			num2 = LevelDesign.Instance.rewardTier[difficultLevel].reward1Amount[num2];
			break;
		case 2:
			num2 = LevelDesign.Instance.rewardTier[difficultLevel].reward2Amount[num2];
			break;
		}
		return num2;
	}

	private void ReturnMain()
	{
		Debug.Log("SaveData");
		GameObject gameObject = GameObject.Find("_AudioManager");
		gameObject.transform.parent = null;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localEulerAngles = Vector3.zero;
		DontDestroyOnLoad(gameObject);
		Application.LoadLevel("GameMain");
	}

	private void LoadData()
	{
		BlockInfo blockInfo = new BlockInfo();
		for (int i = 0; i < 3; i++)
		{
			List<BlockInfo> weaponList = GetWeaponList(i);
			for (int j = 0; j < weaponList.Count; j++)
			{
				blockInfo = weaponList[j];
				int num = int.Parse(blockInfo.blockName.Substring(0, 2));
				Vector3 localPosition = new Vector3(blockInfo.blockPosX, blockInfo.blockPosY, blockInfo.blockPosZ);
				GameObject gameObject = Object.Instantiate(BlockObjects.Instance.blockObject[num]);
				Object.Destroy(gameObject.GetComponent<MeshCollider>());
				gameObject.transform.parent = itemRoot[i];
				gameObject.transform.localPosition = localPosition;
				gameObject.transform.localEulerAngles = Vector3.zero;
			}
			weapons[i].transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
//			itemRoot[i].parent.SendMessage("StartCombine");
			weapons[i].SetActive(false);
		}
		weapons[1].transform.localScale *= 1.5f;
		weapons[2].transform.localScale *= 1.5f;
		foreach (Transform item in itemRoot[1])
		{
			if (item.name.Substring(0, 2) == "10")
			{
				weapons[1].transform.Find("FirePos").position = new Vector3(item.position.x, item.position.y + 0.01f, item.position.z + 0.2f);
			}
		}
		foreach (Transform item2 in itemRoot[2])
		{
			if (item2.name.Substring(0, 2) == "12")
			{
				weapons[2].transform.Find("FirePos").position = new Vector3(item2.position.x, item2.position.y + 0.01f, item2.position.z + 0.6f);
			}
		}
		weapons[1].SetActive(true);
		state = STATE.START;
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

	private void InitWeaponData()
	{
		for (int i = 0; i < 3; i++)
		{
			fpsController.WeaponList[i].WeaponPower = DataManager.Instance.gamePrefs.weaponData[i].totalDamage;
			fpsController.WeaponList[i].fireRate = DataManager.Instance.gamePrefs.weaponData[i].totalRateOfFire;
			fpsController.WeaponList[i].bulletperClip = DataManager.Instance.gamePrefs.weaponData[i].totalBulletPerClip;
		}
	}

	public void UpdateWalletLabel()
	{
		goldLabel[0].text = DataManager.Instance.gamePrefs.gold.ToString();
		cashLabel[0].text = DataManager.Instance.gamePrefs.cash.ToString();
		goldLabel[1].text = DataManager.Instance.gamePrefs.gold.ToString();
		cashLabel[1].text = DataManager.Instance.gamePrefs.cash.ToString();
	}

	public void UpdateKillCountGUI()
	{
		float num = (float)kill / 200f;
		if (num > 1f)
		{
			num = 1f;
		}
		killBar.value = num;
	}

	public void UpdateKillLabelGUI()
	{
		killLabel.text = kill.ToString();
	}

	public void UpdateHealthGUI()
	{
		healthBar.value = fpsController.Health / fpsController.MaxHealth;
	}

	public void UpdateShieldGUI()
	{
		shieldBar.value = fpsController.Shield / fpsController.MaxShield;
	}

	public void UpdateAmmoGUI()
	{
		bulletLabel.text = fpsController.CurrentWeapon.bulletinMagasine.ToString() + "/" + fpsController.CurrentWeapon.bulletleft;
	}

	public void UpdateGrenadeGUI()
	{
		grenadeLabel.text = fpsController.Grenade.ToString();
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
