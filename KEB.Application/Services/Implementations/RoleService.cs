using AutoMapper;
using KEB.Application.DTOs.RoleDTO;
using KEB.Application.Services.Interfaces;
using KEB.Domain.Entities;
using KEB.Domain.ValueObject;
using KEB.Infrastructure.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork ;
        private readonly IMapper _mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<APIResponse<RoleDisplayDto>> GetRoles()
        {
            var response = new APIResponse<RoleDisplayDto>();
            Expression<Func<Role, bool>> filter = x => true;
            try
            {
                var result = await _unitOfWork.Roles.GetAllAsync(filter: filter);
                response.Result = _mapper.Map<List<RoleDisplayDto>>(result.ToList());
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }
    }
}
