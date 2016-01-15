using System;
using System.Threading.Tasks;
using System.Web.Http;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Owin;
using Owin;
using RandomTextList.Code;
using RandomTextList.Controllers;

[assembly: OwinStartup(typeof(RandomTextList.Startup))]

namespace RandomTextList
{
    public class Startup
    {
        private WindsorContainer _container;

        public void Configuration(IAppBuilder app)
        {
            _container = new WindsorContainer();

            RegisterDependencies();

            var httpConfig = new HttpConfiguration();
            httpConfig.SetContainer(_container);
            httpConfig.MapHttpAttributeRoutes();
            httpConfig.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            app.UseWebApi(httpConfig);
        }

        private void RegisterDependencies()
        {
            _container.Register(
                Component
                    .For<RandomRecordsController>()
                    .LifestylePerWebRequest());
        }
    }
}
