using FinancialManager.Data.Models;
using FinancialManager.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace FinancialManager.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/product")]
    [SwaggerTag("Products RESTFull endpoint")]
    public class ProductController : ControllerBase
    {
        private ProductRepository _productRepository;

        public ProductController(ProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }

        [HttpGet]
        [SwaggerOperation(description: "Get all products")]
        public ActionResult<IList<Product>> Get(string orderBy = "name")
        {
            var products = this._productRepository.Get(orderBy);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> Get(Guid id)
        {
            return Ok(this._productRepository.Get(id));
        }

        [HttpGet("{id}/prices")]
        public ActionResult<IList<Product>> GetPrices(Guid id)
        {
            return Ok(this._productRepository.GetPrices(id));
        }

        [HttpPost]
        public ActionResult Post(Product product)
        {
            var result = this._productRepository.Add(product);

            return result ? Created($"api/v1/product/{product.Id}", product) : BadRequest();
        }

        [HttpPut]
        public async Task<ActionResult> Put(Product product)
        {
            var result = await this._productRepository.UpdateAsync(product);

            return result ? Ok($"api/v1/product/{product.Id}") : BadRequest();
        }

        [HttpPost("{id}/price")]
        public ActionResult Post(Guid id, ProductPrice productPrice)
        {
            if (productPrice.Price <= 0)
                return BadRequest("ProductPrice with no Price is now allowed");

            var result = this._productRepository.AddPrice(id, productPrice);

            if (result)
                return Ok(productPrice);
            else
                return NotFound(JsonSerializer.Serialize(new
                {
                    ProductId = id,
                    Response = "Product not Found",
                    Message = "Product price not included"
                }));
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            return this._productRepository.Delete(id) ? Ok() : NotFound();
        }
    }
}