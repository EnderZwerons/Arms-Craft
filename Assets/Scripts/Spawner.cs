using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	private static Spawner instance;

	private Dictionary<int, string> curStage = new Dictionary<int, string>();

	[HideInInspector]
	public List<Transform> spawnPoint;

	[HideInInspector]
	public List<Transform> itemPoint;

	public STAGE stageNumber;

	public float spawnRateEnemy;

	public float spawnRateItem = 10f;

	private float spawnTimeEnemy;

	[HideInInspector]
	public int spawnCountItem;

	private int spawnNumEnemy;

	public int enemyLimit = 20;

	private int enemyCount;

	private int itemLimit;

	private Vector3 spawnPos;

	private int difficultLevel;

	private float goldDrop;

	private float itemDrop;

	private float healthPow;

	private float monsterAI;

	private float moveSpdPow;

	private bool canSpawnRandom;

	private int spawnRandomPause, difficultyIncreaseCurrent;

	public int difficultyIncrease = 30, enemySpawnIncrease = 2, specialEnemyChance = 10, specialEnemyChanceIncrease = 1;

	public float spawnRateDiffIncrease = 0.95f, spawnRateItemDiffIncrease = 0.9f, enemyMoveSpeedDiffIncrease = 1.05f, enemyHealthDiffIncrease = 1.05f, 
	itemDropDiffIncrease = 1.1f, goldDropDiffIncrease = 1.2f, enemyAttackSpeedDiffIncrease = 1.03f;

	public GameObject[] itemPack = new GameObject[2];

	public List<DropInfo> dropInfo = new List<DropInfo>();

	private List<float> difficultyMults = new List<float>
	{
		1f, 1.05f, 1.15f
	};

	public static Spawner Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.Find("_Spawner").GetComponent<Spawner>();
			}
			return instance;
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private void Start()
	{
		InitTransform();
		InitStage();
		InitDropInfo();
	}

	private void Update()
	{
		if (GameManager.Instance.isPlay)
		{
			SpawnEnemy();
			SpawnItem();
		}
	}

	private void InitTransform()
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			switch (transform.name)
			{
			case "SpawnPoint":
				spawnPoint.Add(transform);
				break;
			case "ItemPoint":
				itemPoint.Add(transform);
				break;
			}
		}
		itemLimit = itemPoint.Count;
	}

	private void InitStage()
	{
		switch (stageNumber)
		{
		case STAGE.STAGE1:
			curStage = LevelDesign.Instance.stage1;
			difficultLevel = 1 + LevelDesign.Instance.difficultDesign.difficultLevel;
			goldDrop = 1f * LevelDesign.Instance.difficultDesign.goldDrop * LevelDesign.Instance.eventDrop[0].goldDrop;
			itemDrop = 1f * LevelDesign.Instance.difficultDesign.itemDrop * LevelDesign.Instance.eventDrop[0].itemDrop;
			healthPow = 1f * LevelDesign.Instance.difficultDesign.healthPow;
			monsterAI = LevelDesign.Instance.difficultDesign.enemyAI;
			moveSpdPow = LevelDesign.Instance.difficultDesign.enemyMoveSpdPow;
			break;
		case STAGE.STAGE2:
			curStage = LevelDesign.Instance.stage2;
			difficultLevel = 1 + LevelDesign.Instance.difficultDesign.difficultLevel;
			goldDrop = 1f * LevelDesign.Instance.difficultDesign.goldDrop * LevelDesign.Instance.eventDrop[1].goldDrop;
			itemDrop = 1f * LevelDesign.Instance.difficultDesign.itemDrop * LevelDesign.Instance.eventDrop[1].itemDrop;
			healthPow = 1f * LevelDesign.Instance.difficultDesign.healthPow;
			monsterAI = LevelDesign.Instance.difficultDesign.enemyAI;
			moveSpdPow = LevelDesign.Instance.difficultDesign.enemyMoveSpdPow;
			break;
		case STAGE.STAGE3:
			curStage = LevelDesign.Instance.stage3;
			difficultLevel = 1 + LevelDesign.Instance.difficultDesign.difficultLevel;
			goldDrop = 1f * LevelDesign.Instance.difficultDesign.goldDrop * LevelDesign.Instance.eventDrop[2].goldDrop;
			itemDrop = 1f * LevelDesign.Instance.difficultDesign.itemDrop * LevelDesign.Instance.eventDrop[2].itemDrop;
			healthPow = 1f * LevelDesign.Instance.difficultDesign.healthPow;
			monsterAI = LevelDesign.Instance.difficultDesign.enemyAI;
			moveSpdPow = LevelDesign.Instance.difficultDesign.enemyMoveSpdPow;
			break;
		case STAGE.STAGE4:
			curStage = LevelDesign.Instance.stage4;
			difficultLevel = 1 + LevelDesign.Instance.difficultDesign.difficultLevel;
			goldDrop = 1f * LevelDesign.Instance.difficultDesign.goldDrop * LevelDesign.Instance.eventDrop[3].goldDrop;
			itemDrop = 1f * LevelDesign.Instance.difficultDesign.itemDrop * LevelDesign.Instance.eventDrop[3].itemDrop;
			healthPow = 1f * LevelDesign.Instance.difficultDesign.healthPow;
			monsterAI = LevelDesign.Instance.difficultDesign.enemyAI;
			moveSpdPow = LevelDesign.Instance.difficultDesign.enemyMoveSpdPow;
			break;
		}
	}

	private void InitDropInfo()
	{
		int index = (int)stageNumber;
		for (int i = 0; i < 6; i++)
		{
			DropInfo dropInfo = new DropInfo();
			dropInfo.itemObj = StuffObjects.Instance.GetObjectStuff(LevelDesign.Instance.dropItem[index].dropItemInfo[i].itenName);
			dropInfo.chance = (float)LevelDesign.Instance.dropItem[index].dropItemInfo[i].chance * itemDrop;
			this.dropInfo.Add(dropInfo);
		}
	}

	private void SpawnEnemy()
	{
		if (Time.time > spawnTimeEnemy && enemyCount < enemyLimit)
		{
			StartCoroutine("SpawnEnemyRoutine");
			spawnNumEnemy++;
			spawnRandomPause++;
			if (spawnNumEnemy > 134)
			{
				canSpawnRandom = true;
				spawnNumEnemy = 100;
			}
			difficultyIncreaseCurrent++;
			if (difficultyIncreaseCurrent >= difficultyIncrease)
			{
				difficultyIncreaseCurrent = 0;
				enemyLimit += enemySpawnIncrease;
				spawnRateEnemy *= spawnRateDiffIncrease * difficultyMults[LevelDesign.Instance.difficultDesign.difficultLevel];
				spawnRateItem *= spawnRateItemDiffIncrease * difficultyMults[LevelDesign.Instance.difficultDesign.difficultLevel];
				goldDrop *= goldDropDiffIncrease * difficultyMults[LevelDesign.Instance.difficultDesign.difficultLevel];
				healthPow *= enemyHealthDiffIncrease * difficultyMults[LevelDesign.Instance.difficultDesign.difficultLevel];
				monsterAI *= enemyAttackSpeedDiffIncrease * difficultyMults[LevelDesign.Instance.difficultDesign.difficultLevel];
				moveSpdPow *= enemyMoveSpeedDiffIncrease * difficultyMults[LevelDesign.Instance.difficultDesign.difficultLevel];
				specialEnemyChance -= specialEnemyChanceIncrease;
			}
			spawnTimeEnemy = Time.time + spawnRateEnemy;
		}
		enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
	}

	private void SpawnSpecialEnemy(EnemyController enemy)
	{
		SPECIALENEMY enemyType = SpecialEnemyData.GetRandomSpecialEnemy;
		Color color = SpecialEnemyData.SpecialColors[enemyType];
		SpecialEnemyData.EnemyData data = SpecialEnemyData.SpecialStats[enemyType];
		SkinnedMeshRenderer renderer = enemy.GetComponentInChildren<SkinnedMeshRenderer>();
		Texture original = renderer.sharedMaterial.mainTexture;
		renderer.sharedMaterial = new Material(renderer.sharedMaterial.shader);
		renderer.sharedMaterial.SetColor("_Color", color);
		renderer.sharedMaterial.mainTexture = original;
		enemy.health *= data.health;
		enemy.damage *= data.damage;
		enemy.attackSpeed /= data.attackSpeed;
		enemy.moveSpeed *= data.moveSpeed;
		enemy.chargeRange *= data.chargeRange;
		enemy.attackRange *= data.attackRange;
		enemy.goldAmt *= data.goldDrop;
		enemy.dropTier = Mathf.FloorToInt((float)enemy.dropTier * data.itemDrop);
		enemy.transform.localScale *= data.scale;
		enemy.mainMaterial = renderer.sharedMaterial;
	}

	private IEnumerator SpawnEnemyRoutine()
	{
		bool spawnRandom = false;
		if (spawnRandomPause > 5)
		{
			spawnRandomPause = 0;
			spawnRandom = canSpawnRandom;
		}
		string enemyName = curStage[spawnRandom ? BetterRandom.Range(0, 134) : spawnNumEnemy];
		int num = EnemyObjectName(enemyName);
		GameObject monsterObject = EnemyObjects.Instance.enemyObject[num];
		int rnd = Random.Range(0, spawnPoint.Count);
		GameObject monster = Object.Instantiate(monsterObject, spawnPoint[rnd].position, Quaternion.identity) as GameObject;
		monster.transform.localScale = monster.transform.localScale;
		EnemyController monsterController = monster.GetComponent<EnemyController>();
		monsterController.goldAmt *= goldDrop;
		monsterController.health *= healthPow;
		monsterController.attackSpeed *= monsterAI;
		monsterController.moveSpeed += monsterController.moveSpeed * moveSpdPow;
		if (BetterRandom.GetBool(Mathf.Clamp(specialEnemyChance - LevelDesign.Instance.difficultDesign.difficultLevel, 0, specialEnemyChance)))
		{
			SpawnSpecialEnemy(monsterController);
		}
		yield return null;
	}

	private void SpawnItem()
	{
		if (GameManager.Instance.kill >= spawnCountItem)
		{
			StartCoroutine("SpawnItemRoutine");
			spawnCountItem = GameManager.Instance.kill + Mathf.FloorToInt(spawnRateItem);
		}
	}

	private IEnumerator SpawnItemRoutine()
	{
		int rnd2 = Random.Range(0, 4);
		rnd2 = ((rnd2 == 1) ? 1 : 0);
		for (int i = 0; i < itemLimit; i++)
		{
			if (itemPoint[i].childCount == 0)
			{
				GameObject item = Object.Instantiate(itemPack[rnd2], itemPoint[i].position, Quaternion.identity) as GameObject;
				item.transform.parent = itemPoint[i];
				item.transform.localPosition = Vector3.zero;
				break;
			}
		}
		yield return null;
	}

	private int EnemyObjectName(string name)
	{
		return int.Parse(name.Substring(1, 3)) - 1;
	}

	public IEnumerator EnemyItemDrop(ItemDrop itemDrop)
	{
		Transform trans = itemDrop.trans;
		int tier = itemDrop.tier;
		if (difficultLevel < tier)
		{
			tier = difficultLevel;
		}
		int loop = (tier - 1) * 2 + 2;
		for (int i = 0; i < loop; i++)
		{
			int rnd = Random.Range(1, 101);
			if (1 <= rnd && (float)rnd <= dropInfo[i].chance)
			{
				int rndPos = Random.Range(-3, 4);
				Object.Instantiate(dropInfo[i].itemObj, trans.position + new Vector3(rndPos, 0.5f, rndPos), Quaternion.identity);
			}
		}
		yield return null;
	}
}
