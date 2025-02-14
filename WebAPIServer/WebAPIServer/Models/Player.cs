using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
		public string PlayerPw { get; set; } = ""; // 기본값 추가
	}
}
