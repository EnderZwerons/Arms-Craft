using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
	[Serializable]
	public class BlockAmount
	{
		public bool isAdd;

		public int blockNum;
	}

	[Serializable]
	public class CurWeaponIndicator
	{
		public UILabel[] curWeaponLabel = new UILabel[4];
	}

	[Serializable]
	public class SelectBlockIndocator
	{
		public UISprite selectBlockSprite;

		public UILabel[] selectBlockLabel = new UILabel[4];
	}

	public GameObject[] weapon2Buttons;

	public GameObject[] weapon3Buttons;

	public bool delete;

	private int curWeaponNbr;

	private int curBlockNbr;

	public GameObject[] weapons;

	public Transform[] itemRoot;

	public UICamera uiCam;

	public CurWeaponIndicator curWeaponIndicator;

	public SelectBlockIndocator selectBlockIndocator;

	public UILabel blockPointIndicator;

	public UILabel[] itemAmount = new UILabel[10];

	public Dictionary<string, GameObject> panelList = new Dictionary<string, GameObject>();

	public List<BlockAmount> bAmt = new List<BlockAmount>();

	public TouchLook touchLook;

	public TouchBuilder touchBuilder;

	public bool isUpdate;

	private int curBP;

	private int maxBP;

	public UILabel warningLabel;

	public UISprite highLight;

	public Transform selectLight;

	public GameObject touchLight;

	public UILabel noAdsLabel;

	private void Start()
	{
		InitBlockPoint();
		InitPanelList();
		if (DataManager.Instance.gamePrefs.isNoAds)
		{
			noAdsLabel.enabled = false;
		}
	}

	private void Update()
	{
		//if (Input.GetKeyDown(KeyCode.Escape))
		//{
		//	EtceteraAndroid.showAlert("Attention", "Do you really want to exit?", "Cancel!", "Ok!");
		//}
	}

	private void InitBlockPoint()
	{
		int level = DataManager.Instance.gamePrefs.level;
		int upgradeUpper = DataManager.Instance.gamePrefs.upgradeUpper;
		maxBP = 10 + (level - 1) + (upgradeUpper - 1) * 2;
	}

	private void UpdateBlockPointLabel()
	{
		blockPointIndicator.text = curBP + "/" + maxBP;
	}

	private void UpdateBlockDataLabel()
	{
		for (int i = 0; i < 10; i++)
		{
			itemAmount[i].text = DataManager.Instance.gamePrefs.blockAmt[i].ToString();
		}
	}

	private void ReturnMenu()
	{
		Debug.Log("ReturnMenu()");
		if (isUpdate)
		{
			Debug.Log("Show popup");
			ShowObject(panelList["Save"]);
		}
		else
		{
			for (int i = 0; i < itemRoot[curWeaponNbr].childCount; i++)
			{
				UnityEngine.Object.Destroy(itemRoot[curWeaponNbr].GetChild(i).gameObject);
			}
			Transform transform = GameObject.Find("RootGO").transform;
			transform.localEulerAngles = new Vector3(0f, 90f, 0f);
			transform.GetChild(0).localPosition = Vector3.zero;
			transform.GetChild(0).GetChild(0).localPosition = new Vector3(0f, 0f, -10f);
			weapons[curWeaponNbr].SetActive(false);
			GoPanel("Menu");
		}
		touchLook.enabled = false;
		touchBuilder.enabled = false;
	}

	private void SelectPopup(bool save)
	{
		Debug.Log("ReturnMenu(bool)");
		if (save)
		{
			if (!GameObject.FindWithTag("Muzzle") && curWeaponNbr != 0)
			{
				StartCoroutine("NotFindMuzzle");
				return;
			}
			SaveData();
		}
		else
		{
			for (int i = 0; i < itemRoot[curWeaponNbr].childCount; i++)
			{
				UnityEngine.Object.Destroy(itemRoot[curWeaponNbr].GetChild(i).gameObject);
			}
			for (int j = 0; j < bAmt.Count; j++)
			{
				if (bAmt[j].isAdd)
				{
					int blockNum = bAmt[j].blockNum;
					DataManager.Instance.gamePrefs.blockAmt[blockNum]++;
				}
				else
				{
					int blockNum2 = bAmt[j].blockNum;
					DataManager.Instance.gamePrefs.blockAmt[blockNum2]--;
				}
			}
			bAmt.Clear();
			weapons[curWeaponNbr].SetActive(false);
			GoPanel("Menu");
		}
		Transform transform = GameObject.Find("RootGO").transform;
		transform.localEulerAngles = new Vector3(0f, 90f, 0f);
		transform.GetChild(0).localPosition = Vector3.zero;
		transform.GetChild(0).GetChild(0).localPosition = new Vector3(0f, 0f, -10f);
	}

	private IEnumerator NotFindMuzzle()
	{
		HideObject(panelList["Save"]);
		touchLight.SetActive(true);
		yield return new WaitForSeconds(1f);
		touchLight.SetActive(false);
		touchLook.enabled = true;
		touchBuilder.enabled = true;
	}

	private void SaveData()
	{
		Debug.Log("Save Data");
		List<BlockInfo> weaponList = GetWeaponList(curWeaponNbr);
		weaponList.Clear();
		float num = 0f + LevelDesign.Instance.basicWeaponOption[curWeaponNbr].basicDamage;
		float num2 = 0f;
		int num3 = 0 + LevelDesign.Instance.basicWeaponOption[curWeaponNbr].basicBullet;
		int num4 = 0;
		for (int i = 0; i < itemRoot[curWeaponNbr].childCount; i++)
		{
			BlockInfo blockInfo = new BlockInfo();
			Transform child = itemRoot[curWeaponNbr].GetChild(i);
			blockInfo.blockName = child.name.Substring(0, 2);
			blockInfo.blockPosX = child.position.x;
			blockInfo.blockPosY = child.position.y;
			blockInfo.blockPosZ = child.position.z;
			weaponList.Add(blockInfo);
			if (child.tag == "Part")
			{
				Block component = child.GetComponent<Block>();
				num += component.damage;
				num2 += component.rate;
				num3 += component.bullet;
				num4++;
			}
		}
		float basicRate = LevelDesign.Instance.basicWeaponOption[curWeaponNbr].basicRate;
		float num5 = num2 / (float)num4;
		if (float.IsNaN(num5) || curWeaponNbr == 0)
		{
			num5 = 0f;
		}
		DataManager.Instance.gamePrefs.weaponData[curWeaponNbr].totalDamage = num;
		DataManager.Instance.gamePrefs.weaponData[curWeaponNbr].totalRateOfFire = 1f / ((basicRate + num5) / 2f);
		DataManager.Instance.gamePrefs.weaponData[curWeaponNbr].totalBulletPerClip = num3;
		DataManager.Instance.gamePrefs.weaponData[curWeaponNbr].totalBlockAmt = num4;
		for (int j = 0; j < itemRoot[curWeaponNbr].childCount; j++)
		{
			UnityEngine.Object.Destroy(itemRoot[curWeaponNbr].GetChild(j).gameObject);
		}
		weapons[curWeaponNbr].SetActive(false);
		GoPanel("Menu");
		bAmt.Clear();
		DataManager.Instance.SaveData();
	}

	private void LoadData()
	{
		Debug.Log("Load Data");
		BlockInfo blockInfo = new BlockInfo();
		List<BlockInfo> weaponList = GetWeaponList(curWeaponNbr);
		for (int i = 0; i < weaponList.Count; i++)
		{
			blockInfo = weaponList[i];
			int num = int.Parse(blockInfo.blockName.Substring(0, 2));
			GameObject gameObject = UnityEngine.Object.Instantiate(position: new Vector3(blockInfo.blockPosX, blockInfo.blockPosY, blockInfo.blockPosZ), original: BlockObjects.Instance.blockObject[num], rotation: Quaternion.identity) as GameObject;
			gameObject.transform.parent = itemRoot[curWeaponNbr];
		}
	}

	private List<BlockInfo> GetWeaponList(int num)
	{
		List<BlockInfo> result = new List<BlockInfo>();
		switch (num)
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

	private void SelectWeaponBuild(int num)
	{
		GoPanel("Build");
		curWeaponNbr = num;
		GameObject[] array = weapons;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		weapons[num].SetActive(true);
		AddButton();
		isUpdate = false;
		LoadData();
		SelectWeaponBlock(0);
		UpdateBlockDataLabel();
		StartCoroutine(UpdateWeaponIndicator());
		if (num == 0)
		{
			curWeaponIndicator.curWeaponLabel[2].color = new Color(1f, 0.784f, 0f, 1f);
		}
		else
		{
			curWeaponIndicator.curWeaponLabel[2].color = new Color(1f, 1f, 1f, 1f);
		}
	}

	private void AddButton()
	{
		if (curWeaponNbr == 1)
		{
			GameObject[] array = weapon2Buttons;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(true);
			}
		}
		else
		{
			GameObject[] array2 = weapon2Buttons;
			foreach (GameObject gameObject2 in array2)
			{
				gameObject2.SetActive(false);
			}
		}
		if (curWeaponNbr == 2)
		{
			GameObject[] array3 = weapon3Buttons;
			foreach (GameObject gameObject3 in array3)
			{
				gameObject3.SetActive(true);
			}
		}
		else
		{
			GameObject[] array4 = weapon3Buttons;
			foreach (GameObject gameObject4 in array4)
			{
				gameObject4.SetActive(false);
			}
		}
	}

	private void SelectWeaponBlock(int num)
	{
		if (num == 99)
		{
			delete = true;
		}
		else
		{
			delete = false;
			curBlockNbr = num;
		}
		UpdateSelectBlockIndicator(num);
		SelectLayout(num);
	}

	private void SelectLayout(int num)
	{
		Vector2 vector = Vector2.zero;
		switch (num)
		{
		case 0:
			vector = new Vector2(-330f, 0f);
			break;
		case 1:
			vector = new Vector2(-264f, 0f);
			break;
		case 2:
			vector = new Vector2(-198f, 0f);
			break;
		case 3:
			vector = new Vector2(-132f, 0f);
			break;
		case 4:
			vector = new Vector2(-66f, 0f);
			break;
		case 5:
			vector = new Vector2(0f, 0f);
			break;
		case 6:
			vector = new Vector2(66f, 0f);
			break;
		case 7:
			vector = new Vector2(132f, 0f);
			break;
		case 8:
			vector = new Vector2(198f, 0f);
			break;
		case 9:
			vector = new Vector2(264f, 0f);
			break;
		case 10:
			vector = new Vector2(464f, 64f);
			break;
		case 11:
			vector = new Vector2(464f, 0f);
			break;
		case 12:
			vector = new Vector2(464f, 64f);
			break;
		case 13:
			vector = new Vector2(464f, 0f);
			break;
		case 99:
			vector = new Vector2(330f, 0f);
			break;
		}
		selectLight.localPosition = vector;
	}

	public IEnumerator UpdateWeaponIndicator()
	{
		yield return new WaitForEndOfFrame();
		Debug.Log("Update Weapon Indicator");
		float damage = 0f + LevelDesign.Instance.basicWeaponOption[curWeaponNbr].basicDamage;
		float rate = 0f;
		int bullet = 0 + LevelDesign.Instance.basicWeaponOption[curWeaponNbr].basicBullet;
		int blockCnt = 0;
		curBP = 0;
		Block[] componentsInChildren = itemRoot[curWeaponNbr].GetComponentsInChildren<Block>();
		foreach (Block block in componentsInChildren)
		{
			damage += block.damage;
			rate += block.rate;
			bullet += block.bullet;
			blockCnt++;
			curBP += block.cost;
		}
		float basicRate = LevelDesign.Instance.basicWeaponOption[curWeaponNbr].basicRate;
		float blockRate = rate / (float)blockCnt;
		if (float.IsNaN(blockRate) || curWeaponNbr == 0)
		{
			blockRate = 0f;
		}
		curWeaponIndicator.curWeaponLabel[1].text = damage.ToString("f2");
		curWeaponIndicator.curWeaponLabel[2].text = ((basicRate + blockRate) / 2f).ToString("f2");
		if (curWeaponNbr == 0)
		{
			curWeaponIndicator.curWeaponLabel[3].text = "  ●";
		}
		else
		{
			curWeaponIndicator.curWeaponLabel[3].text = bullet.ToString();
		}
		UpdateBlockDataLabel();
		UpdateBlockPointLabel();
	}

	private void UpdateSelectBlockIndicator(int num)
	{
		Debug.Log("Update SelectBlock Indicator");
		string spriteName = BlockObjects.Instance.BlockSpriteName(num);
		selectBlockIndocator.selectBlockSprite.spriteName = spriteName;
		if (num >= 10)
		{
			for (int i = 0; i < 4; i++)
			{
				selectBlockIndocator.selectBlockLabel[i].text = null;
			}
		}
		else
		{
			Block component = BlockObjects.Instance.blockObject[num].GetComponent<Block>();
			selectBlockIndocator.selectBlockLabel[0].text = ":" + component.damage;
			selectBlockIndocator.selectBlockLabel[1].text = ":" + component.rate;
			selectBlockIndocator.selectBlockLabel[2].text = ":" + component.bullet;
			selectBlockIndocator.selectBlockLabel[3].text = ":" + component.cost;
		}
	}

	private void InitPanelList()
	{
		panelList.Add("Menu", GameObject.Find("Panel_Menu"));
		panelList.Add("Build", GameObject.Find("Panel_Build"));
		panelList.Add("Save", GameObject.Find("Panel_Save"));
		panelList["Build"].SetActive(false);
		panelList["Save"].SetActive(false);
	}

	private IEnumerator OverFlowBlockPoint()
	{
		GameObject.Find("BackButton").GetComponent<BoxCollider>().enabled = false;
		touchLook.enabled = false;
		touchBuilder.enabled = false;
		for (int i = 0; i < 2; i++)
		{
			highLight.enabled = true;
			warningLabel.enabled = true;
			yield return new WaitForSeconds(0.3f);
			highLight.enabled = false;
			warningLabel.enabled = false;
			yield return new WaitForSeconds(0.3f);
		}
		GameObject.Find("BackButton").GetComponent<BoxCollider>().enabled = true;
		touchLook.enabled = true;
		touchBuilder.enabled = true;
	}

	private void GoScene(string name)
	{
		Debug.Log("Go Scene Name : " + name);
		Application.LoadLevel(name);
	}

	private void GoPanel(string name)
	{
		foreach (KeyValuePair<string, GameObject> panel in panelList)
		{
			if (panel.Key == name)
			{
				panel.Value.SetActive(true);
			}
			else
			{
				panel.Value.SetActive(false);
			}
		}
		if (name == "Build")
		{
			touchLook.enabled = true;
			touchBuilder.enabled = true;
		}
		else
		{
			touchLook.enabled = false;
			touchBuilder.enabled = false;
		}
	}

	private void ShowObject(GameObject target)
	{
		target.SetActive(true);
	}

	private void HideObject(GameObject target)
	{
		target.SetActive(false);
	}

	public void OnGridBuild(Transform hit)
	{
		GameObject gameObject = BlockObjects.Instance.blockObject[curBlockNbr];
		if (curBlockNbr == 10 || curBlockNbr == 12)
		{
			if ((bool)GameObject.FindWithTag("Muzzle"))
			{
				Debug.Log("Already Used");
				return;
			}
		}
		else if (curBlockNbr == 11 || curBlockNbr == 13)
		{
			if ((bool)GameObject.FindWithTag("Sight"))
			{
				Debug.Log("Already Used");
				return;
			}
		}
		else
		{
			if (curBP + gameObject.GetComponent<Block>().cost > maxBP)
			{
				Debug.Log("Overflow BP!");
				StartCoroutine(OverFlowBlockPoint());
				return;
			}
			if (DataManager.Instance.gamePrefs.blockAmt[curBlockNbr] <= 0)
			{
				Debug.Log("Not Enough block : " + curBlockNbr);
				DataManager.Instance.gamePrefs.blockAmt[curBlockNbr] = 0;
				return;
			}
			DataManager.Instance.gamePrefs.blockAmt[curBlockNbr]--;
			BlockAmount blockAmount = new BlockAmount();
			blockAmount.isAdd = true;
			blockAmount.blockNum = curBlockNbr;
			bAmt.Add(blockAmount);
		}
		Debug.Log("On Grid Build");
		isUpdate = true;
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, hit.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity) as GameObject;
		gameObject2.transform.parent = itemRoot[curWeaponNbr];
		gameObject2 = null;
		StartCoroutine(UpdateWeaponIndicator());
	}

	public void OnPartBuild(Transform hit, int meshNum)
	{
		GameObject gameObject = BlockObjects.Instance.blockObject[curBlockNbr];
		if (curBlockNbr == 10 || curBlockNbr == 12)
		{
			if ((bool)GameObject.FindWithTag("Muzzle"))
			{
				Debug.Log("Already Used");
				return;
			}
		}
		else if (curBlockNbr == 11 || curBlockNbr == 13)
		{
			if ((bool)GameObject.FindWithTag("Sight"))
			{
				Debug.Log("Already Used");
				return;
			}
		}
		else
		{
			if (curBP + gameObject.GetComponent<Block>().cost > maxBP)
			{
				Debug.Log("Overflow BP!");
				StartCoroutine(OverFlowBlockPoint());
				return;
			}
			if (DataManager.Instance.gamePrefs.blockAmt[curBlockNbr] <= 0)
			{
				Debug.Log("Not Enough block : " + curBlockNbr);
				DataManager.Instance.gamePrefs.blockAmt[curBlockNbr] = 0;
				return;
			}
			DataManager.Instance.gamePrefs.blockAmt[curBlockNbr]--;
			BlockAmount blockAmount = new BlockAmount();
			blockAmount.isAdd = true;
			blockAmount.blockNum = curBlockNbr;
			bAmt.Add(blockAmount);
		}
		Debug.Log("On Part Build : " + meshNum);
		isUpdate = true;
		switch (meshNum)
		{
		case 0:
		case 1:
		{
			GameObject gameObject7 = UnityEngine.Object.Instantiate(gameObject, hit.transform.position + Vector3.down, Quaternion.identity) as GameObject;
			gameObject7.transform.parent = itemRoot[curWeaponNbr];
			gameObject7 = null;
			break;
		}
		case 2:
		case 3:
		{
			GameObject gameObject6 = UnityEngine.Object.Instantiate(gameObject, hit.transform.position + Vector3.up, Quaternion.identity) as GameObject;
			gameObject6.transform.parent = itemRoot[curWeaponNbr];
			gameObject6 = null;
			break;
		}
		case 4:
		case 5:
		{
			GameObject gameObject5 = UnityEngine.Object.Instantiate(gameObject, hit.transform.position + Vector3.forward, Quaternion.identity) as GameObject;
			gameObject5.transform.parent = itemRoot[curWeaponNbr];
			gameObject5 = null;
			break;
		}
		case 6:
		case 7:
		{
			GameObject gameObject4 = UnityEngine.Object.Instantiate(gameObject, hit.transform.position + Vector3.left, Quaternion.identity) as GameObject;
			gameObject4.transform.parent = itemRoot[curWeaponNbr];
			gameObject4 = null;
			break;
		}
		case 8:
		case 9:
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject, hit.transform.position + Vector3.back, Quaternion.identity) as GameObject;
			gameObject3.transform.parent = itemRoot[curWeaponNbr];
			gameObject3 = null;
			break;
		}
		case 10:
		case 11:
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, hit.transform.position + Vector3.right, Quaternion.identity) as GameObject;
			gameObject2.transform.parent = itemRoot[curWeaponNbr];
			gameObject2 = null;
			break;
		}
		}
		StartCoroutine(UpdateWeaponIndicator());
	}

	public void OnPartDestroy(GameObject target)
	{
		isUpdate = true;
		int num = int.Parse(target.name.Substring(0, 2));
		DataManager.Instance.gamePrefs.blockAmt[num]++;
		BlockAmount blockAmount = new BlockAmount();
		blockAmount.isAdd = false;
		blockAmount.blockNum = num;
		bAmt.Add(blockAmount);
		UnityEngine.Object.Destroy(target);
		StartCoroutine(UpdateWeaponIndicator());
	}

	public void OnDestroySPart(GameObject target)
	{
		isUpdate = true;
		UnityEngine.Object.Destroy(target);
	}
}
