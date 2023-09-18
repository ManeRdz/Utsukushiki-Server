namespace server.Models.Dtos
{
    public class GetProductsDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string ProductDescription { get; set; } = null!;

        public string ProductCategory { get; set; } = null!;

        public int ProductPrice { get; set; }

        public string ProductDir { get; set; } = null!;

        public int ProductStock { get; set; }
    }
}
