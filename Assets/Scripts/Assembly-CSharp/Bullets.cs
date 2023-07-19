using System.Collections;
using UnityEngine;

public class Bullets : MonoBehaviour
{
	private void Fire()
	{
		StartCoroutine(Durations());
	}

	private IEnumerator Durations()
	{
		yield return new WaitForSeconds(1f);
		GetComponent<Rigidbody>().Sleep();
		yield return new WaitForSeconds(0.1f);
		ObjectPoolManager.DestroyObject(base.gameObject);
	}
}
