﻿using System.Web.Mvc;

namespace PIMWebMVC.Controllers
{
    /// <summary>
    /// Create View to display corresponding Error
    /// </summary>
    public class ErrorController : Controller
    {
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult ServerError()
        {
            return View();
        }

        public ActionResult BadRequest()
        {
            return View();
        }
    }
}