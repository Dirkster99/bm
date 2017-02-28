using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace System.IO.Utils
{
    public static class AttributeUtils
    {
        //http://www.minddriven.de/index.php/technology/dot-net/net-winrt-get-custom-attributes-from-enum-value
        public static IEnumerable<Attribute> GetCustomAttributes(object obj)
        {
            return obj.GetType().GetTypeInfo().GetDeclaredField(obj.ToString()).GetCustomAttributes();
        }        
    }

    public static class AttributeUtils<T> where T : Attribute
    {
        public static IEnumerable<T> GetCustomAttributes(object obj)
        {
            Type type = obj.GetType();
            //if (type.IsEnum)
            FieldInfo fi = type.GetTypeInfo().GetDeclaredField(obj.ToString());
            if (fi != null)
                return fi.GetCustomAttributes<T>();
            else return new List<T>();
            //return type.GetTypeInfo().GetCustomAttributes<T>();
        }

        public static IEnumerable<T> GetCustomAttributes(Type type)
        {
            return type.GetTypeInfo().GetCustomAttributes<T>();
        }


        /// <summary>
        /// Given a set of properties, return a list of properties that support the attribute T, as well as the filterStr method.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="filterMethod"></param>
        /// <returns></returns>
        public static object[] FilterPropertiesForAttribute(object[] properties, Func<T, bool> filterMethod)
        {
            List<object> retList = new List<object>();

            foreach (object item in properties)
            {
                T attribute = FindAttribute(item);
                if (attribute != null && filterMethod(attribute))
                    retList.Add(item);
            }

            return retList.ToArray();
        }

        /// <summary>
        /// Given a set of properties, return a list of properties that support the attribute T.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static object[] FilterPropertiesForAttribute(object[] properties)
        {
            return FilterPropertiesForAttribute(properties, (a) => { return true; });
        }


        static Dictionary<object, IEnumerable<T>> propertyAttributeDic = new Dictionary<object, IEnumerable<T>>();

        /// <summary>
        /// Find the specific attribute from object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindAllAttributes(object obj)
        {
            lock (propertyAttributeDic)
            {
                if (propertyAttributeDic.ContainsKey(obj))
                    return propertyAttributeDic[obj];
            }

            List<T> retList = new List<T>();

            foreach (var attribute in
                AttributeUtils<T>.GetCustomAttributes(obj))
            {
                retList.Add((T)attribute);
            }

            lock (propertyAttributeDic)
            {
                if (!(propertyAttributeDic.ContainsKey(obj)))
                    propertyAttributeDic.Add(obj, retList);
            }

            return retList;
        }

        /// <summary>
        /// Find the specific attribute from object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>        
        public static T FindAttribute(object obj)
        {            
            return FindAllAttributes(obj).FirstOrDefault();
        }

    }
}
