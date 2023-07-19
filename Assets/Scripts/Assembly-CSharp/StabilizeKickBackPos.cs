using UnityEngine;

public class StabilizeKickBackPos : MonoBehaviour
{
	public float returnSpeed = 2f;

	private void Update()
	{
		base.transform.localPosition = Vector3.Slerp(base.transform.localPosition, Vector3.zero, Time.deltaTime * returnSpeed);
	}
}
