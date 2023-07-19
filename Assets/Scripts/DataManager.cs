using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataManager : MonoBehaviour
{
	[Serializable]
	public class GamePrefs
	{
		public bool sndBgm = true;

		public bool sndEffect = true;

		public float sensitivity = 2f;

		public int gold = 1000;

		public int cash = 50;

		public bool isNoAds;

		public int level = 1;

		public int exp;

		public int killCnt;

		public int headCnt;

		public int meleeCnt;

		public long playTime;

		public int useBullets;

		public int hitBullets;

		public int hitPlayer;

		public int downPlayer;

		public int revivalPlayer;

		public int upgradeUpper = 1;

		public int upgradeLower = 1;

		public int[] blockAmt;

		public int[] blockMaterial;

		public List<BlockInfo> weapon0 = new List<BlockInfo>();

		public List<BlockInfo> weapon1 = new List<BlockInfo>();

		public List<BlockInfo> weapon2 = new List<BlockInfo>();

		public WeaponData[] weaponData;

		public LabData[] labData;

		public Energy energy;

		public bool stageViewTutorial;
	}

	private static DataManager instance;

	public GamePrefs gamePrefs = new GamePrefs();

	public long serverTimeStamp;

	public float intervalTime;

	public int blockAmtLength = 10;

	public int blockMaterialLength = 15;

	public int labDataLength = 3;

	public int weaponDataLength = 3;

	public bool isStarter;

	private bool isLoading;

	public static DataManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.Find("_DataManager").GetComponent<DataManager>();
			}
			return instance;
		}
	}

	private void OnApplicationQuit()
	{
		if (isLoading)
		{
			instance = null;
			SaveData();
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void SaveData()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = File.Create(Application.persistentDataPath + "/GameData.bin");
		binaryFormatter.Serialize(fileStream, gamePrefs);
		fileStream.Close();
	}

	public void LoadData()
	{
		isLoading = true;
		if (File.Exists(Application.persistentDataPath + "/GameData.bin"))
		{
			isStarter = false;
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Open(Application.persistentDataPath + "/GameData.bin", FileMode.Open);
			gamePrefs = (GamePrefs)binaryFormatter.Deserialize(fileStream);
			fileStream.Close();
			if (gamePrefs.blockAmt.Length != blockAmtLength)
			{
				int[] blockAmt = gamePrefs.blockAmt;
				gamePrefs.blockAmt = new int[blockAmtLength];
				for (int i = 0; i < blockAmt.Length; i++)
				{
					gamePrefs.blockAmt[i] = blockAmt[i];
				}
			}
			if (gamePrefs.blockMaterial.Length != blockMaterialLength)
			{
				int[] blockMaterial = gamePrefs.blockMaterial;
				gamePrefs.blockMaterial = new int[blockMaterialLength];
				for (int j = 0; j < blockMaterial.Length; j++)
				{
					gamePrefs.blockMaterial[j] = blockMaterial[j];
				}
			}
			if (gamePrefs.labData.Length != labDataLength)
			{
				LabData[] labData = gamePrefs.labData;
				gamePrefs.labData = new LabData[labDataLength];
				for (int k = 0; k < labData.Length; k++)
				{
					gamePrefs.labData[k] = labData[k];
				}
			}
			if (gamePrefs.weaponData.Length != weaponDataLength)
			{
				WeaponData[] weaponData = gamePrefs.weaponData;
				gamePrefs.weaponData = new WeaponData[weaponDataLength];
				for (int l = 0; l < weaponData.Length; l++)
				{
					gamePrefs.weaponData[l] = weaponData[l];
				}
			}
		}
		else
		{
			isStarter = true;
			//PlayerPrefs.SetInt("autoFire", 1);
			gamePrefs.blockAmt = new int[blockAmtLength];
			gamePrefs.blockMaterial = new int[blockMaterialLength];
			gamePrefs.labData = new LabData[labDataLength];
			gamePrefs.weaponData = new WeaponData[weaponDataLength];
			gamePrefs.sensitivity = 4f;
			gamePrefs.weapon0.Add(SetWeaponBlock("00", 0f, 0f, 0f));
			gamePrefs.weapon0.Add(SetWeaponBlock("00", 0f, 1f, 0f));
			gamePrefs.weapon0.Add(SetWeaponBlock("00", 0f, 2f, 0f));
			gamePrefs.weapon0.Add(SetWeaponBlock("00", 0f, 3f, 0f));
			gamePrefs.weapon1.Add(SetWeaponBlock("00", 0f, 0f, 0f));
			gamePrefs.weapon1.Add(SetWeaponBlock("00", 0f, 0f, -1f));
			gamePrefs.weapon1.Add(SetWeaponBlock("10", 0f, 0f, 1f));
			gamePrefs.weapon1.Add(SetWeaponBlock("11", 0f, 1f, 0f));
			gamePrefs.weapon2.Add(SetWeaponBlock("00", 0f, 0f, 1f));
			gamePrefs.weapon2.Add(SetWeaponBlock("00", 0f, 0f, 0f));
			gamePrefs.weapon2.Add(SetWeaponBlock("00", 0f, 0f, -1f));
			gamePrefs.weapon2.Add(SetWeaponBlock("12", 0f, 0f, 2f));
			gamePrefs.weapon2.Add(SetWeaponBlock("13", 0f, 1f, 1f));
			gamePrefs.weaponData[0] = SetWeaponData(172f, 2f / 3f, 0, 4);
			gamePrefs.weaponData[1] = SetWeaponData(86f, 0.3333333f, 10, 2);
			gamePrefs.weaponData[2] = SetWeaponData(29f, 0.1666667f, 40, 3);
			gamePrefs.labData[0] = SetLabData(true, false, false, 0, 0L);
			gamePrefs.labData[1] = SetLabData(false, false, false, 0, 0L);
			gamePrefs.labData[2] = SetLabData(false, false, false, 0, 0L);
		}
	}

	private BlockInfo SetWeaponBlock(string name, float x, float y, float z)
	{
		BlockInfo blockInfo = new BlockInfo();
		blockInfo.blockName = name;
		blockInfo.blockPosX = x;
		blockInfo.blockPosY = y;
		blockInfo.blockPosZ = z;
		return blockInfo;
	}

	private WeaponData SetWeaponData(float tDamage, float tRate, int tBullet, int tAmt)
	{
		WeaponData weaponData = new WeaponData();
		weaponData.totalDamage = tDamage;
		weaponData.totalRateOfFire = tRate;
		weaponData.totalBulletPerClip = tBullet;
		weaponData.totalBlockAmt = tAmt;
		return weaponData;
	}

	private LabData SetLabData(bool _isOpen, bool _isUse, bool _isDone, int _blockNum, long _beganTimeStamp)
	{
		LabData labData = new LabData();
		labData.isOpen = _isOpen;
		labData.isUse = _isUse;
		labData.isDone = _isDone;
		labData.blockNum = _blockNum;
		labData.beganTimeStamp = _beganTimeStamp;
		return labData;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (isLoading && pauseStatus && instance != null)
		{
			SaveData();
		}
	}
}
