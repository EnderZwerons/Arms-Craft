using UnityEngine;

public class TouchEvent : MonoBehaviour
{
	public GameObject target;

	public string functionName;

	public int value;

	private TutorialManager tutorialManager;

	private void Awake()
	{
		tutorialManager = Object.FindObjectOfType<TutorialManager>();
	}

	private void OnMouseUp()
	{
		if (tutorialManager.eventNum == value)
		{
			Debug.Log("OnMouseUp : " + base.gameObject.name);
			target.SendMessage(functionName, value, SendMessageOptions.DontRequireReceiver);
		}
	}
}
