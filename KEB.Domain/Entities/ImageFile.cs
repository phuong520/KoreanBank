using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class ImageFile 
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public string ContentType { get; set; }
        public User User { get; set; }
        public Question QuestionForImage { get; set; }
        public Question QuestionForAudio { get; set; }
        public PaperDetail PaperDetailImage { get; set; }
        public PaperDetail PaperDetailAudio { get; set; }
    }
}
