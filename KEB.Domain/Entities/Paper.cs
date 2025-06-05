using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class Paper:BaseEntity
    {
        public string PaperName { get; set; }
        public Skill Skill {  get; set; }
        public PaperStatus PaperStatus {  get; set; }
        public bool IsReviewed { get; set; }
        public Exam Exam { get; set; }
        public Guid ExamId { get; set; }
        public List<PaperDetail> PaperDetails { get; set; }
    }

}
