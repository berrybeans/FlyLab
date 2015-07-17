using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;

namespace FlyLab.Controllers
{
    public class ErrorController : Controller
    {

        public ViewResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View();
        }

        public ViewResult InternalError()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return View();
        }

        public ViewResult Unauthorized()
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return View();
        }
    }
}
