using KEB.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services
{
    public class APIResponse<T>
    {
        public APIResponse()
        {
            Message = "";
            StatusCode = HttpStatusCode.OK;
            IsSuccess = true;
            Result = new List<T>();
        }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; }
        public List<T> Result { get; set; }
        public Pagination Pagination { get; set; }
        public int TotalCount { get; set; }
    }
}
