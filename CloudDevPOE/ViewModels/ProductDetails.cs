namespace CloudDevPOE.ViewModels
{
	public class ProductDetails
	{
		public int ProductID { get; set; }
		public int UserID { get; set; }
		public string ProductName { get; set; }
		public string ProductCategory { get; set; }
		public string ProductDescription { get; set; }
		public decimal ProductPrice { get; set; }
		public List<string> ImageUrls { get; set; } = new List<string>();
	}
}