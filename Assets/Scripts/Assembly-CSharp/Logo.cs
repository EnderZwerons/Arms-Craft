using System.Collections;
using UnityEngine;

public class Logo : MonoBehaviour
{
	public GameObject[] LogoPanel;

	public string NextScene;

	private void Awake()
	{
		Application.targetFrameRate = 240;
	}

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(0.5f);
		LogoPanel[0].SetActive(true);
		yield return new WaitForSeconds(0.5f);
		LogoPanel[1].SetActive(true);
		yield return new WaitForSeconds(7f);
		AsyncOperation async = Application.LoadLevelAdditiveAsync(NextScene);
		while (!async.isDone)
		{
			yield return true;
		}
	}
}
