using COGInterfaceCommand.Common.COG;
using System;
using System.IO;

namespace COGInterfaceCommand.Command
{
    public class HierarchyCommand : SftpCommand
    {
        string fileType = "Hierarchy";

        public HierarchyCommand(string Server, string Login, string Password, int Port, string backup, string inbound, string outbound, string localDownload, string localUpload) 
            : base(Server, Login, Password, Port, backup, inbound, outbound, localDownload, localUpload)
        {
        }

        public void GetHierarchyItems(string fileName)
        {
            try
            {
                int totalline = totallinesfile(fileName);
                FileStream fStream = new FileStream(fileName, FileMode.Open);
                StreamReader reader = new StreamReader(fStream);
                DateTime current = DateTime.Now;
                string file = new FileInfo(fileName).Name;
                int i = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    HierarchyItem item = new HierarchyItem(line, file, totalline)
                    {
                        CreatedDate = current,
                        COGFileName = new FileInfo(fileName).Name
                    };
                    item.ProcessObject();
                    i = i + 1;
                }

                reader.Close();
                fStream.Close();
            }
            catch { throw; }
        }

        public void GetHierarchyItems()
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
                        GetHierarchyItems(file.FullName); //file.FullName
                        //@"D:\Projects\ProjectVu\ACFC\COGInterfaceCommand\bin\Debug\BACKUP\Hierarchy\COG_HIERARCHY_ACFC_201910101638.TXT"
                        DirectoryInfo info = new DirectoryInfo(BackupFolder + "\\" + fileType);
                        if (!info.Exists)
                            info.Create();

                        file.MoveTo(BackupFolder + "\\" + fileType + "\\" + file.Name);
                    }
                    catch { throw; }
                }
                //Put acknowlegdment back for COG, than move local file to backup folder
                this.PutFileToSFTPServer(InFolder,0);
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