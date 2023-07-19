using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabManager : MonoBehaviour
{
	[Serializable]
	private struct ConsumeMaterial
	{
		public int materialNumber;

		public int materialAmount;
	}

	private static LabManager instance;

	public UISprite selectBlockTextures;

	public UILabel selectBlockPrice;

	public UILabel selectBlockTime;

	public Transform selectButton;

	public UILabel selectBlockName;

	public UILabel[] blockMaterialLabel = new UILabel[3];

	public UISprite[] blockMaterialSprite = new UISprite[3];

	public UISprite[] selectLight = new UISprite[3];

	public UISprite labAnimSprite;

	public GameObject[] labState = new GameObject[3];

	private int slotNbr = -1;

	private int blockNbr;

	private bool isPickup;

	public UILabel[] stuffLabel = new UILabel[15];

	private LabData curLabData = new LabData();

	private List<ConsumeMaterial> consumeMaterial = new List<ConsumeMaterial>();

	public GameObject[] unLockObject;

	public static LabManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.Find("_LabManager").GetComponent<LabManager>();
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

	private void Start()
	{
		UpdateStuffLabel();
		UpdateLabUnlockSprite();
	}

	public void InitLabManager(bool isEnable)
	{
		if (isEnable)
		{
			SelectSlot(0);
			UpdateLabState();
		}
		else
		{
			slotNbr = -1;
			StopCoroutine("UpdatePerSecond");
		}
	}

	private void Update()
	{
		for (int i = 0; i < 3; i++)
		{
			if (DataManager.Instance.gamePrefs.labData[i].isUse)
			{
				long num = RealTimeStamp.Instance.timeStamp - DataManager.Instance.gamePrefs.labData[i].beganTimeStamp;
				//change this to createTime if you like waiting 24 hours 
				long num2 = LevelDesign.Instance.blockMaterial[DataManager.Instance.gamePrefs.labData[i].blockNum].CreateTime - num;
				if (num2 >= 0)
				{
					labState[i].transform.Find("Label_Timer").GetComponent<UILabel>().text = ConvertTime(num2);
				}
				else
				{
					labState[i].transform.Find("Label_Timer").GetComponent<UILabel>().text = "DONE!";
				}
			}
		}
	}

	public void UpdateStuffLabel()
	{
		for (int i = 0; i < 15; i++)
		{
			stuffLabel[i].text = DataManager.Instance.gamePrefs.blockMaterial[i].ToString();
		}
	}

	private void UpdateLabState()
	{
		for (int i = 0; i < 3; i++)
		{
			if (DataManager.Instance.gamePrefs.labData[i].isUse)
			{
				labState[i].transform.Find("Sprite").GetComponent<UISprite>().enabled = true;
				labState[i].transform.Find("Sprite").GetComponent<UISprite>().spriteName = "b0" + (DataManager.Instance.gamePrefs.labData[i].blockNum + 1).ToString("d2");
				labState[i].transform.Find("Label").GetComponent<UILabel>().enabled = false;
				labState[i].transform.Find("Label_Timer").GetComponent<UILabel>().enabled = true;
			}
			else
			{
				labState[i].transform.Find("Sprite").GetComponent<UISprite>().enabled = false;
				labState[i].transform.Find("Label").GetComponent<UILabel>().enabled = true;
				labState[i].transform.Find("Label_Timer").GetComponent<UILabel>().enabled = false;
			}
		}
	}

	private void UpdateLabUnlockSprite()
	{
		if (DataManager.Instance.gamePrefs.labData[1].isOpen)
		{
			unLockObject[0].SetActive(false);
		}
		if (DataManager.Instance.gamePrefs.labData[2].isOpen)
		{
			unLockObject[1].SetActive(false);
		}
	}

	private void SelectSlot(int slotNum)
	{
		if (!DataManager.Instance.gamePrefs.labData[slotNum].isOpen)
		{
			switch (slotNum)
			{
			case 1:
				if (DataManager.Instance.gamePrefs.cash >= 20)
				{
					DataManager.Instance.gamePrefs.cash -= 20;
					DataManager.Instance.gamePrefs.labData[1].isOpen = true;
				}
				else
				{
					//EtceteraAndroid.showAlert("Not enough the Gem", string.Empty, "OK");
				}
				break;
			case 2:
				if (DataManager.Instance.gamePrefs.cash >= 100)
				{
					DataManager.Instance.gamePrefs.cash -= 100;
					DataManager.Instance.gamePrefs.labData[2].isOpen = true;
				}
				else
				{
					//EtceteraAndroid.showAlert("Not enough the Gem", string.Empty, "OK");
				}
				break;
			}
			DataManager.Instance.SaveData();
			MainManager component = GameObject.Find("_MainManager").GetComponent<MainManager>();
			component.SendMessage("UpdateWalletLabel");
			UpdateLabUnlockSprite();
		}
		else
		{
			if (slotNbr == slotNum)
			{
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				if (i == slotNum)
				{
					selectLight[i].enabled = true;
				}
				else
				{
					selectLight[i].enabled = false;
				}
			}
			slotNbr = slotNum;
			isPickup = false;
			StopCoroutine("UpdatePerSecond");
			curLabData = DataManager.Instance.gamePrefs.labData[slotNbr];
			labAnimSprite.spriteName = "lab_ing0";
			if (curLabData.isUse)
			{
				int blockNum = curLabData.blockNum;
				BlockMaterial blockMaterial = LevelDesign.Instance.blockMaterial[blockNum];
				UpdateLabData(true, blockMaterial);
				StartCoroutine("UpdatePerSecond");
			}
			else
			{
				UpdateLabData(false, null);
			}
		}
	}

	private void SelectBlock(int blockNum)
	{
		if (curLabData.isUse)
		{
			Debug.Log("Already used");
			return;
		}
		Debug.Log("Select Block : " + blockNum);
		blockNbr = blockNum;
		isPickup = true;
		BlockMaterial blockMaterial = LevelDesign.Instance.blockMaterial[blockNbr];
		UpdateBoardView(false, blockMaterial);
	}

	private IEnumerator UpdatePerSecond()
	{
		int a = 1;
		selectButton.GetComponent<UIButton>().isEnabled = false;
		selectBlockPrice.text = "WAIT";
		while (true)
		{
			if (curLabData.isUse)
			{
				long elapsedTime = RealTimeStamp.Instance.timeStamp - curLabData.beganTimeStamp;
				//change this to createTime if you like waiting 24 hours
				long lateTime = LevelDesign.Instance.blockMaterial[curLabData.blockNum].CreateTime - elapsedTime;
				if (lateTime < 0)
				{
					break;
				}
				selectBlockTime.text = ConvertTime(lateTime);
				if (1 <= a && a <= 3)
				{
					labAnimSprite.spriteName = "lab_ing1";
				}
				else if (4 <= a && a <= 6)
				{
					labAnimSprite.spriteName = "lab_ing2";
				}
				else if (7 <= a && a <= 9)
				{
					labAnimSprite.spriteName = "lab_ing3";
				}
				a++;
				if (a > 9)
				{
					a = 1;
				}
			}
			yield return new WaitForSeconds(0.0333f);
		}
		DataManager.Instance.gamePrefs.labData[slotNbr].isDone = true;
		selectButton.GetComponent<UIButton>().isEnabled = true;
		selectBlockTime.text = "00:00:00";
		selectBlockPrice.text = "DONE!";
		labAnimSprite.spriteName = "lab_ing0";
	}

	private void UpdateLabData(bool isUse, BlockMaterial blockMaterial)
	{
		if (isUse)
		{
			UpdateBoardView(true, blockMaterial);
			return;
		}
		selectButton.GetComponent<UIButton>().isEnabled = false;
		selectBlockTextures.spriteName = null;
		selectBlockPrice.text = "EMPTY";
		selectBlockTime.text = string.Empty;
		selectBlockName.text = string.Empty;
		for (int i = 0; i < 3; i++)
		{
			blockMaterialLabel[i].text = string.Empty;
			blockMaterialSprite[i].spriteName = null;
		}
	}

	private void UpdateBoardView(bool isSlot, BlockMaterial blockMaterial)
	{
		if (isSlot)
		{
			Debug.Log("Update Board View : SlotView");
			int num = blockMaterial.blockMaterialInfo.Length;
			for (int i = 0; i < 3; i++)
			{
				blockMaterialSprite[i].spriteName = null;
				blockMaterialLabel[i].text = string.Empty;
			}
			for (int j = 0; j < num; j++)
			{
				blockMaterialSprite[j].spriteName = blockMaterial.blockMaterialInfo[j].materialName;
				blockMaterialSprite[j].MakePixelPerfect();
				blockMaterialLabel[j].text = blockMaterial.blockMaterialInfo[j].materialAmount + "/" + blockMaterial.blockMaterialInfo[j].materialAmount;
			}
			selectBlockName.text = BlockObjects.Instance.blockObject[curLabData.blockNum].GetComponent<Block>().names;
			selectBlockTextures.spriteName = BlockObjects.Instance.BlockSpriteName(curLabData.blockNum);
			return;
		}
		Debug.Log("Update Board View : SelectBlockView");
		int[] blockMaterial2 = DataManager.Instance.gamePrefs.blockMaterial;
		int num2 = blockMaterial.blockMaterialInfo.Length;
		bool isEnabled = true;
		consumeMaterial.Clear();
		for (int k = 0; k < 3; k++)
		{
			blockMaterialSprite[k].spriteName = null;
			blockMaterialLabel[k].text = string.Empty;
		}
		for (int l = 0; l < num2; l++)
		{
			blockMaterialSprite[l].spriteName = blockMaterial.blockMaterialInfo[l].materialName;
			blockMaterialSprite[l].MakePixelPerfect();
			int num3 = int.Parse(blockMaterial.blockMaterialInfo[l].materialName.Substring(1, 3)) - 1;
			blockMaterialLabel[l].text = blockMaterial2[num3] + "/" + blockMaterial.blockMaterialInfo[l].materialAmount;
			ConsumeMaterial item = default(ConsumeMaterial);
			item.materialNumber = num3;
			item.materialAmount = blockMaterial.blockMaterialInfo[l].materialAmount;
			consumeMaterial.Add(item);
			if (blockMaterial2[num3] < blockMaterial.blockMaterialInfo[l].materialAmount)
			{
				isEnabled = false;
			}
		}
		selectBlockName.text = BlockObjects.Instance.blockObject[blockNbr].GetComponent<Block>().names;
		selectBlockTextures.spriteName = BlockObjects.Instance.BlockSpriteName(blockNbr);
		//change this to createTime if you like waiting 24 hours
		selectBlockTime.text = ConvertTime(blockMaterial.CreateTime);
		selectBlockPrice.text = "BUILD";
		selectButton.GetComponent<UIButton>().isEnabled = isEnabled;
	}

	private void CreateBlock()
	{
		if (curLabData.isUse && !curLabData.isDone)
		{
			Debug.Log("Building...");
			return;
		}
		if (curLabData.isUse && curLabData.isDone)
		{
			GetBlock();
			return;
		}
		if (!isPickup)
		{
			Debug.Log("is Pickup : " + isPickup);
			return;
		}
		DataManager.Instance.gamePrefs.labData[slotNbr].beganTimeStamp = RealTimeStamp.Instance.timeStamp;
		DataManager.Instance.gamePrefs.labData[slotNbr].blockNum = blockNbr;
		DataManager.Instance.gamePrefs.labData[slotNbr].isUse = true;
		for (int i = 0; i < consumeMaterial.Count; i++)
		{
			int materialNumber = consumeMaterial[i].materialNumber;
			int materialAmount = consumeMaterial[i].materialAmount;
			DataManager.Instance.gamePrefs.blockMaterial[materialNumber] -= materialAmount;
		}
		int blockNum = curLabData.blockNum;
		BlockMaterial blockMaterial = LevelDesign.Instance.blockMaterial[blockNum];
		UpdateLabData(true, blockMaterial);
		StartCoroutine("UpdatePerSecond");
		UpdateLabState();
	}

	private void GetBlock()
	{
		int blockNum = DataManager.Instance.gamePrefs.labData[slotNbr].blockNum;
		Debug.Log("Get Block : " + blockNum);
		DataManager.Instance.gamePrefs.blockAmt[blockNum]++;
		DataManager.Instance.gamePrefs.labData[slotNbr].beganTimeStamp = 0L;
		DataManager.Instance.gamePrefs.labData[slotNbr].blockNum = 0;
		DataManager.Instance.gamePrefs.labData[slotNbr].isUse = false;
		DataManager.Instance.gamePrefs.labData[slotNbr].isDone = false;
		isPickup = false;
		selectButton.GetComponent<UIButton>().isEnabled = false;
		selectBlockTextures.spriteName = null;
		selectBlockPrice.text = "EMPTY";
		selectBlockTime.text = string.Empty;
		selectBlockName.text = string.Empty;
		for (int i = 0; i < 3; i++)
		{
			blockMaterialLabel[i].text = string.Empty;
			blockMaterialSprite[i].spriteName = null;
		}
		UpdateLabState();
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
