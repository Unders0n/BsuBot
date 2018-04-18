using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Autofac.Integration.WebApi;
using Microsoft.Bot.Builder.Dialogs;

namespace BsuBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var config = GlobalConfiguration.Configuration;

            

            Conversation.UpdateContainer(
                builder =>
                {
                    // Register your Web API controllers.
                    builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
                    builder.RegisterWebApiFilterProvider(config);

                });

            // Set the dependency resolver to be Autofac.
            config.DependencyResolver = new AutofacWebApiDependencyResolver(Conversation.Container);


            GlobalConfiguration.Configure(cfg =>
            {
                cfg.MapHttpAttributeRoutes();

                cfg.Routes.MapHttpRoute(
                    "DefaultApi",
                    "api/{controller}/{id}",
                    new { id = RouteParameter.Optional }
                );

                // Register your Web API controllers.
              
            });
        }
    }
}
