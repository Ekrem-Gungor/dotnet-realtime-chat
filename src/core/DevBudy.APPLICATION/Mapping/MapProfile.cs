using AutoMapper;
using DevBudy.DOMAIN.Entities.Concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            //#region AppUserMapping
            //CreateMap<AppUserDto, AppUser>().ReverseMap().ForMember(target => target.Id, opt => opt.MapFrom(src => src.Id));
            //#endregion

            //#region AppRoleMapping
            //CreateMap<AppRole, AppRoleDto>().ReverseMap();
            //#endregion

            //#region AppUserRoleMapping
            //CreateMap<AppUserRole, AppUserRoleDto>().ReverseMap();
            //#endregion

            //#region AppUserProfileMapping
            //CreateMap<AppUserProfile, AppUserProfileDto>().ReverseMap();
            //#endregion
        }
    }
}
