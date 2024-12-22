using AutoMapper;
using ElasticSearchAdvancedSearch.Dtos;
using ElasticSearchAdvancedSearch.Models;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace ElasticSearchAdvancedSearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IElasticClient _client;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;

        public UserController(IElasticClient client, ILogger<UserController> logger, IMapper mapper)
        {
            _client = client;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            try 
            {
            var results = await _client.SearchAsync<Product>(
                s => s.Query(
                    q => q.MatchAll()
                ).Size(100)
            );

            return Ok(_mapper.Map<List<ProductReadDto>>(results.Documents.ToList()));
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
                        d => d.Query("*" + keyword + "*")
                    )
                ).Size(100)
            );

            return Ok(_mapper.Map<List<ProductReadDto>>(results.Documents.ToList()));
            }catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }
    }
}