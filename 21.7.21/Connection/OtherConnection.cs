using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CB_TallyConnector.Log;
using CB_TallyConnector.Connection;
using CB_TallyConnector.XMLRequestConnection;

namespace CB_TallyConnector.Connection
{
    

    public class OtherConnection
    {
        XMLRequest xML = new XMLRequest();
        APIConnection aPIConnection = new APIConnection();

        public bool InternetConnected { get; set; }
        public string TallyIsConnected { get; set; }
        public string MacAddress { get; set; }
        public string TallyLicenseNumber { get; set; }
        public bool boolTallyRunning { get; private set; }

        Logger logger = new Logger();

        enum ErrorCodeForOtherConnectionEnum
        {

            FunctionGetMACAddress,
            FunctionTallyConnection,
            FunctionInternetConnection,
            FunctionTallyLicenseInfo,
            FunctionAsyncTaskforTallyConnectionWorking,
            FunctionAsyncTaskforInternetConnectionWorking
        }

        public OtherConnection()
        {
            GetMACAddress();
            AsyncTallyConnectionWorking();
            AsyncInternetConnectionWorking();

           
        }

       
        private async void AsyncTallyConnectionWorking()
        {
            await AsyncTaskforTallyConnectionWorking(); 
        }
        private async void AsyncInternetConnectionWorking()
        {    
            await AsyncTaskforInternetConnectionWorking();
        }
        


        private async Task AsyncTaskforInternetConnectionWorking()
        {
            try
            {
                bool status;

                status = await Task.Run(() => IsConnectedToInternet());

                this.InternetConnected = status;
            }
            catch (Exception ex)
            {

                ErrorCodeForOtherConnectionEnum errorCodeFunc = ErrorCodeForOtherConnectionEnum.FunctionAsyncTaskforInternetConnectionWorking;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);

            }
           
        }
        private async Task AsyncTaskforTallyConnectionWorking()
        {
            try
            {
                string TallyRunningStatus;

                TallyRunningStatus = await Task.Run(() => IsTallyRunning());

                this.TallyIsConnected = TallyRunningStatus;

                if (TallyRunningStatus == "Tally.ERP 9 Server is Running")
                {
                    this.boolTallyRunning = true;
                }
                else
                {
                    this.boolTallyRunning = false;
                }
            }
            catch (Exception ex)
            {

                ErrorCodeForOtherConnectionEnum errorCodeFunc = ErrorCodeForOtherConnectionEnum.FunctionAsyncTaskforTallyConnectionWorking;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);

            }
           

        }
        

        public string TallyLicenseInfoResponseFromTally()
        {
            XmlDocument doc = new XmlDocument();

            try
            {

                doc.LoadXml(xML.TallyLicenseInfoXMLRequest());


                XmlNodeList xmlNodeList = doc.GetElementsByTagName("RESULT");
                var LicNumber = xmlNodeList[0].InnerText;

                return LicNumber;

            }
            catch (Exception ex)
            {
                ErrorCodeForOtherConnectionEnum errorCodeFunc = ErrorCodeForOtherConnectionEnum.FunctionTallyLicenseInfo;
                logger.Log(errorCodeFunc + " : " + ex.Message);

            }

            return "Nil";

        }


        public string IsTallyRunning()
        {
            string ltallyservicereps = "";
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(aPIConnection.TallyServerRequestAPI());
                XmlNodeList Response = xmlDocument.GetElementsByTagName("RESPONSE");

                ltallyservicereps = Response[0].InnerText;

            }
            catch (Exception ex)
            {
                ErrorCodeForOtherConnectionEnum errorCodeFunc = ErrorCodeForOtherConnectionEnum.FunctionTallyConnection;
                logger.Log(errorCodeFunc + " : " + ex.Message);

            }
            return ltallyservicereps;
        }


        public bool IsConnectedToInternet()
        {
            string host = "www.google.com";
            bool result = false;
            Ping p = new Ping();

            try
            {
                PingReply reply = p.Send(host, 3000);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch (Exception ex)
            {

                ErrorCodeForOtherConnectionEnum errorCodeFunc = ErrorCodeForOtherConnectionEnum.FunctionInternetConnection;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);

            }

            return result;

        }

        private void GetMACAddress()
        {
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                String sMacAddress = string.Empty;
                foreach (NetworkInterface adapter in nics)
                {
                    if (sMacAddress == String.Empty)// only return MAC Address from first card  
                    {
                        IPInterfaceProperties properties = adapter.GetIPProperties();
                        sMacAddress = adapter.GetPhysicalAddress().ToString();
                    }
                }
                this.MacAddress = sMacAddress;
            }
            catch (Exception ex)
            {
                ErrorCodeForOtherConnectionEnum errorCodeFunc = ErrorCodeForOtherConnectionEnum.FunctionGetMACAddress;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }
          
            
        }

    }
}
