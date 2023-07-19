using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPool
{
	public GameObject prefab;

	public int bufferSize = 10;

	public Stack<GameObject> m_Objects;

	public void Initialize()
	{
		m_Objects = new Stack<GameObject>();
		for (int i = 0; i < bufferSize; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
			gameObject.transform.parent = GameObject.Find("_PoolManager").transform;
			gameObject.name = prefab.name;
			gameObject.SetActive(false);
			m_Objects.Push(gameObject);
		}
	}

	public GameObject GetObject()
	{
		if (m_Objects.Count > 0)
		{
			GameObject gameObject = m_Objects.Pop();
			gameObject.SetActive(true);
			return gameObject;
		}
		return null;
	}

	public void ReturnObject(GameObject obj)
	{
		obj.SetActive(false);
		m_Objects.Push(obj);
	}
}
