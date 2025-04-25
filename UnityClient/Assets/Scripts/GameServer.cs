using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Text;

public class GameServer : MonoBehaviour
{
	public static GameServer Instance;
	private ClientWebSocket socket;
	private CancellationTokenSource ct;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public async void Connect(string ip, int port, string token)
	{
		string uri = $"ws://{ip}:{port}/ws";
		socket = new ClientWebSocket();
		ct = new CancellationTokenSource();

		try
		{
			Debug.Log($"[연결 시도] {uri}");
			await socket.ConnectAsync(new Uri(uri), ct.Token);

			if (socket.State == WebSocketState.Open)
			{
				Debug.Log("[WebSocket 연결 성공]");
				ReceiveData();            // 수신 루프 시작
				await SendMessage(token); // 토큰 전송
			}
		}
		catch (Exception ex)
		{
			Debug.LogError($"[WebSocket 연결 실패] {ex.Message}");
		}
	}


	public async Task SendMessage(string message)
	{
		if (socket?.State == WebSocketState.Open)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(message);
			await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, ct.Token);
			Debug.Log($"[메시지 전송] {message}");
		}
	}

	private async void ReceiveData()
	{    
		var buffer = new byte[1024];

		try
		{
			while (socket.State == WebSocketState.Open)
			{
				var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), ct.Token);

				if (result.MessageType == WebSocketMessageType.Close)
				{
					Debug.LogWarning("[WebSocket 종료 요청]");
					await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
				}
				else
				{
					string msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
					Debug.Log($"[서버 수신] {msg}");
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[수신 오류] " + ex.Message);
		}
	}

	private async void OnApplicationQuit()
	{
		if (socket != null && socket.State == WebSocketState.Open)
		{
			await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "App Quit", CancellationToken.None);
		}
	}
}
