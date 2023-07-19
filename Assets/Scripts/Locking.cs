using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locking : MonoBehaviour
{
	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	void Update()
	{
		if (!PCControls.OnPC)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.Escape))
		{
			PCControls.CursorLocked = !PCControls.CursorLocked;
		}
	}
}
