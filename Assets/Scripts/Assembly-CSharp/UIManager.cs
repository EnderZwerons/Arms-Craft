using UnityEngine;

public class UIManager : MonoBehaviour
{
	public GameObject panelWallet;

	public GameObject panelBuild;

	public GameObject panelShop;

	public GameObject itemGO;

	public GameObject rootGO;

	public UILabel goldLabel;

	public UILabel cashLabel;

	private void ShowObject(GameObject target)
	{
		target.SetActive(true);
		switch (target.name)
		{
		case "Panel_Shop":
			UpdateWalletLabel();
			GameObject.Find("_ShopManager").SendMessage("UpdateInvenLabel");
			panelWallet.SetActive(true);
			panelBuild.SetActive(false);
			itemGO.SetActive(false);
			rootGO.SetActive(false);
			break;
		case "Panel_MoneyShop":
			panelShop.SetActive(false);
			break;
		}
	}

	private void HideObject(GameObject target)
	{
		target.SetActive(false);
		switch (target.name)
		{
		case "Panel_Shop":
			GameObject.Find("_BuildManager").SendMessage("UpdateBlockDataLabel");
			panelWallet.SetActive(false);
			panelBuild.SetActive(true);
			itemGO.SetActive(true);
			rootGO.SetActive(true);
			break;
		case "Panel_MoneyShop":
			panelShop.SetActive(true);
			break;
		}
	}

	private void BuyGold(int num)
	{
		InAppManager.Instance.PurchaseGold(num);
	}

	private void BuyCash(int num)
	{
		InAppManager.Instance.PurchaseCash(num);
	}

	public void UpdateWalletLabel()
	{
		goldLabel.text = DataManager.Instance.gamePrefs.gold.ToString();
		cashLabel.text = DataManager.Instance.gamePrefs.cash.ToString();
	}
}
