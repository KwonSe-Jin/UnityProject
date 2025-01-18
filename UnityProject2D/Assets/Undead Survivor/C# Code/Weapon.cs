using UnityEngine;

//Ǯ�Ŵ������� �޾ƿ� ������� �����ϴ� ��ũ��Ʈ
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
		// ���� ȸ�� 
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
		// ����
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
			// �Ѿ� ����
			Transform bullet;
			if (i < transform.childCount)
			{
				// ���� ������Ʈ Ȱ��
				bullet = transform.GetChild(i);
			}
			else
			{
				// Ǯ������ ��������
				bullet = GameManager.instance.pool.Get(prefabid).transform;
				// �θ� ���� (���� Weapon ������Ʈ)
				bullet.parent = transform;
			}


			bullet.localPosition = Vector3.zero;
			bullet.localRotation = Quaternion.identity;

			Vector3 rotVec = Vector3.forward * 360 * i / count;

			bullet.Rotate(rotVec);
			bullet.Translate(bullet.up * 1.5f, Space.World);

			//// bullet�� ��ġ�� weapon ��ġ�� ����
			//bullet.position = transform.position;
			//// ���� �������� �̵� (up �������� �ణ ���� ��ġ)
			//bullet.Translate(Vector3.up, Space.Self);

			// �Ѿ� �ʱ�ȭ (damage�� -1�� ���� ������ �ǹ�)
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
