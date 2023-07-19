using UnityEngine;

public class UIResize : MonoBehaviour
{
	private void Awake()
	{
		Vector2 vector = new Vector2(Screen.width, Screen.height);
		Camera component = GetComponent<Camera>();
		float orthographicSize = vector.y / vector.x;
		component.orthographicSize = orthographicSize;
	}
}
