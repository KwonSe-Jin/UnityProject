using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2  InputVec;
    public float    Speed;
    Rigidbody2D Rigid;
	SpriteRenderer SpriteR;
	Animator Anim;

	void Awake()
	{
		Rigid = GetComponent<Rigidbody2D>();
		SpriteR = GetComponent<SpriteRenderer>();
		Anim = GetComponent<Animator>();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
  //  void Update()
  //  {
		//InputVec.x = Input.GetAxisRaw("Horizontal");
		//InputVec.y = Input.GetAxisRaw("Vertical"); 
  //  }
    void FixedUpdate()
	{
        Vector2 NextVec = InputVec * Time.fixedDeltaTime * Speed;
		// 위치 이동 (현재 위치 + inputVec)
		Rigid.MovePosition(Rigid.position + NextVec);
        
	}

	void OnMove(InputValue value)
	{
		Debug.Log("OnTriggerExit2D called with tag: ");
		InputVec = value.Get<Vector2>();
	}
	void LateUpdate()
	{
		Anim.SetFloat("Speed",InputVec.magnitude);
		if(InputVec.x != 0)
		{
			SpriteR.flipX = InputVec.x < 0;
		}	
	}
}
