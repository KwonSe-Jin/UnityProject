using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class CharacterData
{
	public int characterId;
	public string name;
	public string description;
	public int price;
}

[Serializable]
public class BuyCharacterRequest
{
	public int characterId;
}

public class ShopManager : MonoBehaviour
{
	[Header("UI")]
	public Button buttonExit;
	public Button[] buyButtons;
	public TMP_Text[] nameLabels;
	public TMP_Text[] priceLabels;

	private const string shopApiBase = "https://localhost:7187/api/shop"; // API 서버 주소
	private List<CharacterData> shopCharacters = new List<CharacterData>();

	private void Start()
	{
		buttonExit.onClick.AddListener(OnExitClicked);

		for (int i = 0; i < buyButtons.Length; i++)
		{
			int index = i;
			buyButtons[i].onClick.AddListener(() => OnBuyButtonClicked(index));
		}

		StartCoroutine(LoadShopItems());
	}

	void OnExitClicked()
	{
		UIManager.Instance.ShowPanel(UIPanelType.GameRoom);
	}

	void OnBuyButtonClicked(int index)
	{
		if (index >= shopCharacters.Count)
		{
			Debug.LogWarning($"잘못된 인덱스: {index}");
			return;
		}

		int characterId = shopCharacters[index].characterId;
		Debug.Log($"[Buy Clicked] 캐릭터 ID: {characterId}");
		StartCoroutine(SendBuyRequest(characterId));
	}

	IEnumerator LoadShopItems()
	{
		string token = PlayerPrefs.GetString("token");

		using (UnityWebRequest request = UnityWebRequest.Get(shopApiBase))
		{
			request.SetRequestHeader("Authorization", $"Bearer {token}");
			yield return request.SendWebRequest();

			if (request.result != UnityWebRequest.Result.Success)
			{
				Debug.LogError($"상점 아이템 로딩 실패: {request.error}");
				yield break;
			}

			string json = "{\"items\":" + request.downloadHandler.text + "}";
			CharacterListWrapper wrapper = JsonUtility.FromJson<CharacterListWrapper>(json);
			shopCharacters = new List<CharacterData>(wrapper.items);

			Debug.Log($"[상점 로딩 완료] {shopCharacters.Count}개");

			// UI 반영
			for (int i = 0; i < shopCharacters.Count && i < nameLabels.Length; i++)
			{
				nameLabels[i].text = shopCharacters[i].name;
				priceLabels[i].text = $"{shopCharacters[i].price} G";
			}
		}
	}

	IEnumerator SendBuyRequest(int characterId)
	{
		string token = PlayerPrefs.GetString("token");

		if (string.IsNullOrEmpty(token))
		{
			Debug.LogError("토큰 없음. 로그인 필요.");
			yield break;
		}

		BuyCharacterRequest data = new BuyCharacterRequest { characterId = characterId };
		string jsonBody = JsonUtility.ToJson(data);
		Debug.Log($"[Buy JSON] {jsonBody}");

		using (UnityWebRequest request = new UnityWebRequest($"{shopApiBase}/buy", "POST"))
		{
			byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
			request.uploadHandler = new UploadHandlerRaw(bodyRaw);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Authorization", $"Bearer {token}");
			request.SetRequestHeader("Content-Type", "application/json");

			yield return request.SendWebRequest();

			Debug.Log($"[응답 코드] {request.responseCode}");
			Debug.Log($"[응답 내용] {request.downloadHandler.text}");

			if (request.result != UnityWebRequest.Result.Success)
			{
				Debug.LogError($"[구매 실패] {request.error}");
			}
			else
			{
				Debug.Log($"[구매 성공] {request.downloadHandler.text}");
				// TODO: UI 반영 등
			}
		}
	}

	// JsonUtility는 배열 파싱이 안 되기 때문에 Wrapper 클래스 필요
	[Serializable]
	private class CharacterListWrapper
	{
		public CharacterData[] items;
	}
}
