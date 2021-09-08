using CB_TallyConnector.Configuration;
using CB_TallyConnector.Log;
using CB_TallyConnector.ResponseModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;


namespace CB_TallyConnector.Connection
{

    enum ErrorCodeForAPIConnectionEnum
    {
        FunctionTallyServerRequestAPI,
        FunctionClearBalanceWebServerRequestAPI,
        FunctionGetguidCBdatabasewithlastId,
        FunctionUpdateguidCBdatabasewithcurrentId,
        FunctionCheckGUIDClearBalanceWebServer,
        FunctionSignUpOnClearBalanceWebServerWithOTP,
        FunctionUpdateMACClearBalanceWebServer,
        FunctionSignUpOnClearBalanceWebServerwithGuidvalidation,
        FunctionSignUpOnClearBalanceWebServer,
        FunctionClearBlanceSendReqst,
        FunctionClearBlanceSendReqstLedger,
        FunctionTallySendReqst,
        FunctionJSONstringResponseforWebServerAgainstPostedJSONStringForLedger,
        FunctionJSONstringResponseforWebServerAgainstPostedJSONStringForLedgerClosingBalance,
        FunctionJSONstringResponseforWebServerAgainstPostedJSONStringForVoucher,
        FunctionJSONstringReponseforStatictisReport,
        FunctionJSONstringReponseforBalanceSheetReport,
        FunctionClearBlanceSendReqstVoucher,
        FunctionUpdateEmailOnClearBalanceWebServer,
        FunctionSignUpOnTrackpayoutWebServer,
        FunctiongetlastDetailTrackPayoutWebServer,
        FunctionCustomer_Track_payout_POST_SendReqst,
        FunctionInvoice_Track_payout_POST_SendReqst,
        FunctionTPO_JSONstringResponseforWebServerAgainstPostedJSONString


    }


    public class APIConnection
    {
        ApplicationConfigration appConfiguration = new ApplicationConfigration();

        Logger logger = new Logger();

        public string IPAddress { get; set; }

        public int salesvoucherCount { get; protected set; }
        public int receiptvouchercount { get; protected set; }
        public int creditnoteCount { get; protected set; }
        public int paymentvoucherCount { get; protected set; }
        public int purchasevoucherCount { get; protected set; }
        public int debitnoteCount { get; protected set; }
        public int newledgerCount { get; protected set; }
        public int updatedledgerCount { get; protected set; }
        public string customerPosttag { get; private set; }
        public string invoiceAndReceiptPostTag { get; private set; }
        public string invoicePostTag { get; private set; }
        public string receiptPostTag { get; private set; }
        public string JsonStringMsg { get; private set; }

        public APIConnection()
        {
            this.IPAddress = "https://preprod.clearbalance.co.in";

        }

        public string TallyServerRequestAPI()
        {
            String lResponsestr = "";
            String LResult = "";
            String lTallyIPAddress = "";
            String lTallyPort = "";
            try
            {
                appConfiguration.GetConfigDetail();

                lTallyIPAddress = appConfiguration.TallyIPAddress;
                lTallyPort = appConfiguration.TallyPort;

                String lTallyLocalHost = "http://" + lTallyIPAddress + ":" + lTallyPort;

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lTallyLocalHost);
                httpWebRequest.Timeout = 600000;
                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();

                LResult = lResponsestr;

            }
            catch (Exception ex)
            {

                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionTallyServerRequestAPI;
                logger.Log(errorCodeFunc + " : " + ex.Message);
                //logger.Log("Error Code 103 : Tally application is not responding." + "\n" + "Restart the Tally Application and Tally Connector again.");

            }

            return LResult;
        }

        public string ClearBalanceWebServerRequestAPI()
        {
            String lResponsestr = "";
            String LResult = "";
            String lCompanyinitial = "";
            try
            {
                appConfiguration.GetConfigDetail();


                lCompanyinitial = "";
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = lCBIPAddress + "/" + lCompanyinitial + "/post-data.php";
                //String lClearBalanceHost = lCBIPAddress + "/" + "testapril" + "/Testvch.php";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Timeout = 600000;

                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();

                LResult = lResponsestr;
            }
            catch (Exception ex)
            {
                //  logger.Log("Erro Code 101 : Clear Balance Web Server is not responding. Please try after few minutes");
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionClearBalanceWebServerRequestAPI;
                logger.Log(errorCodeFunc + " : " + ex.Message);

            }

            return LResult;

        }

        public string GetguidCBdatabasewithlastId(string GUID, string Key)
        {
            String prqtstr = "{" + '"' + "guid" + '"' + ":" + '"' + GUID + '"' + "," +
                                       '"' + "key" + '"' + ":" + '"' + Key + '"' +
                                       "}";

            String lResponsestr = "";
            String LResult = "";

            try
            {
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = lCBIPAddress + "/admin/master-alter-id.php";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)prqtstr.Length;
                httpWebRequest.ContentType = "application/text";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(prqtstr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionGetguidCBdatabasewithlastId;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return LResult;
        }


        public string UpdateguidCBdatabasewithcurrentId(string GUID, string ALTVCHID, string ALTMSTID, string Key)
        {
            String prqtstr = "{" + '"' + "guid" + '"' + ":" + '"' + GUID + '"' + "," +
                                       '"' + "AltMstID" + '"' + ":" + '"' + ALTMSTID + '"' + "," +
                                       '"' + "AltVchID" + '"' + ":" + '"' + ALTVCHID + '"' + "," +
                                       '"' + "key" + '"' + ":" + '"' + Key + '"' +
                                       "}";

            String lResponsestr = "";
            String LResult = "";

            try
            {
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = lCBIPAddress + "/admin/master-alter-id.php";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)prqtstr.Length;
                httpWebRequest.ContentType = "application/text";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(prqtstr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionUpdateguidCBdatabasewithcurrentId;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return LResult;
        }

        public string CheckGUIDClearBalanceWebServer(string GUID, string Key)
        {
            String prqtstr = "{" + '"' + "guid" + '"' + ":" + '"' + GUID + '"' + "," +
                                         '"' + "key" + '"' + ":" + '"' + Key + '"' +
                                            "}";

            String lResponsestr = "";
            String LResult = "";

            try
            {
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = lCBIPAddress + "/admin/validateGUID.php";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)prqtstr.Length;
                httpWebRequest.ContentType = "application/text";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(prqtstr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionCheckGUIDClearBalanceWebServer;
                logger.Log(errorCodeFunc + " : " + ex.Message);

            }

            return LResult;


        }

        public string SignUpOnClearBalanceWebServerWithOTP(string pWebReqstStr, string otpstr)
        {

            String lResponsestr = "";
            String LResult = "";



            try
            {
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = lCBIPAddress + "/admin/website-signup.php?otp=" + otpstr;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)pWebReqstStr.Length;
                httpWebRequest.ContentType = "application/text";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(pWebReqstStr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();

                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionSignUpOnClearBalanceWebServerWithOTP;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }
            return LResult;

        }

        public string UpdateMACClearBalanceWebServer(string macaddress, string companyinitials, string GUID)
        {
            String prqtstr = "{" + '"' + "mac_address" + '"' + ":" + '"' + macaddress + '"' + "," +
                                         '"' + "subdomain_name" + '"' + ":" + '"' + companyinitials + '"' + "," +
                                         '"' + "guid" + '"' + ":" + '"' + GUID + '"' +
                                            "}";

            String lResponsestr = "";
            String LResult = "";

            try
            {
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = lCBIPAddress + "/admin/validateMacaddress.php";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)prqtstr.Length;
                httpWebRequest.ContentType = "application/text";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(prqtstr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionUpdateMACClearBalanceWebServer;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return LResult;


        }

        public string SignUpOnClearBalanceWebServerwithGuidvalidation(string pWebReqstStr)
        {

            String lResponsestr = "";
            String LResult = "";



            try
            {
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = lCBIPAddress + "/admin/validateGUID.php";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)pWebReqstStr.Length;
                httpWebRequest.ContentType = "application/text";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(pWebReqstStr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionSignUpOnClearBalanceWebServerwithGuidvalidation;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return LResult;


        }

        public string SignUpOnClearBalanceWebServer(string pWebReqstStr)
        {

            String lResponsestr = "";
            String LResult = "";

            try
            {
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = lCBIPAddress + "/admin/checkCustomerSignup.php";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)pWebReqstStr.Length;
                httpWebRequest.ContentType = "application/text";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(pWebReqstStr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionSignUpOnClearBalanceWebServer;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return LResult;


        }

        public string UpdateEmailOnClearBalanceWebServer(string pWebReqstStr)
        {

            String lResponsestr = "";
            String LResult = "";

            try
            {
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = lCBIPAddress + "/admin/update-master-email.php";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)pWebReqstStr.Length;
                httpWebRequest.ContentType = "application/text";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(pWebReqstStr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionUpdateEmailOnClearBalanceWebServer;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return LResult;


        }


        public string ClearBlanceSendReqst(string pWebReqstStr, string COMPANYINITIALS)
        {

            String lResponsestr = "";
            String LResult = "";
            String lCompanyinitial = "";


            try
            {


                lCompanyinitial = COMPANYINITIALS;
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = lCBIPAddress + "/" + lCompanyinitial + "/post-data.php";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)pWebReqstStr.Length;
                httpWebRequest.ContentType = "application/xml";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(pWebReqstStr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionClearBlanceSendReqst;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return LResult;
        }

        public string ClearBlanceSendReqstLedger(string pWebReqstStr, string COMPANYINITIALS)
        {

            String lResponsestr = "";
            String LResult = "";
            String lCompanyinitial = "";

            try
            {

                lCompanyinitial = COMPANYINITIALS;
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;
                String lClearBalanceHost = lCBIPAddress + "/" + lCompanyinitial + "/Customer-post-data.php";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)pWebReqstStr.Length;
                httpWebRequest.ContentType = "application/text";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(pWebReqstStr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionClearBlanceSendReqstLedger;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return LResult;
        }

        public string ClearBlanceSendReqstVoucher(string pWebReqstStr, string COMPANYINITIALS)
        {

            String lResponsestr = "";
            String LResult = "";
            String lCompanyinitial = "";


            try
            {


                lCompanyinitial = COMPANYINITIALS;
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = lCBIPAddress + "/" + lCompanyinitial + "/vouchers-post-data-new.php";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)pWebReqstStr.Length;
                httpWebRequest.ContentType = "application/text";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(pWebReqstStr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionClearBlanceSendReqstVoucher;
                logger.Log(errorCodeFunc + " : " + ex.Message);

            }

            return LResult;
        }

        public string TallySendReqst(string pWebReqstStr)
        {

            String lResponsestr = "";
            String LResult = "";
            String lTallyIPAddress = "";
            String lTallyPort = "";
            try
            {
                appConfiguration.GetConfigDetail();

                lTallyIPAddress = appConfiguration.TallyIPAddress;
                lTallyPort = appConfiguration.TallyPort;

                String lTallyLocalHost = "http://" + lTallyIPAddress + ":" + lTallyPort;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lTallyLocalHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)pWebReqstStr.Length;
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(pWebReqstStr);
                lstreamWriter.Close();

                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();



                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionTallySendReqst;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return LResult;
        }

        public string JSONstringResponseforWebServerAgainstPostedJSONStringForLedger(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {

            string JSONdoc = "";
            string lCBResponse = "";

            try
            {
                XMLResponseModel xMLResponseModel = new XMLResponseModel();

                DataTable d1 = xMLResponseModel.TallyCustomerMasterXMLResponse(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, COMPANYINITIALS);
                this.newledgerCount = d1.Rows.Count;

                JObject doc =
                    new JObject(
                        new JProperty("customers",
                            from DataRow customer in d1.Rows
                            select new JObject(
                                    new JProperty("guid", customer["GUID"]),
                                    new JProperty("customer_name", customer["CUSTOMERNAME"]),
                                    new JProperty("parent_type", customer["PARENTTYPE"]),
                                    new JProperty("address", customer["ADDRESS"]),
                                    new JProperty("state", customer["STATE"]),
                                    new JProperty("pin_code", customer["PINCODE"]),
                                    new JProperty("contact_person", customer["CONTACT_PERSON"]),
                                    new JProperty("phone_no", customer["PHONE_NO"]),
                                    new JProperty("mobile_no", customer["MOBILE_NO"]),
                                    new JProperty("email", customer["EMAIL"]),
                                    new JProperty("cc_to", customer["CC_TO"]),
                                    new JProperty("default_credit_period", customer["DEFAULT_CREDIT_PERIOD"]),
                                    new JProperty("opening_balance", customer["OPENING_BALANCE"]),
                                    new JProperty("closing_balance", customer["CLOSING_BALANCE"])


                                           )));

                JSONdoc = doc.ToString();
                lCBResponse = ClearBlanceSendReqstLedger(JSONdoc, COMPANYINITIALS);
            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionJSONstringResponseforWebServerAgainstPostedJSONStringForLedger;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lCBResponse;

        }

        public string JSONstringResponseforWebServerAgainstPostedJSONStringForLedgerClosingBalance(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {

            string JSONdoc = "";
            string lCBResponse = "";

            try
            {
                XMLResponseModel xMLResponseModel = new XMLResponseModel();

                DataTable d1 = xMLResponseModel.TallyVoucherLedgerClosingBalance(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);

                this.updatedledgerCount = d1.Rows.Count;

                JObject doc =
                    new JObject(
                        new JProperty("customers",
                            from DataRow customer in d1.Rows
                            select new JObject(
                                    new JProperty("guid", customer["GUID"]),
                                    new JProperty("customer_name", customer["NAME"]),
                                    new JProperty("parent_type", customer["PARENTTYPE"]),
                                    new JProperty("closing_balance", customer["CLOSINGBALANCE"]),
                                    new JProperty("action", "update")

                                           )));

                JSONdoc = doc.ToString();
                lCBResponse = ClearBlanceSendReqstLedger(JSONdoc, COMPANYINITIALS);
            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionJSONstringResponseforWebServerAgainstPostedJSONStringForLedgerClosingBalance;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lCBResponse;

        }

        public string JSONstringResponseforWebServerAgainstPostedJSONStringForVoucher(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {

            string JSONdoc = "";
            string lCBResponse = "";
            string JSONdoc1 = "";

            try
            {
                XMLResponseModel xMLResponseModel = new XMLResponseModel();
                DataTable d10 = new DataTable();
                DataTable d11 = new DataTable();

                DataTable d2 = xMLResponseModel.SalesVoucherXMPResponseFromTally(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);
                DataTable d3 = xMLResponseModel.ReceiptVoucherXMPResponseFromTally(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);
                DataTable d4 = xMLResponseModel.CreditNoteVoucherXMPResponseFromTally(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);
                DataTable d5 = xMLResponseModel.JournalInvoiceVoucherXMLResponseFromTally(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);
                DataTable d6 = xMLResponseModel.JournalReceiptVoucherXMLResponseFromTally(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);
                DataTable d7 = xMLResponseModel.PaymentVoucherXMPResponseFromTally(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);
                DataTable d8 = xMLResponseModel.PurchaesVoucherXMPResponseFromTally(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);
                DataTable d9 = xMLResponseModel.DebitNoteVoucherXMPResponseFromTally(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);


                this.salesvoucherCount = d2.Rows.Count;
                this.receiptvouchercount = d3.Rows.Count;
                this.creditnoteCount = d4.Rows.Count;
                this.paymentvoucherCount = d7.Rows.Count;
                this.purchasevoucherCount = d8.Rows.Count;
                this.debitnoteCount = d9.Rows.Count;


                d10.Merge(d3);
                d10.Merge(d4);
                d10.Merge(d5);
                d10.Merge(d6);
                d10.Merge(d7);
                d10.Merge(d8);
                d10.Merge(d9);

                d11.Merge(d2);

                ArrayList invoiceUniqueRecords = new ArrayList();
                ArrayList invoiceDuplicateRecords = new ArrayList();

                ArrayList settlementUniqueRecords = new ArrayList();
                ArrayList settlementDuplicateRecords = new ArrayList();

                foreach (DataRow dRow in d10.Rows)
                {
                    if (invoiceUniqueRecords.Contains(dRow["GUID"]))
                        invoiceDuplicateRecords.Add(dRow);
                    else
                        invoiceUniqueRecords.Add(dRow["GUID"]);
                }

                foreach (DataRow dRow in d11.Rows)
                {
                    if (settlementUniqueRecords.Contains(dRow["GUID"]))
                        settlementDuplicateRecords.Add(dRow);
                    else
                        settlementUniqueRecords.Add(dRow["GUID"]);
                }

                // Remove duplicate rows from DataTable added to DuplicateRecords
                foreach (DataRow dRow in invoiceDuplicateRecords)
                {
                    d10.Rows.Remove(dRow);
                }

                // Remove duplicate rows from DataTable added to DuplicateRecords
                foreach (DataRow dRow in settlementDuplicateRecords)
                {
                    d11.Rows.Remove(dRow);
                }


                d3.Merge(d4);
                d3.Merge(d6);
                d3.Merge(d8);


                d3.Merge(d5);
                d3.Merge(d7);
                d3.Merge(d9);



                /*JObject doc =
                    new JObject(
                        new JProperty("invoice",
                                        from DataRow invoice in d2.Rows
                                        select new JObject(
                                                    new JProperty("guid", invoice["GUID"]),
                                                    new JProperty("voucher_type", invoice["TALLYVOUCHERTYPENAME"]),
                                                    new JProperty("vouchertype_alias", invoice["VOUCHERTYPENAME"]),
                                                    new JProperty("parent_type", invoice["PARENTTYPE"]),
                                                    new JProperty("customer", invoice["PARTYNAME"]),
                                                    new JProperty("invoice_date", invoice["DATE"]),
                                                    new JProperty("invoice_no", invoice["VOUCHERNUMBER"]),
                                                    new JProperty("invoice_amount", invoice["AMOUNT"]),
                                                    new JProperty("narration", invoice["NARRATION"])
                                                    )),
                                    new JProperty("settlement",
                                        from DataRow invoice in d3.Rows
                                        select new JObject(
                                                    new JProperty("guid", invoice["GUID"]),
                                                    new JProperty("voucher_type", invoice["TALLYVOUCHERTYPENAME"]),
                                                    new JProperty("vouchertype_alias", invoice["VOUCHERTYPENAME"]),
                                                    new JProperty("parent_type", invoice["PARENTTYPE"]),
                                                    new JProperty("customer", invoice["PARTYNAME"]),
                                                    new JProperty("voucher_date", invoice["DATE"]),
                                                    new JProperty("voucher_no", invoice["VOUCHERNUMBER"]),
                                                    new JProperty("voucher_amount", invoice["AMOUNT"]),
                                                    new JProperty("narration", invoice["NARRATION"])

                                                    ))

                                           );*/

                JObject doc =
                    new JObject(
                         new JProperty("invoice",
                                from DataRow guidtable in d11.Rows
                                select new JObject(
                                                new JProperty("guid", guidtable["GUID"]),
                                                new JProperty("voucher_type", guidtable["TALLYVOUCHERTYPENAME"]),
                                                new JProperty("voucher_ledger",
                                                      from DataRow invoice in d2.Rows
                                                      where invoice["guid"].ToString().Equals(guidtable["GUID"])
                                                      select new JObject(
                                                                   new JProperty("voucher_type", invoice["TALLYVOUCHERTYPENAME"]),
                                                                    new JProperty("vouchertype_alias", invoice["VOUCHERTYPENAME"]),
                                                                    new JProperty("parent_type", invoice["PARENTTYPE"]),
                                                                    new JProperty("customer", invoice["PARTYNAME"]),
                                                                    new JProperty("invoice_date", invoice["DATE"]),
                                                                    new JProperty("invoice_no", invoice["VOUCHERNUMBER"]),
                                                                    new JProperty("invoice_amount", invoice["AMOUNT"]),
                                                                    new JProperty("narration", invoice["NARRATION"])
                                                                )))),

                                new JProperty("settlement",
                                            from DataRow guidtable2 in d10.Rows
                                            select new JObject(
                                                        new JProperty("guid", guidtable2["GUID"]),
                                                        new JProperty("voucher_type", guidtable2["TALLYVOUCHERTYPENAME"]),
                                                        new JProperty("voucher_ledger",
                                                                  from DataRow invoice in d3.Rows
                                                                  where invoice["guid"].ToString().Equals(guidtable2["GUID"])
                                                                  select new JObject(
                                                                               new JProperty("voucher_type", invoice["TALLYVOUCHERTYPENAME"]),
                                                                                new JProperty("vouchertype_alias", invoice["VOUCHERTYPENAME"]),
                                                                                new JProperty("parent_type", invoice["PARENTTYPE"]),
                                                                                new JProperty("customer", invoice["PARTYNAME"]),
                                                                                new JProperty("voucher_date", invoice["DATE"]),
                                                                                new JProperty("voucher_no", invoice["VOUCHERNUMBER"]),
                                                                                new JProperty("voucher_amount", invoice["AMOUNT"]),
                                                                                new JProperty("narration", invoice["NARRATION"])
                                                                            ))))

                                                       );

                JSONdoc = doc.ToString();
                lCBResponse = ClearBlanceSendReqstVoucher(JSONdoc, COMPANYINITIALS);
            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionJSONstringResponseforWebServerAgainstPostedJSONStringForVoucher;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lCBResponse;

        }



        public string JSONstringReponseforStatictisReport(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string COMPANYINITIALS)
        {

            string JSONdoc = "";
            string lCBResponse = "";

            try
            {
                XMLResponseModel xMLResponseModel = new XMLResponseModel();

                DataTable d1 = xMLResponseModel.TallyStatictisReportXMLResponse(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE);

                JObject doc =
                    new JObject(
                        new JProperty("Statictis",
                            from DataRow statictis in d1.Rows
                            select new JObject(
                                    new JProperty("from_date", BOOKBEGINNINGFROM),
                                    new JProperty("to_date", LASTVOUCHERENTRYDATE),
                                    new JProperty("voucher_type", statictis["TYPE"]),
                                    new JProperty("actual_voucher", statictis["TOTALVOUCHER"]),
                                    new JProperty("cancelled_voucher", statictis["TOTALCANCELLEDVOUCHER"])

                                           )));

                JSONdoc = doc.ToString();
                //lCBResponse = ClearBlanceSendReqst(JSONdoc, COMPANYINITIALS);
            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionJSONstringReponseforStatictisReport;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return JSONdoc;

        }

        public string JSONstringReponseforBalanceSheetReport(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string COMPANYINITIALS)
        {

            string JSONdoc = "";
            string lCBResponse = "";

            try
            {
                XMLResponseModel xMLResponseModel = new XMLResponseModel();

                DataTable d1 = xMLResponseModel.TallyBalanceSheetReportXMLResponse(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE);

                JObject doc =
                    new JObject(
                        new JProperty("Balancesheet",
                            from DataRow balancesheet in d1.Rows
                            select new JObject(
                                    new JProperty("from_date", BOOKBEGINNINGFROM),
                                    new JProperty("to_date", LASTVOUCHERENTRYDATE),
                                    new JProperty("account_name", balancesheet["ACCOUNTNAME"]),
                                    new JProperty("sub_amount", balancesheet["SUBAMOUNT"]),
                                    new JProperty("main_amount", balancesheet["MAINAMOUNT"])

                                           )));

                JSONdoc = doc.ToString();
                //lCBResponse = ClearBlanceSendReqst(JSONdoc, COMPANYINITIALS);
            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionJSONstringReponseforBalanceSheetReport;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return JSONdoc;

        }



        /// <summary>
        /// Track Pay out Sign_Up API 
        /// </summary>
        /// <returns></returns>
        public string SignUpOnTrackpayoutWebServer(string pWebReqstStr)
        {

            String lResponsestr = "";
            String LResult = "";

            try
            {
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = "http://localhost:8001/api/tally/utility/company/register_company";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)pWebReqstStr.Length;
                httpWebRequest.ContentType = "application/json";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(pWebReqstStr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionSignUpOnTrackpayoutWebServer;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return LResult;


        }

        public string SetLastDetailOnTrackpayoutWebServer(string pWebReqstStr)
        {

            String lResponsestr = "";
            String LResult = "";

            try
            {
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = "http://localhost:8001/api/tally/utility/company/set_last_details";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)pWebReqstStr.Length;
                httpWebRequest.ContentType = "application/json";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(pWebReqstStr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionSignUpOnTrackpayoutWebServer;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return LResult;


        }

        public string CheckGUIDTrackPayoutWebServer(string GUID)
        {


            String lResponsestr = "";
            String LResult = "";

            try
            {
                //GET / api / tally / utility / company / get_companyid_by_guid ? company_gu_id = 10001 - GUID - 2021 HTTP / 1.1
                //Host: trackpayout.contisofttechno.com:8001
                //Content - Type: application / json
                //Cache - Control: no - cache
                //Postman - Token: ebab1991 - 8e37 - 1e47 - 5330 - 667b60ce9a17

                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = "http://localhost:8001/api/tally/utility/company/get_companyid_by_guid?company_gu_id=" + GUID;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 600000;
                //httpWebRequest.ContentLength = (long)prqtstr.Length;
                //httpWebRequest.ContentType = "application/json";
                //StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                //lstreamWriter.Write(prqtstr);
                //lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionCheckGUIDClearBalanceWebServer;
                logger.Log(errorCodeFunc + " : " + ex.Message);

            }

            return LResult;


        }


        public string getlastDetailTrackPayoutWebServer(string companyid)
        {


            String lResponsestr = "";
            String LResult = "";

            try
            {
                //GET / api / tally / utility / company / get_companyid_by_guid ? company_gu_id = 10001 - GUID - 2021 HTTP / 1.1
                //Host: trackpayout.contisofttechno.com:8001
                //Content - Type: application / json
                //Cache - Control: no - cache
                //Postman - Token: ebab1991 - 8e37 - 1e47 - 5330 - 667b60ce9a17

                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                string lClearBalanceHost = "http://localhost:8001/api/tally/utility/company/get_last_details?company_id=" + companyid;

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 600000;
                //httpWebRequest.ContentLength = (long)prqtstr.Length;
                //httpWebRequest.ContentType = "application/json";
                //StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                //lstreamWriter.Write(prqtstr);
                //lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctiongetlastDetailTrackPayoutWebServer;
                logger.Log(errorCodeFunc + " : " + ex.Message);

            }

            return LResult;


        }

        public string checkcompanyValidityTrackPayoutWebServer(string companyid)
        {


            String lResponsestr = "";
            String LResult = "";

            try
            {
                //GET / api / tally / utility / company / get_companyid_by_guid ? company_gu_id = 10001 - GUID - 2021 HTTP / 1.1
                //Host: trackpayout.contisofttechno.com:8001
                //Content - Type: application / json
                //Cache - Control: no - cache
                //Postman - Token: ebab1991 - 8e37 - 1e47 - 5330 - 667b60ce9a17

                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                string lClearBalanceHost = "http://localhost:8001/api/tally/utility/company/is_valid?company_id=" + companyid;

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 600000;
                //httpWebRequest.ContentLength = (long)prqtstr.Length;
                //httpWebRequest.ContentType = "application/json";
                //StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                //lstreamWriter.Write(prqtstr);
                //lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctiongetlastDetailTrackPayoutWebServer;
                logger.Log(errorCodeFunc + " : " + ex.Message);

            }

            return LResult;


        }

        //

        public string setlastDetailTrackPayoutWebServer(string companyid, string last_voucher_alter_id, string last_master_alter_id)
        {


            string currentdateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            String prqtstr = "{" + '"' + "company_id" + '"' + ":" + '"' + companyid + '"' + "," +
                                        '"' + "last_master_alter_id" + '"' + ":" + '"' + last_master_alter_id + '"' + "," +
                                        '"' + "last_voucher_alter_id" + '"' + ":" + '"' + last_voucher_alter_id + '"' + "," +
                                         '"' + "last_sync_date" + '"' + ":" + '"' + currentdateTime + '"' +
                                            "}";

            String lResponsestr = "";
            String LResult = "";

            try
            {
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = "http://localhost:8001/api/tally/utility/company/set_last_details";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)prqtstr.Length;
                httpWebRequest.ContentType = "application/json";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(prqtstr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionCheckGUIDClearBalanceWebServer;
                logger.Log(errorCodeFunc + " : " + ex.Message);

            }

            return LResult;


        }


        public bool TPO_JSONstringResponseforWebServerAgainstPostedJSONString(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {
            string JSONdoc = "";
            bool lCBResponse = false;
            bool invoiceResponse = false;
            bool receiptResponse = false;
            string JSONcustomerdoc = "";

            string response_from_invoice_api;
            string response_from_customer_api;
            string customerPostFlag;

            string iPostFlag = "", rPostFlag = "";
            string JSONreceiptdoc;

            try
            {
                XMLResponseModel xMLResponseModel = new XMLResponseModel();

                // DataTable d1 = AllLedgerXMPResponseFromTally();
                DataTable customerDataTable = xMLResponseModel.TOP_TallyCustomerMasterXMLResponse(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, COMPANYINITIALS);
                DataTable voucherDataTable = xMLResponseModel.TPO_SalesVoucherXMPResponseFromTally(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);
                DataTable receiptvoucherDataTable = xMLResponseModel.TPO_ReceiptVoucherXMPResponseFromTally(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);

                this.newledgerCount = customerDataTable.Rows.Count;
                this.salesvoucherCount = voucherDataTable.Rows.Count;
                this.receiptvouchercount = receiptvoucherDataTable.Rows.Count;


                JArray customerdoc =
                       new JArray(
                         from DataRow customer in customerDataTable.Rows
                         select new JObject(
                                new JProperty("company_id", COMPANYINITIALS),
                                 new JProperty("guid", customer["GUID"]),
                                 new JProperty("customer_name", customer["CUSTOMERNAME"]),
                                 new JProperty("customer_email", customer["EMAIL"]),
                                 new JProperty("customer_contact", customer["MOBILE_NO"]),
                                 new JProperty("customer_country", customer["COUNTRY"]),
                                 new JProperty("customer_state", customer["STATE"]),
                                 new JProperty("customer_city", "NA"),
                                 new JProperty("payment_term", customer["DEFAULT_CREDIT_PERIOD"]),
                                 new JProperty("gst_no", customer["PARTYGSTIN"]),
                                 new JProperty("contact_person_name", customer["CONTACT_PERSON"]),
                                 new JProperty("contact_person_email", "NA")
                                 ));



                JArray voucherdoc =
                     new JArray(
                           from DataRow vouchers in voucherDataTable.Rows
                           select new JObject(
                                   new JProperty("company_id", COMPANYINITIALS),
                                   new JProperty("invoice_gu_id", vouchers["GUID"]),
                                   new JProperty("customer_guid", vouchers["CUSTOMERNAME"]),
                                   new JProperty("category_id", "NA"),
                                   new JProperty("po_no", vouchers["PONO"]),
                                   new JProperty("po_date", vouchers["PODate"]),
                                   new JProperty("invoice_no", vouchers["INVOICENO"]),
                                   new JProperty("bill_reference_no", vouchers["BILLREFERENCENO"]),
                                   new JProperty("invoice_date", vouchers["INVOICEDATE"]),
                                   new JProperty("invoice_amount", vouchers["INVOICEAMOUNT"]),
                                   new JProperty("igst", vouchers["IGSTAMOUNT"]),
                                   new JProperty("cgst", vouchers["CGSTAMOUNT"]),
                                   new JProperty("sgst", vouchers["SGSTAMOUNT"]),
                                   new JProperty("roundoff", vouchers["RNDOFFAMT"]),
                                   new JProperty("description", vouchers["DESCRIPTION"]),
                                   new JProperty("payment_term", vouchers["PAYMENTTERM"]),
                                   new JProperty("payment_due_date", vouchers["PAYMENTDUEDATE"]),
                                   new JProperty("start_reminder_from", "0"),
                                   new JProperty("reminder_frequency", "0"),
                                   new JProperty("payment_status", "0"),
                                   new JProperty("payment_type", "NA"),
                                   new JProperty("payment_date", "NA"),
                                   new JProperty("more_receipents", "NA")
                                   ));


                JArray receiptvoucherDoc =
                   new JArray(
                     from DataRow receiptVoucher in receiptvoucherDataTable.Rows
                     select new JObject(
                            new JProperty("company_id", COMPANYINITIALS),
                            new JProperty("receipt_gu_id", receiptVoucher["GUID"]),
                             new JProperty("customer_guid", receiptVoucher["PARTYNAME"]),
                             new JProperty("invoice_no", receiptVoucher["INVOICENO"]),
                             new JProperty("paid_amount", receiptVoucher["AMOUNT"]),
                             new JProperty("deduction_amount", "0"),
                             new JProperty("description", receiptVoucher["NARRATION"]),
                             new JProperty("payment_date", receiptVoucher["DATE"])
                             ));


                JSONcustomerdoc = customerdoc.ToString();
                JSONdoc = voucherdoc.ToString();
                JSONreceiptdoc = receiptvoucherDoc.ToString();

                this.JsonStringMsg = JSONdoc;

                response_from_customer_api = Customer_Track_payout_POST_SendReqst(JSONcustomerdoc);


                JObject c = JObject.Parse(response_from_customer_api);
                customerPostFlag = (string)c.SelectToken("status");



                if (customerPostFlag == "200")
                {
                    this.customerPosttag = "Customer Sync Successfully.";
                }
                if (customerPostFlag == "201")
                {
                    this.customerPosttag = "Customer up-to-date.";
                }

                if (customerPostFlag == "200" || customerPostFlag == "201")
                {


                    var i = Invoice_Track_payout_POST_SendReqst(JSONdoc);
                    var r = Receipt_Track_payout_POST_SendReqst(JSONreceiptdoc);

                    JObject iJObject = JObject.Parse(i);

                    JObject rJObject = JObject.Parse(r);

                    iPostFlag = (string)iJObject.SelectToken("status");
                    rPostFlag = (string)rJObject.SelectToken("status");



                    if (iPostFlag == "200")
                    {
                        invoiceResponse = true;
                        this.invoicePostTag = "Invoice Sync Successfully";

                    }

                    if (iPostFlag == "201")
                    {
                        invoiceResponse = true;
                        this.invoicePostTag = "Invoice up-to-date.";


                    }

                    if (rPostFlag == "200")
                    {
                        receiptResponse = true;
                        this.receiptPostTag = "receipt sync successfully";


                    }

                    if (rPostFlag == "201")
                    {
                        receiptResponse = true;
                        this.receiptPostTag = "receipt up-to-date.";

                    }

                    if (invoiceResponse && receiptResponse)
                    {
                        lCBResponse = true;
                        return lCBResponse;
                    }

                }

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionTPO_JSONstringResponseforWebServerAgainstPostedJSONString;
                logger.Log(errorCodeFunc + " : " + ex.Message);

            }
            return lCBResponse;
        }

        public string Customer_Track_payout_POST_SendReqst(string pWebReqstStr)
        {

            String lResponsestr = "";
            String LResult = "";
            String lCompanyinitial = "";


            try
            {


                //lCompanyinitial = COMPANYINITIALS;
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = "http://localhost:8001/api/tally/utility/customer/store";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)pWebReqstStr.Length;
                httpWebRequest.ContentType = "application/json";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(pWebReqstStr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionCustomer_Track_payout_POST_SendReqst;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return LResult;
        }

        public string Invoice_Track_payout_POST_SendReqst(string pWebReqstStr)
        {

            String lResponsestr = "";
            String LResult = "";
            String lCompanyinitial = "";


            try
            {


                //lCompanyinitial = COMPANYINITIALS;
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = "http://localhost:8001/api/tally/utility/invoice/store";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)pWebReqstStr.Length;
                httpWebRequest.ContentType = "application/json";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(pWebReqstStr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionInvoice_Track_payout_POST_SendReqst;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return LResult;
        }

        public string Receipt_Track_payout_POST_SendReqst(string pWebReqstStr)
        {

            String lResponsestr = "";
            String LResult = "";
            String lCompanyinitial = "";


            try
            {


                //lCompanyinitial = COMPANYINITIALS;
                string lCBIPAddress = this.IPAddress;// appConfiguration.CBIPAddress;

                String lClearBalanceHost = "http://localhost:8001/api/tally/utility/invoice/credit_notes";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(lClearBalanceHost);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 600000;
                httpWebRequest.ContentLength = (long)pWebReqstStr.Length;
                httpWebRequest.ContentType = "application/json";
                StreamWriter lstreamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                lstreamWriter.Write(pWebReqstStr);
                lstreamWriter.Close();


                HttpWebResponse lhttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream lreceivestream = lhttpWebResponse.GetResponseStream();
                StreamReader lstreamReader = new StreamReader(lreceivestream, Encoding.UTF8);
                lResponsestr = lstreamReader.ReadToEnd();
                lhttpWebResponse.Close();
                lstreamReader.Close();
                LResult = lResponsestr;

            }
            catch (Exception ex)
            {
                ErrorCodeForAPIConnectionEnum errorCodeFunc = ErrorCodeForAPIConnectionEnum.FunctionInvoice_Track_payout_POST_SendReqst;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return LResult;
        }


    }
}
