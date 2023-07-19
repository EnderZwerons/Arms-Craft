using UnityEngine;

public class ConsumeShopManager : MonoBehaviour
{
	private FPSController fpsController;

	private int itemNbr;

	private int itemPrice;

	public UILabel itemPriceLabel;

	private void Start()
	{
		fpsController = GameObject.FindWithTag("Player").GetComponent<FPSController>();
		SelectItem(0);
	}

	private void SelectItem(int n)
	{
		itemNbr = n;
		switch (n)
		{
		case 0:
			itemPrice = 1;
			break;
		case 1:
			itemPrice = 1;
			break;
		case 2:
			itemPrice = 2;
			break;
		case 3:
			itemPrice = 1;
			break;
		}
		itemPriceLabel.text = itemPrice.ToString();
	}

	private void BuyItem()
	{
		if (DataManager.Instance.gamePrefs.cash < itemPrice)
		{
			return;
		}
		switch (itemNbr)
		{
		case 0:
		{
			for (int i = 1; i < 3; i++)
			{
				fpsController.WeaponList[i].bulletleft += fpsController.WeaponList[i].bulletperClip * 5;
			}
			GameManager.Instance.UpdateAmmoGUI();
			break;
		}
		case 1:
			fpsController.Grenade += 5;
			GameManager.Instance.UpdateGrenadeGUI();
			break;
		case 2:
			fpsController.Health = fpsController.MaxHealth;
			GameManager.Instance.UpdateHealthGUI();
			break;
		case 3:
			fpsController.Shield = fpsController.MaxShield;
			GameManager.Instance.UpdateShieldGUI();
			break;
		}
		DataManager.Instance.gamePrefs.cash -= itemPrice;
		GameManager.Instance.UpdateWalletLabel();
	}
}
