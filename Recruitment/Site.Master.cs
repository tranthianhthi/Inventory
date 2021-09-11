using Recruitment.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using System.Web.UI;

namespace Recruitment
{
    public partial class SiteMaster : MasterPage
    {

        private string Login =
            "<input runat=\"server\" type=\"text\" placeholder=\"Tên đăng nhập\" id=\"username\" class=\"form-control form-control-sm\" >" + 
            "<input runat=\"server\" type=\"password\" placeholder=\"Mật khẩu\" id=\"psw\" class=\"form-control form-control-sm\" >";

        string logoutDiv = "<div id=\"divLogout\" >" +
                            "<label id=\"lblWelcome\" >{0}</ label >" +
                            "</div>";

        //private string Logout =
        //    "<div id=\"divLogout\" >" +
        //              "<label id=\"lblWelcome\" >{0}</ label >" + 
        //              "<button type=\"submit\" class=\"btn btn-primary btn-sm\">Thoát</button>" +
        //        "</div>";


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["username"] == null)
            {

            }
            else
            {
                divLogin.InnerHtml = string.Format(logoutDiv, "Xin chào " + Session["user"] + "!");
                //Session["username"] = userId;
                //hdusername.Value = userId;
                Button1.Text = "Thoát";
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string userId = username.Value;
            string password = psw.Value;

            try
            {
                if (Session["username"] == null)
                {
                    List<SqlParameter> paras = new List<SqlParameter>()
                    {
                        new SqlParameter("@userId", userId),
                        new SqlParameter("@Password", password)
                    };

                    DataTable tb = new DataTable();
                    tb = Configuration.ExecuteQueryData(Configuration.ConnectionString, Configuration.SignIn, paras);

                    if (tb.Rows.Count == 1)
                    {
                        

                        divLogin.InnerHtml = string.Format(logoutDiv, "Xin chào " + tb.Rows[0]["name"] + "!");
                        Session["username"] = userId;
                        Session["user"] = tb.Rows[0]["name"].ToString();
                        hdaccount.Value = userId;
                        hdusername.Value = tb.Rows[0]["name"].ToString();
                        Button1.Text = "Thoát";
                    }
                    else
                    {

                    }
                }
                else
                {
                    Session["username"] = null;
                    Session["user"] = null;
                    hdaccount.Value = "";
                    hdusername.Value = "";
                    divLogin.InnerHtml = Login;
                    Button1.Text = "Đăng nhập";
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}