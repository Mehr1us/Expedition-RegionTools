using RWCustom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mehr1us.expedition
{
    internal class Retrieve
    {
        public static MethodInfo GetMethod(Type parentClass, string methodName)
        {
            return parentClass.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        }

        public static MethodInfo GetStaticMethod(Type parentClass, string methodName)
        {
            return parentClass.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        }
        
        public static FieldInfo GetField(Type parentClass, string methodName)
        {
            return parentClass.GetField(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        }

        public static FieldInfo GetStaticField(Type parentClass, string methodName)
        {
            return parentClass.GetField(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        }
    }
}
