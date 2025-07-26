using AutoMapper;
using RecipeBookaApi.DA.Models;
using RecipeBookApi.Dtos.Recipe;
using RecipeBookApi.Dtos.User;
using Microsoft.AspNetCore.Http;

namespace RecipeBookaApi.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<RegisterUserDto, TbUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        CreateMap<TbUser, UserDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Email));

        CreateMap<RecipeDto, TbRecipe>()
            .ForMember(dest => dest.Id, opt => opt.Condition(src => src.id > 0))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions))
            .ForMember(dest => dest.Img, opt => opt.MapFrom(src => src.Img))
            .ForMember(dest => dest.CurrentState, opt => opt.MapFrom(src => src.CurrentState))
            .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate));

        CreateMap<TbRecipe, VwRecipeDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Img, opt => opt.MapFrom(src => src.Img))
            .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
            .ForMember(dest => dest.CurrentState, opt => opt.MapFrom(src => src.CurrentState))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : "Unknown"));
    }
}
