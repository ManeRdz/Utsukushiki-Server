namespace server.Models.Dtos
{
    public class AddProductToStoreDto
    {
        public string ProductName { get; set; } = null!;

        public string ProductDescription { get; set; } = null!;

        public string ProductCategory { get; set; } = null!;

        public int ProductPrice { get; set; }
        public int ProductStock { get; set; }
        public IFormFile ProductImage { get; set; } = null!;
    }
}
