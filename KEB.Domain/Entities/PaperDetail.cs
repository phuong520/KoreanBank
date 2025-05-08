using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class PaperDetail
    {
        public Paper Paper { get; set; }
        public Guid PaperId { get; set; }
        public Question Question { get; set; }
        public Guid QuestionId { get; set; }
        public float Mark {  get; set; }
        public int OrderInPaper { get; set; }
        [NotMapped]
        public ImageFile? Attachment {  get; set; }
        public Guid? AttachmentId { get; set; }
    }
}
