using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	// 게임 시간 설정
	[Header("# Game Control")]
	public bool isLive;
	public float gameTime;
	public float MAXgameTime = 2 * 10f;
	[Header("# Player Info")]
	public int health;
	public int maxHealth = 100;
	public int level;
	public int kill;
	public int exp;
	public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600};
	[Header("# GameObject")]
	public PoolManager pool;
	public Player player;
	public LevelUp UIlevelUp;
	void Awake()
	{
		instance = this;
	}
	void Start()
	{
		health = maxHealth;

		//임시
		UIlevelUp.Select(0);
	}
	void Update()
	{
		if (!isLive)
			return;
        gameTime += Time.deltaTime;

		if (gameTime > MAXgameTime)
		{
			gameTime = MAXgameTime;
		}
	}
	// 경험치 증가 함수 새로 작성
	public void GetExp()
	{
		exp++;
		if(exp >= nextExp[Mathf.Min(level, nextExp.Length-1)])
		{
			level++;
			exp = 0;
			UIlevelUp.Show();
		}
	}

	public void Stop()
	{
		isLive = false;
		Time.timeScale = 0;
	}
	public void Resume()
	{
		isLive = true;
		Time.timeScale = 1;
	}
}