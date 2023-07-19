using UnityEngine;

public class ShopManager : MonoBehaviour
{
	public UISprite selectBlockTextures;

	public UILabel selectBlockPrice;

	public UILabel selectBlockName;

	public UILabel[] selectBlockInfo = new UILabel[4];

	public UILabel[] itemAmount = new UILabel[10];

	private int blockNumber;

	private int blockPrice;

	private void Start()
	{
		SelectBlock(0);
	}

	public void UpdateInvenLabel()
	{
		for (int i = 0; i < DataManager.Instance.blockAmtLength; i++)
		{
			itemAmount[i].text = DataManager.Instance.gamePrefs.blockAmt[i].ToString();
		}
	}

	private void UpdateSelectBlockLabel()
	{
		Block component = BlockObjects.Instance.blockObject[blockNumber].GetComponent<Block>();
		selectBlockName.text = component.names;
		selectBlockInfo[0].text = ":" + component.damage;
		selectBlockInfo[1].text = ":" + component.rate;
		selectBlockInfo[2].text = ":" + component.bullet;
		selectBlockInfo[3].text = ":" + component.cost;
	}

	private void PlayTweenAnim()
	{
		UIPlayTween uIPlayTween = new UIPlayTween();
		uIPlayTween.resetOnPlay = true;
		uIPlayTween.tweenTarget = itemAmount[blockNumber].transform.parent.Find("blockSprite").gameObject;
		uIPlayTween.Play(true);
	}

	private void SelectBlock(int blockNum)
	{
		blockNumber = blockNum;
		selectBlockTextures.spriteName = "b" + (blockNumber + 1).ToString("D3");
		blockPrice = BlockTiers(blockNumber);
		selectBlockPrice.text = blockPrice.ToString();
		UpdateSelectBlockLabel();
	}

	private void BuyBlock()
	{
		if (DataManager.Instance.gamePrefs.cash >= blockPrice)
		{
			Debug.Log("Buy Successful");
			DataManager.Instance.gamePrefs.cash -= blockPrice;
			DataManager.Instance.gamePrefs.blockAmt[blockNumber]++;
			DataManager.Instance.SaveData();
			if (Application.loadedLevelName == "GameMain")
			{
				MainManager component = GameObject.Find("_MainManager").GetComponent<MainManager>();
				component.SendMessage("UpdateWalletLabel");
			}
			else
			{
				GameObject.Find("_UIManager").SendMessage("UpdateWalletLabel");
			}
			UpdateInvenLabel();
			PlayTweenAnim();
		}
	}

	private int BlockTiers(int num)
	{
		int num2 = 0;
		return LevelDesign.Instance.blockMaterial[num].buyPrice;
	}
}
