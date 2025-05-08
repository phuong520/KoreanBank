using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ReferenceDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface IReferenceService
    {
        Task<APIResponse<ReferenceDisplayDto>> GetAllReferences();
        Task<APIResponse<ReferenceDisplayDto>> AddNewReference(AddReferenceDto request);
        Task<APIResponse<ReferenceDisplayDto>> UpdateReference(UpdateReference request);
        Task<APIResponse<ReferenceDisplayDto>> DeleteReference(Delete request);

    }
}
