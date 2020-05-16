using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace Ch.Common
{
    /// <summary>
    /// Mapper转换
    /// </summary>
    public class Mapper
    {
        /// <summary>
        /// DataReader转换为实体
        /// </summary>
        /// <typeparam name="T">转换的类型</typeparam>
        /// <param name="reader">DataReader对象</param>
        /// <returns></returns>
        public static T DataReaderToObject<T>(IDataReader reader) where T : new()
        {
            var obj = new T();
            if (reader.Read())
            {
                DynamicBuilder<T> builder = DynamicBuilder<T>.CreateBuilder(reader);
                obj = builder.Build(reader);
            }
            else
            {
                obj = default(T);
            }
            reader.Close();
            return obj;
        }

        /// <summary>
        /// DataReader转换为list
        /// </summary>
        /// <typeparam name="T">要转换的类型</typeparam>
        /// <param name="reader">DataReader对象</param>
        /// <returns></returns>
        public static IList<T> DataReaderToList<T>(IDataReader reader) where T : new()
        {
            DynamicBuilder<T> builder = DynamicBuilder<T>.CreateBuilder(reader);
            IList<T> list = new List<T>();
            while (reader.Read())
            {
                list.Add(builder.Build(reader));
            }
            reader.Close();
            return list;
        }

        /// <summary>
        /// 转换DataRow到指定类型的对象
        /// </summary>
        public static T DataRowToObject<T>(DataRow row) where T : new()
        {
            var obj = new T();
            // 如果row为空，则返回null对象
            if (row == null)
            {
                return default(T);
            }
            // 遍历各个数据列给对象赋值（这样更为合理，以防止对象有附加的只读或计算属性）
            try
            {
                Type dataType = obj.GetType();
                foreach (DataColumn column in row.Table.Columns)
                {
                    string field = column.ColumnName;
                    PropertyInfo p = dataType.GetProperty(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                    if (p != null && p.CanWrite)
                    {
                        if (row[field] != DBNull.Value && row[field].GetType() != typeof(DBNull))
                            p.SetValue(obj, row[field], null);
                        else
                            p.SetValue(obj, null, null);
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {
                return obj;
            }
        }

        /// <summary>
        /// 转换DataTable到List列表
        /// </summary>
        public static List<T> DataTableToObject<T>(DataTable table) where T : new()
        {
            var lst = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                lst.Add(DataRowToObject<T>(row));
            }
            return lst;
        }

        /// <summary>
        /// 从Get/Post提交参数中映射到实体 by linzy 20140331
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ParamsToObject<T>() where T : new()
        {
            T m = new T();

            //获得该类的属性
            var property = m.GetType().GetProperties();
            foreach (var p in property)
            {
                string value = System.Web.HttpContext.Current.Request[p.Name];
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }
                value = System.Web.HttpContext.Current.Server.UrlDecode(value).Trim();
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }
                switch (p.PropertyType.Name)
                {
                    case "String":
                        p.SetValue(m, string.IsNullOrEmpty(value) ? "" : value, null);
                        break;
                    case "Int32":
                        p.SetValue(m, string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(value), null);
                        break;
                    case "Boolean":
                        p.SetValue(m, (value == "1" || string.Compare(value, "true", true) == 0) ? true : false, null);
                        break;
                    case "DateTime":
                        p.SetValue(m, string.IsNullOrEmpty(value) ? DateTime.MinValue : Convert.ToDateTime(value), null);
                        break;
                    case "Decimal":
                        p.SetValue(m, string.IsNullOrEmpty(value) ? 0 : Convert.ToDecimal(value), null);
                        break;
                }
            }

            return m;
        }
    }

    internal class DynamicBuilder<T>
    {
        private static readonly MethodInfo getValueMethod = typeof(IDataRecord).GetMethod("get_Item", new[] { typeof(int) });
        private static readonly MethodInfo isDBNullMethod = typeof(IDataRecord).GetMethod("IsDBNull", new[] { typeof(int) });

        private Load handler;

        private DynamicBuilder()
        {
        }

        public T Build(IDataRecord dataRecord)
        {
            return handler(dataRecord);
        }

        public static DynamicBuilder<T> CreateBuilder(IDataRecord dataRecord)
        {
            var dynamicBuilder = new DynamicBuilder<T>();

            var method = new DynamicMethod("DynamicCreate", typeof(T), new[] { typeof(IDataRecord) }, typeof(T), true);
            ILGenerator generator = method.GetILGenerator();

            LocalBuilder result = generator.DeclareLocal(typeof(T));
            generator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            for (int i = 0; i < dataRecord.FieldCount; i++)
            {
                PropertyInfo propertyInfo = typeof(T).GetProperty(dataRecord.GetName(i),
                                                                   BindingFlags.Instance | BindingFlags.Public |
                                                                   BindingFlags.IgnoreCase);
                Label endIfLabel = generator.DefineLabel();

                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                    generator.Emit(OpCodes.Brtrue, endIfLabel);

                    generator.Emit(OpCodes.Ldloc, result);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, getValueMethod);
                    generator.Emit(OpCodes.Unbox_Any, dataRecord.GetFieldType(i));
                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

                    generator.MarkLabel(endIfLabel);
                }
            }

            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
            return dynamicBuilder;
        }

        #region Nested type: Load

        private delegate T Load(IDataRecord dataRecord);

        #endregion
    }
}
