using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace PORequest
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    var p = Request.Path.ToLower().Trim();
        //    if (p.EndsWith("/crystalimagehandler.aspx") && p != "/crystalimagehandler.aspx")
        //    {
        //        var fullPath = Request.Url.AbsoluteUri.ToLower();
        //        var NewURL = fullPath.Replace(".aspx", "");
        //        Response.Redirect(NewURL);
        //    }
        //}
    }
}