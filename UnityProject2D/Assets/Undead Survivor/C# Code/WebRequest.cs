using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class WebRequest : MonoBehaviour
{
	private string baseUrl = "https://localhost:7187/api/player"; // API 서버 URL

	// 플레이어 정보 가져오기 (GET)
	public IEnumerator GetPlayerInfo(int playerId)
	{
		string url = $"{baseUrl}/{playerId}";
		UnityWebRequest request = UnityWebRequest.Get(url);

		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			Debug.Log("Player Info: " + request.downloadHandler.text);
		}
		else
		{
			Debug.LogError("Error: " + request.error);
		}
	}

	void Start()
	{
		StartCoroutine(GetPlayerInfo(1));          // 플레이어 정보 가져오기
	}
}

// 🎯 플레이어 데이터 클래스
[System.Serializable]
public class PlayerData
{
	public string name;
	public int level;
}
