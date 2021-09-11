using COGInterfaceCommand.Common.COG;
using System;
using System.IO;
using System.Text;

namespace COGInterfaceCommand.Command
{
    public class ItemCommand : SftpCommand
    {
        string fileType = "Item";

        public ItemCommand(string Server, string Login, string Password, int Port, string backup, string inbound, string outbound, string localDownload, string localUpload) 
            : base(Server, Login, Password, Port, backup, inbound, outbound, localDownload, localUpload)
        {
        }

        public void GetItems(string fileName)
        {
            string backupFile = fileName;// Environment.CurrentDirectory + "\\" + fileName;
            try
            {
                int totalline = totallinesfile(fileName);
                FileStream fStream = new FileStream(backupFile, FileMode.Open);
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
                    ItemMaster item = new ItemMaster(line,file,totalline);                  
                    item.CreatedDate = current;
                    item.COGFileName = new FileInfo(fileName).Name;
                    item.ProcessObject();
                    i = i + 1;
                }
                reader.Close();
                fStream.Close();
            }
            catch { throw; }
        }

        public void GetItems()
        {
            string newFile = string.Format(FileName, fileType, DateTime.Now.ToString("yyyyMMddHHmm"));
            try
            {
                this.GetFileFromSFTPServer(OutFolder + "/" + fileType);

                DirectoryInfo dir = new DirectoryInfo(LocalDownloadFolder);
                FileInfo[] files = dir.GetFiles("*" + fileType + "*.txt");
                foreach (FileInfo file in files)
                {
                    try
                    {
                        GetItems(@"D:\Projects\ProjectVu\ACFC\COGInterfaceCommand\bin\Debug\Downloads\COG_ITEM_ACFC_201910110926_1.TXT"); //file.FullName
                        //D:\\Projects\\ProjectVu\\ACFC\\COGInterfaceCommand\\bin\\Debug\\Downloads\\COG_ITEM_ACFC_201910070925.TXT
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