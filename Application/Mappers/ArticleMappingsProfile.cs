using AutoMapper;
using Application.ViewModels.Request;
using Application.ViewModels.Response;
using Domain.Entities;
using Infrastructure.Commons.Bases.Response;
using Utilities.Statics;

namespace Application.Mappers
{
    public class ArticleMappingsProfile : Profile
    {
        public ArticleMappingsProfile()
        {
            CreateMap<Article, ArticleResponseViewModel>()
                .ReverseMap();

            CreateMap<BaseEntityResponse<Article>, BaseEntityResponse<ArticleResponseViewModel>>()
                .ReverseMap();

            CreateMap<ArticleRequestViewModel, Article>();

            /*CreateMap<Category, CategorySelectResponseViewModel>()
                .ForMember(x => x.CategoryId, x => x.MapFrom(y => y.Id))
                .ReverseMap();*/
        }
    }
}
