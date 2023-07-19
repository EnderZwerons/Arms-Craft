using System.Collections;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
	private static EnergyManager instance;

	public UISprite energySprite;

	public UILabel energyLabel;

	public double rateTime;

	public static EnergyManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.Find("_EnergyManager").GetComponent<EnergyManager>();
			}
			return instance;
		}
	}

	public void InitEnergyManager()
	{
		StartManager();
	}

	private void StartManager()
	{
		StartCoroutine(UpdatePerSecond());
		UpdateEnergySprite();
	}

	public void UseEnergy()
	{
		if (DataManager.Instance.gamePrefs.energy.isFull)
		{
			DataManager.Instance.gamePrefs.energy.energyAmt--;
			DataManager.Instance.gamePrefs.energy.isFull = false;
			DataManager.Instance.gamePrefs.energy.beganTimeStamp = RealTimeStamp.Instance.timeStamp;
			DataManager.Instance.gamePrefs.energy.cosumeEnergy++;
		}
		else
		{
			DataManager.Instance.gamePrefs.energy.energyAmt--;
			DataManager.Instance.gamePrefs.energy.cosumeEnergy++;
		}
		UpdateEnergySprite();
	}

	private void UpdateEnergySprite()
	{
		energySprite.spriteName = "topbar_battery" + DataManager.Instance.gamePrefs.energy.energyAmt;
	}

	private void UpdateEnergyLabel(string t)
	{
		energyLabel.text = t;
	}

	private void UpdateEnergyLabel(double n)
	{
		double num = 0.0;
		double num2 = 0.0;
		num = n % 3600.0 / 60.0;
		num2 = n % 3600.0 % 60.0;
		energyLabel.text = num.ToString("D2") + ":" + num2.ToString("D2");
	}

	private IEnumerator UpdatePerSecond()
	{
		double time2 = 0.0;
		double num2 = 0.0;
		while (true)
		{
			time2 = RealTimeStamp.Instance.timeStamp - DataManager.Instance.gamePrefs.energy.beganTimeStamp;
			num2 = time2 / rateTime;
			DataManager.Instance.gamePrefs.energy.energyAmt = (int)num2 + (6 - DataManager.Instance.gamePrefs.energy.cosumeEnergy);
			if (DataManager.Instance.gamePrefs.energy.energyAmt >= 6)
			{
				break;
			}
			if (time2 % rateTime == 0.0)
			{
				UpdateEnergyLabel(0.0);
			}
			else
			{
				UpdateEnergyLabel(rateTime - time2 % rateTime);
			}
			UpdateEnergySprite();
			yield return new WaitForSeconds(0.5f);
		}
		DataManager.Instance.gamePrefs.energy.energyAmt = 6;
		DataManager.Instance.gamePrefs.energy.isFull = true;
		DataManager.Instance.gamePrefs.energy.cosumeEnergy = 0;
		UpdateEnergyLabel(string.Empty);
		UpdateEnergySprite();
	}
}
