using UnityEngine;
using UnityEngine.InputSystem;
public class Spawner : MonoBehaviour
{

	public Transform[] spawnPoint;
	public SpawnData[] spawnData;

	public float levelTime;
	// 오브젝트 소환 레벨 설정 변수
	int level;

	float SpawnTimer;

	void Awake()
	{
		spawnPoint = GetComponentsInChildren<Transform>();
		levelTime = GameManager.instance.MAXgameTime / spawnData.Length;
	}
	void Update()
	{
		if (!GameManager.instance.isLive)
			return;
		SpawnTimer += Time.deltaTime;
		level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);

		if (SpawnTimer > spawnData[level].spawnTime) {
			SpawnTimer = 0;
			Spawn();
		}

		
	}
	void Spawn()
	{
		GameObject enemy = GameManager.instance.pool.Get(0);
		enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
		enemy.GetComponent<Enemy>().Init(spawnData[level]);
	}
}

[System.Serializable]
public class SpawnData
{
	public float spawnTime;
	public int spriteType;
	public int health;
	public float speed;
}