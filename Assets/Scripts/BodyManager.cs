using UnityEngine;

public class BodyManager : MonoBehaviour
{
	public UILabel ArmsLevelLabel;

	public UILabel LegsLevelLabel;

	public UILabel ArmsPriceLabel;

	public UILabel LegsPriceLabel;

	private int armsUpPrice;

	private int legsUpPrice;

	private void Start()
	{
		UpdateUpgradeLabel();
	}

	private void UpgradeArms()
	{
		if (DataManager.Instance.gamePrefs.gold >= armsUpPrice)
		{
			DataManager.Instance.gamePrefs.gold -= armsUpPrice;
			DataManager.Instance.gamePrefs.upgradeUpper++;
			DataManager.Instance.SaveData();
			MainManager component = GameObject.Find("_MainManager").GetComponent<MainManager>();
			component.SendMessage("UpdateWalletLabel");
			UpdateUpgradeLabel();
		}
		else
		{
			//EtceteraAndroid.showAlert("Not enough the Gem", string.Empty, "OK");
		}
	}

	private void UpgradeLegs()
	{
		if (DataManager.Instance.gamePrefs.gold >= legsUpPrice)
		{
			DataManager.Instance.gamePrefs.gold -= legsUpPrice;
			DataManager.Instance.gamePrefs.upgradeLower++;
			DataManager.Instance.SaveData();
			MainManager component = GameObject.Find("_MainManager").GetComponent<MainManager>();
			component.SendMessage("UpdateWalletLabel");
			UpdateUpgradeLabel();
		}
		else
		{
			//EtceteraAndroid.showAlert("Not enough the Gem", string.Empty, "OK");
		}
	}

	private void UpdateUpgradeLabel()
	{
		int upgradeUpper = DataManager.Instance.gamePrefs.upgradeUpper;
		int upgradeLower = DataManager.Instance.gamePrefs.upgradeLower;
		armsUpPrice = 25 * (upgradeUpper * upgradeUpper + 5);
		legsUpPrice = 25 * (upgradeLower * upgradeLower + 5);
		ArmsLevelLabel.text = "LV." + upgradeUpper;
		LegsLevelLabel.text = "LV." + upgradeLower;
		ArmsPriceLabel.text = armsUpPrice.ToString();
		LegsPriceLabel.text = legsUpPrice.ToString();
	}
}
