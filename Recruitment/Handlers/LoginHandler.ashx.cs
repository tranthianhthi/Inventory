using Newtonsoft.Json;
using Recruitment.Common;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;

namespace Recruitment.Handlers
{
    /// <summary>
    /// Summary description for LoginHandler
    /// </summary>
    public class LoginHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string strJson = new StreamReader(context.Request.InputStream).ReadToEnd();

            //deserialize the object
            LoginInfo objUsr = Deserialize<LoginInfo>(strJson);
            string response = "";

            try
            {
                List<SqlParameter> paras = new List<SqlParameter>()
                {
                    new SqlParameter("@userId", objUsr.username ),
                    new SqlParameter("@Password", objUsr.password)
                };

                DataTable tb = new DataTable();
                tb = Configuration.ExecuteQueryData(Configuration.ConnectionString, Configuration.SignIn, paras);

                if (tb.Rows.Count == 1)
                {
                    string logoutDiv = "<div id=\"divLogout\" >" +
                        "<label id=\"lblWelcome\" >{0}</ label >" +
                        "<button type=\"submit\" class=\"btn btn-primary btn-sm\">Thoát</button>" +
                        "</div>";
                    logoutDiv = string.Format(logoutDiv, "Xin chào " + objUsr.username  + "!");
                    Dictionary<string, string> result = new Dictionary<string, string>()
                    {
                        {"result", "true" },
                        {"message", "logoutDiv" }
                    };

                    response = JsonConvert.SerializeObject(result);
                }
                else
                {
                    Dictionary<string, string> result = new Dictionary<string, string>()
                    {
                        {"result", "false" },
                        {"message", "Đăng nhập lỗi!<br/>Vui lòng kiểm tra tên & mật khẩu hoặc liên hệ IT để được hỗ trợ." }
                    };

                    response = JsonConvert.SerializeObject(result);
                }
                //Console.Write(response);
                context.Response.Write(response);
            }
            catch { throw; }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public class LoginInfo
        {
            public string username { get; set; }
            public string password { get; set; }
        }


        // Converts the specified JSON string to an object of type T
        public T Deserialize<T>(string context)
        {
            string jsonData = context;

            //cast to specified objectType
            var obj = (T)new JavaScriptSerializer().Deserialize<T>(jsonData);
            return obj;
        }
    }
}