using KEB.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.UserDTO
{
    public class GetUser
    {
       
        public string Keyword {  get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public bool SortAscending { get; set; } = true;
        public Pagination? PaginationRequest { get; set; } = new();

    }
}
