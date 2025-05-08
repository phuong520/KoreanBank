using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface IQuestionWithFileService
    {
        Task<string> GetTemplateUrl(bool forMultipleChoice);
        Task UploadExcelTemplate(string? message = "");

    }
}
