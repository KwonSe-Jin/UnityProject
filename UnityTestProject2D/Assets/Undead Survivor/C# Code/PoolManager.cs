using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	// prefabs�� ������ ����
	public GameObject[] Prefabs;
	// Ǯ�� �����ϴ� ����Ʈ <���� 1:1����>
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

		// ������ Ǯ���� deactive������ ������Ʈ ���� 

		// range-for�� ���� ����
		foreach(GameObject item in pools[index]) {
			if (!item.activeSelf)
			{
				// �߰��ϸ� select ������ �Ҵ�
				select = item;
				select.SetActive(true);
				break;
			}
		}
		// ���ٸ� (��� Ȱ��ȭ �����̸� active�� ���)
		if (!select )
		{
			// ���Ӱ� ���� �� �Ҵ� 
			select = Instantiate(Prefabs[index], transform);
			pools[index].Add(select);
		}


		return select;
	}
}
