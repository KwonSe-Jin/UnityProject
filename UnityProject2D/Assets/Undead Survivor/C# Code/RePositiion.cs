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
		Debug.Log("OnTriggerExit2D called with tag: " + collision.tag); // 함수 진입 로그
		if (!collision.CompareTag("Area"))
			return;

		Vector3 playerPos = GameManager.instance.player.transform.position;
		Vector3 myPos = transform.position;
		float diffX = Mathf.Abs(playerPos.x - myPos.x);
		float diffY = Mathf.Abs(playerPos.y - myPos.y);

		Vector3 playerDir = GameManager.instance.player.InputVec;
		float dirX = playerDir.x < 0 ? -1 : 1;
		float dirY = playerDir.y < 0 ? -1 : 1;

		switch (transform.tag)
		{
			case "Ground":
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
					// 몬스터 재배치 ( 플레이어 이동 방향에 따라 맞은편에서 등장)
					transform.Translate(playerDir * 20 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));
				}
				break;
		}
	}
}