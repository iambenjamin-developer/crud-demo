using Application.DTOs.Categories;
using Application.DTOs.Products;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Categories
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();

            //Products
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();

            CreateMap<Product, CreateProductDto>();
            CreateMap<CreateProductDto, Product>();

            CreateMap<Product, UpdateProductDto>();
            CreateMap<UpdateProductDto, Product>();
        }
    }
}
