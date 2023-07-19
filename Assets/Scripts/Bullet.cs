using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(2f);
		Object.Destroy(base.gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		Object.Destroy(base.gameObject);
	}
}
