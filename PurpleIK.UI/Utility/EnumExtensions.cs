using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Core
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            Type enumType = value.GetType();
            string enumName = Enum.GetName(enumType, value);

            MemberInfo member = enumType.GetMember(enumName)[0];
            DisplayAttribute displayAttribute = member.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute == null ? value.ToString() : displayAttribute.Name;
        }
    }
}
