using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Models;
using server.Models.Dtos;

namespace server.Controllers
{
    [Route("api/")]
    [ApiController]
    public class PublicProductsController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        public PublicProductsController(EcommerceDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("get-products")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                List<Product> products = await _context.Products.OrderByDescending(e => e.ProductId).ToListAsync();
                List<GetProductsDto> parsedProducts = products.Select(product => new GetProductsDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductDescription = product.ProductDescription,
                    ProductCategory = product.ProductCategory,
                    ProductPrice = product.ProductPrice,
                    ProductStock = product.ProductStock,
                    ProductDir = product.ProductDir,
                }).ToList();
                return StatusCode(StatusCodes.Status200OK, parsedProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex });
            }

        }
        [HttpPost]
        [Route("get-product")]
        public IActionResult GetProduct([FromBody] GetProductDto request ) 
        {
        var product = _context.Products.FirstOrDefault(cd=>cd.ProductId == request.ProductId );
        if (product == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { Message = "Product not found" });
            }
        return StatusCode(StatusCodes.Status200OK, product);
        }
    }
}
