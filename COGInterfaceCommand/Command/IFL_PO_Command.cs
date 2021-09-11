using COGInterfaceCommand.Common.COG;
using COGInterfaceCommand.Common;
using System.Data;
using System;

namespace COGInterfaceCommand.Command
{
   public class IFL_PO_Command
    {
        IFL_PO_Master po = new IFL_PO_Master(null,0,'A');
        public void GetCogDataMaster(char mode,string asn, string docname)
        {
            Configurations config = new Configurations();
            DataTable td = new DataTable();            
            int poline = 1;
            try
            {
                string sql = "select A.STYLE,B.DOCUMENT_NAME,B.ASN_NUMBER,B.CREATED_DATE, SUM(A.SHIPPED_QTY) AS QTY from COG_ASN_SKU A INNER JOIN COG_ASN B " +
                "ON A.ASN_NUMBER=B.ASN_NUMBER WHERE B.ASN_NUMBER='"+asn+"' AND B.DOCUMENT_NAME='"+docname+"' GROUP BY A.STYLE,B.DOCUMENT_NAME,B.ASN_NUMBER,B.CREATED_DATE";
                td = config.ExecuteQueryData(config.RPConnection, sql, null);               
                if (td.Rows.Count != 0)
                {
                    foreach (DataRow row in td.Rows)
                    {
                        po = new IFL_PO_Master(row, poline, mode);
                        po.ProcessObject(asn,docname);
                        poline += poline;
                    }
                }
            }
            catch
            {
                throw;
            }            
        }
        public void GetCogPklMaster(char mode, string asn, string docname)
        {
            Configurations config = new Configurations();
            DataTable td = new DataTable();
            int poline = 1;
            string sql = "select a.colour,a.sizedesc,a.style,a.license_plate_ref,b.barcode,c.country as origin,sum(shipped_qty) qty from cog_asn_sku a " +
                "INNER JOIN cog_barcodemaster b on a.style = b.style and a.colour = b.color and a.sizedesc = b.sizedesc and b.primary = 'Y' " +
                "inner join cog_country_code c on a.origin_country_code = c.country_code where a.asn_number='"+asn+"' and a.document_name='"+docname+ "' " +
                "group by a.colour,a.sizedesc,a.style,a.license_plate_ref,b.barcode,c.country order by license_plate_ref";
            td = config.ExecuteQueryData(config.RPConnection, sql, null);
            try
            {
                if (td.Rows.Count != 0)
                {
                    foreach (DataRow row in td.Rows)
                    {
                        IFL_PKL_Mater pkl = new IFL_PKL_Mater(row, poline, mode,asn,docname);
                        pkl.ProcessObject( asn, docname);
                        IFL_SA_Mater sa = new IFL_SA_Mater(row, poline, mode, asn, docname);
                        sa.ProcessObject(asn, docname);
                    }
                }
            }
            catch
            {
                throw;
            }
        }     
        public void WriteCVSFile(string asn, string docname)
        {
            Configurations config = new Configurations();
            IFL_PO_Master po = new IFL_PO_Master(null,0,'A');           
            string pofilename = Environment.CurrentDirectory + "\\IFL\\PO\\Cog_PO_" + asn + ".csv";//"D://Projects//Documents//Interface CottonOn//Cog_PO_"+asn+".csv";
            string pklfilename = Environment.CurrentDirectory + "\\IFL\\PKL\\Cog_PKL_" + asn + ".csv";//"D://Projects//Documents//Interface CottonOn//Cog_PKL_" + asn + ".csv";
            string safilename = Environment.CurrentDirectory + "\\IFL\\SA\\Cog_SA_" + asn + ".csv";  //"D://Projects//Documents//Interface CottonOn//Cog_SA_" + asn + ".csv";
            string sqlpo = "select Warehouse,Owner,PO,to_char(trunc(PO_Date), 'DD-MM-YYYY') PO_Date,PO_Line_No,Qty from COG_IFL_PO where cog_asn_number='" + asn+"' and cog_document_name='"+docname+"'";
            string sqlpkl = "select COG_PO_NO,COG_PKL,CARTON_ID,COG_BARCODE,COG_ORIGIN,COG_LOT_NO,to_char(trunc(COG_EXPIRY_DATE), 'DD-MM-YYYY') COG_EXPIRY_DATE,QTY FROM COG_IFL_PKL WHERE cog_asn_number='" + asn + "' and cog_document_name='" + docname + "'";
            string sqlsa = "SELECT WAREHOUSE,TRANS_CODE,ALLOCATIONTYPE,REMARKS1,REMARKS2,SUBSTR(ODERREF,1,30) ODERREF,to_char(trunc(REQ_DELIVERY_DATE), 'DD-MM-YYYY') REQ_DELIVERY_DATE,BARCODE,LOCATIONCODE,INVSTATUS,CARTONID," +
                "PACKBEFORESHIP,COT01 FROM COG_IFL_SA where cog_asn_number='" + asn + "' and cog_asn_document_name='" + docname + "'";
            var dspo= config.ExecuteQueryData(config.RPConnection, sqlpo, null);
            var dspkl = config.ExecuteQueryData(config.RPConnection, sqlpkl, null);
            var dssa = config.ExecuteQueryData(config.RPConnection, sqlsa, null);
            po.CreateCSVFile(dspo, pofilename);
            po.CreateCSVFile(dspkl, pklfilename);
            po.CreateCSVFile(dssa, safilename);
        }
        public void DeleteExistPO(string asn, string docname)
        {            
            po.DeleteProcessObject(asn, docname);
        }
       

    }
}
