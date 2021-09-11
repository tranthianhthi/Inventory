using System;
using System.Web;

namespace Recruitment.Handlers
{
    /// <summary>
    /// Summary description for UploadFile
    /// </summary>
    public class UploadFile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string newFileName = string.Empty;
            if (context.Request.Files.Count > 0)
            {
                HttpFileCollection SelectedFiles = context.Request.Files;

                for (int i = 0; i < SelectedFiles.Count; i++)
                {
                    HttpPostedFile PostedFile = SelectedFiles[i];

                    string FileName = context.Server.MapPath("~/Uploads/" + PostedFile.FileName);

                    newFileName = FileName.Substring(0, FileName.LastIndexOf('.')) + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + FileName.Substring(FileName.LastIndexOf('.'));

                    PostedFile.SaveAs(newFileName);
                }
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("Please Select Files");
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(newFileName);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}