using COGInterfaceCommand.Common.COG;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;


namespace COGInterfaceCommand.Command
{
    public class BarcodeCommand : SftpCommand
    {
        readonly string fileType = "Barcode";
        
        /*
         * •	Prefix :- COG_
         * •	File identification :- Barcode
         * •	Destination :- Name of COG business partner
         * •	Postfix :- Date format YYYYMMDDHH24MI
         * •	File Type :- txt
         * */

        public BarcodeCommand(string Server, string Login, string Password, int Port, string backup, string inbound, string outbound, string localDownload, string localUpload)
            : base(Server, Login, Password, Port, backup, inbound, outbound, localDownload, localUpload)
        {
        }

        public void GetBarcodes(string fileName)
        {
            try
            {
                int totalline = totallinesfile(fileName);
                
                FileStream fStream = new FileStream(fileName, FileMode.Open);
                StreamReader reader = new StreamReader(fStream);
                DateTime current = DateTime.Now;
                string file = new FileInfo(fileName).Name;               
                int i = 0;
                if (totalline == 0)
                {
                    using (StreamWriter writer = new StreamWriter(Environment.CurrentDirectory + "\\Uploads\\ACFC_Ack_" + file, false, Encoding.UTF8))
                        writer.Write(file + "|Y|File Empty|0");
                }
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();                   
                    BarcodeMaster barcode = new BarcodeMaster(line, file, totalline)
                    {
                        CreatedDate = current,
                        COGFileName = new FileInfo(fileName).Name
                    };
                    barcode.ProcessObject();
                    i = i+1;
                }
                reader.Close();
                fStream.Close();
            }
            catch { throw; }
        }

        public void GetBarcodes()
        {
            try
            {
                this.GetFileFromSFTPServer(OutFolder + "/" + fileType);             
            
                DirectoryInfo dir = new DirectoryInfo(LocalDownloadFolder);
                FileInfo[] files = dir.GetFiles("*" + fileType + "*.txt");
                foreach(FileInfo file in files)
                {
                    try
                    {
                        GetBarcodes(@"D:\Projects\ProjectVu\ACFC\COGInterfaceCommand\bin\Debug\Downloads\COG_BARCODE_ACFC_201910110926_1.TXT"); //file.FullName
                        //D:\\Projects\\ProjectVu\\ACFC\\COGInterfaceCommand\\bin\\Debug\\Downloads\\COG_BARCODE_ACFC_201910070925.TXT"
                        DirectoryInfo info = new DirectoryInfo(BackupFolder + "\\" + fileType);
                        if (!info.Exists)
                            info.Create();
                        file.MoveTo(BackupFolder + "\\" + fileType + "\\" + file.Name);                        
                    }
                    catch { throw; }
                }
                //Put acknowlegdment back for COG, than move local file to backup folder
                this.PutFileToSFTPServer(InFolder);               
                string[] filelist = Directory.GetFiles(LocalUploadFolder);
                if (filelist != null && filelist.Length > 0)
                {
                    foreach (string file in filelist)
                    {
                        File.Move(file, string.Concat(LocalUploadFolder + "\\Acknow", "\\" + Path.GetFileName(file)));
                    }
                }
            }
            catch { throw; }           
        }
       
    }
}