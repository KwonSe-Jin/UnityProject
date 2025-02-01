using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;
using System.Threading.Tasks;

public class Player : MonoBehaviour
{
    public Vector2  InputVec;
    public float    Speed;

	public Scanner scanner;
	public Hand[] hands;
	public RuntimeAnimatorController[] animCon;
    Rigidbody2D Rigid;
	SpriteRenderer SpriteR;
	Animator Anim;

	private NetWorkManager netWorkManager;
	private Vector2 lastSentInput = Vector2.zero;
	private Vector2 lastSentPosition = Vector2.zero;

	void Awake()
	{
		Rigid = GetComponent<Rigidbody2D>();
		SpriteR = GetComponent<SpriteRenderer>();
		Anim = GetComponent<Animator>();
		scanner = GetComponent<Scanner>();
		hands = GetComponentsInChildren<Hand>(true);
		
		netWorkManager = FindObjectOfType<NetWorkManager>();
	}

	void OnEnable()
	{
		Speed *= Character.Speed;
		Anim.runtimeAnimatorController = animCon[GameManager.instance.playerID];
	}

	// Update is called once per frame
	//  void Update()
	//  {
	//InputVec.x = Input.GetAxisRaw("Horizontal");
	//InputVec.y = Input.GetAxisRaw("Vertical"); 
	//  }
	void FixedUpdate()
	{
		if (!GameManager.instance.isLive)
			return;
		Vector2 NextVec = InputVec * Time.fixedDeltaTime * Speed;
		// 위치 이동 (현재 위치 + inputVec)
		Rigid.MovePosition(Rigid.position + NextVec);
		//if (InputVec != lastSentInput)
		//{
		//	lastSentInput = InputVec;
		//	netWorkManager?.SendPlayerInput(GameManager.instance.playerID, InputVec);
		//}
		// 플레이어 위치 전송
		SendPositionToServer();
	}
	void SendPositionToServer()
	{
		Vector2 currentPosition = Rigid.position;

		// 위치가 변한 경우에만 전송
		if (currentPosition != lastSentPosition)
		{
			lastSentPosition = currentPosition;
			netWorkManager?.SendPlayerPosition(GameManager.instance.playerID, currentPosition);
		}
	}
	void OnMove(InputValue value)
	{
		if (!GameManager.instance.isLive)
			return;
		Debug.Log("OnTriggerExit2D called with tag: ");
		InputVec = value.Get<Vector2>();
	}
	void LateUpdate()
	{
		if (!GameManager.instance.isLive)
			return;
		Anim.SetFloat("Speed",InputVec.magnitude);
		if(InputVec.x != 0)
		{
			SpriteR.flipX = InputVec.x < 0;
		}	
	}

	void OnCollisionStay2D(Collision2D collision)
	{
		if(!GameManager.instance.isLive)
			return;

		GameManager.instance.health -= Time.deltaTime *	10;

		if(GameManager.instance.health < 0)
		{
			for(int i = 2; i < transform.childCount; i++) {
				transform.GetChild(i).gameObject.SetActive(false);
			}
			Anim.SetTrigger("Dead");
			GameManager.instance.GameOver();
		}
	}
}
