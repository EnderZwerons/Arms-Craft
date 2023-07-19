using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
	private Animation anim;

	private void Start()
	{
		anim = GetComponentInChildren<Animation>();
	}

	public void UpdateAnimation(string animName)
	{
		anim.CrossFade(animName);
	}
}
