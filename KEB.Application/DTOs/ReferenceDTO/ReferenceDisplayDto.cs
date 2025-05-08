using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ReferenceDTO
{
    public class ReferenceDisplayDto
    {
        public Guid Id { get; set; }
        public string ReferenceName { get; set; }
        public string ReferenceAuthor { get; set; }
        public string Description { get; set; }
        public int PublishedYear { get; set; }
        public string ReferenceLink { get; set; }
        public int NumOfQuestions { get; set; }

    }
}
