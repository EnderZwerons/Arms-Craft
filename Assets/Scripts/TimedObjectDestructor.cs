using System;
using UnityEngine;

public class TimedObjectDestructor : MonoBehaviour
{
	public float timeOut;

	public bool detachChildren;

	public TimedObjectDestructor()
	{
		timeOut = 1f;
	}

	public virtual void Awake()
	{
		Invoke("DestroyNow", timeOut);
	}

	public virtual void DestroyNow()
	{
		if (detachChildren)
		{
			transform.DetachChildren();
		}
		UnityEngine.Object.DestroyObject(gameObject);
	}
}
