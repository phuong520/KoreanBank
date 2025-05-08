using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class References : BaseEntity
    {
        public string ReferenceName { get; set; }
        public string ReferenceAuthor { get; set; }
        public string Description { get; set; }
        public int PublishedYear { get; set; }
        public string ReferencesLink {  get; set; }
        public List<Question> Questions { get; set; }
    }
}
