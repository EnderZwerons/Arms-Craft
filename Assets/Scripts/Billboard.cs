using UnityEngine;

public class Billboard : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		Vector3 forward = Camera.main.transform.forward;
		forward.y = 0f;
		base.transform.rotation = Quaternion.LookRotation(forward);
	}
}
