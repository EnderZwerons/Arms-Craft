using UnityEngine;

public class StabilizeKickBack : MonoBehaviour
{
	public float returnSpeed = 2f;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
	}
}
