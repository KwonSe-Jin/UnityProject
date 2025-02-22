using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCrypt.Net;

namespace WebAPIServer.Models
{
	[Table("player_data")] // 테이블 이름을 'player_data'로 설정
	public class Player
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)] // ID 자동 증가 설정
		public int PlayerId { get; set; }

		[Required]  // NULL 값 방지
		public string PlayerName { get; set; } = ""; // 기본값 추가

		[Required]
		public string PasswordHash { get; set; } = ""; // 기본값 추가

		// 비밀번호 해싱 및 검증 함수 추가
		public void SetPassword(string password)
		{
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
		}

		public bool VerifyPassword(string password)
		{
			return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
		}
	}
}
