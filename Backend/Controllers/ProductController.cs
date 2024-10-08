using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Route("add")]
        public ActionResult CreateProduct(Product product, IFormFile Image, int stock)
        {
            try
            {
                if (Image != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        Image.OpenReadStream().CopyTo(memoryStream);
                        product.Image = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
                else
                {
                    product.Image = "";
                }

                bool created = _productService.CreateProduct(product, stock);
                if (created)
                {
                    return Ok("Product added to the system successfully.");
                }
                return BadRequest("Product creation failed due to validation constraints.");
            }
            catch (Exception ex)
            {
                // Log the exception (you can add more specific logging)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("update/{id}")]
        public ActionResult UpdateProduct(string id, [FromBody] Product updatedProduct)
        {
            bool updated = _productService.UpdateProduct(id, updatedProduct);
            if (updated)
            {
                return Ok("Product updated successfully.");
            }
            return BadRequest("Product update failed due to reservation not found.");
        }

        [HttpGet]
        [Route("getAll")]
        public ActionResult<List<Product>> GetAllProducts()
        {
            List<Product> products = _productService.GetAllProducts();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public ActionResult<List<Product>> GetProductById(string id)
        {
            List<Product> product = _productService.GetProductById(id);
            if (product.Count != 0)
            {
                return Ok(product);
            }
            return BadRequest("product not found.");
        }


        [HttpDelete("delete/{id}")]
        public ActionResult DeleteProduct(string id)
        {
            bool canceled = _productService.DeleteProduct(id);
            if (canceled)
            {
                return Ok("Product deleted successfully.");
            }
            return BadRequest("Product delete failed due to Profile not found.");
        }


        [HttpPut("updateStatus/{id}")]
        public ActionResult UpdateProductStatus(string id, [FromBody] Product model)
        {
            // Assuming status = 1 means active, status = 0 means inactive
            int status = model.Status;

            bool updated = _productService.UpdateProductStatus(id, status);

            if (updated)
            {
                return Ok(new { message = "Product status updated successfully." });
            }

            return NotFound(new { message = "Product not found or update failed." });
        }

        [HttpGet]
        [Route("getProducts")]
        //for create inventory 
        public ActionResult<List<Product>> GetProducts()
        {
            List<Product> products = _productService.GetProducts();
            return Ok(products);
        }


    }
}
