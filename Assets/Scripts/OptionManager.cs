using UnityEngine;

public class OptionManager : MonoBehaviour
{
	public UIToggle[] soundBgm = new UIToggle[2];

	public UIToggle[] soundEffect = new UIToggle[2];

	public UISlider sensiBar;

	public void InitOption()
	{
		bool sndBgm = DataManager.Instance.gamePrefs.sndBgm;
		bool sndEffect = DataManager.Instance.gamePrefs.sndEffect;
		float value = (DataManager.Instance.gamePrefs.sensitivity - 3f) * 0.2f + 0.5f;
		if (sndBgm)
		{
			soundBgm[0].value = true;
		}
		else
		{
			soundBgm[1].value = true;
		}
		if (sndEffect)
		{
			soundEffect[0].value = true;
		}
		else
		{
			soundEffect[1].value = true;
		}
		sensiBar.value = value;
	}

	public void OnSoundBgm()
	{
		DataManager.Instance.gamePrefs.sndBgm = soundBgm[0].value;
		AudioManager.Instance.InitAudio();
	}

	public void OnSoundEffect()
	{
		DataManager.Instance.gamePrefs.sndEffect = soundEffect[0].value;
		AudioManager.Instance.InitAudio();
	}

	public void OnSensitivity()
	{
		float sensitivity = (sensiBar.value - 0.5f) * 5f + 3f;
		DataManager.Instance.gamePrefs.sensitivity = sensitivity;
	}
}
