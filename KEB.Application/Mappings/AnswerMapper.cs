using AutoMapper;
using KEB.Application.DTOs.AnswerDTO;
using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Mappings
{
    public class AnswerMapper : Profile
    {
        public AnswerMapper()
        {

            CreateMap<Answer, AddAnswerDTO>().ReverseMap();

        }
    }
}
