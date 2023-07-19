using UnityEngine;

public class AudioManager : MonoBehaviour
{
	private static AudioManager instance;

	public AudioSource soundBgm;

	public AudioSource soundEffect;

	public AudioSource soundEffect1;

	public AudioSource soundEffect2;

	public AudioSource soundEffect3;

	public AudioClip BGM_Menu;

	public AudioClip[] BGM_inGame;

	public AudioClip SFX_HeadShot;

	public AudioClip SFX_Throw;

	public AudioClip[] SFX_PickUp;

	public SOUND sound = SOUND.NONE;

	public static AudioManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.Find("_AudioManager").GetComponent<AudioManager>();
			}
			return instance;
		}
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void InitAudio()
	{
		bool sndBgm = DataManager.Instance.gamePrefs.sndBgm;
		bool sndEffect = DataManager.Instance.gamePrefs.sndEffect;
		if (sndBgm)
		{
			soundBgm.mute = false;
		}
		else
		{
			soundBgm.mute = true;
		}
		if (sndEffect)
		{
			soundEffect.mute = false;
			soundEffect1.mute = false;
			soundEffect2.mute = false;
			soundEffect3.mute = false;
		}
		else
		{
			soundEffect.mute = true;
			soundEffect1.mute = true;
			soundEffect2.mute = true;
			soundEffect3.mute = true;
		}
	}

	public void OnPlayBgm(SOUND snd)
	{
		switch (snd)
		{
		case SOUND.BGM:
			soundBgm.clip = BGM_Menu;
			soundBgm.Play();
			break;
		case SOUND.PLAY:
		{
			string loadedLevelName = Application.loadedLevelName;
			int num = 0;
			num = int.Parse(loadedLevelName.Substring(13, 1)) - 1;
			soundBgm.clip = BGM_inGame[num];
			soundBgm.Play();
			break;
		}
		}
	}

	public void OnPlayEffect()
	{
		soundEffect.PlayOneShot(SFX_HeadShot);
	}

	public void OnPlayEffect1()
	{
		soundEffect1.PlayOneShot(SFX_Throw);
	}

	public void OnPlayEffect2(AudioClip clip)
	{
		soundEffect2.PlayOneShot(clip);
	}

	public void OnPlayEffect3(int num)
	{
		soundEffect3.PlayOneShot(SFX_PickUp[num]);
	}
}
