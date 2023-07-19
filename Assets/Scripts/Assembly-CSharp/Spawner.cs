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

	public int spawnRateItem;

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

	public GameObject[] itemPack = new GameObject[2];

	public List<DropInfo> dropInfo = new List<DropInfo>();

	public static Spawner Inatance
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
			if (spawnNumEnemy > 134)
			{
				spawnNumEnemy = 100;
			}
			spawnTimeEnemy = Time.time + spawnRateEnemy;
		}
		enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
	}

	private IEnumerator SpawnEnemyRoutine()
	{
		string enemyName = curStage[spawnNumEnemy];
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
		yield return null;
	}

	private void SpawnItem()
	{
		if (GameManager.Instance.kill >= spawnCountItem)
		{
			StartCoroutine("SpawnItemRoutine");
			spawnCountItem = GameManager.Instance.kill + spawnRateItem;
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
		int num = 0;
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
