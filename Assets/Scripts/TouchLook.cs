using UnityEngine;

public class TouchLook : MonoBehaviour
{
	public float Sensitivity = 2f;

	private float sensitivityX = 2000f;

	private float sensitivityY = 2000f;

	public bool invertX;

	public bool invertY;

	public Transform movePivot;

	public Transform cam;

	public float zoomSpeed;

	public float minPinchSpeed = 5f;

	public float varianceInDistances = 5f;

	private Vector3 curTransform;

	private Vector2 oriPos;

	private float curDist;

	private float prevDist;

	private float DPI;

	private Vector3 mouseDelta;

	private void Start()
	{
		Sensitivity = DataManager.Instance.gamePrefs.sensitivity;
		DPI = Screen.dpi / 100f;
		if (DPI == 0f)
		{
			DPI = 1.6f;
		}
		if (PCControls.OnPC)
		{
			mouseDelta = Input.mousePosition;
		}
	}

	private void Update()
	{
		if (PCControls.OnPC)
		{
			if (Input.GetMouseButton(0))
			{
				Vector2 drag = (Input.mousePosition - mouseDelta) * Time.smoothDeltaTime;
				float x = drag.x * (Sensitivity * sensitivityX * 0.01f) / DPI;
				x = ((!invertX) ? (x * -1f) : x);
				float y = drag.y * (Sensitivity * sensitivityY * 0.01f) / DPI;
				y = ((!invertX) ? (y * -1f) : y);
				base.transform.localEulerAngles += new Vector3(-y, x, 0f);
			}
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			if (scroll != 0)
			{
				cam.localPosition += new Vector3(0f, 0f, scroll == 0.1f ? zoomSpeed * 30f : -(zoomSpeed * 30f));
			}
			mouseDelta = Input.mousePosition;
		}
		else
		{
			if (Input2.touchCount == 1)
			{
				switch (Input2.touches[0].phase)
				{
				case TouchPhase.Began:
					break;
				case TouchPhase.Moved:
				{
					Vector2 vector = Input2.touches[0].deltaPosition * Time.smoothDeltaTime;
					float num = vector.x * (Sensitivity * sensitivityX * 0.01f) / DPI;
					num = ((!invertX) ? (num * -1f) : num);
					float num2 = vector.y * (Sensitivity * sensitivityY * 0.01f) / DPI;
					num2 = ((!invertY) ? (num2 * -1f) : num2);
					base.transform.localEulerAngles += new Vector3(num2, num, 0f);
					break;
				}
				case TouchPhase.Ended:
					break;
				case TouchPhase.Canceled:
					break;
				case TouchPhase.Stationary:
					break;
				}
			}
			else
			{
				if (Input2.touchCount != 2)
				{
					return;
				}
				Touch touch = Input2.touches[0];
				Touch touch2 = Input2.touches[1];
				Debug.Log("touch1 ? : " + touch);
				Debug.Log("touch2 ? : " + touch2);
				if (touch.phase != TouchPhase.Moved || touch2.phase != TouchPhase.Moved)
				{
					return;
				}
				float num3 = Vector2.Angle(touch.deltaPosition, touch2.deltaPosition);
				Debug.Log(string.Concat("touch1Delta : ", touch.deltaPosition, "  //  touch2Delta : ", touch2.deltaPosition));
				Debug.Log("touchAngle : " + num3);
				if (num3 == 90f)
				{
					return;
				}
				if (num3 > 90f)
				{
					Vector2 vector2 = touch.position - touch2.position;
					Vector2 vector3 = touch.position - touch.deltaPosition - (touch2.position - touch2.deltaPosition);
					float num4 = vector2.magnitude - vector3.magnitude;
					if (cam.localPosition.z > -30f && num4 <= 1f)
					{
						cam.localPosition += new Vector3(0f, 0f, zoomSpeed * num4);
					}
					else if (cam.localPosition.z < -5f && num4 > 1f)
					{
						cam.localPosition += new Vector3(0f, 0f, zoomSpeed * num4);
					}
				}
				else
				{
					Vector2 vector4 = Vector2.Lerp(touch.deltaPosition, touch2.deltaPosition, 0.5f);
					movePivot.transform.localPosition += new Vector3((0f - vector4.x) * Time.smoothDeltaTime, (0f - vector4.y) * Time.smoothDeltaTime, 0f);
				}
			}
		}
	}
}
