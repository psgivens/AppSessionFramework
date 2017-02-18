using PhillipScottGivens.Library.AppSessionFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework.WPF
{
    public class SessionTemplateAttribute : Attribute
    {        
        public SessionTemplateAttribute(Type type)
        {
            SessionType = type;
            //Type typeDescriptorProvider = typeof(SessionTypeDescriptionProvider<>).MakeGenericType(type);
            //MethodInfo intializeMethod = typeDescriptorProvider.GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            //intializeMethod.Invoke(null, null);
        }
        public Type SessionType { get; private set; }
    }
}
