using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Enums
{
    public enum QuestionStatus
    {
        [Description("Chờ duyệt")]
        Pending = 1,
        [Description("Trùng lặp")]
        Duplicated = 2,
        [Description("Không chấp nhận")]
        Denied = 3,
        [Description("Sẵn sàng")]
        Ok = 4
    }
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return enumValue.ToString();
            }
        }
    }

}
