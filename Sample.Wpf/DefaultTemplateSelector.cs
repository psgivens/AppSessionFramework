using PhillipScottGivens.Library.AppSessionFramework.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Wpf {
    public class DefaultTemplateSelector : UserControlTemplateSelector {
        public DefaultTemplateSelector()
            : base(typeof(DefaultTemplateSelector).Assembly) { }

        //private static readonly List<Assembly> _userInterfaceAssemblies = new List<Assembly>();

        //public static void AddAssemblies(IEnumerable<Assembly> assemblies) {
        //    _userInterfaceAssemblies.AddRange(assemblies);
        //}

        //private static Assembly[] GetUIAssemblies() {
        //    var config = InsightConfiguration.Instance;
        //    var appReferences = config.AppReferencesElement.GetAppReferences();
        //    return appReferences
        //        .Select(item => Assembly.LoadWithPartialName(item.UIAssemblyName))
        //        .ToArray();
        //}
    }
}
