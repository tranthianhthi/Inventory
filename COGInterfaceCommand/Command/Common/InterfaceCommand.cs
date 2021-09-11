using System;

namespace COGInterfaceCommand.Command
{
    public class InterfaceCommand
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="backupFolder"></param>
        /// <param name="inFolder"></param>
        /// <param name="outFolder"></param>
        /// <param name="localDownloadFolder"></param>
        /// <param name="localUploadFolder"></param>
        public InterfaceCommand(string backupFolder, string inFolder, string outFolder, string localDownloadFolder, string localUploadFolder) 
        {
            BackupFolder = Environment.CurrentDirectory + "\\" + backupFolder;
            InFolder = inFolder;
            OutFolder = outFolder;
            LocalDownloadFolder = Environment.CurrentDirectory + "\\" + localDownloadFolder;
            LocalUploadFolder = Environment.CurrentDirectory + "\\" + localUploadFolder;
        }

        public string BackupFolder { get; set; }
        public string InFolder { get; set; }// = "Inbound";
        public string OutFolder { get; set; } // = "Outbound";
        public string LocalDownloadFolder { get; set; }
        public string LocalUploadFolder { get; set; }
    }
}
