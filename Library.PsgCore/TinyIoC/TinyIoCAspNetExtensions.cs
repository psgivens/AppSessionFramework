using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PhillipScottGivens.Libraries.PsgCore.TinyIoC
{
    public class HttpContextLifetimeProvider : TinyIoCContainer.ITinyIoCObjectLifetimeProvider
    {
        private readonly string _KeyName = String.Format("TinyIoC.HttpContext.{0}", Guid.NewGuid());

        public object GetObject()
        {
            throw new NotSupportedException("To support, uncomment the body of GetObject");
         //   return HttpContext.Current.Items[_KeyName];
        }

        public void SetObject(object value)
        {
            throw new NotSupportedException("To support, uncomment the body of SetObject");
            //HttpContext.Current.Items[_KeyName] = value;
        }

        public void ReleaseObject()
        {
            var item = GetObject() as IDisposable;

            if (item != null)
                item.Dispose();

            SetObject(null);
        }
    }

    public static class TinyIoCAspNetExtensions
    {
        public static TinyIoC.TinyIoCContainer.RegisterOptions AsPerRequestSingleton(this TinyIoC.TinyIoCContainer.RegisterOptions registerOptions)
        {
            return TinyIoCContainer.RegisterOptions.ToCustomLifetimeManager(registerOptions, new HttpContextLifetimeProvider(), "per request singleton");
        }
    }
}
