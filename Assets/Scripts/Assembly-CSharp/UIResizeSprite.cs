using UnityEngine;

public class UIResizeSprite : MonoBehaviour
{
	private void Awake()
	{
		Vector2 vector = new Vector2(Screen.width, Screen.height);
		UISprite component = GetComponent<UISprite>();
		float num = 1024f / vector.x * vector.y;
		component.height = (int)num + 2;
	}
}
