using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	// 게임 시간 설정
	public float gameTime;
	public float MAXgameTime = 2 * 10f;

	public PoolManager pool;
	public Player player;
	void Awake()
	{
		instance = this;
	}
	void Update()
	{
		gameTime += Time.deltaTime;

		if (gameTime > MAXgameTime)
		{
			gameTime = MAXgameTime;
		}


	}
}