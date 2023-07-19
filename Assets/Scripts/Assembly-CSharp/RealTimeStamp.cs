using System.Collections;
using UnityEngine;

public class RealTimeStamp : MonoBehaviour
{
	private static RealTimeStamp instance;

	public long timeStamp;

	private long serverTimeStamp;

	private float intervalTime;

	public static RealTimeStamp Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.Find("_RealTimeStamp").GetComponent<RealTimeStamp>();
			}
			return instance;
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			StartTimeStamp();
		}
	}

	private void OnApplicationQuit()
	{
		instance = null;
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private void Start()
	{
		StartTimeStamp();
	}

	private void StartTimeStamp()
	{
		StopCoroutine("UpdateTimeStamp");
		serverTimeStamp = DataManager.Instance.serverTimeStamp;
		intervalTime = DataManager.Instance.intervalTime;
		StartCoroutine("UpdateTimeStamp");
	}

	private IEnumerator UpdateTimeStamp()
	{
		while (true)
		{
			timeStamp = serverTimeStamp + (long)Time.realtimeSinceStartup;
			yield return new WaitForSeconds(0.1f);
		}
	}
}
