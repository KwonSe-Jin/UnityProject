using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

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

		statusCheckCoroutine = StartCoroutine(CheckMatchingStatusRepeatedly());
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
    public void OnMatchComplete()
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
		yield return PostWithAuth($"{baseUrl}/request",
			onSuccess: (response) =>
			{
				Debug.Log("[��Ī ��û ����] " + response);
			},
			onError: (error) =>
			{
				Debug.LogError("[��Ī ��û ����] " + error);
			});
	}

	private IEnumerator CancelMatching()
	{
		yield return PostWithAuth($"{baseUrl}/cancel",
			onSuccess: (response) =>
			{
				Debug.Log("[��Ī ��� ����] " + response);
			},
			onError: (error) =>
			{
				Debug.LogError("[��Ī ��� ����] " + error);
			});
	}

	private IEnumerator CheckMatchingStatusRepeatedly()
	{
		while (!isMatchingCompleted)
		{
			yield return PostWithAuth($"{baseUrl}/status",
				onSuccess: (response) =>
				{
					Debug.Log("[��Ī ���� Ȯ��] " + response);
					// �����ϰ� ���ڿ� ���� ���η� üũ
					if (response.Contains("complete") || response.Contains("SUCCESS"))
					{
						OnMatchComplete();
					}
				},
				onError: (error) =>
				{
					Debug.LogWarning("[���� Ȯ�� ����] " + error);
				});

			yield return new WaitForSeconds(1.5f); // 1.5�� ����
		}
	}

	private IEnumerator PostWithAuth(string url, System.Action<string> onSuccess = null, System.Action<string> onError = null)
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
