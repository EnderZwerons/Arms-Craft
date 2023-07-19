using UnityEngine;

public class DebugManager : MonoBehaviour
{
	private Vector2 screenSize;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		screenSize = new Vector2(Screen.width, Screen.height);
	}

	private void OnGUI()
	{
		if ((bool)GameObject.Find("_DataManager") && GUI.Button(new Rect(10f, 150f, 100f, 100f), "Get Gold " + DataManager.Instance.gamePrefs.gold))
		{
			DataManager.Instance.gamePrefs.gold += 10000;
		}
		if ((bool)GameObject.Find("Panel_Lab") && GameObject.Find("Panel_Lab").activeSelf)
		{
			for (int i = 0; i < 15; i++)
			{
				if (i < 8)
				{
					if (GUI.Button(new Rect(screenSize.x - 100f, i * 80 + 100, 70f, 70f), i.ToString() + " : " + DataManager.Instance.gamePrefs.blockMaterial[i]))
					{
						DataManager.Instance.gamePrefs.blockMaterial[i]++;
						if ((bool)GameObject.Find("_LabManager"))
						{
							GameObject.Find("_LabManager").SendMessage("UpdateBlockMaterialData");
						}
					}
				}
				else if (GUI.Button(new Rect(screenSize.x - 200f, (i - 8) * 80 + 100, 70f, 70f), i.ToString() + " : " + DataManager.Instance.gamePrefs.blockMaterial[i]))
				{
					DataManager.Instance.gamePrefs.blockMaterial[i]++;
					if ((bool)GameObject.Find("_LabManager"))
					{
						GameObject.Find("_LabManager").SendMessage("UpdateBlockMaterialData");
					}
				}
			}
		}
		if ((bool)GameObject.Find("_GameManager"))
		{
			GameManager component = GameObject.Find("_GameManager").GetComponent<GameManager>();
			if (GUI.Button(new Rect(300f, 10f, 100f, 100f), "Pause : " + component.state))
			{
				if (component.state != STATE.PAUSE)
				{
					component.state = STATE.PAUSE;
				}
				else
				{
					component.state = STATE.IDLE;
				}
			}
		}
		if (GUI.Button(new Rect(400f, 10f, 100f, 100f), "Timescale : " + Time.timeScale))
		{
			if (Time.timeScale == 1f)
			{
				Time.timeScale = 0f;
			}
			else
			{
				Time.timeScale = 1f;
			}
		}
	}
}
