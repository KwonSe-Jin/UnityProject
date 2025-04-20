using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPIServer.Models
{
	[Table("user_characters")]
	public class UserCharacter
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int UserCharacterId { get; set; }
		public int PlayerId { get; set; }
		[ForeignKey("PlayerId")]
		public Player Player { get; set; }
		public int CharacterId { get; set; }
		[ForeignKey("CharacterId")]
		public Character Character { get; set; }
		public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
	}
}
