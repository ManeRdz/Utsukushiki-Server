﻿namespace server.Models.Dtos
{
    public class EditProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;

        public string ProductDescription { get; set; } = null!;

        public string ProductCategory { get; set; } = null!;

        public int ProductPrice { get; set; }
        public int ProductStock { get; set; }

        public string ProductDir { get; set; } = null!;

        public IFormFile? ProductImage { get; set; }
    }
}