using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	public GameObject[] scene;

	public GameObject[] obj;

	public GameObject[] panel;

	public GameObject[] touchLight;

	public GameObject[] touchHere;

	public GameObject[] weaponBlock;

	public int eventNum;

	private bool isSkip;

	private IEnumerator flow;

	private void Start()
	{
		eventNum = 0;
		PlayEvent(eventNum);
	}

	private void PlayEvent(int num)
	{
		switch (num)
		{
		case 0:
			StartCoroutine("OnBlink", touchLight[0].GetComponent<UISprite>());
			StartCoroutine("OnActive", touchHere[0]);
			break;
		case 1:
			StartCoroutine("OnBlink", touchLight[1].GetComponent<UISprite>());
			StartCoroutine("OnActive", touchHere[1]);
			break;
		case 2:
			StartCoroutine("OnBlink", touchLight[2].GetComponent<UISprite>());
			StartCoroutine("OnActive", touchHere[2]);
			break;
		case 3:
			StartCoroutine("OnBlink", touchLight[3]);
			break;
		case 4:
			StartCoroutine("OnBlink", touchLight[4]);
			break;
		case 5:
			StartCoroutine("OnBlink", touchLight[5].GetComponent<UISprite>());
			StartCoroutine("OnActive", touchHere[5]);
			break;
		case 6:
			StartCoroutine("OnBlink", touchLight[6]);
			break;
		case 7:
			StartCoroutine("OnBlink", touchLight[7].GetComponent<UISprite>());
			StartCoroutine("OnActive", touchHere[7]);
			break;
		case 8:
			StartCoroutine("OnBlink", touchLight[8]);
			break;
		case 9:
			StartCoroutine("OnBlink", touchLight[9].GetComponent<UISprite>());
			StartCoroutine("OnActive", touchHere[9]);
			break;
		case 10:
			StartCoroutine("OnBlink", touchLight[10].GetComponent<UISprite>());
			StartCoroutine("OnActive", touchHere[10]);
			break;
		}
	}

	private IEnumerator OnBlink(UISprite sprite)
	{
		yield return new WaitForSeconds(0.25f);
		while (true)
		{
			sprite.color = new Color(1f, 0f, 0f, 1f);
			yield return new WaitForSeconds(0.5f);
			sprite.color = new Color(1f, 0f, 0f, 0f);
			yield return new WaitForSeconds(0.5f);
		}
	}

	private IEnumerator OnBlink(GameObject target)
	{
		yield return new WaitForSeconds(0.25f);
		while (true)
		{
			target.SetActive(true);
			yield return new WaitForSeconds(0.5f);
			target.SetActive(false);
			yield return new WaitForSeconds(0.5f);
		}
	}

	private IEnumerator OnActive(GameObject target)
	{
		yield return new WaitForSeconds(0.25f);
		target.SetActive(true);
	}

	private void FinishTutorial(int num)
	{
		Debug.Log("FinishTutorial : " + num);
		StopCoroutine("OnBlink");
		touchLight[num].SetActive(false);
		switch (num)
		{
		case 0:
			scene[0].SetActive(false);
			scene[1].SetActive(true);
			obj[0].SetActive(false);
			touchHere[0].SetActive(false);
			break;
		case 1:
			panel[0].SetActive(false);
			panel[1].SetActive(true);
			obj[1].SetActive(true);
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Sprite").GetComponent<UISprite>().spriteName = null;
			panel[1].transform.Find("Anchor - Top/WeaponIndicator/Label1_Damage").GetComponent<UILabel>().text = "80.00";
			panel[1].transform.Find("Anchor - Top/WeaponIndicator/Label2_FireRate").GetComponent<UILabel>().text = "1.00";
			panel[1].transform.Find("Anchor - Top/WeaponIndicator/Label3_Bullet").GetComponent<UILabel>().text = "10";
			panel[1].transform.Find("Anchor - Top/BlockPointIndicator/Label_BlockPoint").GetComponent<UILabel>().text = "0/10";
			touchHere[1].SetActive(false);
			break;
		case 2:
			panel[1].transform.Find("Anchor - Bottom/ItemSlots/Button0").GetComponent<Collider>().enabled = false;
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Sprite").GetComponent<UISprite>().spriteName = "b001";
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Label0_Damage").GetComponent<UILabel>().text = "DAMAGE   :3";
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Label1_FireRate").GetComponent<UILabel>().text = "FIRE RATE:4";
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Label2_Bullet").GetComponent<UILabel>().text = "BULLET   :0";
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Label3_Cost").GetComponent<UILabel>().text = "BP. COST :2";
			obj[1].transform.Find("ItemGO/Weapon1/Weapon1Part/WeaponBlock0").GetComponent<Collider>().enabled = true;
			touchHere[2].SetActive(false);
			break;
		case 3:
			obj[1].transform.Find("ItemGO/Weapon1/Weapon1Part/WeaponBlock0").GetComponent<Collider>().enabled = false;
			weaponBlock[0].SetActive(true);
			panel[1].transform.Find("Anchor - Top/WeaponIndicator/Label1_Damage").GetComponent<UILabel>().text = "83.00";
			panel[1].transform.Find("Anchor - Top/WeaponIndicator/Label2_FireRate").GetComponent<UILabel>().text = "3.00";
			panel[1].transform.Find("Anchor - Top/WeaponIndicator/Label3_Bullet").GetComponent<UILabel>().text = "10";
			panel[1].transform.Find("Anchor - Top/BlockPointIndicator/Label_BlockPoint").GetComponent<UILabel>().text = "2/10";
			panel[1].transform.Find("Anchor - Bottom/ItemSlots/Button0/Label").GetComponent<UILabel>().text = "1";
			obj[1].transform.Find("ItemGO/Weapon1/Weapon1Part/WeaponBlock1").GetComponent<Collider>().enabled = true;
			break;
		case 4:
			obj[1].transform.Find("ItemGO/Weapon1/Weapon1Part/WeaponBlock1").GetComponent<Collider>().enabled = false;
			weaponBlock[1].SetActive(true);
			panel[1].transform.Find("Anchor - Top/WeaponIndicator/Label1_Damage").GetComponent<UILabel>().text = "86.00";
			panel[1].transform.Find("Anchor - Top/WeaponIndicator/Label2_FireRate").GetComponent<UILabel>().text = "3.00";
			panel[1].transform.Find("Anchor - Top/WeaponIndicator/Label3_Bullet").GetComponent<UILabel>().text = "10";
			panel[1].transform.Find("Anchor - Top/BlockPointIndicator/Label_BlockPoint").GetComponent<UILabel>().text = "4/10";
			panel[1].transform.Find("Anchor - Bottom/ItemSlots/Button0/Label").GetComponent<UILabel>().text = "0";
			panel[1].transform.Find("Anchor - Bottom/ItemSlots/Button10").GetComponent<Collider>().enabled = true;
			break;
		case 5:
			panel[1].transform.Find("Anchor - Bottom/ItemSlots/Button10").GetComponent<Collider>().enabled = false;
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Sprite").GetComponent<UISprite>().spriteName = "blk_hole";
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Label0_Damage").GetComponent<UILabel>().text = string.Empty;
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Label1_FireRate").GetComponent<UILabel>().text = string.Empty;
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Label2_Bullet").GetComponent<UILabel>().text = string.Empty;
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Label3_Cost").GetComponent<UILabel>().text = string.Empty;
			obj[1].transform.Find("ItemGO/Weapon1/Weapon1Part/WeaponBlock2").GetComponent<Collider>().enabled = true;
			touchHere[5].SetActive(false);
			break;
		case 6:
			weaponBlock[2].SetActive(true);
			obj[1].transform.Find("ItemGO/Weapon1/Weapon1Part/WeaponBlock2").GetComponent<Collider>().enabled = false;
			panel[1].transform.Find("Anchor - Bottom/ItemSlots/Button11").GetComponent<Collider>().enabled = true;
			break;
		case 7:
			panel[1].transform.Find("Anchor - Bottom/ItemSlots/Button11").GetComponent<Collider>().enabled = false;
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Sprite").GetComponent<UISprite>().spriteName = "blk_eyepin";
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Label0_Damage").GetComponent<UILabel>().text = string.Empty;
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Label1_FireRate").GetComponent<UILabel>().text = string.Empty;
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Label2_Bullet").GetComponent<UILabel>().text = string.Empty;
			panel[1].transform.Find("Anchor - Bottom/SelectBlockIndicator/Label3_Cost").GetComponent<UILabel>().text = string.Empty;
			obj[1].transform.Find("ItemGO/Weapon1/Weapon1Part/WeaponBlock3").GetComponent<Collider>().enabled = true;
			touchHere[7].SetActive(false);
			break;
		case 8:
			weaponBlock[3].SetActive(true);
			obj[1].transform.Find("ItemGO/Weapon1/Weapon1Part/WeaponBlock3").GetComponent<Collider>().enabled = false;
			panel[1].transform.Find("Anchor - Bottom/BackButton").GetComponent<Collider>().enabled = true;
			break;
		case 9:
			obj[1].transform.Find("RootGO").GetComponent<TouchLook>().enabled = false;
			panel[2].SetActive(true);
			touchHere[9].SetActive(false);
			break;
		case 10:
			FinishTutorial();
			touchHere[10].SetActive(false);
			break;
		}
		eventNum++;
		if (eventNum <= touchLight.Length - 1)
		{
			PlayEvent(eventNum);
		}
	}

	private void FinishTutorial()
	{
		Debug.Log("FinishTutorial");
		Application.LoadLevel("GameMain");
	}
}
