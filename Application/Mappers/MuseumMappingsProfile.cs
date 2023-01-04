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
                /*.ForMember(x => x.CategoryId, x => x.MapFrom(y => y.Id))*/
                .ForMember(x => x.ThemeId, x => x.MapFrom(y => (Themes)(y.Theme)))
                .ReverseMap();

            CreateMap<BaseEntityResponse<Museum>, BaseEntityResponse<MuseumResponseViewModel>>()
                .ReverseMap();

            CreateMap<MuseumRequestViewModel, Museum>();

            CreateMap<Museum, MuseumSelectResponseViewModel>()
                .ForMember(x => x.MuseumId, x => x.MapFrom(y => y.Id))
                .ReverseMap();
        }
    }
}
