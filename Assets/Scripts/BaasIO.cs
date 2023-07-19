using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public class BaasIO : MonoBehaviour
{
	public delegate void Action(string date);

	public string URL;

	private string textureURL;

	private string linkURL;

	public UISprite loadSprite;

	public UILabel loadLabel;

	public bool isConnect = true;

	public bool isSuccess;

	public GameObject PanelRoot;

	public GameObject ConnectLayer;

	public GameObject AlertLayer;

	public GameObject UpdateLayer;

	public GameObject BaasEventLayer;

	public UITexture baasAds;

	public UIStretch adsStrech;

	[method: MethodImpl(32)]
	public static event Action getDataSuccessfulEvent;

	[method: MethodImpl(32)]
	public static event Action getDataFailedEvent;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void TurnOn()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			AlertLayer.SetActive(true);
		}
		else if (IsCheckDontSeeAds())
		{
			StartCoroutine(GetData(URL));
			StartCoroutine(ConnectAnim());
		}
		else
		{
			BaasIO.getDataFailedEvent("NULL");
		}
	}

	public bool IsCheckDontSeeAds()
	{
		int @int = PlayerPrefs.GetInt("adst", 0);
		if (@int != 0)
		{
			if (@int == DateTime.Now.Day)
			{
				return false;
			}
			return true;
		}
		return true;
	}

	private void CheckDontSeeAds()
	{
		PlayerPrefs.SetInt("adst", DateTime.Now.Day);
		PlayerPrefs.Save();
		PanelRoot.SetActive(false);
	}

	private void RestartGame()
	{
		UnityEngine.Object.Destroy(GameObject.Find("_AudioManager"));
		UnityEngine.Object.Destroy(GameObject.Find("_BaasIO"));
		UnityEngine.Object.Destroy(GameObject.Find("_InAppManager"));
		UnityEngine.Object.Destroy(GameObject.Find("_GameObjects"));
		UnityEngine.Object.Destroy(GameObject.Find("_DataManager"));
		UnityEngine.Object.Destroy(GameObject.Find("_LevelDesign"));
		Application.LoadLevel("GameLogo");
	}

	public IEnumerator ConnectAnim()
	{
		isConnect = false;
		isSuccess = false;
		PanelRoot.SetActive(true);
		ConnectLayer.SetActive(true);
		int num = 0;
		string[] text = new string[7] { "CONNECT", "CONNECT.", "CONNECT..", "CONNECT...", "CONNECT..", "CONNECT.", "CONNECT" };
		while (!isConnect)
		{
			DateTime dTime = DateTime.Now.AddSeconds(0.15000000596046448);
			while (DateTime.Now < dTime)
			{
				yield return null;
			}
			yield return null;
			loadSprite.spriteName = "connect_" + num;
			loadLabel.text = text[num];
			num++;
			if (num > 6)
			{
				num = 0;
			}
		}
		if (isSuccess)
		{
			BaasEventLayer.SetActive(true);
			ConnectLayer.SetActive(false);
		}
		else
		{
			ConnectLayer.SetActive(false);
		}
	}

	private IEnumerator GetData(string url)
	{
		WWW www = new WWW(url);
		yield return www;
		DateTime dTime = DateTime.Now.AddSeconds(2.0);
		while (DateTime.Now < dTime)
		{
			yield return null;
		}
		if (string.IsNullOrEmpty(www.error))
		{
			string tmpTextAsset = Encoding.UTF8.GetString(www.bytes);
			if (TableGenerator.GetIndexIntKey(tmpTextAsset)[4].Equals("0"))
			{
				if (int.Parse(TableGenerator.GetIndexIntKey(tmpTextAsset)[3]) == 8)
				{
					textureURL = TableGenerator.GetIndexIntKey(tmpTextAsset)[0];
					linkURL = TableGenerator.GetIndexIntKey(tmpTextAsset)[1];
					StartCoroutine(GetImage(textureURL));
				}
				else
				{
					isConnect = true;
					isSuccess = false;
					UpdateLayer.SetActive(true);
				}
			}
			else
			{
				BaasIO.getDataFailedEvent("NULL");
			}
		}
		else
		{
			BaasIO.getDataFailedEvent("NULL");
		}
	}

	private IEnumerator GetImage(string url)
	{
		WWW www = new WWW(url);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			baasAds.mainTexture = www.texture;
			adsStrech.enabled = true;
			isSuccess = true;
			isConnect = true;
			BaasIO.getDataSuccessfulEvent(www.responseHeaders["DATE"]);
		}
		else
		{
			BaasIO.getDataFailedEvent("NULL");
		}
	}

	private void ExitAds()
	{
		PanelRoot.SetActive(false);
	}

	private void OpenAds()
	{
		Application.OpenURL(linkURL);
	}

	public void GoToUpdateStore()
	{
		Application.OpenURL("market://details?id=com.infinitypocket.AC");
	}
}
