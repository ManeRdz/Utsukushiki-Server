using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using server.Models;
using server.Models.Dtos;

namespace server.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AdminProductsController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        private readonly string _rutaServidor;

        public AdminProductsController(EcommerceDbContext context, IConfiguration config)
        {
            _context = context;
            _rutaServidor = config.GetSection("Configuration").GetSection("Ruta").Value;


        }
        [HttpPost]
        [Route("add-product")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddProductToStore([FromForm] AddProductToStoreDto request) 
        {
            try
            {
                string fileExtension = Path.GetExtension(request.ProductImage.FileName).ToLower();

                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Not supported media" });
                }
               
                string rutaImage = Path.Combine(_rutaServidor, request.ProductImage.FileName);

                using (FileStream newImage = System.IO.File.Create(rutaImage))
                {
                    request.ProductImage.CopyTo(newImage);
                    newImage.Flush();
                };

                Product product = new()
                {
                    ProductName = request.ProductName,
                    ProductDescription = request.ProductDescription,
                    ProductCategory = request.ProductCategory,
                    ProductPrice = request.ProductPrice,
                    ProductStock = request.ProductStock,
                    ProductDir = rutaImage
                };
               await _context.Products.AddAsync(product);
               await _context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, new { message = "Product saved" });

            }catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = ex });
            }
        }
        [HttpPost]
        [Route("edit-product")]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> EditProduct([FromForm] EditProductDto request)
        {
            try
            {
                
                    if (request.ProductImage != null)
                    {
                        string rutaImage = Path.Combine(_rutaServidor, request.ProductImage.FileName);
                        using (FileStream newImage = System.IO.File.Create(rutaImage))
                        {
                            request.ProductImage.CopyTo(newImage);
                            newImage.Flush();
                        };
                        Product productWithImage = new()
                        {
                            ProductId = request.ProductId,
                            ProductName = request.ProductName,
                            ProductDescription = request.ProductDescription,
                            ProductCategory = request.ProductCategory,
                            ProductPrice = request.ProductPrice,
                            ProductStock = request.ProductStock,
                            ProductDir = rutaImage
                        };
                        _context.Products.Update(productWithImage);
                        await _context.SaveChangesAsync();
                        return StatusCode(StatusCodes.Status200OK, new { message = "Product edited", image = productWithImage.ProductDir });
                               
                    }
                    else
                    {
                        string rutaImage = request.ProductDir;
                        Product productWithoutImage = new()
                        {
                            ProductId = request.ProductId,
                            ProductName = request.ProductName,
                            ProductDescription = request.ProductDescription,
                            ProductCategory = request.ProductCategory,
                            ProductPrice = request.ProductPrice,
                            ProductStock = request.ProductStock,
                            ProductDir = rutaImage
                        };
                        _context.Products.Update(productWithoutImage);
                        await _context.SaveChangesAsync();
                        return StatusCode(StatusCodes.Status200OK, new { message = "Product edited", image = productWithoutImage.ProductDir });
                    }

                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = ex.Message, exception = ex.InnerException?.Message });
            }
        }
        [HttpDelete]
        [Route("delete-product")]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> DeleteProduct([FromBody] DeleteProductDto request) 
        {
            try{
                var validProduct = await _context.Products.FindAsync(request.ProductId);
                if (validProduct != null)
                {
                    var productInCart = await _context.Carts.Where(c => c.ProductId == request.ProductId).ToListAsync();
                    _context.Carts.RemoveRange(productInCart);
                    System.IO.File.Delete(validProduct.ProductDir);
                    _context.Products.Remove(validProduct);
                    await _context.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, new { message = "Product deleted" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { message = "Product not found" });

                }
            }catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex.Message,  innerException = ex.InnerException?.Message });

            }
        }



    }
}
