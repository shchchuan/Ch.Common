using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Ch.Common
{
    public class EnumHelper
    {
        /// <summary>
        /// 获取枚举值描述
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static string GetDescription(object enumType)
        {
            try
            {
                Type _enumType = enumType.GetType();
                DescriptionAttribute dna = null;
                FieldInfo fi = _enumType.GetField(Enum.GetName(_enumType, enumType));
                dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                if (dna != null && string.IsNullOrEmpty(dna.Description) == false)
                    return dna.Description;
            }
            catch (Exception)
            {
            }
            return enumType.ToString();
        }
        ///<summary>  
        /// 获取枚举值+描述  
        ///</summary>  
        ///<param name="enumType">Type,该参数的格式为typeof(需要读的枚举类型)</param>  
        ///<returns>键值对</returns>  
        public static Dictionary<int, string> GetEnumItemValueDesc(Type enumType)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            Type typeDescription = typeof(DescriptionAttribute);
            FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            int strValue = 0;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    strValue = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = field.Name;
                    }
                    dic.Add(strValue, strText);
                }
            }
            return dic;
        }
    }
}
