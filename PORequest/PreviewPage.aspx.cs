using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;

namespace PORequest
{
    public partial class PreviewPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["POCRPreview"] != null)
            {
                ReportDocument cryRpt = new ReportDocument();
                cryRpt = (ReportDocument)Session["POCRPreview"];
                string reportFormat = "PDF";
                switch (reportFormat)
                {
                    case "XLS":
                        cryRpt.ExportToHttpResponse(ExportFormatType.Excel, Response, true,
                            "_Report"); //AFCF_Task_Movement_Report
                        break;
                    case "XLSR":
                        cryRpt.ExportToHttpResponse(ExportFormatType.ExcelRecord, Response, true,
                            "_Report");
                        break;
                    case "PDF":
                        cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false,
                            "_Report");
                        break;
                    case "CSV":
                        cryRpt.ExportToHttpResponse(ExportFormatType.CharacterSeparatedValues, Response, true,
                            "_Report");
                        break;
                    case "TSV":
                        cryRpt.ExportToHttpResponse(ExportFormatType.TabSeperatedText, Response, true,
                            "_Report");
                        break;
                    case "DOC":
                        cryRpt.ExportToHttpResponse(ExportFormatType.WordForWindows, Response, true,
                            "_Report");
                        break;
                    case "TXT":
                        cryRpt.ExportToHttpResponse(ExportFormatType.Text, Response, true,
                            "_Report");
                        break;
                    case "HTML":
                        cryRpt.ExportToHttpResponse(ExportFormatType.HTML40, Response, true,
                            "_Report");
                        break;
                }
            }
        }
    }
}