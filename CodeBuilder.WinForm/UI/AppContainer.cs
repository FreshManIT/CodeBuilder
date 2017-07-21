using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace CodeBuilder.WinForm.UI
{
    public class AppContainer : Container
    {
        public AppContainer()
        {
            _services = new ServiceContainer();
            _services.AddService(typeof(IServiceContainer), _services);
        }

        private ServiceContainer _services;

        /// <summary>
        /// services
        /// </summary>
        public IServiceContainer Services => _services;

        /// <summary>
        /// Get service
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        protected override object GetService(Type service)
        {
            object s = _services.GetService(service) ?? base.GetService(service);
            return s;
        }

        /// <summary>
        /// get site
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static ISite GetSite(Control control)
        {
            while (control != null && control.Site == null)
                control = control.Parent;
            return control?.Site;
        }

        public static IContainer GetContainer(Control control)
        {
            ISite site = GetSite(control);
            return site?.Container;
        }

        public static object GetService(Control control, Type service)
        {
            ISite site = GetSite(control);
            return site?.GetService(service);
        }

        public static AppContainer GetAppContainer(Control control)
        {
            return GetContainer(control) as AppContainer;
        }
    } 
}
