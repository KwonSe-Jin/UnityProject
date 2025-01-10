using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	// prefabs를 보관할 변수
	public GameObject[] Prefabs;
	// 풀을 관리하는 리스트 <둘은 1:1관계>
	List<GameObject>[] pools;

	void Awake()
	{
		pools = new List<GameObject>[Prefabs.Length];
		
		for(int i = 0; i < pools.Length; i++)
		{
			pools[i] = new List<GameObject> ();

		}
		Debug.Log(pools.Length); 

	}
	public GameObject Get(int index)
	{
		GameObject select = null;

		// 선택한 풀에서 deactive상태인 오브젝트 접근 

		// range-for문 같은 느낌
		foreach(GameObject item in pools[index]) {
			if (!item.activeSelf)
			{
				// 발견하면 select 변수에 할당
				select = item;
				select.SetActive(true);
				break;
			}
		}
		// 없다면 (모두 활성화 상태이면 active인 경우)
		if (!select )
		{
			// 새롭게 생성 후 할당 
			select = Instantiate(Prefabs[index], transform);
			pools[index].Add(select);
		}


		return select;
	}
}
