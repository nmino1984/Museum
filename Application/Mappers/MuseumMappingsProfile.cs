using AutoMapper;
using Application.ViewModels.Request;
using Application.ViewModels.Response;
using Domain.Entities;
using Infrastructure.Commons.Bases.Response;
using Utilities.Statics;

namespace Application.Mappers
{
    public class MuseumMappingsProfile : Profile
    {
        public MuseumMappingsProfile()
        {
            CreateMap<Museum, MuseumResponseViewModel>()
                .ForMember(dest => dest.listArticles, opt => opt.MapFrom(src => src.Articles.ToList()))
                .ForMember(dest => dest.ThemeId, opt => opt.MapFrom(src => src.Theme))
                .ForMember(dest => dest.Theme, opt => opt.MapFrom(src => (Themes)(src.Theme)))
                .ReverseMap();

            CreateMap<BaseEntityResponse<Museum>, BaseEntityResponse<MuseumResponseViewModel>>()
                .ReverseMap();

            CreateMap<MuseumRequestViewModel, Museum>()
                .ReverseMap();

            CreateMap<Museum, MuseumSelectResponseViewModel>()
                .ForMember(x => x.MuseumId, x => x.MapFrom(y => y.Id))
                .ReverseMap();
        }
    }
}
