using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CB_TallyConnector.Log;

namespace CB_TallyConnector.EnDeCryption
{
    public class DeCryptFile
    {
        private string TCConfigpath;
        private string key = "ClearBal";
        private string FileName;
        private string CurrentPath;

        Logger logger = new Logger();

        private enum ErrorcodeforDeCryptFile
        {
            FunctionDecryptFile,
            FunctionctorDeCryptFile
        }

        public DeCryptFile()
        {
            try
            {
 
                this.TCConfigpath = Directory.GetCurrentDirectory();
                this.FileName = "TCConfig.txt";
                this.CurrentPath = this.TCConfigpath + "/" + this.FileName;

                if (File.Exists(this.CurrentPath))
                {
                    DecryptFile(this.CurrentPath, key);
                }
                
            }
            catch (Exception ex)
            {
                ErrorcodeforDeCryptFile errorCodeFunc = ErrorcodeforDeCryptFile.FunctionctorDeCryptFile;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 9, 9) + ", " + ex.Message);

            }

        }


     

        public void DecryptFile(string filepath, string key)
        {
            try
            {
                byte[] encrypted = File.ReadAllBytes(filepath);
                var DES = new DESCryptoServiceProvider();

                DES.IV = Encoding.UTF8.GetBytes(key);
                DES.Key = Encoding.UTF8.GetBytes(key);
                DES.Mode = CipherMode.CBC;
                DES.Padding = PaddingMode.PKCS7;

                var memStream = new MemoryStream();

                CryptoStream cryptoStream = new CryptoStream(memStream, DES.CreateDecryptor(), CryptoStreamMode.Write);
                cryptoStream.Write(encrypted, 0, encrypted.Length);
                cryptoStream.FlushFinalBlock();

                File.WriteAllBytes(filepath, memStream.ToArray());

            }
            catch (Exception ex)
            {
                ErrorcodeforDeCryptFile errorCodeFunc = ErrorcodeforDeCryptFile.FunctionDecryptFile;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 9, 9) + ", " + ex.Message);
            }
        }

    }
}
