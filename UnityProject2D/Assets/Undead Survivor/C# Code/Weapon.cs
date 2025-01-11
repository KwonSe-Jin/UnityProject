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
		player = GetComponentInParent<Player>();	
	}
	void Start()
	{
		Init();
	}
	// Update is called once per frame
	void Update()
	{
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
		this.damage = damage;
		this.count += count;
		if (id == 0)
			Batch();
	}
	public void Init()
	{
		switch (id)
		{
			case 0:
				speed = -150;
				Batch();

				break;

			default:
				speed = 0.3f;
				break;
		}
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
