namespace Producer.Domain
{
	public class Product : Content
	{
		public string ImageUrl { get; set; }

		public string BuyLinkUrl { get; set; }

		public double Price { get; set; }

		public bool Available { get; set; }
	}
}
