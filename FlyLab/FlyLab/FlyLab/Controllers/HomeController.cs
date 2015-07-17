using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlyLab.Models;

namespace FlyLab.Controllers.Home
{
    public class HomeController : Controller
    {
        private LabEntityContainer db = new LabEntityContainer();
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var moduleList = db.Module.Where(t => t.Active).ToList().OrderBy(t => t.Call_id);
            return View(moduleList);
        }

        public PartialViewResult NavBar()
        {
            var modules = db.Module.Where(t => t.Active).ToList().OrderBy(t => t.Call_id);

            return PartialView("_NavBar", modules);
        }
    }
}
