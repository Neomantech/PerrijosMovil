using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;

namespace PerrijosGatijos.Services
{
	public static class ObjectExtensions
	{
        public static IDictionary<string, string> ToDictionary<T>(this T source)
        {
            var dictionary = new Dictionary<string, string>();
            var type = source.GetType();
            var properties = GetProperties(type);

            foreach (var property in properties)
            {
                var value = property.GetValue(source, null);

                if (value != null)
                {
                    var fieldName = GetFieldNameForProperty(property);
                    dictionary.Add(fieldName, $"{value}");
                }
            }

            return dictionary;
        }


        public static string ToQueryString<T>(this T source)
        {
            return string.Join("&", source.ToDictionary()
                       .Select(c => $"{WebUtility.UrlEncode(c.Key)}={WebUtility.UrlEncode(c.Value)}")
                       .ToArray());
        }

        private static PropertyInfo[] GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                       .Where(p => p.CanRead && p.GetMethod?.IsPublic == true)
                       .ToArray();
        }



        private static string GetFieldNameForProperty(PropertyInfo propertyInfo)
        {
            var name = propertyInfo.GetCustomAttributes<DataMemberAttribute>(true)
                               .Select(a => a.Name)
                               .FirstOrDefault()
                   ?? propertyInfo.Name;


            return name;
        }
    }
}

