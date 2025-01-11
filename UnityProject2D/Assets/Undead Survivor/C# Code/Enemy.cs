using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public float speed;
	public float health;
	public float Maxhealth;
	public RuntimeAnimatorController[] animController;
	public Rigidbody2D target;

	bool islive;

	Rigidbody2D rigid;
	Collider2D coll;
	Animator anim;
	SpriteRenderer spriteRenderer;
	WaitForFixedUpdate wait;

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		coll = GetComponent<Collider2D>();
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		wait = new WaitForFixedUpdate();
	}
	void FixedUpdate()
	{
		if (!islive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
			return;
		// 타겟 위치 - 내 위치를 계산하여 방향 벡터 
		Vector2 dirVec = target.position - rigid.position;

		// 다음 프레임에서 이동할 거리 = 방향 벡터(normalized) * 속도 * 프레임 계산
		Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

		// 현재 위치에 다음 이동 벡터를 더해 적을 이동
		rigid.MovePosition(rigid.position + nextVec);

		// 물리 속도 -> 이동에 영향 주지 않도록 속도 제거
		rigid.linearVelocity = Vector2.zero;
	}
	void LateUpdate()
	{
		if (!islive)
			return;
		spriteRenderer.flipX = target.position.x < rigid.position.x;
	}

	void OnEnable()
	{
		target = GameManager.instance.player.GetComponent<Rigidbody2D>();
		islive = true;
		coll.enabled = true;
		rigid.simulated = true;
		spriteRenderer.sortingOrder = 2;
		anim.SetBool("Dead", false);
		health = Maxhealth;
	}

	public void Init(SpawnData data)
	{
		// 매개변수의 속성을 몬스터 속성 변경에 활용
		anim.runtimeAnimatorController = animController[data.spriteType];
		speed = data.speed;
		Maxhealth = data.health;
		health = data.health;
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.CompareTag("Bullet") || !islive)
			return;

		health -= collision.GetComponent<Bullet>().damage;
		StartCoroutine(KnockBack());
		if(health > 0)
		{
			// hit action
			anim.SetTrigger("Hit");
		}
		else
		{
			islive = false;
			coll.enabled = false;
			rigid.simulated = false;
			spriteRenderer.sortingOrder = 1;
			anim.SetBool("Dead", true);

			GameManager.instance.kill++;
			GameManager.instance.GetExp();
			// 없어지게
			//Dead();
		}
	}

	IEnumerator KnockBack()
	{
		yield return wait; // 하나의 물리 프레임 딜레이;

		Vector3 playerPos = GameManager.instance.player.transform.position;
		Vector3 dirVec = transform.position - playerPos;
		rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse); 
		
	}

	void Dead()
	{
		// 몬스터 deactive 
		gameObject.SetActive(false);
	}
}
