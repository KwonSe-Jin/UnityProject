using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	// ���� �ð� ����
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