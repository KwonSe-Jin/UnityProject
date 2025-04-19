using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class LoginRequest
{
	public string playerName;
	public string password;
}

[System.Serializable]
public class LoginResponse
{
	public string message;
	public string token;
	public int playerId;
}

public class LoginManager : MonoBehaviour
{
	[Header("Input Fields")]
	public TMP_InputField InputField_ID;
	public TMP_InputField InputField_PW;

	[Header("Buttons")]
	public Button buttonLogin;
	public Button buttonSignin;
	public Button buttonExit;

	private const string apiBaseUrl = "https://localhost:7187/api/player"; // API ���� �ּ�

	private void Start()
	{
		buttonLogin.onClick.AddListener(OnLoginClicked);
		buttonSignin.onClick.AddListener(OnSigninClicked);
		buttonExit.onClick.AddListener(OnExitClicked);
	}

	public void OnLoginClicked()
	{
		StartCoroutine(LoginCoroutine());
	}

	public void OnSigninClicked()
	{
		StartCoroutine(SignUpCoroutine());
	}

	public void OnExitClicked()
	{
		Debug.Log("�α��� â ����.");
		UIManager.Instance.ShowPanel(UIPanelType.GameRoom);
	}

	private IEnumerator LoginCoroutine()
	{
		LoginRequest requestData = new LoginRequest
		{
			playerName = InputField_ID.text,
			password = InputField_PW.text
		};

		string json = JsonUtility.ToJson(requestData);

		UnityWebRequest request = new UnityWebRequest($"{apiBaseUrl}/login", "POST");
		byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
		request.uploadHandler = new UploadHandlerRaw(bodyRaw);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");

		yield return request.SendWebRequest();

		if (request.result != UnityWebRequest.Result.Success)
		{
			Debug.Log($"[Login Fail] ��Ʈ��ũ ����: {request.error}");
		}
		else
		{
			var response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
			Debug.Log($"[Login] {response.message}");

			if (!string.IsNullOrEmpty(response.token))
			{
				Debug.Log($"��ū: {response.token}, ID: {response.playerId}");
		
				PlayerPrefs.SetString("token", response.token);
				PlayerPrefs.SetInt("playerId", response.playerId);
				PlayerPrefs.Save();

				// ���� �α��� ���� ó�� 
			}
		}
	}

	private IEnumerator SignUpCoroutine()
	{
		LoginRequest requestData = new LoginRequest
		{
			playerName = InputField_ID.text,
			password = InputField_PW.text
		};

		string json = JsonUtility.ToJson(requestData);

		UnityWebRequest request = new UnityWebRequest($"{apiBaseUrl}/register", "POST");
		byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
		request.uploadHandler = new UploadHandlerRaw(bodyRaw);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");

		yield return request.SendWebRequest();

		if (request.result != UnityWebRequest.Result.Success)
		{
			Debug.Log($"[Register Fail] ��Ʈ��ũ ����: {request.error}");
		}
		else
		{
			Debug.Log($"[Register] ����: {request.downloadHandler.text}");
		}
	}
}
