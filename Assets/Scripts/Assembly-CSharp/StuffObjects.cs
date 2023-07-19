using UnityEngine;

public class StuffObjects : MonoBehaviour
{
	private static StuffObjects instance;

	public GameObject[] stuffObject;

	public static StuffObjects Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.Find("_GameObjects").GetComponent<StuffObjects>();
			}
			return instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnApplicationQuit()
	{
		instance = null;
	}

	public GameObject GetObjectStuff(string stuffName)
	{
		int num = 0;
		num = int.Parse(stuffName.Substring(1, 3)) - 1;
		return stuffObject[num];
	}
}
