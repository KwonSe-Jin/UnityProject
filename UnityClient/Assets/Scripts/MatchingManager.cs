using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class MatchStatusResponse
{
	public int errorCode;
	public string status;
	public string roomIP;
	public int roomPort;
	public string roomToken;
}

public class MatchingManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button buttonExit;

    [Header("Matching")]
    public Image circularLoader;
    public TextMeshProUGUI matchingText;
    public float rotationSpeed = 200f;

	private Coroutine statusCheckCoroutine;

	// ��Ī ���¿� ���� TEXT ����
	public bool isMatchingCompleted = false;
    
    //api ���� �ּ�
	private const string baseUrl = "https://localhost:7187/api/match";

	// ��Ī ��ư Ŭ�� ��
	private void OnEnable()
	{
		matchingText.text = "Matching ...";
		circularLoader.gameObject.SetActive(true);
		isMatchingCompleted = false;
		StartCoroutine(RequestMatching());

		if (statusCheckCoroutine != null)
			StopCoroutine(statusCheckCoroutine);

		statusCheckCoroutine = StartCoroutine(CheckMatchingStatus());
	}
	void Start() 
    {
        buttonExit.onClick.AddListener(OnExitClicked);
    }

    void Update()
    {
        if (!isMatchingCompleted)
        {
            // ���� �ε� �̹��� ȸ��
            circularLoader.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }
    }

    // �ܺο��� ��Ī �Ϸ� �� ȣ��
    public void OnMatchComplete(string ip, int port, string token)
    {
		isMatchingCompleted = true;
        matchingText.text = "Match Complete !";
        circularLoader.gameObject.SetActive(false); // ������ ����

		if (statusCheckCoroutine != null)
		{
			StopCoroutine(statusCheckCoroutine);
			statusCheckCoroutine = null;
		}

		//  ���� ���� ���� + scene �̵� �߰�
		Debug.Log("���� ���� �غ� �Ϸ�");
		GameServer.Instance.Connect(ip, port, token);
	}

    void OnExitClicked()
    {
		isMatchingCompleted = false;

		StartCoroutine(CancelMatching());

		if (statusCheckCoroutine != null)
		{
			StopCoroutine(statusCheckCoroutine);
			statusCheckCoroutine = null;
		}
		UIManager.Instance.ShowPanel(UIPanelType.GameRoom);
    }

	private IEnumerator RequestMatching()
	{
		yield return HttpRequest($"{baseUrl}/request",
			(response) => Debug.Log("[��Ī ��û ����] " + response),
			(error) => Debug.LogError("[��Ī ��û ����] " + error));
	}

	private IEnumerator CancelMatching()
	{
		yield return HttpRequest($"{baseUrl}/cancel",
			(response) => Debug.Log("[��Ī ��� ����] " + response),
			(error) => Debug.LogError("[��Ī ��� ����] " + error));
	}

	private IEnumerator CheckMatchingStatus()
	{
		while (!isMatchingCompleted)
		{
			yield return HttpRequest($"{baseUrl}/status",
				(response) =>
                {
                    Debug.Log("[��Ī ���� Ȯ��] " + response);
                    var matchData = JsonUtility.FromJson<MatchStatusResponse>(response);

					if (matchData.status == "Matched" && matchData.errorCode == 0)
					{
						Debug.Log($"[��Ī �Ϸ�] IP: {matchData.roomIP}, Port: {matchData.roomPort}");
						OnMatchComplete(matchData.roomIP, matchData.roomPort, matchData.roomToken);
					}
				},
                (error) => Debug.LogWarning("[���� Ȯ�� ����] " + error));

            yield return new WaitForSeconds(1.5f);
		}
	}

	private IEnumerator HttpRequest(string url, System.Action<string> onSuccess = null, System.Action<string> onError = null)
	{
		UnityWebRequest request = new UnityWebRequest(url, "POST");
		request.uploadHandler = new UploadHandlerRaw(new byte[0]);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");

		string token = PlayerPrefs.GetString("token");
		if (!string.IsNullOrEmpty(token))
		{
			request.SetRequestHeader("Authorization", $"Bearer {token}");
		}

		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			onSuccess?.Invoke(request.downloadHandler.text);
		}
		else
		{
			onError?.Invoke(request.error);
		}
	}
}
