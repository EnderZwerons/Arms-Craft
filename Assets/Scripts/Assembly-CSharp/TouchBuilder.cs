using UnityEngine;

public class TouchBuilder : MonoBehaviour
{
	private BuildManager buildManager;

	private Ray ray;

	private RaycastHit hit;

	private int blockNum;

	private Vector3 touchBeganPos;

	public Transform ItemRoot;

	public bool move;

	private void Start()
	{
		buildManager = GameObject.Find("_BuildManager").GetComponent<BuildManager>();
	}

	private void Update()
	{
		if (Input2.touchCount != 1)
		{
			return;
		}
		switch (Input2.touches[0].phase)
		{
		case TouchPhase.Began:
			touchBeganPos = Input.mousePosition;
			break;
		case TouchPhase.Moved:
		{
			float num = Vector3.Distance(touchBeganPos, Input.mousePosition);
			if (!move && num >= 30f)
			{
				move = true;
			}
			break;
		}
		case TouchPhase.Ended:
			if (move)
			{
				move = false;
				touchBeganPos = Vector2.zero;
				break;
			}
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (!Physics.Raycast(ray, out hit))
			{
				break;
			}
			if (hit.transform.tag == "Grid")
			{
				if (!buildManager.delete)
				{
					buildManager.OnGridBuild(hit.transform);
				}
			}
			else if (hit.transform.tag == "Part")
			{
				if (!buildManager.delete)
				{
					buildManager.OnPartBuild(hit.transform, hit.triangleIndex);
				}
				else
				{
					buildManager.OnPartDestroy(hit.transform.gameObject);
				}
			}
			else if (hit.transform.tag == "Muzzle" || hit.transform.tag == "Sight")
			{
				if (!buildManager.delete)
				{
					Debug.Log("Not Create");
				}
				else
				{
					buildManager.OnDestroySPart(hit.transform.gameObject);
				}
			}
			break;
		case TouchPhase.Stationary:
			break;
		}
	}
}
