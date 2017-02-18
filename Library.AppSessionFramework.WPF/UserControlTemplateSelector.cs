using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace PhillipScottGivens.Library.AppSessionFramework.WPF
{
    public class UserControlTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<Type, DataTemplate> templates
            = new Dictionary<Type, DataTemplate>();

        public UserControlTemplateSelector(params Assembly[] templateAssemblies)
        {
            var types = templateAssemblies.SelectMany(item => item.GetTypes());
            int c = types.Count();
            foreach (var type in types)
            {
                object[] attributes = type.GetCustomAttributes(typeof(SessionTemplateAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    templates.Add(((SessionTemplateAttribute)attributes[0]).SessionType,
                        CreateTemplate(type));
                }
            }

            templates = (from type in types
                         let attributes = type.GetCustomAttributes(typeof(SessionTemplateAttribute), false)
                         where attributes != null && attributes.Length > 0
                         select new
                         {
                             SessionType = ((SessionTemplateAttribute)attributes[0]).SessionType,
                             Template = CreateTemplate(type)
                         })
                         .ToDictionary(
                            item => item.SessionType, 
                            item => item.Template);
        }

        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (item != null && typeof(SessionBase).IsAssignableFrom(item.GetType())) {
                Type targetType = item.GetType();
                DataTemplate template;
                if (templates.TryGetValue(targetType, out template))
                    return template;
            }

            if (typeof(Presenter).IsInstanceOfType(item))
            {
                Type targetType = GetTypeFromIHasValue(item);
                DataTemplate template;
                if (templates.TryGetValue(targetType, out template))
                    return template;
            }

            return base.SelectTemplate(item, container);
        }

        public DataTemplate CreateTemplate<VType>()
        {
            Type type = typeof(VType);
            return CreateTemplate(type);
        }

        public Type GetTypeFromIHasValue(object @object)
        {
            return @object.GetType()
                    .GetInterfaces()
                    .Where(t => t.IsGenericType
                        && t.GetGenericTypeDefinition() == typeof(IHasValue<>))
                    .Select(t => t.GetGenericArguments()[0]).First();
        }

        public DataTemplate CreateTemplate(Type type)
        {
            string xaml =
                @"<DataTemplate 
                    xmlns:views=""clr-namespace:{0};assembly={1}""
                    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""  >            
                        <views:{2} />
                    </DataTemplate>";
//            string xaml =
//                @"<DataTemplate 
//                    xmlns:views=""clr-namespace:{0};assembly={1}""
//                    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""  >            
//                        <views:{2} />
//                    </DataTemplate>";

            xaml = string.Format(xaml, type.Namespace, type.Assembly.GetName().Name, type.Name);
                        
            StringReader stringReader = new StringReader(xaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            return XamlReader.Load(xmlReader) as DataTemplate;

        }
    }
}
