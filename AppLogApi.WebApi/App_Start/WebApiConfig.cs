namespace AppLogApi.WebApi
{
    using System.Web.Http;
    using AppLogApi.WebApi.Attributes;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            config.Filters.Add(new AuthorizeAttribute());
            config.Filters.Add(new ExceptionHandlingAttribute());
        }
    }
}
