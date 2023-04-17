using FinancialManager.Data.Models;
using FinancialManager.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FinancialManager.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/product")]
    public class ProductController : ControllerBase
    {
        private ProductRepository _productRepository;

        public ProductController(ProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }

        [HttpGet]
        public ActionResult<IList<Product>> Get()
        {
            var products = this._productRepository.Get();
            return Ok(products);
        }

        [HttpGet("{id}/prices")]
        public async Task<ActionResult<IList<Product>>> Get(Guid id)
        {
            return Ok(this._productRepository.GetPrices(id));
        }

        [HttpPost]
        public ActionResult Post(Product product)
        {
            var result = this._productRepository.Add(product);
        
            return result ? Created($"api/v1/product/{product.Id}", product) : BadRequest();
        }

        [HttpPost("{id}/price")]
        public ActionResult Post(Guid id, ProductPrice productPrice)
        {
            if (productPrice.Price <= 0)
                return BadRequest("ProductPrice with no Price is now allowed");

            this._productRepository.AddPrice(id, productPrice);

            return Ok(productPrice);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            return this._productRepository.Delete(id) ? Ok() : NotFound();
        }
    }
}