using UnityEngine;

public class BlockObjects : MonoBehaviour
{
	private static BlockObjects instance;

	public GameObject[] blockObject;

	public static BlockObjects Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.Find("_GameObjects").GetComponent<BlockObjects>();
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

	public string BlockSpriteName(int num)
	{
		switch (num)
		{
		case 0:
			return "b001";
		case 1:
			return "b002";
		case 2:
			return "b003";
		case 3:
			return "b004";
		case 4:
			return "b005";
		case 5:
			return "b006";
		case 6:
			return "b007";
		case 7:
			return "b008";
		case 8:
			return "b009";
		case 9:
			return "b010";
		case 10:
			return "blk_hole";
		case 11:
			return "blk_eyepin";
		case 12:
			return "blk_hole";
		case 13:
			return "blk_eyepin";
		case 99:
			return "blk_erase";
		default:
			return null;
		}
	}
}
