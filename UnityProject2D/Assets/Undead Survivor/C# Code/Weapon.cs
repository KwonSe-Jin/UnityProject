using UnityEngine;

//풀매니저에서 받아온 무기등을 관리하는 스크립트
public class Weapon : MonoBehaviour
{
	public int id;
	public int prefabid;
	public float damage;
	public int count;
	public float speed;
	float Timer;
	Player player;

	void Awake()
	{
		player = GameManager.instance.player;
	}

	// Update is called once per frame
	void Update()
	{
		if (!GameManager.instance.isLive)
			return;
		// 무기 회전 
		switch (id)
		{
			case 0:
				transform.Rotate(Vector3.back * speed * Time.deltaTime);
				break;

			default:
				Timer += Time.deltaTime;

				if (Timer > speed)
				{
					Timer = 0f;
					Fire();
				}
				break;
		}

		// test
		if (Input.GetButtonDown("Jump"))
		{
			LevelUP(10, 1);
		}
	}
	public void LevelUP(float damage, int count)
	{
		this.damage = damage * Character.Damage;
		this.count += count;
		if (id == 0)
			Batch();


		player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
	}
	public void Init(ItemData data)
	{
		// 세팅
		name = "Weapon " + data.itemID;
		transform.parent = player.transform;
		transform.localPosition = Vector3.zero;
		// property set
		id = data.itemID;
		damage = data.baseDamage * Character.Damage;
		count = data.baseCount + Character.Count;

		for(int i = 0; i < GameManager.instance.pool.Prefabs.Length; i++) {
			if(data.projectile == GameManager.instance.pool.Prefabs[i])
			{
				prefabid = i;
				break;
			}
		}

		switch (id)
		{
			case 0:
				speed = 150 * Character.WeaponSpeed;
				Batch();

				break;

			default:
				speed = 0.5f * Character.WeaponRate;
				break;
		}

		// Hand Set
		Hand hand = player.hands[(int)data.itemType];
		hand.spriteRenderer.sprite = data.hand;
		hand.gameObject.SetActive(true);
		player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
	}

	void Batch()
	{
		for (int i = 0; i < count; i++)
		{
			// 총알 생성
			Transform bullet;
			if (i < transform.childCount)
			{
				// 기존 오브젝트 활용
				bullet = transform.GetChild(i);
			}
			else
			{
				// 풀링에서 가져오기
				bullet = GameManager.instance.pool.Get(prefabid).transform;
				// 부모 설정 (현재 Weapon 오브젝트)
				bullet.parent = transform;
			}


			bullet.localPosition = Vector3.zero;
			bullet.localRotation = Quaternion.identity;

			Vector3 rotVec = Vector3.forward * 360 * i / count;

			bullet.Rotate(rotVec);
			bullet.Translate(bullet.up * 1.5f, Space.World);

			//// bullet의 위치를 weapon 위치로 설정
			//bullet.position = transform.position;
			//// 위쪽 방향으로 이동 (up 방향으로 약간 위로 배치)
			//bullet.Translate(Vector3.up, Space.Self);

			// 총알 초기화 (damage와 -1은 무한 관통을 의미)
			bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero);

		}
	}

	void Fire()
	{
		if (!player.scanner.nearestTarget)
			return;

		Vector3 targetPos = player.scanner.nearestTarget.position;
		Vector3 dir = targetPos - transform.position;
		dir = dir.normalized;



		Transform bullet = GameManager.instance.pool.Get(prefabid).transform;
		bullet.position = transform.position;
		bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
		bullet.GetComponent<Bullet>().Init(damage, count, dir);

	}
}
