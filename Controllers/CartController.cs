using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using server.Models;
using server.Models.Dtos;

namespace server.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly EcommerceDbContext _context;

        public CartController(EcommerceDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Authorize(Roles="user")]
        [Route("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto request)
        {

            try
            {
                var validUser =  await _context.Users.FirstOrDefaultAsync(c => c.UserId == request.UserId);
                var validProduct =  await _context.Products.FirstOrDefaultAsync(c=> c.ProductId == request.ProductId);
                if (validUser == null) 
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { message = "User not found" });
                }else if(validProduct == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { message = "Product not found" });
                }
                var productAlreadyInCart = await _context.Carts.FirstOrDefaultAsync(c => c.ProductId == request.ProductId && c.UserId == request.UserId);
                if (productAlreadyInCart == null)
                {
                    await _context.Carts.AddAsync(new Cart { UserId = request.UserId, ProductId = request.ProductId, Cantidad = request.Cantidad });
                    await _context.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, new { message = "Product added correctly" });


                }
                else
                {
                    int totalQuantity = productAlreadyInCart.Cantidad + request.Cantidad;
                    if(totalQuantity > validProduct.ProductStock)
                    {
                        return StatusCode(StatusCodes.Status200OK, new {message= "Not enough stock", quantityInCart = productAlreadyInCart.Cantidad});
                    }
                    else
                    {
                      productAlreadyInCart.Cantidad += request.Cantidad;
                      await _context.SaveChangesAsync();
                      return StatusCode(StatusCodes.Status200OK, new { message = "Product already in your cart" });                   
                    }
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest,ex.Message);
            }
            

        }
        [HttpPost]
        [Authorize(Roles = "user")]
        [Route("get-products-in-cart")]
        public async Task<IActionResult> GetProductsInCart([FromBody] GetProductsInCartDto user )
        {
            try{
                var validUser = await _context.Users.FirstOrDefaultAsync(c => c.UserId == user.UserId);
                if (validUser == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { message = "User not found" });
                }
                var products = _context.Products
                .Join(_context.Carts, p => p.ProductId, c => c.ProductId, (p, c) => new { Product = p, Cart = c })
                .Where(pc => pc.Cart.UserId == user.UserId)
                .Select(pc => new
                {
                    pc.Product.ProductId,
                    pc.Product.ProductName,
                    pc.Product.ProductPrice,
                    pc.Product.ProductDir,
                    pc.Product.ProductStock,
                    pc.Cart.Cantidad,
                })
                .ToList();
                return StatusCode(StatusCodes.Status200OK,  products );
            }catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new {Error = ex.Message});
            }

        }
        [HttpPost]
        [Route("plus-one")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddOneToQuantity([FromBody] AddToCartDto request)
        {
            var validUser = await _context.Users.FirstOrDefaultAsync(c => c.UserId == request.UserId);
            var validProduct = await _context.Products.FirstOrDefaultAsync(c => c.ProductId == request.ProductId);
            if (validUser == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = "User not found" });
            }
            else if (validProduct == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = "Product not found" });
            }
            else
            {
                var productExistsInCart = await _context.Carts.FirstOrDefaultAsync(cd => cd.ProductId == validProduct.ProductId && cd.UserId == validUser.UserId);
                if (productExistsInCart != null)
                {
                    if (productExistsInCart.Cantidad == validProduct.ProductStock)
                    {
                        return StatusCode(StatusCodes.Status200OK, new { message = "Not enough stock" });
                    }
                    else
                    {
                        productExistsInCart.Cantidad++;
                        await _context.SaveChangesAsync();
                        return StatusCode(StatusCodes.Status200OK, new { message = "Quantity changed" });
                    }
                }
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { message = "Product not found in cart" });
                }
            }
        }
        [HttpPost]
        [Route("minus-one")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> RemoveOneToQuantity([FromBody] AddToCartDto request)
        {
            var validUser = await _context.Users.FirstOrDefaultAsync(c => c.UserId == request.UserId);
            var validProduct = await _context.Products.FirstOrDefaultAsync(c => c.ProductId == request.ProductId);
            if (validUser == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = "User not found" });
            }
            else if (validProduct == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = "Product not found" });
            }
            else
            {
                var productExistsInCart = await _context.Carts.FirstOrDefaultAsync(cd => cd.ProductId == validProduct.ProductId && cd.UserId == validUser.UserId);
                if (productExistsInCart != null)
                {
                    if (productExistsInCart.Cantidad == 1)
                    {
                        return StatusCode(StatusCodes.Status200OK);
                    }
                    else
                    {
                        productExistsInCart.Cantidad--;
                        await _context.SaveChangesAsync();
                        return StatusCode(StatusCodes.Status200OK, new { message = "Quantity changed" });
                    }
                }
                {
                    return StatusCode(StatusCodes.Status200OK, new { message = "Product not found in cart" });
                }
            }
        }
        [HttpDelete]
        [Route("delete-from-cart")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteFromCart([FromBody] AddToCartDto request)
        {
            var isProductInCart = await _context.Carts.FirstAsync(cd => cd.ProductId == request.ProductId && cd.UserId == request.UserId);
            if(isProductInCart == null)
            {
                return StatusCode(StatusCodes.Status200OK, new { message = "Product not found in your cart" });
            }
            else
            {
                 _context.Carts.Remove(isProductInCart);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { message = "Product deleted from your cart" });
            }
        }
        
        

        
        
    }
}
