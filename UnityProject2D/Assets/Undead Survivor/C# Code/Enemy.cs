using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public Rigidbody2D target;

	bool islive = true;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
		rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
    void FixedUpdate()
	{
		if (!islive)
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
	}
}
