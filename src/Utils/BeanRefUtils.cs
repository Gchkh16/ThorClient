using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using ThorClient.Core.Model.BlockChain;

namespace ThorClient.Utils
{
    public class BeanRefUtils
    {
        public static Dictionary<string, object> GetFieldValueMap(object bean)
        {
            var cls = bean.GetType();
            var valueMap = new Dictionary<string, object>();
            var methods = cls.GetMethods();
            var properties = cls.GetProperties();
            foreach (var prop in properties)
            {
                try
                {
                    if (prop.CanRead && prop.GetGetMethod(true).IsPublic)
                    {
                        valueMap[prop.Name] = prop.GetValue(bean);
                    }
                }
                catch (Exception e)
                {
                    continue;
                }
            }
            return valueMap;
        }

        public static void SetFieldValue(object bean, Dictionary<string, object> valMap)
        {
            var cls = bean.GetType();
            // get all methods from the bean.
            var fields = cls.GetProperties();

            foreach (var field in fields)
            {
                try
                {
                    string fieldKeyName = field.Name;
                    var value = valMap[fieldKeyName];
                    if (null != value && !"".Equals(value))
                    {
                        var fieldType = field.PropertyType;
                        if (fieldType == typeof(string))
                        {
                            field.SetValue(bean, value);
                        }
                        else if (fieldType == typeof(DateTime))
                        {
                            var temp = ParseDate(value.ToString());
                            field.SetValue(bean, temp);
                        }
                        else if (fieldType == typeof(int))
                        {
                            int intval = int.Parse(value.ToString());
                            field.SetValue(bean, intval);
                        }
                        else if (fieldType == typeof(long))
                        {
                            long temp = long.Parse(value.ToString());
                            field.SetValue(bean, temp);
                        }
                        else if (fieldType == typeof(double))
                        {
                            double temp = double.Parse(value.ToString());
                            field.SetValue(bean, temp);
                        }
                        else if (fieldType == typeof(bool))
                        {
                            bool temp = bool.Parse(value.ToString());
                            field.SetValue(bean, temp);
                        }
                        else if (fieldType == typeof(byte)) {
                            var temp = (byte[])value;
                            field.SetValue(bean, temp);
                        } else if (fieldType == typeof(byte))
                        {
                            byte temp = byte.Parse(value.ToString());
                            field.SetValue(bean, temp);
                        }
                        else
                        {
                            Debug.WriteLine("not support type" + fieldType);
                        }
                    }
                } catch (Exception e) {
                    continue;
                }
            }
        }

        private static DateTime? ParseDate(string datestr)
        {
            if (string.IsNullOrEmpty(datestr))
            {
                return null;
            }
            try
            {
                var fmtstr = datestr.IndexOf(':') > 0 ? "yyyy-MM-dd HH:mm:ss" : "yyyy-MM-dd";
                return DateTime.ParseExact(datestr, fmtstr, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
