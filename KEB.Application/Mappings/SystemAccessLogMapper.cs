using AutoMapper;
using KEB.Application.DTOs.SystemAccessLogDTO;
using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Mappings
{
    public class SystemAccessLogMapper : Profile
    {
        public SystemAccessLogMapper() {
            CreateMap<SystemAccessLog, AccessLogDisplayDto>().ReverseMap();
            CreateMap<SystemAccessLog, AddQuestionHistoryDto>();

        }
    }
}
