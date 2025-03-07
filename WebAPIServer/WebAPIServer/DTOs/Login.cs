namespace WebAPIServer.DTOs
{
	//  회원가입 요청 모델
	public class PlayerRegisterRequest
	{
		public string PlayerName { get; set; }
		public string Password { get; set; }
	}

	//  로그인 요청 모델
	public class PlayerLoginRequest
	{
		public string PlayerName { get; set; }
		public string Password { get; set; }
	}
}
