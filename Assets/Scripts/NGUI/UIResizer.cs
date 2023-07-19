using UnityEngine;

public class UIResizer : MonoBehaviour
{
	public Transform[] resizeObject;

	private void Start()
	{
		Vector2 vector = new Vector2(Screen.width, Screen.height);
		float num = 640f / vector.y;
		float num2 = vector.x * num;
		float num3 = num2 / 1136f;
		Transform[] array = resizeObject;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].localScale *= num3;
		}
	}
}
