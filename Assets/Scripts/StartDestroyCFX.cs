using System.Collections;
using UnityEngine;

public class StartDestroyCFX : MonoBehaviour
{
	private void OnEnable()
	{
		StartCoroutine(CheckIfAlive());
	}

	private IEnumerator CheckIfAlive()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			if (!GetComponent<ParticleSystem>().IsAlive(true))
			{
				ObjectPoolManager.DestroyObject(base.gameObject);
			}
		}
	}
}
