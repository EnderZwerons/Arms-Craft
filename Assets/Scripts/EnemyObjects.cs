using UnityEngine;

public class EnemyObjects : MonoBehaviour
{
	private static EnemyObjects instance;

	public GameObject[] enemyObject;

	public static EnemyObjects Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.Find("_GameObjects").GetComponent<EnemyObjects>();
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
}
