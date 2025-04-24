using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RePosition : MonoBehaviour
{
	Collider2D coll;
	void Awake()
	{
		coll = GetComponent	<Collider2D> ();
	}
	void OnTriggerExit2D(Collider2D collision)
	{
		
		if (!collision.CompareTag("Area"))
			return;

		Vector3 playerPos = GameManager.instance.player.transform.position;
		Vector3 myPos = transform.position;
	

		switch (transform.tag)
		{
			case "Ground":
				float diffX = playerPos.x - myPos.x;
				float diffY = playerPos.y - myPos.y;
				float dirX = diffX < 0 ? -1 : 1;
				float dirY = diffY < 0 ? -1 : 1;
				diffX = Mathf.Abs(diffX);
				diffY = Mathf.Abs(diffY);
				if (diffX > diffY)
				{
					transform.Translate(Vector3.right * dirX * 40);
				}
				else if (diffX < diffY)
				{
					transform.Translate(Vector3.up * dirY * 40);
				}
				else
				{
					transform.Translate(dirX * 40, dirY * 40, 0);
				}
				break;
			case "Enemy":
				if (coll.enabled)
				{
					Vector3 dist = playerPos - myPos;
					Vector3 ran = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
					// 몬스터 재배치 ( 플레이어 이동 방향에 따라 맞은편에서 등장
					transform.Translate(ran + dist * 2);
				}
				break;
		}
	}
}