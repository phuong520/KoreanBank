using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Enums
{
    public enum AddQuestionStatus
    {
        [Description("Đang làm")]
        InProgress = 1,
        [Description("Hoàn thành")]
        Completed = 2,
        [Description("Hoàn thành trễ")]
        CompletedLate = 3,
        [Description("Quá hạn")]
        Overdue = 4,
        [Description("Vô hạn nên không có trạng thái gì")]
        NoDue = 5
    }
}
