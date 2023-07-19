using UnityEngine;

public class Dissolving : MonoBehaviour
{
	public Transform target;

	public float dissolveTime;

	private float amount;

	private bool isDone;

	private void Update()
	{
		if (!isDone)
		{
			amount += dissolveTime * 0.5f * Time.smoothDeltaTime;
			target.GetComponent<Renderer>().material.SetFloat("_DissolveAmount", amount);
			if (amount >= 1f)
			{
				isDone = true;
				Object.Destroy(base.gameObject);
			}
		}
	}
}
