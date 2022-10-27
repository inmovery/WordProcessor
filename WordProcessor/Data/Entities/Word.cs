using System.ComponentModel.DataAnnotations;
using WordProcessor.Data.Entities.Base;

namespace WordProcessor.Data.Entities
{
	public class Word : BaseEntity
	{
		[Required]
		public string Content { get; set; } = default!;

		[Required]
		public int Count { get; set; }

		[Required]
		public double Frequency { get; set; }
	}
}
