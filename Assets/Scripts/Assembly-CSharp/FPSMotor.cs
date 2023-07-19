using System;
using UnityEngine;

[Serializable]
public class FPSMotor
{
	public enum HandType
	{
		RightHanded,
		LeftHanded
	}

	public enum GUIPad
	{
		AnalogPad,
		Slide,
		Button,
		None
	}

	public enum FirePos
	{
		Window1,
		Window2,
		None
	}

	public enum ReloadPos
	{
		Window1,
		Window2,
		None
	}

	public enum JumpPos
	{
		Window1,
		Window2,
		None
	}

	public enum AxeLimit
	{
		AllAxis,
		XAxis,
		YAxis
	}

	[Serializable]
	public class GUIObj
	{
		public Texture SocleGUI;

		public Texture StickGUI;

		public Texture FireGui;

		public Texture ReloadGui;

		public Texture JumpGui;
	}

	private class Btn
	{
		public bool FingerDown;

		public Rect FingerBound;

		public int FingerId = -1;

		public Vector2 FingerCenter;

		public Rect FingerStickRect;

		public Rect FingerSocleRect;
	}

	private float deadZoneInPercent = 20f;

	private Color Window1PadColor;

	private Color Window2PadColor;

	private Color TranspColor = new Color(0f, 0f, 0f, 0f);

	private Rect LeftBound;

	private Rect RightBound;

	private Btn Left = new Btn();

	private Btn Right = new Btn();

	private Btn Fire = new Btn();

	private Btn Reload = new Btn();

	private Btn Jump = new Btn();

	private Vector2 FixedPos;

	private Vector2 FixedPos2;

	private Vector2 FixedPos3;

	private float guiRatio;

	[HideInInspector]
	public Vector3 scale = Vector3.zero;

	[HideInInspector]
	public float scrW;

	[HideInInspector]
	public float scrH = 640f;

	public GUIPad LeftGUIPadStyle;

	public GUIPad RightGUIPadStyle;

	public AxeLimit LeftAxeLimit;

	public AxeLimit RightAxeLimit;

	public FirePos BtnFirePos = FirePos.Window2;

	public ReloadPos BtnReloadPos = ReloadPos.Window2;

	public JumpPos BtnJumpPos = JumpPos.Window2;

	public Color GUIColor = new Color(1f, 1f, 1f, 0.8f);

	public HandType HandStyle;

	public GUIObj GUIObject;

	[NonSerialized]
	public bool HideCrossHair;

	[NonSerialized]
	public bool WeaponActionEnded;

	[NonSerialized]
	public float WeaponTouchDiff;

	[NonSerialized]
	public float WeaponTouchPercentDist;

	[NonSerialized]
	public float DPI;

	[NonSerialized]
	public Vector2 HalfScreen;

	[NonSerialized]
	public Vector2 ScreenSize;

	[NonSerialized]
	public Vector2 Window1InputPad;

	[NonSerialized]
	public Vector2 Window2InputPad;

	[NonSerialized]
	public Vector2 Window1InputSlide;

	[NonSerialized]
	public Vector2 Window2InputSlide;

	[NonSerialized]
	public bool WindowFireBtnPressed;

	[NonSerialized]
	public bool WindowReloadBtnPressed;

	[NonSerialized]
	public bool WindowJumpBtnPressed;

	[NonSerialized]
	public bool Window1Pressed;

	[NonSerialized]
	public bool Window2Pressed;

	[NonSerialized]
	public int Window1TapNbr;

	[NonSerialized]
	public int Window2TapNbr;

	public void InitControl()
	{
		ScreenSize = new Vector2(Screen.width, Screen.height);
		HalfScreen = ScreenSize * 0.5f;
		float num = scrH / ScreenSize.y;
		scrW = ScreenSize.x * num;
		scale.x = ScreenSize.x / scrW;
		scale.y = ScreenSize.y / scrH;
		scale.z = 1f;
		DPI = Screen.dpi / 100f;
		if (DPI == 0f)
		{
			DPI = 1.6f;
		}
		Window1PadColor = TranspColor;
		Window2PadColor = TranspColor;
		LeftBound = new Rect(0f, 0f, ScreenSize.x * 0.5f, ScreenSize.y * 0.9f);
		RightBound = new Rect(ScreenSize.x * 0.5f, 0f, ScreenSize.x * 0.5f, ScreenSize.y * 0.6f);
		switch (HandStyle)
		{
		case HandType.RightHanded:
			Left.FingerBound = LeftBound;
			Right.FingerBound = RightBound;
			break;
		case HandType.LeftHanded:
			Left.FingerBound = RightBound;
			Right.FingerBound = LeftBound;
			break;
		}
		FirePos btnFirePos = BtnFirePos;
		if (btnFirePos == FirePos.Window2)
		{
			FixedPos = new Vector2(scrW - 210f, scrH - 540f);
		}
		ReloadPos btnReloadPos = BtnReloadPos;
		if (btnReloadPos == ReloadPos.Window2)
		{
			FixedPos2 = new Vector2(scrW - 100f, scrH - 340f);
		}
		JumpPos btnJumpPos = BtnJumpPos;
		if (btnJumpPos == JumpPos.Window2)
		{
			FixedPos3 = new Vector2(scrW - 77f, scrH - 570f);
		}
		Fire.FingerBound = new Rect(FixedPos.x - 64f, FixedPos.y - 64f, 128f, 128f);
		Reload.FingerBound = new Rect(FixedPos2.x - 64f, FixedPos2.y - 64f, 128f, 128f);
		Jump.FingerBound = new Rect(FixedPos3.x - 64f, FixedPos3.y - 64f, 128f, 128f);
	}

	public void Command()
	{
		InGameTouchCtrl();
	}

	public void InGameTouchCtrl()
	{
		Touch[] touches = Input2.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			Touch touch = touches[i];
			if (touch.phase == TouchPhase.Began)
			{
				if (LeftGUIPadStyle == GUIPad.AnalogPad && !Left.FingerDown)
				{
					Left.FingerDown = Left.FingerBound.Contains(touch.position);
					if (Left.FingerDown)
					{
						Left.FingerId = touch.fingerId;
						Left.FingerCenter = touch.position / scale.x;
						Left.FingerSocleRect = (Left.FingerStickRect = new Rect(Left.FingerCenter.x - 80f, scrH - Left.FingerCenter.y - 80f, 160f, 160f));
						Window1PadColor = GUIColor;
						continue;
					}
				}
				GUIPad rightGUIPadStyle = RightGUIPadStyle;
				if (rightGUIPadStyle != GUIPad.Slide)
				{
					continue;
				}
				FirePos btnFirePos;
				ReloadPos btnReloadPos;
				JumpPos btnJumpPos;
				if (!Right.FingerDown)
				{
					Right.FingerDown = Right.FingerBound.Contains(touch.position);
					if (!Right.FingerDown)
					{
						continue;
					}
					Right.FingerId = touch.fingerId;
					btnFirePos = BtnFirePos;
					if (btnFirePos == FirePos.Window2 && /*!GameManager.Instance.autoFire*/true)
					{
						Fire.FingerDown = Fire.FingerBound.Contains(touch.position / scale.x);
						if (Fire.FingerDown)
						{
							Fire.FingerId = touch.fingerId;
							WindowFireBtnPressed = true;
						}
					}
					btnReloadPos = BtnReloadPos;
					if (btnReloadPos == ReloadPos.Window2)
					{
						Reload.FingerDown = Reload.FingerBound.Contains(touch.position / scale.x);
						if (Reload.FingerDown)
						{
							Reload.FingerId = touch.fingerId;
							WindowReloadBtnPressed = true;
						}
					}
					btnJumpPos = BtnJumpPos;
					if (btnJumpPos == JumpPos.Window2)
					{
						Jump.FingerDown = Jump.FingerBound.Contains(touch.position / scale.x);
						if (Jump.FingerDown)
						{
							Jump.FingerId = touch.fingerId;
							WindowJumpBtnPressed = true;
						}
					}
					continue;
				}
				btnFirePos = BtnFirePos;
				if (btnFirePos == FirePos.Window2 && /*!GameManager.Instance.autoFire*/true)
				{
					Fire.FingerDown = Fire.FingerBound.Contains(touch.position / scale.x);
					if (Fire.FingerDown)
					{
						Fire.FingerId = touch.fingerId;
						WindowFireBtnPressed = true;
					}
				}
				btnReloadPos = BtnReloadPos;
				if (btnReloadPos == ReloadPos.Window2)
				{
					Reload.FingerDown = Reload.FingerBound.Contains(touch.position / scale.x);
					if (Reload.FingerDown)
					{
						Reload.FingerId = touch.fingerId;
						WindowReloadBtnPressed = true;
					}
				}
				btnJumpPos = BtnJumpPos;
				if (btnJumpPos == JumpPos.Window2)
				{
					Jump.FingerDown = Jump.FingerBound.Contains(touch.position / scale.x);
					if (Jump.FingerDown)
					{
						Jump.FingerId = touch.fingerId;
						WindowJumpBtnPressed = true;
					}
				}
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				if (LeftGUIPadStyle == GUIPad.AnalogPad && Left.FingerDown && Left.FingerId == touch.fingerId)
				{
					float num = 60f;
					Vector2 vector = default(Vector2);
					switch (LeftAxeLimit)
					{
					case AxeLimit.AllAxis:
						vector = touch.position / scale.x - Left.FingerCenter;
						break;
					case AxeLimit.XAxis:
						vector = new Vector2(touch.position.x, Left.FingerCenter.y) - Left.FingerCenter;
						break;
					case AxeLimit.YAxis:
						vector = new Vector2(Left.FingerCenter.x, touch.position.y) - Left.FingerCenter;
						break;
					}
					float num2 = vector.magnitude * 100f / num;
					if (num2 >= 100f)
					{
						num2 = 100f;
					}
					if (num2 > deadZoneInPercent)
					{
						Window1InputPad = vector.normalized * num2 / 100f;
					}
					else
					{
						Window1InputPad = Vector2.zero;
					}
					Vector2 vector2 = Vector2.ClampMagnitude(vector, num);
					Left.FingerStickRect = new Rect(Left.FingerCenter.x + vector2.x - 80f, scrH - (Left.FingerCenter.y + vector2.y) - 80f, 160f, 160f);
				}
				GUIPad rightGUIPadStyle = RightGUIPadStyle;
				if (rightGUIPadStyle == GUIPad.Slide && Right.FingerDown && Right.FingerId == touch.fingerId)
				{
					switch (RightAxeLimit)
					{
					case AxeLimit.AllAxis:
						Window2InputSlide = touch.deltaPosition * Time.smoothDeltaTime;
						break;
					case AxeLimit.XAxis:
						Window2InputSlide = new Vector2(touch.deltaPosition.x, 0f) * Time.smoothDeltaTime;
						break;
					case AxeLimit.YAxis:
						Window2InputSlide = new Vector2(0f, touch.deltaPosition.y) * Time.smoothDeltaTime;
						break;
					}
				}
			}
			else if (touch.phase == TouchPhase.Stationary)
			{
				GUIPad rightGUIPadStyle = RightGUIPadStyle;
				if (rightGUIPadStyle == GUIPad.Slide && Right.FingerDown && Right.FingerId == touch.fingerId)
				{
					Window2InputSlide = Vector2.zero;
				}
			}
			else
			{
				if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
				{
					continue;
				}
				if (LeftGUIPadStyle == GUIPad.AnalogPad && Left.FingerDown && Left.FingerId == touch.fingerId)
				{
					Window1InputPad = Vector2.zero;
					Left.FingerDown = false;
					Left.FingerId = -1;
					Window1PadColor = TranspColor;
				}
				GUIPad rightGUIPadStyle = RightGUIPadStyle;
				if (rightGUIPadStyle != GUIPad.Slide)
				{
					continue;
				}
				if (Right.FingerDown && Right.FingerId == touch.fingerId)
				{
					Window2InputSlide = Vector2.zero;
					Right.FingerDown = false;
					Right.FingerId = -1;
					FirePos btnFirePos = BtnFirePos;
					if (btnFirePos == FirePos.Window2 && /*!GameManager.Instance.autoFire*/ true && Fire.FingerId == touch.fingerId)
					{
						WindowFireBtnPressed = false;
						Fire.FingerDown = false;
						Fire.FingerId = -1;
					}
					ReloadPos btnReloadPos = BtnReloadPos;
					if (btnReloadPos == ReloadPos.Window2 && Reload.FingerId == touch.fingerId)
					{
						WindowReloadBtnPressed = false;
						Reload.FingerDown = false;
						Reload.FingerId = -1;
					}
					JumpPos btnJumpPos = BtnJumpPos;
					if (btnJumpPos == JumpPos.Window2 && Jump.FingerId == touch.fingerId)
					{
						WindowJumpBtnPressed = false;
						Jump.FingerDown = false;
						Jump.FingerId = -1;
					}
				}
				else
				{
					FirePos btnFirePos = BtnFirePos;
					if (btnFirePos == FirePos.Window2 && /*!GameManager.Instance.autoFire*/ true && Fire.FingerId == touch.fingerId)
					{
						WindowFireBtnPressed = false;
						Fire.FingerDown = false;
						Fire.FingerId = -1;
					}
					ReloadPos btnReloadPos = BtnReloadPos;
					if (btnReloadPos == ReloadPos.Window2 && Reload.FingerId == touch.fingerId)
					{
						WindowReloadBtnPressed = false;
						Reload.FingerDown = false;
						Reload.FingerId = -1;
					}
					JumpPos btnJumpPos = BtnJumpPos;
					if (btnJumpPos == JumpPos.Window2 && Jump.FingerId == touch.fingerId)
					{
						WindowJumpBtnPressed = false;
						Jump.FingerDown = false;
						Jump.FingerId = -1;
					}
				}
			}
		}
	}

	public void OnGUIComponents()
	{
		if (PCControls.OnPC)
		{
			return;
		}
		if (LeftGUIPadStyle == GUIPad.AnalogPad)
		{
			GUI.color = Window1PadColor;
			GUI.DrawTexture(Left.FingerSocleRect, GUIObject.SocleGUI);
			GUI.DrawTexture(Left.FingerStickRect, GUIObject.StickGUI);
		}
		else if (Window1PadColor != TranspColor)
		{
			GUI.color = TranspColor;
		}
		if (RightGUIPadStyle == GUIPad.AnalogPad)
		{
			GUI.color = Window2PadColor;
			GUI.DrawTexture(Right.FingerSocleRect, GUIObject.SocleGUI);
			GUI.DrawTexture(Right.FingerStickRect, GUIObject.StickGUI);
		}
		else if (Window2PadColor != TranspColor)
		{
			GUI.color = TranspColor;
		}
		GUI.color = GUIColor;
		if (/*!GameManager.Instance.autoFire*/true)
		{
			GUI.DrawTexture(new Rect(Fire.FingerBound.x, scrH - FixedPos.y - 64f, Fire.FingerBound.width, Fire.FingerBound.height), GUIObject.FireGui);
		}
		GUI.DrawTexture(new Rect(Reload.FingerBound.x, scrH - FixedPos2.y - 64f, Reload.FingerBound.width, Reload.FingerBound.height), GUIObject.ReloadGui);
		GUI.DrawTexture(new Rect(Jump.FingerBound.x, scrH - FixedPos3.y - 64f, Jump.FingerBound.width, Jump.FingerBound.height), GUIObject.JumpGui);
	}

	public void PauseMotor()
	{
		Window1InputPad = Vector2.zero;
		Left.FingerDown = false;
		Left.FingerId = -1;
		Window1PadColor = TranspColor;
		Window2InputSlide = Vector2.zero;
		Right.FingerDown = false;
		Right.FingerId = -1;
		WindowFireBtnPressed = false;
		Fire.FingerDown = false;
		Fire.FingerId = -1;
		WindowReloadBtnPressed = false;
		Reload.FingerDown = false;
		Reload.FingerId = -1;
		WindowJumpBtnPressed = false;
		Jump.FingerDown = false;
		Jump.FingerId = -1;
	}
}
