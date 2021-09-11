using System;
using System.IO;

namespace ACFC.Common
{
    public class CommonServices
    {
        public static string CP1258 = 
            "AìAÌAÒAÞAòÃ@ÃìÃÌÃÒÃÞÃòÂ@ÂìÂÌÂÒÂÞÂòEìEÌEÒEÞEòÊ@ÊìÊÌÊÒÊÞÊòIìIÌIÒIÞIòOìOÌOÒOÞOòÔ@ÔìÔÌÔÒÔÞÔòÕ@ÕìÕÌÕÒÕÞÕòUìUÌUÒUÞUòÝ@ÝìÝÌÝÒÝÞÝòYìYÌYÒYÞYòÐ@" + 
            "aìaÌaÒaÞaòã@ãìãÌãÒãÞãòâ@âìâÌâÒâÞâòeìeÌeÒeÞeòê@êìêÌêÒêÞêòiìiÌiÒiÞiòoìoÌoÒoÞoòô@ôìôÌôÒôÞôòõ@õìõÌõÒõÞõòuìuÌuÒuÞuòý@ýìýÌýÒýÞýòyìyÌyÒyÞyòð@";

        public static string uni = 
            "ÁÀ" + (char)7842 + "Ã" + (char)7840 + (char)258 + (char)7854 + (char)7856 + (char)7858 + (char)7860 + (char)7862 + "Â" + (char)7844 + (char)7846 + (char)7848 + (char)7850 + (char)7852 + "ÉÈ" + (char)7866 + (char)7868 + (char)7864 + "Ê" + (char)7870 + (char)7872 + (char)7874 + (char)7876 + (char)7878 + "ÍÌ" + (char)7880 + (char)296 + (char)7882 + "ÓÒ" + (char)7886 + "Õ" + (char)7884 + "Ô" + (char)7888 + (char)7890 + (char)7892 + (char)7894 + (char)7896 + (char)416 + (char)7898 + (char)7900 + (char)7902 + (char)7904 + (char)7906 + "ÚÙ" + (char)7910 + (char)360 + (char)7908 + (char)431 + (char)7912 + (char)7914 + (char)7916 + (char)7918 + (char)7920 + "Ý" + (char)7922 + (char)7926 + (char)7928 + (char)7924 + (char)272 + 
            "áà" + (char)7843 + "ã" + (char)7841 + (char)259 + (char)7855 + (char)7857 + (char)7859 + (char)7861 + (char)7863 + "â" + (char)7845 + (char)7847 + (char)7849 + (char)7851 + (char)7853 + "éè" + (char)7867 + (char)7869 + (char)7865 + "ê" + (char)7871 + (char)7873 + (char)7875 + (char)7877 + (char)7879 + "íì" + (char)7881 + (char)297 + (char)7883 + "óò" + (char)7887 + "õ" + (char)7885 + "ô" + (char)7889 + (char)7891 + (char)7893 + (char)7895 + (char)7897 + (char)417 + (char)7899 + (char)7901 + (char)7903 + (char)7905 + (char)7907 + "úù" + (char)7911 + (char)361 + (char)7909 + (char)432 + (char)7913 + (char)7915 + (char)7917 + (char)7919 + (char)7921 + "ý" + (char)7923 + (char)7927 + (char)7929 + (char)7925 + (char)273;


        public static void CopyFile(string srcPath, string desPath, bool isOverwrite)
        {
            FileInfo f = new FileInfo(srcPath);
            if (f.Exists)
            {
                f.CopyTo(desPath, isOverwrite);
            }
            else
            {
                throw new FileNotFoundException("Không tìm thấy file.", srcPath);
            }
        }

        public static void CopyFilesFromFolder(string srcPath, string desPath, string fileExtension = "", bool isOverwrite = true)
        {
            DirectoryInfo d = new DirectoryInfo(srcPath);
            if (d.Exists)
            {
                string filter = "*" + (string.IsNullOrEmpty(fileExtension) ? "" : fileExtension);
                foreach (FileInfo f in d.GetFiles(filter))
                {
                    string destinationFile = string.Format("{0}\\{1}", desPath, f.Name);
                    f.CopyTo(destinationFile, isOverwrite);
                }
            }
        }


        public static string UniTo1258(string srcString)
        {
            try
            {
                // Chuyển unicode dựng sẵn sang unicode tổ hợp
                srcString = UnicodeToCompound(srcString);

                // Chuyển từ unicode tổ hợp sang VNCP1258
                string temp = "";
                for (int i = 0; i < srcString.Length; i++)
                {
                    char c = srcString[i];
                    int index = uni.IndexOf(c);
                    if (index > -1)
                    {
                        temp = temp + CP1258.Substring(index * 2, 2);//Strings.Mid(CP1258, f * 2 - 1, 2);
                        if (temp[temp.Length - 1].ToString() == "@")
                            temp = temp.Substring(0, temp.Length - 1);
                    }
                    else
                        temp = temp + c;
                }

                return temp;
            }
            catch
            {
                throw;
            }
        }


        #region test


        public static string[] Unicode = new string[] {
             "\u1EBB" , //ẻ
             "\u00E9" , //é
             "\u00E8" , //è
             "\u1EB9" , //ẹ
             "\u1EBD" , //ẽ
             "\u1EC3" , //ể
             "\u1EBF" , //ế
             "\u1EC1" , //ề
             "\u1EC7" , //ệ
             "\u1EC5" , //ễ
             "\u1EF7" , //ỷ
             "\u00FD" , //ý
             "\u1EF3" , //ỳ
             "\u1EF5" , //ỵ
             "\u1EF9" , //ỹ
             "\u1EE7" , //ủ
             "\u00FA" , //ú
             "\u00F9" , //ù
             "\u1EE5" , //ụ
             "\u0169" , //ũ
             "\u1EED" , //ử
             "\u1EE9" , //ứ
             "\u1EEB" , //ừ
             "\u1EF1" , //ự
             "\u1EEF" , //ữ
             "\u1EC9" , //ỉ
             "\u00ED" , //í
             "\u00EC" , //ì
             "\u1ECB" , //ị
             "\u0129" , //ĩ
             "\u1ECF" , //ỏ
             "\u00F3" , //ó
             "\u00F2" , //ò
             "\u1ECD" , //ọ
             "\u00F5" , //õ
             "\u1EDF" , //ở
             "\u1EDB" , //ớ
             "\u1EDD" , //ờ
             "\u1EE3" , //ợ
             "\u1EE1" , //ỡ
             "\u1ED5" , //ổ
             "\u1ED1" , //ố
             "\u1ED3" , //ồ
             "\u1ED9" , //ộ
             "\u1ED7" , //ỗ
             "\u1EA3" , //ả
             "\u00E1" , //á
             "\u00E0" , //à
             "\u1EA1" , //ạ
             "\u00E3" , //ã
             "\u1EB3" , //ẳ
             "\u1EAF" , //ắ
             "\u1EB1" , //ằ
             "\u1EB7" , //ặ
             "\u1EB5" , //ẵ
             "\u1EA9" , //ẩ
             "\u1EA5" , //ấ
             "\u1EA7" , //ầ
             "\u1EAD" , //ậ
             "\u1EAB" , //ẫ
             "\u1EBA" , //Ẻ
             "\u00C9" , //É
             "\u00C8" , //È
             "\u1EB8" , //Ẹ
             "\u1EBC" , //Ẽ
             "\u1EC2" , //Ể
             "\u1EBE" , //Ế
             "\u1EC0" , //Ề
             "\u1EC6" , //Ệ
             "\u1EC4" , //Ễ
             "\u1EF6" , //Ỷ
             "\u00DD" , //Ý
             "\u1EF2" , //Ỳ
             "\u1EF4" , //Ỵ
             "\u1EF8" , //Ỹ
             "\u1EE6" , //Ủ
             "\u00DA" , //Ú
             "\u00D9" , //Ù
             "\u1EE4" , //Ụ
             "\u0168" , //Ũ
             "\u1EEC" , //Ử
             "\u1EE8" , //Ứ
             "\u1EEA" , //Ừ
             "\u1EF0" , //Ự
             "\u1EEE" , //Ữ
             "\u1EC8" , //Ỉ
             "\u00CD" , //Í
             "\u00CC" , //Ì
             "\u1ECA" , //Ị
             "\u0128" , //Ĩ
             "\u1ECE" , //Ỏ
             "\u00D3" , //Ó
             "\u00D2" , //Ò
             "\u1ECC" , //Ọ
             "\u00D5" , //Õ
             "\u1EDE" , //Ở
             "\u1EDA" , //Ớ
             "\u1EDC" , //Ờ
             "\u1EE2" , //Ợ
             "\u1EE0" , //Ỡ
             "\u1ED4" , //Ổ
             "\u1ED0" , //Ố
             "\u1ED2" , //Ồ
             "\u1ED8" , //Ộ
             "\u1ED6" , //Ỗ
             "\u1EA2" , //Ả
             "\u00C1" , //Á
             "\u00C0" , //À
             "\u1EA0" , //Ạ
             "\u00C3" , //Ã
             "\u1EB2" , //Ẳ
             "\u1EAE" , //Ắ
             "\u1EB0" , //Ằ
             "\u1EB6" , //Ặ
             "\u1EB4" , //Ẵ
             "\u1EA8" , //Ẩ
             "\u1EA4" , //Ấ
             "\u1EA6" , //Ầ
             "\u1EAC" , //Ậ
             "\u1EAA" , //Ẫ
        };

        public static string[] Compound = new string[] {
            //e
            "\u0065\u0309", //ẻ
            "\u0065\u0301", //é
            "\u0065\u0300", //è
            "\u0065\u0323", //ẹ
            "\u0065\u0303", //ẽ
            // ê
            "\u00EA\u0309", //ể
            "\u00EA\u0301", //ế
            "\u00EA\u0300", //ề
            "\u00EA\u0323", //ệ
            "\u00EA\u0303", //ễ
            // y
            "\u0079\u0309", //ỷ
            "\u0079\u0301", //ý
            "\u0079\u0300", //ỳ
            "\u0079\u0323", //ỵ
            "\u0079\u0303", //ỹ
            // u
            "\u0075\u0309", //ủ
            "\u0075\u0301", //ú
            "\u0075\u0300", //ù
            "\u0075\u0323", //ụ
            "\u0075\u0303", //ũ
            // ư
            "\u01B0\u0309", //ử
            "\u01B0\u0301", //ứ
            "\u01B0\u0300", //ừ
            "\u01B0\u0323", //ự
            "\u01B0\u0303", //ữ
            // i
            "\u0069\u0309", //ỉ
            "\u0069\u0301", //í
            "\u0069\u0300", //ì
            "\u0069\u0323", //ị
            "\u0069\u0303", //ĩ
            // o
            "\u006F\u0309", //ỏ
            "\u006F\u0301", //ó
            "\u006F\u0300", //ò
            "\u006F\u0323", //ọ
            "\u006F\u0303", //õ
            // ơ
            "\u01A1\u0309", //ở
            "\u01A1\u0301", //ớ
            "\u01A1\u0300", //ờ
            "\u01A1\u0323", //ợ
            "\u01A1\u0303", //ỡ
            // ô
            "\u00F4\u0309", //ổ
            "\u00F4\u0301", //ố
            "\u00F4\u0300", //ồ
            "\u00F4\u0323", //ộ
            "\u00F4\u0303", //ỗ
            // a
            "\u0061\u0309", //ả
            "\u0061\u0301", //á
            "\u0061\u0300", //à
            "\u0061\u0323", //ạ
            "\u0061\u0303", //ã
            // ă
            "\u0103\u0309", //ẳ
            "\u0103\u0301", //ắ
            "\u0103\u0300", //ằ
            "\u0103\u0323", //ặ
            "\u0103\u0303", //ẵ
            // â
            "\u00E2\u0309", //ẩ
            "\u00E2\u0301", //ấ
            "\u00E2\u0300", //ầ
            "\u00E2\u0323", //ậ
            "\u00E2\u0303", //ẫ
            // E
            "\u0045\u0309", //Ẻ
            "\u0045\u0301", //É
            "\u0045\u0300", //È
            "\u0045\u0323", //Ẹ
            "\u0045\u0303", //Ẽ
            // Ê
            "\u00CA\u0309", //Ể
            "\u00CA\u0301", //Ế
            "\u00CA\u0300", //Ề
            "\u00CA\u0323", //Ệ
            "\u00CA\u0303", //Ễ
            // Y
            "\u0059\u0309", //Ỷ
            "\u0059\u0301", //Ý
            "\u0059\u0300", //Ỳ
            "\u0059\u0323", //Ỵ
            "\u0059\u0303", //Ỹ
            // U
            "\u0055\u0309", //Ủ
            "\u0055\u0301", //Ú
            "\u0055\u0300", //Ù
            "\u0055\u0323", //Ụ
            "\u0055\u0303", //Ũ
            // Ư
            "\u01AF\u0309", //Ử
            "\u01AF\u0301", //Ứ
            "\u01AF\u0300", //Ừ
            "\u01AF\u0323", //Ự
            "\u01AF\u0303", //Ữ
            // I
            "\u0049\u0309", //Ỉ
            "\u0049\u0301", //Í
            "\u0049\u0300", //Ì
            "\u0049\u0323", //Ị
            "\u0049\u0303", //Ĩ
            // O
            "\u004F\u0309", //Ỏ
            "\u004F\u0301", //Ó
            "\u004F\u0300", //Ò
            "\u004F\u0323", //Ọ
            "\u004F\u0303", //Õ
            // Ơ
            "\u01A0\u0309", //Ở
            "\u01A0\u0301", //Ớ
            "\u01A0\u0300", //Ờ
            "\u01A0\u0323", //Ợ
            "\u01A0\u0303", //Ỡ
            // Ô
            "\u00D4\u0309", //Ổ
            "\u00D4\u0301", //Ố
            "\u00D4\u0300", //Ồ
            "\u00D4\u0323", //Ộ
            "\u00D4\u0303", //Ỗ
            // A
            "\u0041\u0309", //Ả
            "\u0041\u0301", //Á
            "\u0041\u0300", //À
            "\u0041\u0323", //Ạ
            "\u0041\u0303", //Ã
            // Ă
            "\u0102\u0309", //Ẳ
            "\u0102\u0301", //Ắ
            "\u0102\u0300", //Ằ
            "\u0102\u0323", //Ặ
            "\u0102\u0303", //Ẵ
            // Â
            "\u00C2\u0309", //Ẩ
            "\u00C2\u0301", //Ấ
            "\u00C2\u0300", //Ầ
            "\u00C2\u0323", //Ậ
            "\u00C2\u0303", //Ẫ
        };



        public static string UnicodeToCompound(string str)
        {
 
            for(int i = 0; i < Unicode.Length; i++)
            {
                str = str.Replace(Unicode[i], Compound[i]);
            }
        
            return str;
        }

        #endregion

    }
}