using System;
using System.Collections;
using UnityEngine;
using Easing;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
	[Serializable]
	public class WeaponClass
	{
		public bool firearms;

		public float fireRate;

		public int bulletperClip;

		public float WeaponPower;

		public float distance;

		public Transform FirePos;

		public Transform KickGO;

		public Transform KickGO2;

		public float KickUpside;

		public float KickSideways;

		public Renderer Muzzle;

		public AudioClip shootSound;

		public AudioClip reloadSound;

		[NonSerialized]
		public int bulletleft;

		[NonSerialized]
		public float nextFireTime;

		[NonSerialized]
		public float bulletinMagasine;

		public float baseInaccuracyHIP = 1.5f;

		public float inaccuracyIncreaseOverTime = 0.2f;

		public float inaccuracyDecreaseOverTime = 0.5f;

		public float maxInaccuracyHIP = 5f;
	}

	[SerializeField]
	public FPSMotor FPSControl = new FPSMotor();

	public GameObject Sparkle_Hit;

	public GameObject Sparkle_Other;

	public Transform PlayerCam;

	public float Sensitivity = 2f;

	public float moveSpeed;

	private Vector3 moveDirection = Vector3.zero;

	private float jumpSpeed = 4f;

	private float gravity = 8f;

	private CharacterController controller;

	private float rotationX;

	private float rotationY;

	private float sensitivityX = 2000f;

	private float sensitivityY = 900f;

	private float minimumY = -60f;

	private float maximumY = 60f;

	private float originalRotation;

	private Transform myTransform;

	private RaycastHit hit;

	[NonSerialized]
	public float InputX;

	[NonSerialized]
	public float InputY;

	[NonSerialized]
	public int Grenade;

	[NonSerialized]
	public float Health;

	[NonSerialized]
	public float Shield;

	public float MaxHealth;

	public float MaxShield;

	public WeaponClass CurrentWeapon;

	[SerializeField]
	public WeaponClass[] WeaponList;

	public LayerMask collisionLayers = -1;

	private bool isJump;

	private bool isFiring;

	public bool reload;

	private bool reloadLight;

	public GameObject[] weaponObject;

	public Animation weaponAnim;

	public Animation fallAnim;

	public Animation playerAnim;

	public Animation mainAnim;

	public GameObject bulletHole;

	private float muzzleRotate;

	public Texture2D crosshairFirstModeHorizontal;

	public Texture2D crosshairFirstModeVertical;

	private float adjustMaxCrosshairSize = 6f;

	private bool isAiming;

	private float baseInaccuracy;

	private float maximumInaccuracy;

	private float triggerTime = 0.05f;

	public GameObject grenadeObject;

	public Transform grenadePos;

	public float grenadeForce;

	public Vector2 bobMult = new Vector2(0.05f, 0.1f), bobSpeed = new Vector2(0.5f, 0.25f);

	public void InitPlayerState()
	{
		WeaponClass[] weaponList = WeaponList;
		foreach (WeaponClass weaponClass in weaponList)
		{
			weaponClass.bulletinMagasine = (weaponClass.bulletleft = weaponClass.bulletperClip);
			weaponClass.bulletleft *= 4;
		}
		Grenade = 5;
		Health = MaxHealth;
		Shield = 0f;
	}

	private void Start()
	{
		Screen.sleepTimeout = -1;
		FPSControl.InitControl();
		controller = GetComponent<CharacterController>();
		originalRotation = base.transform.rotation.eulerAngles.y;
		myTransform = base.transform;
		CurrentWeapon = WeaponList[1];
		weaponAnim["AttackShoot"].layer = 1;
		weaponAnim["AttackMelee"].layer = 1;
		weaponAnim["Reload"].layer = 1;
		Sensitivity = DataManager.Instance.gamePrefs.sensitivity;
		if (PCControls.OnPC)
		{
			PCControls.CursorLocked = true;
		}
	}

	private float muzzleTimer;

	private float muzzleCooldown = 0.025f;

	private void Update()
	{
		if (!GameManager.Instance.isDown && GameManager.Instance.state != STATE.PAUSE)
		{
			FPSControl.Command();
			MovePlayer();
			RotatePlayer();
			OthersPlayerControl();
			AimSensor();
			CheckMuzzle();
		}
	}

	private void CheckMuzzle()
	{
		if (CurrentWeapon.Muzzle != null && CurrentWeapon.Muzzle.enabled)
		{
			if (muzzleTimer <= 0f)
			{
				DisableMuzzle();
			}
			else
			{
				muzzleTimer -= 1 * Time.deltaTime;
			}
		}
	}

	private void OnGUI()
	{
		if (!GameManager.Instance.isDown && GameManager.Instance.state != STATE.PAUSE)
		{
			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, FPSControl.scale);
			FPSControl.OnGUIComponents();
			if (isAiming)
			{
				GUI.color = Color.red;
			}
			else
			{
				GUI.color = Color.white;
			}
			Crosshair();
		}
	}

	private void Crosshair()
	{
		maximumInaccuracy = CurrentWeapon.maxInaccuracyHIP;
		baseInaccuracy = CurrentWeapon.baseInaccuracyHIP;
		if (isFiring)
		{
			triggerTime += CurrentWeapon.inaccuracyIncreaseOverTime;
		}
		else
		{
			triggerTime -= CurrentWeapon.inaccuracyDecreaseOverTime;
		}
		if (triggerTime >= maximumInaccuracy)
		{
			triggerTime = maximumInaccuracy;
		}
		if (triggerTime <= baseInaccuracy)
		{
			triggerTime = baseInaccuracy;
		}
		int width = crosshairFirstModeHorizontal.width;
		int height = crosshairFirstModeHorizontal.height;
		Rect position = new Rect((FPSControl.scrW + (float)width) * 0.5f + triggerTime * adjustMaxCrosshairSize, (FPSControl.scrH - (float)height) * 0.5f, width, height);
		Rect position2 = new Rect((FPSControl.scrW - (float)width) * 0.5f, (FPSControl.scrH + (float)height) * 0.5f + triggerTime * adjustMaxCrosshairSize, width, height);
		Rect position3 = new Rect((FPSControl.scrW - (float)width) * 0.5f - triggerTime * adjustMaxCrosshairSize - (float)width, (FPSControl.scrH - (float)height) * 0.5f, width, height);
		Rect position4 = new Rect((FPSControl.scrW - (float)width) * 0.5f, (FPSControl.scrH - (float)height) * 0.5f - triggerTime * adjustMaxCrosshairSize - (float)height, width, height);
		GUI.DrawTexture(position, crosshairFirstModeHorizontal);
		GUI.DrawTexture(position2, crosshairFirstModeVertical);
		GUI.DrawTexture(position3, crosshairFirstModeHorizontal);
		GUI.DrawTexture(position4, crosshairFirstModeVertical);
	}

	private void RotatePlayer()
	{
		if (!PCControls.OnPC)
		{
			InputX = FPSControl.Window2InputSlide.x * (sensitivityX * Sensitivity * 0.01f) / FPSControl.DPI;
			InputY = FPSControl.Window2InputSlide.y * (sensitivityY * Sensitivity * 0.01f) / FPSControl.DPI;
		}
		else if (PCControls.CursorLocked)
		{
			InputX = (Input.GetAxis("Mouse X") * (sensitivityX * Sensitivity * 0.01f) / FPSControl.DPI) * 0.03f;
			InputY = (Input.GetAxis("Mouse Y") * (sensitivityY * Sensitivity * 0.01f) / FPSControl.DPI) * 0.03f;
		}
		rotationX += InputX;
		rotationY += InputY;
		rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, originalRotation + rotationX, 0f), 0.5f);
		PlayerCam.localRotation = Quaternion.Slerp(PlayerCam.localRotation, Quaternion.Euler(PlayerCam.localRotation.x - rotationY, 0f, 0f), 0.5f);
	}

	public Vector2 UpdateKeyboardControls()
	{
		float num = default(float);
		float num2 = default(float);
		if (Input.GetKey("s"))
		{
			num = -1f;
		}
		if (Input.GetKey("w"))
		{
			num = 1f;
		}
		if (Input.GetKey("d"))
		{
			num2 = 1f;
		}
		if (Input.GetKey("a"))
		{
			num2 = -1f;
		}
		return new Vector2(num2, num);
	}

	private void MovePlayer()
	{
		if (controller.isGrounded)
		{
			if (!PCControls.OnPC)
			{
				float num = ((FPSControl.Window1InputPad.x == 0f || FPSControl.Window1InputPad.y == 0f) ? 1f : 0.7071f);
				moveDirection = myTransform.TransformDirection(new Vector3(FPSControl.Window1InputPad.x * num, 0f, FPSControl.Window1InputPad.y * num));
			}
			else
			{
				moveDirection = myTransform.TransformDirection(UpdateKeyboardControls().x, 0f, UpdateKeyboardControls().y);
				if (moveDirection.magnitude > 1)
				{
					moveDirection /= moveDirection.magnitude;
				}
			}
			moveDirection *= moveSpeed;
			if (moveDirection != Vector3.zero)
			{
				playerAnim.transform.localPosition = Vector3.Lerp(playerAnim.transform.localPosition, new Vector3(-((EaseInOut.Sine((bobSpeed.y * moveSpeed) * Time.time) * bobMult.y) - (0.5f * bobMult.y)), EaseInOut.Sine((bobSpeed.x * moveSpeed) * Time.time) * bobMult.x, 0), (Mathf.Max(bobSpeed.x, bobSpeed.y) * 20f) * Time.deltaTime);
			}
			if (isJump)
			{
				isJump = false;
				fallAnim.Play("Fall");
			}
			if (FPSControl.WindowJumpBtnPressed || PCControls.OnPC && Input.GetKeyDown(KeyCode.Space))
			{
				Jump();
			}
		}
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.smoothDeltaTime);
	}

	private void OthersPlayerControl()
	{
		float num = CurrentWeapon.maxInaccuracyHIP * FPSControl.scale.x;
		Ray ray = PlayerCam.GetComponent<Camera>().ScreenPointToRay(new Vector3(FPSControl.HalfScreen.x + UnityEngine.Random.Range(0f - num, num) * triggerTime, FPSControl.HalfScreen.y + UnityEngine.Random.Range(0f - num, num) * triggerTime, 1f));
		Physics.Raycast(ray, out hit, CurrentWeapon.distance);
		if (!PCControls.OnPC)
		{
			if (/*GameManager.Instance.autoFire*/false)
			{
				if (hit.transform != null)
				{
					if ((hit.collider.tag == "Enemy" || hit.collider.tag == "EnemyHead") && !reload)
					{
						Shoot();
					}
				}
			}
			else if (FPSControl.WindowFireBtnPressed && !reload)
			{
				Shoot();
			}
			if (FPSControl.WindowReloadBtnPressed && !reload)
			{
				StartCoroutine(Reload());
			}
		}
		else
		{
			if (!reload && Input.GetMouseButton(0) && PCControls.CursorLocked)
			{
				Shoot();
			}
			if (Input.GetKeyDown(KeyCode.R) && !reload)
			{
				StartCoroutine(Reload());
			}
			if (Input.GetKeyDown(KeyCode.G))
			{
				GrenadeFire();
			}
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				GameManager.Instance.WeaponChangeForPC(true);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				GameManager.Instance.WeaponChangeForPC(false);
			}
		}
	}

	private void AimSensor()
	{
		Ray ray = PlayerCam.GetComponent<Camera>().ScreenPointToRay(new Vector3(FPSControl.HalfScreen.x, FPSControl.HalfScreen.y, 1f));
		RaycastHit hitInfo;
		Physics.Raycast(ray, out hitInfo, CurrentWeapon.distance);
		if (hitInfo.transform != null)
		{
			if (hitInfo.collider.tag == "Enemy" || hitInfo.collider.tag == "EnemyHead")
			{
				isAiming = true;
			}
			else
			{
				isAiming = false;
			}
		}
		else if (isAiming)
		{
			isAiming = false;
		}
	}

	private void DisableMuzzle()
	{
		if ((bool)CurrentWeapon.Muzzle)
		{
			CurrentWeapon.Muzzle.enabled = false;
		}
	}

	private void Shoot()
	{
		if (CurrentWeapon.firearms)
		{
			if (Time.time > CurrentWeapon.nextFireTime && !isFiring)
			{
				if (CurrentWeapon.bulletinMagasine <= 0f && !reload)
				{
					StartCoroutine(Reload());
					return;
				}
				isFiring = true;
				weaponAnim.Play("AttackShoot");
				CurrentWeapon.bulletinMagasine -= 1f;
				GameManager.Instance.useBullets++;
				if (hit.transform != null)
				{
					if (hit.collider.tag == "Enemy")
					{
						hit.transform.SendMessage("HitDamage", CurrentWeapon.WeaponPower, SendMessageOptions.DontRequireReceiver);
						ObjectPoolManager.GetObject(Sparkle_Hit, hit.point, Quaternion.identity);
						GameManager.Instance.hitBullets++;
					}
					else if (hit.collider.tag == "EnemyHead")
					{
						hit.transform.root.SendMessage("HitDamageHead", CurrentWeapon.WeaponPower, SendMessageOptions.DontRequireReceiver);
						ObjectPoolManager.GetObject(Sparkle_Hit, hit.point, Quaternion.identity);
						GameManager.Instance.hitBullets++;
					}
					else if (hit.collider.tag == "Untagged")
					{
						ObjectPoolManager.GetObject(Sparkle_Other, hit.point, Quaternion.identity);
						Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
						ObjectPoolManager.GetObject(bulletHole, hit.point, rotation);
					}
				}
				muzzleRotate += 28f;
				CurrentWeapon.Muzzle.gameObject.transform.localRotation = Quaternion.AngleAxis(muzzleRotate, Vector3.forward);
				if ((bool)CurrentWeapon.Muzzle)
				{
					muzzleTimer = muzzleCooldown;
					CurrentWeapon.Muzzle.enabled = true;
				}
				AudioManager.Instance.OnPlayEffect2(CurrentWeapon.shootSound);
				GameManager.Instance.UpdateAmmoGUI();
				CurrentWeapon.nextFireTime = Time.time + CurrentWeapon.fireRate;
				KickBack();
			}
			else
			{
				isFiring = false;
			}
		}
		else if (Time.time > CurrentWeapon.nextFireTime && !isFiring)
		{
			isFiring = true;
			weaponAnim.Play("AttackMelee");
			StartCoroutine(MeleeAttack());
			AudioManager.Instance.OnPlayEffect2(CurrentWeapon.shootSound);
			CurrentWeapon.nextFireTime = Time.time + CurrentWeapon.fireRate;
		}
		else
		{
			isFiring = false;
		}
	}

	private IEnumerator MeleeAttack()
	{
		int lastIndex = GameManager.Instance.WeaponIndex;
		yield return new WaitForSeconds(0.333333f);
		if (lastIndex != GameManager.Instance.WeaponIndex)
		{
			yield break;
		}
		if (hit.transform != null)
		{
			if (hit.collider.tag == "Enemy")
			{
				hit.transform.SendMessage("HitDamage", CurrentWeapon.WeaponPower, SendMessageOptions.DontRequireReceiver);
				ObjectPoolManager.GetObject(Sparkle_Hit, hit.point, Quaternion.identity);
			}
			else if (hit.collider.tag == "EnemyHead")
			{
				hit.transform.root.SendMessage("HitDamageHead", CurrentWeapon.WeaponPower, SendMessageOptions.DontRequireReceiver);
				ObjectPoolManager.GetObject(Sparkle_Hit, hit.point, Quaternion.identity);
			}
			else if (hit.transform.tag == "Untagged")
			{
				ObjectPoolManager.GetObject(Sparkle_Other, hit.point, Quaternion.identity);
			}
		}
	}

	private void GrenadeFire()
	{
		if (Grenade > 0)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(grenadeObject, grenadePos.transform.position, Quaternion.identity) as GameObject;
			gameObject.GetComponent<Rigidbody>().AddForce(grenadePos.forward * grenadeForce);
			Grenade--;
			AudioManager.Instance.OnPlayEffect1();
			GameManager.Instance.UpdateGrenadeGUI();
		}
	}

	private IEnumerator Reload()
	{
		if (CurrentWeapon.bulletinMagasine < (float)CurrentWeapon.bulletperClip && CurrentWeapon.bulletleft > 0)
		{
			reload = true;
			isFiring = false;
			weaponAnim.Play("Reload");
			AudioManager.Instance.OnPlayEffect2(CurrentWeapon.reloadSound);
			CurrentWeapon.nextFireTime = 0f;
			CurrentWeapon.bulletleft += (int)CurrentWeapon.bulletinMagasine;
			CurrentWeapon.bulletinMagasine = 0f;
			if (CurrentWeapon.bulletleft > CurrentWeapon.bulletperClip)
			{
				yield return new WaitForSeconds(2f);
				CurrentWeapon.bulletinMagasine = CurrentWeapon.bulletperClip;
				CurrentWeapon.bulletleft -= CurrentWeapon.bulletperClip;
			}
			else
			{
				yield return new WaitForSeconds(2f);
				CurrentWeapon.bulletinMagasine = CurrentWeapon.bulletleft;
				CurrentWeapon.bulletleft = 0;
			}
			GameManager.Instance.UpdateAmmoGUI();
		}
		reload = false;
	}

	private void Jump()
	{
		FPSControl.WindowJumpBtnPressed = false;
		isJump = true;
		moveDirection.y = jumpSpeed;
	}

	public void HitDamage(DamageAngle _damageAngle)
	{
		if (!GameManager.Instance.isDown)
		{
			if (Shield > 0f)
			{
				Shield -= 1f;
				GameManager.Instance.UpdateShieldGUI();
			}
			else
			{
				Health -= _damageAngle.damage;
				GameManager.Instance.UpdateHealthGUI();
			}
			if (Health <= 0f)
			{
				Health = 0f;
				GameManager.Instance.state = STATE.DOWN;
			}
			else
			{
				GameManager.Instance.hitPos = _damageAngle.position;
				GameManager.Instance.state = STATE.DAMAGE;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Stuff")
		{
			other.GetComponent<Item>().inside = false;
			int num = int.Parse(other.name.Substring(6, 3)) - 1;
			DataManager.Instance.gamePrefs.blockMaterial[num]++;
			StartCoroutine("DestroyObject", other.gameObject);
			AudioManager.Instance.OnPlayEffect3(0);
		}
		else if (other.tag == "AmmoPack")
		{
			if (CurrentWeapon.firearms)
			{
				CurrentWeapon.bulletleft += CurrentWeapon.bulletperClip;
				GameManager.Instance.UpdateAmmoGUI();
				StartCoroutine("DestroyObject", other.gameObject);
				AudioManager.Instance.OnPlayEffect3(1);
			}
		}
		else if (other.tag == "HealthPack")
		{
			int num2 = (int)Health + 1;
			if ((float)num2 > MaxHealth)
			{
				Health = MaxHealth;
			}
			else
			{
				Health = num2;
			}
			GameManager.Instance.UpdateHealthGUI();
			GameManager.Instance.ScreenFlashEvent();
			StartCoroutine("DestroyObject", other.gameObject);
			AudioManager.Instance.OnPlayEffect3(2);
		}
		else if (other.tag == "Gold")
		{
			Debug.Log("Get Gold!");
		}
	}

	private IEnumerator DestroyObject(GameObject obj)
	{
		UnityEngine.Object.Destroy(obj);
		yield return null;
	}

	private void KickBack()
	{
		CurrentWeapon.KickGO.localRotation = Quaternion.Euler(CurrentWeapon.KickGO.localRotation.eulerAngles - new Vector3(CurrentWeapon.KickUpside, UnityEngine.Random.Range(0f - CurrentWeapon.KickSideways, CurrentWeapon.KickSideways), 0f));
		CurrentWeapon.KickGO2.localPosition = new Vector3(0f, 0f, CurrentWeapon.KickGO2.localPosition.z - CurrentWeapon.KickUpside * 0.2f);
	}
}
