using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CB_TallyConnector.Log;

namespace CB_TallyConnector.Configuration
{
    public class ApplicationConfigration
    {
        Logger logger = new Logger();

        public string TallyIPAddress { get; private set; }
        public string TallyPort { get; private set; }
        public string SyncInterval { get; private set; }
        public string CBServerAddress { get; private set; }
        public string TallyProductName { get; private set; }
        public string timerValue { get; private set; }


        enum ErrorCodeForApplicationConfigurationEnum
        {

            FunctionGetConfigDetail

        }

        //Get Configuration 
        public void GetConfigDetail()
        {
            try
            {
                var TCConfigpath = Directory.GetCurrentDirectory() + "/" + "TCConfig.txt";


                if (File.Exists(TCConfigpath))
                {


                    var lastConfigLine = new List<string>();
                    var TCConfiguretextLastLine = File.ReadAllLines(TCConfigpath);

                    lastConfigLine.Add(TCConfiguretextLastLine[TCConfiguretextLastLine.Count() - 1]);

                    var TCLastConfiguretext = lastConfigLine[0].ToString().Trim();

                    var TCConfigureWordCount = TCLastConfiguretext.Split(' ');
                    var TCConfigureWordList = new List<string>();

                    foreach (var Word in TCConfigureWordCount)
                    {
                        TCConfigureWordList.Add(Word);
                    }

                    this.TallyIPAddress = TCConfigureWordList[0].ToString().Trim();
                    this.TallyPort = TCConfigureWordList[1].ToString().Trim();
                    
                    this.SyncInterval = TCConfigureWordList[3].ToString().Trim();
                    this.CBServerAddress = TCConfigureWordList[4].ToString().Trim();
                    this.TallyProductName = TCConfigureWordList[5].ToString().Trim();

                    if (this.SyncInterval == "10-Sec")
                    {
                        this.timerValue = 10.ToString();
                    }else
                    if (this.SyncInterval == "1-Mins")
                    {
                        this.timerValue = 60.ToString();
                    }else
                    if (this.SyncInterval == "2-Mins")
                    {
                        this.timerValue = 120.ToString();
                    }
                    else
                    if (this.SyncInterval == "5-Mins")
                    {
                        this.timerValue = 300.ToString();
                    }
                    else
                    if (this.SyncInterval == "10-Mins")
                    {
                        this.timerValue = 600.ToString();
                    }
                    else
                    if (this.SyncInterval == "15-Mins")
                    {
                        this.timerValue = 900.ToString();
                    }
                    else
                    if (this.SyncInterval == "2-Hour")
                    {
                        this.timerValue = 7200.ToString();
                    }
                    else
                    if (this.SyncInterval == "3-Hour")
                    {
                        this.timerValue = 10800.ToString();
                    }
                    else
                    if (this.SyncInterval == "1-Hour")
                    {
                        this.timerValue = 3600.ToString();
                    }
                    else
                    {
                        this.timerValue = 300.ToString();
                    }

                }
                else
                {
                    logger.CreateConfigFile();

                }
            }
            catch (Exception ex)
            {
                ErrorCodeForApplicationConfigurationEnum errorCodeFunc = ErrorCodeForApplicationConfigurationEnum.FunctionGetConfigDetail;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

        }
    }
}
