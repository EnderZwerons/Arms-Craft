using UnityEngine;

public class Gate_Ob : MonoBehaviour
{
	public Animation gateani;

	private bool isOpen;

	private void OnTriggerEnter(Collider other)
	{
		if ((other.tag == "Player" || other.tag == "Enemy") && !isOpen)
		{
			gateani.GetComponent<Animation>().Play("open");
			isOpen = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((other.tag == "Player" || other.tag == "Enemy") && isOpen)
		{
			gateani.GetComponent<Animation>().Play("close");
			isOpen = false;
		}
	}
}
