using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ImageFileDTO
{
    public class ImageFileDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public Byte[] FileData { get; set; }
        public string ContentType { get; set; }
    }
}
