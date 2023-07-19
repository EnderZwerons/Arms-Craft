using System;
using UnityEngine;

public class BaasIOEventListener : MonoBehaviour
{
	private void OnEnable()
	{
		BaasIO.getDataSuccessfulEvent += GetDataSuccessful;
		BaasIO.getDataFailedEvent += GetDataFailed;
	}

	private void OnDisable()
	{
		BaasIO.getDataSuccessfulEvent -= GetDataSuccessful;
		BaasIO.getDataFailedEvent -= GetDataFailed;
	}

	private void GetDataSuccessful(string date)
	{
		if ((bool)GameObject.Find("_BootManager"))
		{
			BootManager component = GameObject.Find("_BootManager").GetComponent<BootManager>();
			component.GetTimeStamp(DateTime.Parse(date).Ticks);
		}
	}

	private void GetDataFailed(string error)
	{
		BaasIO component = GetComponent<BaasIO>();
		component.isConnect = true;
		component.isSuccess = false;
		component.PanelRoot.SetActive(false);
		if ((bool)GameObject.Find("_BootManager"))
		{
			BootManager component2 = GameObject.Find("_BootManager").GetComponent<BootManager>();
			component2.GetTimeStamp(DateTime.Now.Ticks);
		}
	}
}
