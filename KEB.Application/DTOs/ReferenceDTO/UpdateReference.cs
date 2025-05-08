using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ReferenceDTO
{
    public class UpdateReference
    {
        public Guid CreatedBy { get; set; }
        public Guid TargetObjectId { get; set; }
        public string ReferenceName { get; set; }
        public string ReferenceAuthor { get; set; }
        public string Description { get; set; }
        public int PublishedYear { get; set; }
        public string ReferenceLink { get; set; }
        public string? IpAddress { get; set; }

    }
}
