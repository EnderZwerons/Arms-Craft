using System.Collections;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
	public ObjectPool[] objectPools;

	private static ObjectPoolManager m_Instance;

	private Hashtable m_Tables;

	private static ObjectPoolManager Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = (ObjectPoolManager)Object.FindObjectOfType(typeof(ObjectPoolManager));
			}
			return m_Instance;
		}
	}

	private void OnApplicationQuit()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private void Awake()
	{
		m_Tables = new Hashtable();
		ObjectPool[] array = objectPools;
		foreach (ObjectPool objectPool in array)
		{
			objectPool.Initialize();
			m_Tables[objectPool.prefab.name] = objectPool;
		}
	}

	public static GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		if (Instance == null)
		{
			Debug.Log("ObjectPoolManager not loaded");
			return Object.Instantiate(prefab, position, rotation) as GameObject;
		}
		ObjectPool objectPool = Instance.m_Tables[prefab.name] as ObjectPool;
		if (objectPool == null)
		{
			return Object.Instantiate(prefab, position, rotation) as GameObject;
		}
		GameObject @object = objectPool.GetObject();
		if (@object == null)
		{
			return Object.Instantiate(prefab, position, rotation) as GameObject;
		}
		@object.transform.position = position;
		@object.transform.rotation = rotation;
		return @object;
	}

	public static void DestroyObject(GameObject obj)
	{
		ObjectPool objectPool = Instance.m_Tables[obj.name] as ObjectPool;
		if (objectPool == null)
		{
			Object.Destroy(obj);
		}
		else
		{
			objectPool.ReturnObject(obj);
		}
	}
}
