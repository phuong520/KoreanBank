using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool Gender {  get; set; }
        public ImageFile ImageFile { get; set; }
        public Guid? Avatar { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public Guid NotificationId { get; set; }
        public List<Notification> Notifications { get; set; }
        public ICollection<Exam> HostedExams { get; set; }    // Làm Host
        public ICollection<Exam> ReviewedExams { get; set; }  // Làm Reviewer
        public List<SystemAccessLog> SystemAccessLogs { get; set; }
        public List<AddQuestionTask> AddQuestions { get; set; }
    }
}
