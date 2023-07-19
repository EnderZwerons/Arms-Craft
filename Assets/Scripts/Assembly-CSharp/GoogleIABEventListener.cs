using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class GoogleIABEventListener : MonoBehaviour
{
	private void OnEnable()
	{
		GoogleIABManager.billingSupportedEvent += billingSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent += billingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
		GoogleIABManager.purchaseSucceededEvent += purchaseSucceededEvent;
		GoogleIABManager.purchaseFailedEvent += purchaseFailedEvent;
		GoogleIABManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
	}

	private void OnDisable()
	{
		GoogleIABManager.billingSupportedEvent -= billingSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent -= billingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
		GoogleIABManager.purchaseSucceededEvent -= purchaseSucceededEvent;
		GoogleIABManager.purchaseFailedEvent -= purchaseFailedEvent;
		GoogleIABManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
	}

	private void billingSupportedEvent()
	{
		Debug.Log("billingSupportedEvent");
		string[] skus = new string[5] { "gem_10", "gem_50", "gem_100", "gem_200", "gem_500" };
		GoogleIAB.queryInventory(skus);
	}

	private void billingNotSupportedEvent(string error)
	{
		Debug.Log("billingNotSupportedEvent: " + error);
		BaasIO component = GameObject.Find("_BaasIO").GetComponent<BaasIO>();
		component.isConnect = true;
		component.AlertLayer.SetActive(true);
		component.ConnectLayer.SetActive(false);
	}

	private void queryInventorySucceededEvent(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus)
	{
		Debug.Log(string.Format("queryInventorySucceededEvent. total purchases: {0}, total skus: {1}", purchases.Count, skus.Count));
		Utils.logObject(purchases);
		Utils.logObject(skus);
		BootManager component = GameObject.Find("_BootManager").GetComponent<BootManager>();
	}

	private void queryInventoryFailedEvent(string error)
	{
		Debug.Log("queryInventoryFailedEvent: " + error);
		BaasIO component = GameObject.Find("_BaasIO").GetComponent<BaasIO>();
		component.isConnect = true;
		component.AlertLayer.SetActive(true);
		component.ConnectLayer.SetActive(false);
	}

	private void purchaseCompleteAwaitingVerificationEvent(string purchaseData, string signature)
	{
		Debug.Log("purchaseCompleteAwaitingVerificationEvent. purchaseData: " + purchaseData + ", signature: " + signature);
	}

	private void purchaseSucceededEvent(GooglePurchase purchase)
	{
		Debug.Log("purchaseSucceededEvent: " + purchase);
		GoogleIAB.consumeProduct(purchase.productId);
	}

	private void purchaseFailedEvent(string error)
	{
		Debug.Log("purchaseFailedEvent: " + error);
	}

	private void consumePurchaseSucceededEvent(GooglePurchase purchase)
	{
		Debug.Log("consumePurchaseSucceededEvent: " + purchase);
		if (!DataManager.Instance.gamePrefs.isNoAds)
		{
			DataManager.Instance.gamePrefs.isNoAds = true;
			if ((bool)GameObject.Find("_MainManager"))
			{
				MainManager component = GameObject.Find("_MainManager").GetComponent<MainManager>();
				component.noAdsLabel.enabled = false;
			}
			if ((bool)GameObject.Find("_BuildManager"))
			{
				BuildManager component2 = GameObject.Find("_BuildManager").GetComponent<BuildManager>();
				component2.noAdsLabel.enabled = false;
			}
		}
		string positiveButton = "OK";
		switch (purchase.productId)
		{
		case "gem_10":
			DataManager.Instance.gamePrefs.cash += 10;
			DataManager.Instance.SaveData();
			EtceteraAndroid.showAlert("Purchase Successful", "10 Gem", positiveButton);
			break;
		case "gem_50":
			DataManager.Instance.gamePrefs.cash += 60;
			DataManager.Instance.SaveData();
			EtceteraAndroid.showAlert("Purchase Successful", "60 Gem", positiveButton);
			break;
		case "gem_100":
			DataManager.Instance.gamePrefs.cash += 125;
			DataManager.Instance.SaveData();
			EtceteraAndroid.showAlert("Purchase Successful", "125 Gem", positiveButton);
			break;
		case "gem_200":
			DataManager.Instance.gamePrefs.cash += 260;
			DataManager.Instance.SaveData();
			EtceteraAndroid.showAlert("Purchase Successful", "260 Gem", positiveButton);
			break;
		case "gem_500":
			DataManager.Instance.gamePrefs.cash += 700;
			DataManager.Instance.SaveData();
			EtceteraAndroid.showAlert("Purchase Successful", "700 Gem", positiveButton);
			break;
		}
		if (Application.loadedLevelName == "GameMain")
		{
			MainManager component3 = GameObject.Find("_MainManager").GetComponent<MainManager>();
			component3.SendMessage("UpdateWalletLabel");
		}
		else if (Application.loadedLevelName.Substring(0, 8) == "GamePlay")
		{
			GameManager component4 = GameObject.Find("_GameManager").GetComponent<GameManager>();
			component4.SendMessage("UpdateWalletLabel");
		}
		else if (Application.loadedLevelName == "GameBuild")
		{
			UIManager component5 = GameObject.Find("_UIManager").GetComponent<UIManager>();
			component5.SendMessage("UpdateWalletLabel");
		}
	}

	private void consumePurchaseFailedEvent(string error)
	{
		Debug.Log("consumePurchaseFailedEvent: " + error);
	}
}
