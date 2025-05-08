
using AutoMapper;
using KEB.Application.DTOs.UserDTO;
using KEB.Application.Utils;
using KEB.Domain.Entities;

namespace KEB.Application.Mappings
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<UpdateUser, User>().ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => CommonUntils.NormalizeName(src.FullName)));

            //CreateMap<UserCreateDTO, User>().ForMember(dest => dest.Avatar,
            //    opt => opt.MapFrom(src => src.ImageFile.FileName));

            CreateMap<User, UserDisplayDTO>()
                .ForMember(dest => dest.UserId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RoleName,
                    opt => opt.MapFrom(src => src.Role.RoleName))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.ImageFile))
                ;
                
            
            CreateMap<UserCreateDTO, User>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.Avatar, opt => opt.Ignore()) // Giả sử Avatar sẽ được xử lý riêng
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.DateOfBirth)))
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ImageFile, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore());

            CreateMap<User, ShortUserDTO>()
                .ForMember(dest => dest.RoleName,
                    opt => opt.MapFrom(src => src.Role.RoleName));

        }

    }
}
