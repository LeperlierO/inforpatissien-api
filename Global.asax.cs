using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace inforpatissien_api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Thread sleeper = new Thread(Sleeper);
            sleeper.Start();
        }

        protected void Sleeper()
        {
            string url = "https://inforpatissien-api.azurewebsites.net/";
            string parameters = "recipes";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            while (true)
            {
                client.GetAsync(parameters);
                Thread.Sleep(1000);
            }
        }
    }
}
