using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO.Utils
{
    /// <summary>
    /// Enum related utils.
    /// </summary>
    /// <typeparam name="A">Type of the attribute.</typeparam>
    /// <typeparam name="ET">Hosting enum type of the specific attribute.</typeparam>
    public static class EnumAttributeUtils<A, ET> where A : Attribute
    {
        /// <summary>
        /// Find the specific attribute from object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static A FindAttribute(object obj) 
        {
            return AttributeUtils<A>.FindAttribute(obj);
        }

        public static IEnumerable<A> FindAllAttributes(object obj)
        {
            return AttributeUtils<A>.FindAllAttributes(obj);
        }

        /// <summary>
        /// Return the first item in ET that has the attribute A and evalFunc return true.
        /// </summary>
        /// <param name="evalFunc"></param>
        /// <returns></returns>
        public static ET ParseByEval(Func<A, bool> evalFunc)
        {
            foreach (var enumValue in Enum.GetValues(typeof(ET)))
            {
                if (evalFunc(AttributeUtils<A>.FindAttribute(enumValue)))
                    return (ET)enumValue;
            }
            return default(ET);
        }



        
    }
}
