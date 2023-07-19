using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class EtceteraAndroidEventListener : MonoBehaviour
{
	private void OnEnable()
	{
		EtceteraAndroidManager.alertButtonClickedEvent += alertButtonClickedEvent;
		EtceteraAndroidManager.alertCancelledEvent += alertCancelledEvent;
		EtceteraAndroidManager.promptFinishedWithTextEvent += promptFinishedWithTextEvent;
		EtceteraAndroidManager.promptCancelledEvent += promptCancelledEvent;
		EtceteraAndroidManager.twoFieldPromptFinishedWithTextEvent += twoFieldPromptFinishedWithTextEvent;
		EtceteraAndroidManager.twoFieldPromptCancelledEvent += twoFieldPromptCancelledEvent;
		EtceteraAndroidManager.webViewCancelledEvent += webViewCancelledEvent;
		EtceteraAndroidManager.inlineWebViewJSCallbackEvent += inlineWebViewJSCallbackEvent;
		EtceteraAndroidManager.albumChooserCancelledEvent += albumChooserCancelledEvent;
		EtceteraAndroidManager.albumChooserSucceededEvent += albumChooserSucceededEvent;
		EtceteraAndroidManager.photoChooserCancelledEvent += photoChooserCancelledEvent;
		EtceteraAndroidManager.photoChooserSucceededEvent += photoChooserSucceededEvent;
		EtceteraAndroidManager.videoRecordingCancelledEvent += videoRecordingCancelledEvent;
		EtceteraAndroidManager.videoRecordingSucceededEvent += videoRecordingSucceededEvent;
		EtceteraAndroidManager.ttsInitializedEvent += ttsInitializedEvent;
		EtceteraAndroidManager.ttsFailedToInitializeEvent += ttsFailedToInitializeEvent;
		EtceteraAndroidManager.askForReviewDontAskAgainEvent += askForReviewDontAskAgainEvent;
		EtceteraAndroidManager.askForReviewRemindMeLaterEvent += askForReviewRemindMeLaterEvent;
		EtceteraAndroidManager.askForReviewWillOpenMarketEvent += askForReviewWillOpenMarketEvent;
		EtceteraAndroidManager.notificationReceivedEvent += notificationReceivedEvent;
		EtceteraAndroidManager.contactsLoadedEvent += contactsLoadedEvent;
	}

	private void OnDisable()
	{
		EtceteraAndroidManager.alertButtonClickedEvent -= alertButtonClickedEvent;
		EtceteraAndroidManager.alertCancelledEvent -= alertCancelledEvent;
		EtceteraAndroidManager.promptFinishedWithTextEvent -= promptFinishedWithTextEvent;
		EtceteraAndroidManager.promptCancelledEvent -= promptCancelledEvent;
		EtceteraAndroidManager.twoFieldPromptFinishedWithTextEvent -= twoFieldPromptFinishedWithTextEvent;
		EtceteraAndroidManager.twoFieldPromptCancelledEvent -= twoFieldPromptCancelledEvent;
		EtceteraAndroidManager.webViewCancelledEvent -= webViewCancelledEvent;
		EtceteraAndroidManager.inlineWebViewJSCallbackEvent -= inlineWebViewJSCallbackEvent;
		EtceteraAndroidManager.albumChooserCancelledEvent -= albumChooserCancelledEvent;
		EtceteraAndroidManager.albumChooserSucceededEvent -= albumChooserSucceededEvent;
		EtceteraAndroidManager.photoChooserCancelledEvent -= photoChooserCancelledEvent;
		EtceteraAndroidManager.photoChooserSucceededEvent -= photoChooserSucceededEvent;
		EtceteraAndroidManager.videoRecordingCancelledEvent -= videoRecordingCancelledEvent;
		EtceteraAndroidManager.videoRecordingSucceededEvent -= videoRecordingSucceededEvent;
		EtceteraAndroidManager.ttsInitializedEvent -= ttsInitializedEvent;
		EtceteraAndroidManager.ttsFailedToInitializeEvent -= ttsFailedToInitializeEvent;
		EtceteraAndroidManager.askForReviewDontAskAgainEvent -= askForReviewDontAskAgainEvent;
		EtceteraAndroidManager.askForReviewRemindMeLaterEvent -= askForReviewRemindMeLaterEvent;
		EtceteraAndroidManager.askForReviewWillOpenMarketEvent -= askForReviewWillOpenMarketEvent;
		EtceteraAndroidManager.notificationReceivedEvent -= notificationReceivedEvent;
		EtceteraAndroidManager.contactsLoadedEvent -= contactsLoadedEvent;
	}

	private void alertButtonClickedEvent(string positiveButton)
	{
		Debug.Log("alertButtonClickedEvent: " + positiveButton);
		switch (positiveButton)
		{
		case "Buy":
			if (InAppManager.Instance.buyGoldNbr == 0)
			{
				DataManager.Instance.gamePrefs.cash -= 5;
				DataManager.Instance.gamePrefs.gold += 1000;
			}
			else if (InAppManager.Instance.buyGoldNbr == 1)
			{
				DataManager.Instance.gamePrefs.cash -= 25;
				DataManager.Instance.gamePrefs.gold += 5000;
			}
			else if (InAppManager.Instance.buyGoldNbr == 2)
			{
				DataManager.Instance.gamePrefs.cash -= 50;
				DataManager.Instance.gamePrefs.gold += 10000;
			}
			else if (InAppManager.Instance.buyGoldNbr == 3)
			{
				DataManager.Instance.gamePrefs.cash -= 100;
				DataManager.Instance.gamePrefs.gold += 20000;
			}
			else if (InAppManager.Instance.buyGoldNbr == 4)
			{
				DataManager.Instance.gamePrefs.cash -= 250;
				DataManager.Instance.gamePrefs.gold += 50000;
			}
			DataManager.Instance.SaveData();
			break;
		case "Ok!":
			DataManager.Instance.SaveData();
			Application.Quit();
			break;
		}
		if (Application.loadedLevelName == "GameMain")
		{
			MainManager component = GameObject.Find("_MainManager").GetComponent<MainManager>();
			component.SendMessage("UpdateWalletLabel");
		}
		else if (Application.loadedLevelName.Substring(0, 8) == "GamePlay")
		{
			GameManager component2 = GameObject.Find("_GameManager").GetComponent<GameManager>();
			component2.SendMessage("UpdateWalletLabel");
		}
		else if (Application.loadedLevelName == "GameBuild")
		{
			UIManager component3 = GameObject.Find("_UIManager").GetComponent<UIManager>();
			component3.SendMessage("UpdateWalletLabel");
		}
	}

	private void alertCancelledEvent()
	{
		Debug.Log("alertCancelledEvent");
	}

	private void promptFinishedWithTextEvent(string param)
	{
		Debug.Log("promptFinishedWithTextEvent: " + param);
	}

	private void promptCancelledEvent()
	{
		Debug.Log("promptCancelledEvent");
	}

	private void twoFieldPromptFinishedWithTextEvent(string text1, string text2)
	{
		Debug.Log("twoFieldPromptFinishedWithTextEvent: " + text1 + ", " + text2);
	}

	private void twoFieldPromptCancelledEvent()
	{
		Debug.Log("twoFieldPromptCancelledEvent");
	}

	private void webViewCancelledEvent()
	{
		Debug.Log("webViewCancelledEvent");
	}

	private void inlineWebViewJSCallbackEvent(string message)
	{
		Debug.Log("inlineWebViewJSCallbackEvent: " + message);
	}

	private void albumChooserCancelledEvent()
	{
		Debug.Log("albumChooserCancelledEvent");
	}

	private void albumChooserSucceededEvent(string imagePath)
	{
		Debug.Log("albumChooserSucceededEvent: " + imagePath);
		Debug.Log("image size: " + EtceteraAndroid.getImageSizeAtPath(imagePath));
	}

	private void photoChooserCancelledEvent()
	{
		Debug.Log("photoChooserCancelledEvent");
	}

	private void photoChooserSucceededEvent(string imagePath)
	{
		Debug.Log("photoChooserSucceededEvent: " + imagePath);
		Debug.Log("image size: " + EtceteraAndroid.getImageSizeAtPath(imagePath));
	}

	private void videoRecordingCancelledEvent()
	{
		Debug.Log("videoRecordingCancelledEvent");
	}

	private void videoRecordingSucceededEvent(string path)
	{
		Debug.Log("videoRecordingSucceededEvent: " + path);
	}

	private void ttsInitializedEvent()
	{
		Debug.Log("ttsInitializedEvent");
	}

	private void ttsFailedToInitializeEvent()
	{
		Debug.Log("ttsFailedToInitializeEvent");
	}

	private void askForReviewDontAskAgainEvent()
	{
		Debug.Log("askForReviewDontAskAgainEvent");
	}

	private void askForReviewRemindMeLaterEvent()
	{
		Debug.Log("askForReviewRemindMeLaterEvent");
	}

	private void askForReviewWillOpenMarketEvent()
	{
		Debug.Log("askForReviewWillOpenMarketEvent");
	}

	private void notificationReceivedEvent(string extraData)
	{
		Debug.Log("notificationReceivedEvent: " + extraData);
	}

	private void contactsLoadedEvent(List<EtceteraAndroid.Contact> contacts)
	{
		Debug.Log("contactsLoadedEvent");
		Utils.logObject(contacts);
	}
}
