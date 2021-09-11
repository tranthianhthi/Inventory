using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PORequest
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool isLoggedIn = Session["userid"] != null;
            btnLogin.Visible = !isLoggedIn;
            btnLogout.Visible = isLoggedIn;
            //lblWelcome.Text = isLoggedIn ? "Xin chào " + Session["userid"].ToString() + "!" : "";
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session["brandgroup"] = null;
            Session["brand"] = null;
            Session["userid"] = null;

            bool isLoggedIn = Session["userid"] != null;
            btnLogin.Visible = !isLoggedIn;
            btnLogout.Visible = isLoggedIn;
            Response.Redirect("~/Login.aspx");
            //lblWelcome.Text = isLoggedIn ? "Xin chào " + Session["userid"].ToString() + "!" : "";
        }
    }
}