using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	// ���� �ð� ����
	[Header("# Game Control")]
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
	void Awake()
	{
		instance = this;
	}
	void Start()
	{
		health = maxHealth;	
	}
	void Update()
	{
		gameTime += Time.deltaTime;

		if (gameTime > MAXgameTime)
		{
			gameTime = MAXgameTime;
		}
	}
	// ����ġ ���� �Լ� ���� �ۼ�
	public void GetExp()
	{
		exp++;
		if(exp >= nextExp[level])
		{
			level++;
			exp = 0;
		}
	}
}