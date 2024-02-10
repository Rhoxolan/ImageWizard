namespace ImageWizard.Data.Entities
{
	public class ImageEntity
	{
		public int Id { get; set; }

		public string Path { get; set; } = default!;

		public User? User { get; set; } = default;
	}
}
