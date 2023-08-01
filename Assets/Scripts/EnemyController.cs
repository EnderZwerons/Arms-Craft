using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	private enum STATE
	{
		SEARCH,
		CHARGE,
		ATTACK,
		DEATH,
		IDLE
	}

	private STATE state;

	private FPSController fpsController;

	private UnityEngine.AI.NavMeshAgent agent;

	private Animation enemyAnim;

	private Transform target;

	private Transform myTransform;

	private float prevTime;

	private float dist;

	private DamageAngle damageAngle;

	public bool tracker;

	public float health;

	public float damage;

	public float attackSpeed;

	public float moveSpeed;

	public float monsterAI;

	public float chargeRange;

	public float attackRange;

	public float goldAmt;

	public int dropTier;

	private float moveTime;

	[HideInInspector] public Material mainMaterial;

	private Vector2 normalColor = new Vector2(0f, 0f);

	private Vector2 hitColor = new Vector2(0f, 0.5f);

	private Vector2 point;

	private bool isDown;

	private bool isDeath;

	public string idleAnim;

	public string moveAnim;

	public string attackAnim;

	public string death1Anim;

	public string death2Anim;

	public string deathHeadAnim;

	public AudioSource audioSource;

	public AudioClip enemyAttack;

	public AudioClip enemyHit;

	public AudioClip enemyDeath;

	public Transform headEffectPos;

	public LayerMask layerMask;

	private float atkReadyTime;

	private bool isAtk;

	private void Awake()
	{
		enemyAnim = GetComponentInChildren<Animation>();
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		mainMaterial = GetComponentInChildren<SkinnedMeshRenderer>().material;
		target = GameObject.FindWithTag("Player").transform;
		myTransform = base.transform;
		fpsController = target.GetComponent<FPSController>();
		damageAngle.damage = damage;
		enemyAnim[attackAnim].layer = 1;
		if (tracker)
		{
			state = STATE.CHARGE;
		}
		else
		{
			state = STATE.SEARCH;
		}
	}

	private void FixedUpdate()
	{
		if (!isDeath)
		{
			dist = Vector3.Distance(myTransform.position, target.position);
			agent.speed = moveSpeed * 2f;
			if (GameManager.Instance.isDown)
			{
				SearchTaret();
			}
			else
			{
				MoveState();
			}
		}
	}

	private void MoveState()
	{
		switch (state)
		{
		case STATE.SEARCH:
			SearchTaret();
			break;
		case STATE.CHARGE:
			ChargeTarget();
			break;
		case STATE.ATTACK:
			AttackTarget();
			break;
		case STATE.DEATH:
			StartCoroutine("DeathEvent");
			break;
		}
	}

	private void AnimIdle()
	{
		if (!enemyAnim.IsPlaying(idleAnim))
		{
			enemyAnim.CrossFade(idleAnim);
		}
	}

	private void AnimMove()
	{
		if (!enemyAnim.IsPlaying(moveAnim))
		{
			enemyAnim.CrossFade(moveAnim);
		}
	}

	private void AnimAttack()
	{
		enemyAnim.CrossFade(attackAnim);
	}

	private void AnimDeath()
	{
		enemyAnim.Stop();
		if (BetterRandom.RandomBool)
		{
			enemyAnim.Play(death1Anim);
		}
		else
		{
			enemyAnim.Play(death2Anim);
		}
	}

	private void AnimDeathHead()
	{
		enemyAnim.Stop();
		enemyAnim.Play(deathHeadAnim);
	}

	private void SearchTaret()
	{
		if (Time.time > moveTime)
		{
			point = Random.insideUnitCircle * 10f;
			moveTime = Time.time + 3f;
		}
		else
		{
			if (isDown)
			{
				return;
			}
			AnimMove();
			agent.destination = new Vector3(base.transform.position.x + point.x, base.transform.position.y, base.transform.position.z + point.y);
		}
		if (dist <= chargeRange)
		{
			state = STATE.CHARGE;
		}
	}

	private void ChargeTarget()
	{
		if (!isDown)
		{
			AnimMove();
			agent.SetDestination(target.position);
			agent.Resume();
			if (dist <= attackRange)
			{
				isAtk = false;
				agent.Stop();
				AnimIdle();
				state = STATE.ATTACK;
			}
		}
	}

	private void AttackTarget()
	{
		if (isDown)
		{
			return;
		}
		atkReadyTime += Time.deltaTime;
		base.transform.LookAt(new Vector3(target.position.x, base.transform.position.y, target.position.z), Vector3.up);
		if (!isAtk)
		{
			if (atkReadyTime >= attackSpeed && !isDown)
			{
				isAtk = true;
				StartCoroutine(AttackRoutine());
			}
		}
		else if (dist > attackRange)
		{
			state = STATE.CHARGE;
			StopCoroutine("AttackRoutine");
			isAtk = false;
		}
	}

	private IEnumerator AttackRoutine()
	{
		if (isDown)
		{
			yield return null;
		}
		AnimAttack();
		if (DataManager.Instance.gamePrefs.sndEffect)
		{
			audioSource.PlayOneShot(enemyAttack);
		}
		yield return new WaitForSeconds(0.3f);
		if (!isDown)
		{
			if (dist <= attackRange)
			{
				damageAngle.position = base.transform.position;
				target.SendMessage("HitDamage", damageAngle, SendMessageOptions.DontRequireReceiver);
			}
			isAtk = false;
			atkReadyTime = 0f;
		}
	}

	private IEnumerator DeathEvent()
	{
		isDeath = true;
		state = STATE.IDLE;
		agent.Stop();
		Object.Destroy(GetComponent<CapsuleCollider>());
		Object.Destroy(GetComponentInChildren<SphereCollider>());
		Object.Destroy(GetComponent<UnityEngine.AI.NavMeshAgent>());
		if (DataManager.Instance.gamePrefs.sndEffect)
		{
			audioSource.PlayOneShot(enemyDeath);
		}
		DataManager.Instance.gamePrefs.gold += (int)goldAmt;
		GameManager.Instance.UpdateWalletLabel();
		ItemDrop itemDrop = new ItemDrop
		{
			trans = myTransform,
			tier = dropTier
		};
		StartCoroutine(Spawner.Instance.EnemyItemDrop(itemDrop));
		yield return new WaitForSeconds(5f);
		Object.Destroy(base.gameObject);
	}

	private void HitDamage(float damage)
	{
		if (state == STATE.SEARCH)
		{
			state = STATE.CHARGE;
		}
		if (isDown)
		{
			return;
		}
		health -= damage;
		StartCoroutine(HitInteraction());
		if (DataManager.Instance.gamePrefs.sndEffect)
		{
			audioSource.PlayOneShot(enemyHit);
		}
		if (health <= 0f)
		{
			isDown = true;
			health = 0f;
			agent.Stop();
			StopCoroutine("AttackRoutine");
			if (!fpsController.CurrentWeapon.firearms)
			{
				GameManager.Instance.meleeKill++;
			}
			GameManager.Instance.kill++;
			GameManager.Instance.UpdateKillLabelGUI();
			GameManager.Instance.CheckKillReward();
			state = STATE.DEATH;
			AnimDeath();
		}
	}

	private void HitDamageHead(float damage)
	{
		if (state == STATE.SEARCH)
		{
			state = STATE.CHARGE;
		}
		if (isDown)
		{
			return;
		}
		damage *= 2f;
		health -= damage;
		StartCoroutine(HitInteraction());
		if (DataManager.Instance.gamePrefs.sndEffect)
		{
			audioSource.PlayOneShot(enemyHit);
		}
		if (health <= 0f)
		{
			isDown = true;
			health = 0f;
			agent.Stop();
			StopCoroutine("AttackRoutine");
			if (!fpsController.CurrentWeapon.firearms)
			{
				GameManager.Instance.meleeKill++;
			}
			GameManager.Instance.kill++;
			GameManager.Instance.UpdateKillLabelGUI();
			GameManager.Instance.CheckKillReward();
			GameManager.Instance.headShot++;
			GameManager.Instance.HeadShotEffect();
			state = STATE.DEATH;
			AnimDeathHead();
			if (headEffectPos != null)
			{
				headEffectPos.gameObject.SetActive(true);
			}
		}
	}

	private IEnumerator HitInteraction()
	{
		mainMaterial.mainTextureOffset = hitColor;
		yield return new WaitForSeconds(0.05f);
		mainMaterial.mainTextureOffset = normalColor;
	}
}
