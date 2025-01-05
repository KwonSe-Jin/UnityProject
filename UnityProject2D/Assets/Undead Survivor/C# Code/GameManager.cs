using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
	void Awake()
	{
		Debug.Log("Awake called with tag: ");
		instance = this;

	}
}
