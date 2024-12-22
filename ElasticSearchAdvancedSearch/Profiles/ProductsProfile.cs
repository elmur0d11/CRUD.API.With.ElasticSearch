using AutoMapper;
using ElasticSearchAdvancedSearch.Dtos;
using ElasticSearchAdvancedSearch.Models;

namespace ElasticSearchAdvancedSearch.Profiles
{
    public class ProductsProfile : Profile
    {
        public ProductsProfile()
        {
            CreateMap<Product, ProductReadDto>();
        }
    }
}