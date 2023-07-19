using System.Collections;
using UnityEngine;

public class StartDestroy : MonoBehaviour
{
	public float liveTime;

	private void OnEnable()
	{
		StartCoroutine(CheckIfAlive());
	}

	private IEnumerator CheckIfAlive()
	{
		yield return new WaitForSeconds(liveTime);
		ObjectPoolManager.DestroyObject(base.gameObject);
	}
}
