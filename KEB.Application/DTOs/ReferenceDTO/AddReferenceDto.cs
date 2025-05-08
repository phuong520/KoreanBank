using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ReferenceDTO
{
    public class AddReferenceDto
    {
        public Guid CreatedBy { get; set; }
        public string ReferenceName { get; set; } = string.Empty;
        public string ReferenceAuthor { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PublishedYear { get; set; }
        public string ReferenceLink { get; set; } = string.Empty;
        public string? IpAddress { get; set; }

    }
}
