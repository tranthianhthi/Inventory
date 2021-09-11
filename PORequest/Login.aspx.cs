using PORequest.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PORequest
{
    public partial class Login : System.Web.UI.Page
    {
        readonly Configurations configurations = new Configurations();
        protected void Page_Load(object sender, EventArgs e)
        {
           
            if (!IsPostBack) { txtUserName.Focus(); }
            else { txtPassword.Attributes.Add("onkeypress", "return clickButton(event,'" + btnLogin.ClientID + "')"); }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string Userid = txtUserName.Text.Trim();
            string pass = txtPassword.Text.Trim();
            if (Userid == "")
            {
                configurations.ShowThongBao(@"Nhập lại User Name.", this);
                txtUserName.Focus();
                return;
            }
            if (pass == "")
            {
                configurations.ShowThongBao(@"Nhập lại Password.", this);
                txtPassword.Focus();
                return;
            }
            DataTable tb = new DataTable();
            tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetUserID, new List<SqlParameter>() { new SqlParameter("@userid", txtUserName.Text.Trim()), new SqlParameter("@password", txtPassword.Text.Trim()) });
            if (tb.Rows.Count == 0)
            {
                configurations.ShowThongBao("User này không tồn tại trong hệ thống.", this); return;
            }
            else
            {
                foreach (DataRow row in tb.Rows)
                {
                    string[] brand;
                    string strbrand = "";
                    string usergroupr = row["UserGroup"].ToString().Trim();
                    string brandr = row["BelongBrand"].ToString().Trim();
                    brand = brandr.Split(new char[] { '-' });
                    for (var i = 0; i <= brand.Length - 1; i++)
                    {
                        if (i < brand.Length - 1)
                        {
                            strbrand = strbrand + "'" + brand[i] + "',";
                        }
                        else
                        {
                            strbrand = strbrand + "'" + brand[i] + "'";
                        }
                    }
                    if (int.Parse(row["IsLocked"].ToString()) == 1)
                    {
                        configurations.ShowThongBao("This user has been locked. Please ask for admin support", this);
                        return;
                    }
                    
                    Session["brandgroup"] = strbrand.EndsWith(",")? strbrand.Substring(0, strbrand.Length - 1) : strbrand;
                    Session["brand"] = brandr;
                    Session["BM"] = row["BMAuthorities"].ToString() == "1";
                }
                Session["userid"] = txtUserName.Text;
                
                Response.Redirect("RequisitionList.aspx");
            }
        }
    }
}