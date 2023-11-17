using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mehr1us.expedition
{
    internal class Retrieve
    {
        public static MethodInfo getMethod(Type parentClass, string methodName)
        {
            return parentClass.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        }

        public static MethodInfo getStaticMethod(Type parentClass, string methodName)
        {
            return parentClass.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        }
        
        public static FieldInfo getField(Type parentClass, string methodName)
        {
            return parentClass.GetField(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        }

        public static FieldInfo getStaticField(Type parentClass, string methodName)
        {
            return parentClass.GetField(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        }
    }
}
