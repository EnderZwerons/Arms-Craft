using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCControls
{
	public static bool CursorLocked
	{
		get
		{
			return Cursor.lockState == CursorLockMode.Locked && !Cursor.visible;
		}
		set
		{
			Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = !value;
		}
	}

	public static bool OnPC
	{
		get
		{
			return !Application.isMobilePlatform;
		}
	}
}
