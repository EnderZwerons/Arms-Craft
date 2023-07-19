using System.Collections;
using UnityEngine;

public class ObjectExplode : MonoBehaviour
{
	private bool explode;

	public float healthCount = 10f;

	public GameObject explodeEffect;

	private Transform explosionTransform;

	public float explosionRadius = 5f;

	public float explosionDamage = 50f;

	public float explosionTime = 2f;

	public AudioClip explosionSound;

	private void Start()
	{
		explosionTransform = base.transform;
		ExplodeBegan();
	}

	private void ExplodeBegan()
	{
		if (!explode)
		{
			explode = true;
			StartCoroutine(Explode());
		}
	}

	private IEnumerator Explode()
	{
		yield return new WaitForSeconds(explosionTime);
		base.transform.GetComponent<Rigidbody>().Sleep();
		base.transform.GetComponent<Rigidbody>().useGravity = false;
		base.transform.GetComponent<Collider>().enabled = false;
		base.transform.eulerAngles = Vector3.zero;
		explodeEffect.SetActive(true);
		base.transform.Find("Mesh").gameObject.SetActive(false);
		if (DataManager.Instance.gamePrefs.sndEffect)
		{
			GetComponent<AudioSource>().PlayOneShot(explosionSound);
		}
		Collider[] colliders = Physics.OverlapSphere(explosionTransform.position, explosionRadius);
		Collider[] array = colliders;
		foreach (Collider other in array)
		{
			Vector3 closestPoint = other.ClosestPointOnBounds(explosionTransform.position);
			float distance = Vector3.Distance(closestPoint, explosionTransform.position);
			float hitPoints2 = 1f - Mathf.Clamp01(distance / explosionRadius);
			hitPoints2 *= explosionDamage;
			if (other.transform.tag == "Enemy")
			{
				other.GetComponent<Rigidbody>().AddExplosionForce(hitPoints2 * 30f, explosionTransform.position, explosionRadius, hitPoints2 * 10f);
				other.transform.SendMessage("HitDamage", hitPoints2, SendMessageOptions.DontRequireReceiver);
			}
		}
		yield return new WaitForSeconds(2f);
		Object.Destroy(base.gameObject);
	}
}
