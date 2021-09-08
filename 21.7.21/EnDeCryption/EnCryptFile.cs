using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CB_TallyConnector.Log;

namespace CB_TallyConnector.EnDeCryption
{
    class EnCryptFile
    {
        private string TCConfigpath;
        private string key = "ClearBal";
        private string FileName;
        private string CurrentPath;

        Logger logger = new Logger();

        private enum ErrorcodeforEnCryptFile
        {
            FunctionEncryptFile,
            FunctionctorEnCryptFile
        }

        public EnCryptFile()
        {
            try
            {
                this.TCConfigpath = Directory.GetCurrentDirectory();
                this.FileName = "TCConfig.txt";
                this.CurrentPath = this.TCConfigpath + "/" + this.FileName;


                if (File.Exists(this.CurrentPath))
                {
                    EncryptFile(this.CurrentPath, key);
                }

            }
            catch (System.Exception ex)
            {
                ErrorcodeforEnCryptFile errorCodeFunc = ErrorcodeforEnCryptFile.FunctionctorEnCryptFile;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 9, 9) + ", " + ex.Message);
            }


        }

        public void EncryptFile(string filepath, string key)
        {
            try
            {
                byte[] plainContent = File.ReadAllBytes(filepath);
                var DES = new DESCryptoServiceProvider();

                DES.IV = Encoding.UTF8.GetBytes(key);
                DES.Key = Encoding.UTF8.GetBytes(key);
                DES.Mode = CipherMode.CBC;
                DES.Padding = PaddingMode.PKCS7;

                var memStream = new MemoryStream();

                CryptoStream cryptoStream = new CryptoStream(memStream, DES.CreateEncryptor(), CryptoStreamMode.Write);
                cryptoStream.Write(plainContent, 0, plainContent.Length);
                cryptoStream.FlushFinalBlock();

                File.WriteAllBytes(filepath, memStream.ToArray());
            }
            catch (System.Exception ex)
            {
                ErrorcodeforEnCryptFile errorCodeFunc = ErrorcodeforEnCryptFile.FunctionEncryptFile;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 9, 9) + ", " + ex.Message);

            }
        }
    }
}
