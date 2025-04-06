using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPIServer.Models
{
	[Table("characters")]
	public class Character
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CharacterId { get; set; }

		[Required]
		public string Name { get; set; } = "";

		public string Description { get; set; } = "";

		public int Price { get; set; }

		public bool IsOnSale { get; set; } = true;
	}
}
