using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	// 게임 시간 설정
	[Header("# Game Control")]
	public bool isLive;
	public float gameTime;
	public float MAXgameTime = 2 * 10f;
	[Header("# Player Info")]
	public int playerID;
	public float health;
	public float maxHealth = 100;
	public int level;
	public int kill;
	public int exp;
	public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600};
	[Header("# GameObject")]
	public PoolManager pool;
	public Player player;
	public LevelUp UIlevelUp;
	public Result UIResult;
	public GameObject EnemyCleaner;
	void Awake()
	{
		instance = this;
	}
	public void GameStart(int id)
	{
		playerID = id;
		health = maxHealth;

		player.gameObject.SetActive(true);
		UIlevelUp.Select(playerID % 2);
		Resume();
	}
	public void GameOver()
	{
		StartCoroutine(GameOverRoutine());
	}
	IEnumerator GameOverRoutine()
	{
		isLive = false;

		yield return new WaitForSeconds(0.5f);

		UIResult.gameObject.SetActive(true);
		UIResult.Lose();
		Stop();
	}
	public void GameVictory()
	{
		StartCoroutine(GameVictoryRoutine());
	}
	IEnumerator GameVictoryRoutine()
	{
		isLive = false;
		EnemyCleaner.SetActive(true);
		yield return new WaitForSeconds(0.5f);

		UIResult.gameObject.SetActive(true);
		UIResult.Win();
		Stop();
	}


	public void GameRetry()
	{
		SceneManager.LoadScene(0);
	}

	void Update()
	{
		if (!isLive)
			return;
        gameTime += Time.deltaTime;

		if (gameTime > MAXgameTime)
		{
			gameTime = MAXgameTime;
			GameVictory();
		}
	}
	// 경험치 증가 함수 새로 작성
	public void GetExp()
	{
		if (!isLive)
			return;
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