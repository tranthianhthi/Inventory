using COGInterfaceCommand.Common;
using System;
using System.IO;
using System.Net;
using WinSCP;

namespace COGInterfaceCommand.Command
{
    public class SftpCommand : InterfaceCommand
    {
        public string FileName = "COG_{0}_ACFC_{1}.txt";
        public string SFTPServer { get; set; }
        public string SFTPLogin { get; set; }
        public string SFTPPassword { get; set; }
        public int SFTPPort { get; set; }

        public SftpCommand (string Server, string Login, string Password, int Port, string backup, string inbound, string outbound, string localDownload, string localUpload) : base(backup, inbound, outbound, localDownload, localUpload)
        {
            SFTPServer = Server;
            SFTPLogin = Login;
            SFTPPassword = Password;
            SFTPPort = Port;
        }

        private string GetFileFromFTPServer(string fileName, string destinationFile)
        {
            //used to display data into rich text.box
            string result = string.Empty;
            //initialize FtpWebRequest with your FTP Url
            //your FTP url should start with ftp://wwww.youftpsite.com//
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(SFTPServer + fileName);

            //set request method to download file. 
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            //set up credentials. 
            request.Credentials = new NetworkCredential(SFTPLogin, SFTPPassword);
            //initialize Ftp response.
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            //open readers to read data from ftp 
            Stream responsestream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responsestream);
            //read data from FTP
            result = reader.ReadToEnd();
            //save file locally on your pc
            using (StreamWriter file = File.CreateText(destinationFile))
            {
                file.WriteLine(result);
                file.Close();
            }
            //close readers. 
            reader.Close();
            response.Close();
            //return data from file. 
            return result;
        }

        public string GetFileFromSFTPServer(string remoteFolder)
        {
            SessionOptions sessionOptions = new SessionOptions()
            {
                Protocol = Protocol.Sftp,
                HostName = SFTPServer,
                UserName = SFTPLogin,
                Password = SFTPPassword,
                PortNumber = SFTPPort
            };

            using (Session session = new Session())
            {
                sessionOptions.SshHostKeyFingerprint = session.ScanFingerprint(sessionOptions, "SHA-256");
                try
                {
                    session.Open(sessionOptions);
                    session.GetFiles(remoteFolder, LocalDownloadFolder);
                    session.RemoveFiles(remoteFolder+"/*.*"); //remoteFolder
                    return "OK";                    
                }
                catch(Exception ex)
                {
                    return ex.ToString();
                }
            }
        }
        public string PutFileToSFTPServer(string remoteFolder)
        {
            SessionOptions sessionOptions = new SessionOptions()
            {
                Protocol = Protocol.Sftp,
                HostName = SFTPServer,
                UserName = SFTPLogin,
                Password = SFTPPassword,
                PortNumber = SFTPPort
            };
            using (Session session = new Session())
            {
                sessionOptions.SshHostKeyFingerprint = session.ScanFingerprint(sessionOptions, "SHA-256");
                try
                {
                    string result = "";
                    session.Open(sessionOptions);
                    TransferOperationResult transferResult;
                    transferResult = session.PutFiles(LocalUploadFolder + "\\*", remoteFolder, false);

                    transferResult.Check();

                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        result += string.Format("Upload of {0} succeeded." + Environment.NewLine, transfer.FileName);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }
        public void DeleteItemFromDocument(string tableName, string documentName)
        {
            Configurations configurations = new Configurations();
            string command = string.Format("DELETE {0} WHERE Document_Name = '{1}'", tableName, documentName);
            try
            {
                configurations.ExecuteRPCommand(configurations.RPConnection, command, null);
            }
            catch { throw; }
        }
        public int totallinesfile(string filepath)
        {
            using (StreamReader r = new StreamReader(filepath))
            {
                int i = 0;
                while (r.ReadLine() != null) { i++; }
                return i;
            }
        }
    }
}
