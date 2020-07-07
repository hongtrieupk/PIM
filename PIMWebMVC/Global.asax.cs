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
            string action = ErrorActionsConstant.SERVER_ERROR;
            if (exception is ConcurrencyUpdateException)
            {
                action = ErrorActionsConstant.CONCURRENCY_ERROR;
            }
            else
            {
                HttpContext.Current.Session[ErrorActionsConstant.ERROR_MESSAGE_SESSION_KEY] = exception.Message;
                Response.Clear();
                HttpException httpException = exception as HttpException;
                if (httpException != null)
                {

                    switch (httpException.GetHttpCode())
                    {
                        case 404:
                            action = ErrorActionsConstant.NOT_FOUND;
                            break;
                        case 500:
                            action = ErrorActionsConstant.SERVER_ERROR;
                            break;
                        case 401:
                            action = ErrorActionsConstant.ACCESS_DENIED;
                            break;
                        case 400:
                            action = ErrorActionsConstant.BAD_REQUEST;
                            break;
                        case 403:
                            action = ErrorActionsConstant.ACCESS_DENIED;
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
