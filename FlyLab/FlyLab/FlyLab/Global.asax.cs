using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using CWSToolkit;
using System.Web.Hosting;
using FlyLab.Controllers;

namespace FlyLab
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            HostingEnvironment.RegisterVirtualPathProvider(new PathProvider(CWSMasterPages.Bootstrap));
            AreaRegistration.RegisterAllAreas();

            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    #if !DEBUG
        ​protected void Application_Error() {
            var exception = Server.GetLastError();
            var httpException = exception as HttpException;
            Response.Clear();
            Server.ClearError();
            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = "InternalError";
            routeData.Values["exception"] = exception;
            Response.StatusCode = 500;
            if (httpException != null) {
                Response.StatusCode = httpException.GetHttpCode();
	        switch (Response.StatusCode) {
	            case 403:
		        routeData.Values["action"] = "Unauthorized";
		            break;
	            case 404:
		        routeData.Values["action"] = "NotFound";
		            break;
	        }
            }
 
            IController errorsController = new ErrorController();
            var rc = new RequestContext(
                        new HttpContextWrapper(Context),routeData);
            errorsController.Execute(rc);
        }
    #endif
    }
}