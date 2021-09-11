using Microsoft.Reporting.WebForms;
using System;
using ACFC.Common;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Printing;

namespace TestWeb
{
    public partial class TestPage : System.Web.UI.Page
    {

        private int m_currentPageIndex;
        private IList<Stream> m_streams;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (! this.IsPostBack)
            {

            }
            else
            {
                ShowReport();
            }

        }

        private void ShowReport()
        {
            try
            {
                ReportViewer1.Visible = false;
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report1.rdlc");

                ReportViewer1.LocalReport.SetParameters(new ReportParameter("StoreCode", txtALU.Text));
            }
            catch {  }
        }

        private void PrintReportWithoutPreview()
        {
            try
            {
                ReportViewer1.Visible = false;
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report1.rdlc");

                ReportViewer1.LocalReport.SetParameters(new ReportParameter("StoreCode", txtALU.Text));
                LocalReport rp = ReportViewer1.LocalReport;
                Export(rp);
                Print();
            }
            catch { }
        }

        #region Print report without preview

        // Routine to provide to the report renderer, in order to
        //    save an image for each page of the report.
        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
        // Export the given report as an EMF (Enhanced Metafile) file.
        private void Export(LocalReport report)
        {
            string deviceInfo =
              @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>8.5in</PageWidth>
                <PageHeight>11in</PageHeight>
                <MarginTop>0.25in</MarginTop>
                <MarginLeft>0.25in</MarginLeft>
                <MarginRight>0.25in</MarginRight>
                <MarginBottom>0.25in</MarginBottom>
            </DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream,
               out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;
        }
        // Handler for PrintPageEvents
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

        private void Print()
        {
            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                m_currentPageIndex = 0;
                printDoc.Print();
            }
        }

        #endregion 
    }
}