using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Mappings
{
    public class BaseMapper<TDisplay, TCreate, TUpdate, TEntity> : Profile
    {
        public BaseMapper()
        {
            CreateMap<TCreate, TEntity>().ReverseMap();
            CreateMap<TEntity, TDisplay>().ReverseMap();
            CreateMap<TUpdate, TEntity>().ReverseMap();
        }
    }
}
