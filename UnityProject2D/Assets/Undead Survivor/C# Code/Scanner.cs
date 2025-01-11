using UnityEngine;

public class Scanner : MonoBehaviour
{
	public float scanRange;

	public LayerMask targetLayer;
	public RaycastHit2D[] targets;

	public Transform nearestTarget;

	void FixedUpdate()
	{
		targets = Physics2D.CircleCastAll(transform.position, scanRange,
											Vector2.zero, 0, targetLayer);
		nearestTarget = GetNearest();
	}

	Transform GetNearest()
	{
		Transform result = null;
		float diff = 100;

		// 스캔된 모든 대상에 대해 반복
		foreach (RaycastHit2D target in targets)
		{
			Vector3 myPos = transform.position;
			Vector3 targetPos = target.transform.position;

			// 거리 계산
			float curDiff = Vector3.Distance(myPos, targetPos);

			if (curDiff < diff) {
				diff = curDiff;
				result = target.transform;
			}
		}


		return result;
	}

}
