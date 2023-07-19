using UnityEngine;

public class Item : MonoBehaviour
{
	private Transform target;

	public bool inside;

	private void Start()
	{
		target = GameObject.FindWithTag("Player").transform;
	}

	private void Update()
	{
		if (inside)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, target.position, 10f * Time.deltaTime);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Magnet")
		{
			inside = true;
		}
	}
}
