using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class RankingData
{
	public string playerName; // �������� �Ѿ���� JSON Ű�� ��ġ���Ѿ� ��
	public int score;
}

[Serializable]
public class RankingListWrapper
{
	public RankingData[] items;
}

public class RankingManager : MonoBehaviour
{
	[Header("Buttons")]
	public Button buttonExit;
	public Button buttonUpdate;

	[Header("Ranking Prefab")]
	public GameObject rankingItemPrefab;
	public Transform contentParent;
	public int rankingNum = 10;

	private List<GameObject> rankings = new List<GameObject>();

	private const string apiUrl = "https://localhost:7187/api/ranking?count=10";

	private void Start()
	{
		buttonExit.onClick.AddListener(OnExitClicked);
		buttonUpdate.onClick.AddListener(OnUpdateClicked);

		StartCoroutine(RefreshRankingFromServer());
	}

	void OnExitClicked()
	{
		UIManager.Instance.ShowPanel(UIPanelType.GameRoom);
	}

	void OnUpdateClicked()
	{
		StartCoroutine(RefreshRankingFromServer());
	}

	void AddRankingContent(int rank, string nickname, int score)
	{
		GameObject item = Instantiate(rankingItemPrefab, contentParent);
		item.transform.Find("Text_Rank").GetComponent<TMP_Text>().text = rank.ToString();
		item.transform.Find("Text_ID").GetComponent<TMP_Text>().text = nickname;
		item.transform.Find("Text_Score").GetComponent<TMP_Text>().text = score.ToString();
		rankings.Add(item);
	}

	void ClearRankingUI()
	{
		foreach (GameObject item in rankings)
		{
			Destroy(item);
		}
		rankings.Clear();
	}

	IEnumerator RefreshRankingFromServer()
	{
		UnityWebRequest request = UnityWebRequest.Get(apiUrl);
		yield return request.SendWebRequest();

		if (request.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError($"[��ŷ �ҷ����� ����] {request.error}");
			yield break;
		}

		string rawJson = request.downloadHandler.text;
		string wrappedJson = "{\"items\":" + rawJson + "}";

		RankingListWrapper wrapper = JsonUtility.FromJson<RankingListWrapper>(wrappedJson);

		if (wrapper == null || wrapper.items == null)
		{
			Debug.LogError("[��ŷ �Ľ� ����]");
			yield break;
		}

		// 1. ���� UI ����
		ClearRankingUI();

		// 2. UI �߰�
		for (int i = 0; i < wrapper.items.Length; i++)
		{
			var data = wrapper.items[i];
			AddRankingContent(i + 1, data.playerName, data.score);
		}

		Debug.Log("[RankingManager] �������� ��ŷ ���ΰ�ħ �Ϸ�");
	}
}
