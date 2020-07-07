using PIM.Common.CustomExceptions;
using PIMWebMVC.App_Start;
using PIMWebMVC.Constants;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PIMWebMVC
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            AutoMapper.Mapper.Initialize(m =>
            {
                m.AddProfile(new AutoMapperProfile());
            });
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            string culture = "en-US";
            string[] userLanguages = Request.UserLanguages;
            if (userLanguages != null && userLanguages.Count() > 0)
            {
                culture = userLanguages[0];
            }
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            if (exception == null)
            {
                return;
            }
            string action = ErrorsConstant.SERVER_ERROR_ACTION;
            if (exception is ConcurrencyDbException)
            {
                action = ErrorsConstant.CONCURRENCY_ERROR_ACTION;
            }
            else
            {
                HttpContext.Current.Session[ErrorsConstant.ERROR_MESSAGE_SESSION_KEY] = exception.Message;
                Response.Clear();
                HttpException httpException = exception as HttpException;
                if (httpException != null)
                {

                    switch (httpException.GetHttpCode())
                    {
                        case 404:
                            action = ErrorsConstant.NOT_FOUND_ACTION;
                            break;
                        case 500:
                            action = ErrorsConstant.SERVER_ERROR_ACTION;
                            break;
                        case 401:
                            action = ErrorsConstant.ACCESS_DENIED_ACTION;
                            break;
                        case 400:
                            action = ErrorsConstant.BAD_REQUEST_ACTION;
                            break;
                        case 403:
                            action = ErrorsConstant.ACCESS_DENIED_ACTION;
                            break;
                        default:
                            break;
                    }

                }
            }            
            Server.ClearError();
            Response.Redirect($"~/Error/{action}");
        }
    }
}
