using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveIfNotOnPC : MonoBehaviour
{
	void Start()
	{
		gameObject.SetActive(!PCControls.OnPC);
	}
}
