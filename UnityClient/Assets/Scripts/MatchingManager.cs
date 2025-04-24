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

	// 매칭 상태에 따른 TEXT 변경
	public bool isMatchingCompleted = false;
    
    //api 서버 주소
	private const string baseUrl = "https://localhost:7187/api/match";

	// 매칭 버튼 클릭 시
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
            // 원형 로딩 이미지 회전
            circularLoader.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }
    }

    // 외부에서 매칭 완료 시 호출
    public void OnMatchComplete()
    {
		isMatchingCompleted = true;
        matchingText.text = "Match Complete !";
        circularLoader.gameObject.SetActive(false); // 원형바 숨김

		if (statusCheckCoroutine != null)
		{
			StopCoroutine(statusCheckCoroutine);
			statusCheckCoroutine = null;
		}

		//  게임 서버 접속 + scene 이동 추가
		Debug.Log("게임 시작 준비 완료");
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
				Debug.Log("[매칭 요청 성공] " + response);
			},
			onError: (error) =>
			{
				Debug.LogError("[매칭 요청 실패] " + error);
			});
	}

	private IEnumerator CancelMatching()
	{
		yield return PostWithAuth($"{baseUrl}/cancel",
			onSuccess: (response) =>
			{
				Debug.Log("[매칭 취소 성공] " + response);
			},
			onError: (error) =>
			{
				Debug.LogError("[매칭 취소 실패] " + error);
			});
	}

	private IEnumerator CheckMatchingStatusRepeatedly()
	{
		while (!isMatchingCompleted)
		{
			yield return PostWithAuth($"{baseUrl}/status",
				onSuccess: (response) =>
				{
					Debug.Log("[매칭 상태 확인] " + response);
					// 간단하게 문자열 포함 여부로 체크
					if (response.Contains("complete") || response.Contains("SUCCESS"))
					{
						OnMatchComplete();
					}
				},
				onError: (error) =>
				{
					Debug.LogWarning("[상태 확인 실패] " + error);
				});

			yield return new WaitForSeconds(1.5f); // 1.5초 마다
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
