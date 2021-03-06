﻿using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Filters;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Owin;
using Owin;
using RandomTextList.Code;
using RandomTextList.Controllers;
using RandomTextList.DAL;
using RandomTextList.Models;

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

            _container.Register(Component.For<IDBContextFactory>()
                .ImplementedBy<RandomRecordsContextFactory>().LifestyleSingleton());

            _container.Register(Component
                .For<DatabaseWriter<Record>>()
                .ImplementedBy<DatabaseWriter<Record>>()
                .LifestyleSingleton());

            _container.Register(
                Component
                    .For<DbContext>()
                    .ImplementedBy<RandomRecordsContext>()
                    .LifestylePerWebRequest());
            _container.Register(
                Component
                .For<IDatagenerator<Record>>()
                .ImplementedBy<RandomRecordsGenerator>()
                .LifestyleSingleton());

            _container.Register(
                Component
                    .For<RecordsController>()
                    .LifestylePerWebRequest());
        }
    }
}
