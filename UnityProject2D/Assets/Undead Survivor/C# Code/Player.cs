using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2  InputVec;
    public float    Speed;
    Rigidbody2D Rigid;

	void Awake()
	{
		Rigid = GetComponent<Rigidbody2D>();
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
		InputVec = value.Get<Vector2>();
	}
}
