using UnityEngine;

public class InAppManager : MonoBehaviour
{
	private static InAppManager instance;

	[HideInInspector]
	public int buyGoldNbr;

	public static InAppManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.Find("_InAppManager").GetComponent<InAppManager>();
			}
			return instance;
		}
	}

	private void OnApplicationQuit()
	{
		instance = null;
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void RequestProducts()
	{
		string publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAsMpVLMVPbS4z5GaVpJ7wgZTUrEEmic9Rmz6rWc9ammDYg72kFVDBfLHCnCV54RbP0oyy++A5SJM0plQvk2MdUtX7ZXYc2+PHumCMGb47MTedO3tReL+8jv1rH4rYWSsCSoAxZqnmNM9t6egL4lvL8+QBVOT2zZ7XjuhlO4mIF/2039nNIh4ZinxJ6HCeEhX9cb44kwAFOE294Lvl7i55A8iyQcYi4itrRVN4WZaGmAHCSsU8bWcSs8H94izrEx7/z/H3FdKDy1x4piVzvpki7/OPsf5dh4usVz0qCbqK5niaWkp/MnjU6JIXRtlNUii61uGeEM816ePCXYSiIIjcGwIDAQAB";
		GoogleIAB.init(publicKey);
	}

	public void PurchaseGold(int num)
	{
		buyGoldNbr = num;
		switch (num)
		{
		case 0:
			if (DataManager.Instance.gamePrefs.cash >= 5)
			{
				EtceteraAndroid.showAlert("Do you want to buy Gold?", "1000 Gold", "Cancel", "Buy");
			}
			else
			{
				EtceteraAndroid.showAlert("Not enough the Gem", string.Empty, "OK");
			}
			break;
		case 1:
			if (DataManager.Instance.gamePrefs.cash >= 25)
			{
				EtceteraAndroid.showAlert("Do you want to buy Gold?", "5000 Gold", "Cancel", "Buy");
			}
			else
			{
				EtceteraAndroid.showAlert("Not enough the Gem", string.Empty, "OK");
			}
			break;
		case 2:
			if (DataManager.Instance.gamePrefs.cash >= 50)
			{
				EtceteraAndroid.showAlert("Do you want to buy Gold?", "10000 Gold", "Cancel", "Buy");
			}
			else
			{
				EtceteraAndroid.showAlert("Not enough the Gem", string.Empty, "OK");
			}
			break;
		case 3:
			if (DataManager.Instance.gamePrefs.cash >= 100)
			{
				EtceteraAndroid.showAlert("Do you want to buy Gold?", "20000 Gold", "Cancel", "Buy");
			}
			else
			{
				EtceteraAndroid.showAlert("Not enough the Gem", string.Empty, "OK");
			}
			break;
		case 4:
			if (DataManager.Instance.gamePrefs.cash >= 250)
			{
				EtceteraAndroid.showAlert("Do you want to buy Gold?", "50000 Gold", "Cancel", "Buy");
			}
			else
			{
				EtceteraAndroid.showAlert("Not enough the Gem", string.Empty, "OK");
			}
			break;
		}
	}

	public void PurchaseCash(int num)
	{
		switch (num)
		{
		case 0:
			GoogleIAB.purchaseProduct("gem_10");
			break;
		case 1:
			GoogleIAB.purchaseProduct("gem_50");
			break;
		case 2:
			GoogleIAB.purchaseProduct("gem_100");
			break;
		case 3:
			GoogleIAB.purchaseProduct("gem_200");
			break;
		case 4:
			GoogleIAB.purchaseProduct("gem_500");
			break;
		}
	}
}
