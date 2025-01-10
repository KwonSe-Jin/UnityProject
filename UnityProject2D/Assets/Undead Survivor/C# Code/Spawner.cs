using UnityEngine;
using UnityEngine.InputSystem;
public class Spawner : MonoBehaviour
{

	public Transform[] spawnPoint;

	float SpawnTimer;

	void Awake()
	{
		spawnPoint = GetComponentsInChildren<Transform>();
	}
	void Update()
	{
		SpawnTimer += Time.deltaTime;

		if (SpawnTimer > 0.2f) {
			SpawnTimer = 0;
			Spawn();
		}

		
	}
	void Spawn()
	{
		GameObject enemy = GameManager.instance.pool.Get(Random.Range(0, 2));
		enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
	}
}
