using ElasticSearchAdvancedSearch.Models;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace ElasticSearchAdvancedSearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IElasticClient _client;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IElasticClient client, ILogger<AdminController> logger)
        {
            _client = client;
            _logger = logger;
        }

        [HttpPost("Add-Product")]
        public async Task<IActionResult> Post(Product product)
        {
            try
            {
                await _client.IndexDocumentAsync(product);

                return Created("Product Created Successfully", product);
            }catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Get(string keyword)
        {
            try
            {
            var results = await _client.SearchAsync<Product>(
                s => s.Query(
                    q => q.QueryString(
                        d => d.Query(keyword + "*")
                    )
                ).Size(100)
            );

            return Ok(results.Documents.ToList());
            }catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("Get-All-Products")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
            var results = await _client.SearchAsync<Product>(
                s => s.Query(
                    q => q.MatchAll()
                ).Size(100)
            );

            return Ok(results.Documents.ToList());
            }catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpDelete("Delete-Product/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var response = await _client.DeleteAsync<Product>(id);
                
                if (response.IsValid)
                {
                    return Ok($"Product with ID '{id}' deleted successfully.");
                }
                else
                {
                    _logger.LogWarning($"Failed to delete product: {response.ServerError?.Error?.Reason}");
                    return BadRequest($"Failed to delete product: {response.ServerError?.Error?.Reason}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest("An error occurred while attempting to delete the product.");
            }
        }

        [HttpPut("Update-Product/{id}")]
        public async Task<IActionResult> UpdateProduct(string id, Product updatedProduct)
        {
                try
                {
                    var response = await _client.UpdateAsync<Product>(id, u => u
                        .Doc(updatedProduct)
                        .DocAsUpsert(false)
                );

                if (response.IsValid)
                {
                    return Ok($"Product with ID '{id}' updated successfully.");
                }
                else
                {
                    _logger.LogWarning($"Failed to update product: {response.ServerError?.Error?.Reason}");
                    return BadRequest($"Failed to update product: {response.ServerError?.Error?.Reason}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest("An error occurred while updating the product.");
            }
        }

    }
}