using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services
{
    public class APIResponse1<T>
    {
        public APIResponse1()
        {
            Message = "";
            StatusCode = HttpStatusCode.OK;
            IsSuccess = true;
        }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; }
        public T? Result { get; set; }
    }
}
