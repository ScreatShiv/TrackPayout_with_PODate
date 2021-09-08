using System.Windows.Forms;
using System.Xml;
using System.Data;
using System;
using CB_TallyConnector.XMLRequestConnection;
using CB_TallyConnector.Connection;
using CB_TallyConnector.Log;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;



namespace CB_TallyConnector.ResponseModel
{

    enum ErrorCodeForResponseModelEnum
    {
        FunctionCompanyXMLResponsefromTallyService,
        FunctionCreditNoteVoucherXMPResponseFromTally,
        FunctionSalesVoucherXMPResponseFromTally,
        FunctionTallyCompanyVoucherXMLResponse,
        FunctionTallyStatictisXMLReponse,
        FunctionTallyActiveCompanyXMLResponseforSignup,
        FunctionTallyBalanceSheetReportXMLResponse,
        FunctionTallyStatictisReportXMLResponse,
        FunctionTallyCustomerMasterXMLResponse,
        FunctionTallyActiveCompanyXMLResponse,
        FunctionTallyVoucherLedgerClosingBalance,
        FunctionPaymentVoucherXMPResponseFromTally,
        FunctionReceiptVoucherXMPResponseFromTally,
        FunctionJournalReceiptVoucherXMLResponseFromTally,
        FunctionJournalInvoiceVoucherXMLResponseFromTally,
        FunctionDebitNoteVoucherXMPResponseFromTally,
        FunctionPurchaesVoucherXMPResponseFromTally,
    }

    public class XMLResponseModel
    {
        XMLRequest xML = new XMLRequest();

        APIConnection aPIConnection = new APIConnection();

        Logger logger = new Logger();

        
        /// <summary>
        ///  Clear Balance Response from Tally ERP 9 or Tally PRIME
        /// </summary>
        /// <returns></returns>
        public DataTable CompanyXMLResponsefromTallyService()
        {

            DataTable TallyDataNew = new DataTable();
            DataTable TallyData = TallyActiveCompanyXMLResponse();

            try
            {


                
                TallyDataNew.Columns.Add("Company Name");//3
                TallyDataNew.Columns.Add("Company Number");//2
                TallyDataNew.Columns.Add("Books Start Date");//4
                TallyDataNew.Columns.Add("Last Entry Date");//6
                TallyDataNew.Columns.Add("Company ID");//1
                TallyDataNew.Columns.Add("Last Sync DateTime");


                if (TallyData.Rows.Count > 0)
                {

                    foreach (DataRow row in TallyData.Rows)
                    {
                        if (row["REGISTERED"].ToString() == "Yes")
                        {
                            TallyDataNew.Rows.Add(
                                            row["NAME"].ToString(),
                                            row["COMPANYNUMBER"].ToString(),
                                            row["STARTINGFROM"].ToString(),
                                            row["LASTVOUCHERDATE"].ToString(),
                                            row["COMPANYINITIALS"].ToString(),
                                            row["LASTSYNC"].ToString()
                                            );
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionCompanyXMLResponsefromTallyService;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);

            }

            return TallyDataNew;

        }

        public DataTable CreditNoteVoucherXMPResponseFromTally(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();
            try
            {
                DateTime FinacialyearStartdate = Convert.ToDateTime(BOOKBEGINNINGFROM);
                DateTime FinacialyearEndDate = Convert.ToDateTime(LASTVOUCHERENTRYDATE);

                TimeSpan timeSpan = FinacialyearEndDate - FinacialyearStartdate;

                int TotalDays = Int32.Parse(timeSpan.TotalDays.ToString());

                int TotalMonth = TotalDays / 30;

                Boolean IsLeapYear = DateTime.IsLeapYear(FinacialyearStartdate.Year);

                TallyData.Columns.Add("GUID");
                TallyData.Columns.Add("VOUCHERTYPENAME");
                TallyData.Columns.Add("DATE");
                TallyData.Columns.Add("VOUCHERNUMBER");
                TallyData.Columns.Add("PARTYNAME");
                TallyData.Columns.Add("AMOUNT");
                TallyData.Columns.Add("NARRATION");
                TallyData.Columns.Add("TALLYVOUCHERTYPENAME");
                TallyData.Columns.Add("PARENTTYPE");

                for (int l = 0; l <= TotalMonth; l++)
                {

                    DateTime Fdate = FinacialyearStartdate.AddMonths(l);
                    DateTime Ldate = Fdate;
                    int getdayinmonth = DateTime.DaysInMonth(Fdate.Year, Fdate.Month);

                    if (getdayinmonth == 30 || getdayinmonth == 31)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }
                    if (Fdate.Month == 2)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }

                    string FDate = Fdate.ToShortDateString();
                    string LDate = Ldate.ToShortDateString();

                    doc.LoadXml(xML.CreditNoteVoucherCollectionXMLRequest(CURRENTCOMPANY, FDate, LDate, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS));



                    XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/VOUCHER");

                    foreach (XmlNode xn in nodeslIST)
                    {
                        if (xn.Name != "TALLYVOUCHERTYPENAME")
                        {
                            XmlNode tallyvouchertype = doc.CreateElement("TALLYVOUCHERTYPENAME");
                            tallyvouchertype.InnerText = "Credit Note";
                            xn.AppendChild(tallyvouchertype);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "AMOUNT")
                        {
                            XmlNode amount = doc.CreateElement("AMOUNT");
                            amount.InnerText = "0";
                            xn.AppendChild(amount);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "VOUCHERNUMBER")
                        {
                            XmlNode vouchernumber = doc.CreateElement("VOUCHERNUMBER");
                            vouchernumber.InnerText = "0";
                            xn.AppendChild(vouchernumber);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "DATE")
                        {
                            XmlNode date = doc.CreateElement("DATE");
                            date.InnerText = "0";
                            xn.AppendChild(date);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "NARRATION")
                        {
                            XmlNode narration = doc.CreateElement("NARRATION");
                            narration.InnerText = "0";
                            xn.AppendChild(narration);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn["DATE"].Name == "DATE")
                        {

                            XmlNode datenew = doc.CreateElement("strDate");

                            string year = xn["DATE"].InnerText.ToString().Substring(0, 4);
                            string month = xn["DATE"].InnerText.ToString().Substring(4, 2);
                            string date1 = xn["DATE"].InnerText.ToString().Substring(6, 2);

                            string dtstring = year + "-" + month + "-" + date1;

                            datenew.InnerText = dtstring;
                            xn.AppendChild(datenew);
                            doc.DocumentElement.AppendChild(xn);


                        }

                    }

                    if (nodeslIST.Count > 0)
                    {
                        foreach (XmlNode xn in nodeslIST)
                        {
                            if (l > 0)
                            {
                                DataRow dataRow = TallyData.NewRow();
                                dataRow[0] = xn["GUID"].InnerText.ToString();
                                dataRow[1] = xn["VOUCHERTYPENAME"].InnerText.ToString();
                                dataRow[2] = xn["strDate"].InnerText.ToString();
                                dataRow[3] = xn["VOUCHERNUMBER"].InnerText.ToString();// + "-" + xn["GUID"].InnerText.ToString();
                                dataRow[4] = xn["XGUIDPARTY"].InnerText.ToString();
                                dataRow[5] = xn["AMOUNT"].InnerText.ToString().Insert(0, "-");
                                dataRow[6] = xn["NARRATION"].InnerText.ToString();
                                dataRow[7] = xn["TALLYVOUCHERTYPENAME"].InnerText.ToString();
                                dataRow[8] = xn["XPARENTTYPE"].InnerText.ToString();

                                TallyData.Rows.InsertAt(dataRow, 0);
                            }
                            else
                            {
                                TallyData.Rows.Add(xn["GUID"].InnerText.ToString(),
                                                   xn["VOUCHERTYPENAME"].InnerText.ToString(),
                                                   xn["strDate"].InnerText.ToString(),
                                                   xn["VOUCHERNUMBER"].InnerText.ToString(),// + "-" + xn["GUID"].InnerText.ToString(),
                                                   xn["XGUIDPARTY"].InnerText.ToString(),
                                                   xn["AMOUNT"].InnerText.ToString().Insert(0, "-"),
                                                   xn["NARRATION"].InnerText.ToString(),
                                                   xn["TALLYVOUCHERTYPENAME"].InnerText.ToString(),
                                                   xn["XPARENTTYPE"].InnerText.ToString()
                              );
                            }


                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionCreditNoteVoucherXMPResponseFromTally;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }


            return TallyData;

        }

        public DataTable SalesVoucherXMPResponseFromTally(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();
            try
            {

                DateTime FinacialyearStartdate = Convert.ToDateTime(BOOKBEGINNINGFROM);
                DateTime FinacialyearEndDate = Convert.ToDateTime(LASTVOUCHERENTRYDATE);

                TimeSpan timeSpan = FinacialyearEndDate - FinacialyearStartdate;

                int TotalDays = Int32.Parse(timeSpan.TotalDays.ToString());

                int TotalMonth = TotalDays / 30;

                Boolean IsLeapYear = DateTime.IsLeapYear(FinacialyearStartdate.Year);

                TallyData.Columns.Add("GUID");
                TallyData.Columns.Add("VOUCHERTYPENAME");
                TallyData.Columns.Add("DATE");
                TallyData.Columns.Add("VOUCHERNUMBER");
                TallyData.Columns.Add("PARTYNAME");
                TallyData.Columns.Add("AMOUNT");
                TallyData.Columns.Add("NARRATION");
                TallyData.Columns.Add("TALLYVOUCHERTYPENAME");
                TallyData.Columns.Add("PARENTTYPE");



                for (int l = 0; l <= TotalMonth; l++)
                {

                    DateTime Fdate = FinacialyearStartdate.AddMonths(l);
                    DateTime Ldate = Fdate;
                    int getdayinmonth = DateTime.DaysInMonth(Fdate.Year, Fdate.Month);

                    if (getdayinmonth == 30 || getdayinmonth == 31)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }
                    if (Fdate.Month == 2)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }

                    string FDate = Fdate.ToShortDateString();
                    string LDate = Ldate.ToShortDateString();

                    doc.LoadXml(xML.SalesVoucherCollectionXMLRequest(CURRENTCOMPANY, FDate, LDate, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS));



                    XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/VOUCHER");

                    foreach (XmlNode xn in nodeslIST)
                    {

                        if (xn.Name != "TALLYVOUCHERTYPENAME")
                        {
                            XmlNode tallyvouchertype = doc.CreateElement("TALLYVOUCHERTYPENAME");
                            tallyvouchertype.InnerText = "Sales";
                            xn.AppendChild(tallyvouchertype);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "AMOUNT")
                        {
                            XmlNode amount = doc.CreateElement("AMOUNT");
                            amount.InnerText = "0";
                            xn.AppendChild(amount);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "VOUCHERNUMBER")
                        {
                            XmlNode vouchernumber = doc.CreateElement("VOUCHERNUMBER");
                            vouchernumber.InnerText = "0";
                            xn.AppendChild(vouchernumber);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "DATE")
                        {
                            XmlNode date = doc.CreateElement("DATE");
                            date.InnerText = "0";
                            xn.AppendChild(date);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "NARRATION")
                        {
                            XmlNode narration = doc.CreateElement("NARRATION");
                            narration.InnerText = "0";
                            xn.AppendChild(narration);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn["DATE"].Name == "DATE")
                        {

                            XmlNode datenew = doc.CreateElement("strDate");

                            string year = xn["DATE"].InnerText.ToString().Substring(0, 4);
                            string month = xn["DATE"].InnerText.ToString().Substring(4, 2);
                            string date1 = xn["DATE"].InnerText.ToString().Substring(6, 2);

                            string dtstring = year + "-" + month + "-" + date1;

                            datenew.InnerText = dtstring;
                            xn.AppendChild(datenew);
                            doc.DocumentElement.AppendChild(xn);


                        }

                    }

                    if (nodeslIST.Count > 0)
                    {
                        foreach (XmlNode xn in nodeslIST)
                        {
                            if (l > 0)
                            {
                                DataRow dataRow = TallyData.NewRow();
                                dataRow[0] = xn["GUID"].InnerText.ToString();
                                dataRow[1] = xn["VOUCHERTYPENAME"].InnerText.ToString();
                                dataRow[2] = xn["strDate"].InnerText.ToString();
                                dataRow[3] = xn["VOUCHERNUMBER"].InnerText.ToString();// + "-" + xn["GUID"].InnerText.ToString();
                                dataRow[4] = xn["XGUIDPARTY"].InnerText.ToString();
                                dataRow[5] = xn["AMOUNT"].InnerText.ToString().Remove(0, 1);
                                dataRow[6] = xn["NARRATION"].InnerText.ToString();
                                dataRow[7] = xn["TALLYVOUCHERTYPENAME"].InnerText.ToString();
                                dataRow[8] = xn["XPARENTTYPE"].InnerText.ToString();

                                TallyData.Rows.InsertAt(dataRow, 0);
                            }
                            else
                            {
                                TallyData.Rows.Add(xn["GUID"].InnerText.ToString(),
                                                    xn["VOUCHERTYPENAME"].InnerText.ToString(),
                                                    xn["strDate"].InnerText.ToString(),
                                                    xn["VOUCHERNUMBER"].InnerText.ToString(),// + "-" + xn["GUID"].InnerText.ToString(),
                                                    xn["XGUIDPARTY"].InnerText.ToString(),
                                                    xn["AMOUNT"].InnerText.ToString().Remove(0, 1),
                                                    xn["NARRATION"].InnerText.ToString(),
                                                    xn["TALLYVOUCHERTYPENAME"].InnerText.ToString(),
                                                    xn["XPARENTTYPE"].InnerText.ToString()
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionSalesVoucherXMPResponseFromTally;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }


            return TallyData;

        }

        public DataTable PurchaesVoucherXMPResponseFromTally(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();
            try
            {

                DateTime FinacialyearStartdate = Convert.ToDateTime(BOOKBEGINNINGFROM);
                DateTime FinacialyearEndDate = Convert.ToDateTime(LASTVOUCHERENTRYDATE);

                TimeSpan timeSpan = FinacialyearEndDate - FinacialyearStartdate;

                int TotalDays = Int32.Parse(timeSpan.TotalDays.ToString());

                int TotalMonth = TotalDays / 30;

                Boolean IsLeapYear = DateTime.IsLeapYear(FinacialyearStartdate.Year);

                TallyData.Columns.Add("GUID");
                TallyData.Columns.Add("VOUCHERTYPENAME");
                TallyData.Columns.Add("DATE");
                TallyData.Columns.Add("VOUCHERNUMBER");
                TallyData.Columns.Add("PARTYNAME");
                TallyData.Columns.Add("AMOUNT");
                TallyData.Columns.Add("NARRATION");
                TallyData.Columns.Add("TALLYVOUCHERTYPENAME");
                TallyData.Columns.Add("PARENTTYPE");




                for (int l = 0; l <= TotalMonth; l++)
                {

                    DateTime Fdate = FinacialyearStartdate.AddMonths(l);
                    DateTime Ldate = Fdate;
                    int getdayinmonth = DateTime.DaysInMonth(Fdate.Year, Fdate.Month);

                    if (getdayinmonth == 30 || getdayinmonth == 31)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }
                    if (Fdate.Month == 2)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }

                    string FDate = Fdate.ToShortDateString();
                    string LDate = Ldate.ToShortDateString();

                    doc.LoadXml(xML.PurchaseVoucherCollectionXMLRequest(CURRENTCOMPANY, FDate, LDate, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS));

                    XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/VOUCHER");

                    foreach (XmlNode xn in nodeslIST)
                    {

                        if (xn.Name != "TALLYVOUCHERTYPENAME")
                        {
                            XmlNode tallyvouchertype = doc.CreateElement("TALLYVOUCHERTYPENAME");
                            tallyvouchertype.InnerText = "Purchase";
                            xn.AppendChild(tallyvouchertype);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "AMOUNT")
                        {
                            XmlNode amount = doc.CreateElement("AMOUNT");
                            amount.InnerText = "0";
                            xn.AppendChild(amount);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "VOUCHERNUMBER")
                        {
                            XmlNode vouchernumber = doc.CreateElement("VOUCHERNUMBER");
                            vouchernumber.InnerText = "0";
                            xn.AppendChild(vouchernumber);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "DATE")
                        {
                            XmlNode date = doc.CreateElement("DATE");
                            date.InnerText = "0";
                            xn.AppendChild(date);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "NARRATION")
                        {
                            XmlNode narration = doc.CreateElement("NARRATION");
                            narration.InnerText = "0";
                            xn.AppendChild(narration);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn["DATE"].Name == "DATE")
                        {

                            XmlNode datenew = doc.CreateElement("strDate");

                            string year = xn["DATE"].InnerText.ToString().Substring(0, 4);
                            string month = xn["DATE"].InnerText.ToString().Substring(4, 2);
                            string date1 = xn["DATE"].InnerText.ToString().Substring(6, 2);

                            string dtstring = year + "-" + month + "-" + date1;

                            datenew.InnerText = dtstring;
                            xn.AppendChild(datenew);
                            doc.DocumentElement.AppendChild(xn);


                        }

                    }

                    if (nodeslIST.Count > 0)
                    {
                        foreach (XmlNode xn in nodeslIST)
                        {
                            if (l > 0)
                            {
                                DataRow dataRow = TallyData.NewRow();
                                dataRow[0] = xn["GUID"].InnerText.ToString();
                                dataRow[1] = xn["VOUCHERTYPENAME"].InnerText.ToString();
                                dataRow[2] = xn["strDate"].InnerText.ToString();
                                dataRow[3] = xn["VOUCHERNUMBER"].InnerText.ToString();// + "-" + xn["GUID"].InnerText.ToString();
                                dataRow[4] = xn["XGUIDPARTY"].InnerText.ToString();
                                dataRow[5] = xn["AMOUNT"].InnerText.ToString().Insert(0, "-");
                                dataRow[6] = xn["NARRATION"].InnerText.ToString();
                                dataRow[7] = xn["TALLYVOUCHERTYPENAME"].InnerText.ToString();
                                dataRow[8] = xn["XPARENTTYPE"].InnerText.ToString();
                                TallyData.Rows.InsertAt(dataRow, 0);
                            }
                            else
                            {
                                TallyData.Rows.Add(xn["GUID"].InnerText.ToString(),
                                                    xn["VOUCHERTYPENAME"].InnerText.ToString(),
                                                    xn["strDate"].InnerText.ToString(),
                                                    xn["VOUCHERNUMBER"].InnerText.ToString(),// + "-" + xn["GUID"].InnerText.ToString(),
                                                    xn["XGUIDPARTY"].InnerText.ToString(),
                                                    xn["AMOUNT"].InnerText.ToString().Insert(0, "-"),
                                                    xn["NARRATION"].InnerText.ToString(),
                                                    xn["TALLYVOUCHERTYPENAME"].InnerText.ToString(),
                                                    xn["XPARENTTYPE"].InnerText.ToString()
                              );

                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionPurchaesVoucherXMPResponseFromTally;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }


            return TallyData;

        }

        public DataTable DebitNoteVoucherXMPResponseFromTally(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();
            try
            {


                DateTime FinacialyearStartdate = Convert.ToDateTime(BOOKBEGINNINGFROM);
                DateTime FinacialyearEndDate = Convert.ToDateTime(LASTVOUCHERENTRYDATE);

                TimeSpan timeSpan = FinacialyearEndDate - FinacialyearStartdate;

                int TotalDays = Int32.Parse(timeSpan.TotalDays.ToString());

                int TotalMonth = TotalDays / 30;

                Boolean IsLeapYear = DateTime.IsLeapYear(FinacialyearStartdate.Year);


                TallyData.Columns.Add("GUID");
                TallyData.Columns.Add("VOUCHERTYPENAME");
                TallyData.Columns.Add("DATE");
                TallyData.Columns.Add("VOUCHERNUMBER");
                TallyData.Columns.Add("PARTYNAME");
                TallyData.Columns.Add("AMOUNT");
                TallyData.Columns.Add("NARRATION");
                TallyData.Columns.Add("TALLYVOUCHERTYPENAME");
                TallyData.Columns.Add("PARENTTYPE");



                for (int l = 0; l <= TotalMonth; l++)
                {

                    DateTime Fdate = FinacialyearStartdate.AddMonths(l);
                    DateTime Ldate = Fdate;
                    int getdayinmonth = DateTime.DaysInMonth(Fdate.Year, Fdate.Month);

                    if (getdayinmonth == 30 || getdayinmonth == 31)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }
                    if (Fdate.Month == 2)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }

                    string FDate = Fdate.ToShortDateString();
                    string LDate = Ldate.ToShortDateString();


                    doc.LoadXml(xML.DebitNoteVoucherCollectionXMLRequest(CURRENTCOMPANY, FDate, LDate, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS));


                    XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/VOUCHER");

                    foreach (XmlNode xn in nodeslIST)
                    {


                        if (xn.Name != "TALLYVOUCHERTYPENAME")
                        {
                            XmlNode tallyvouchertype = doc.CreateElement("TALLYVOUCHERTYPENAME");
                            tallyvouchertype.InnerText = "Debit Note";
                            xn.AppendChild(tallyvouchertype);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "AMOUNT")
                        {
                            XmlNode amount = doc.CreateElement("AMOUNT");
                            amount.InnerText = "0";
                            xn.AppendChild(amount);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "VOUCHERNUMBER")
                        {
                            XmlNode vouchernumber = doc.CreateElement("VOUCHERNUMBER");
                            vouchernumber.InnerText = "0";
                            xn.AppendChild(vouchernumber);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "DATE")
                        {
                            XmlNode date = doc.CreateElement("DATE");
                            date.InnerText = "0";
                            xn.AppendChild(date);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "NARRATION")
                        {
                            XmlNode narration = doc.CreateElement("NARRATION");
                            narration.InnerText = "0";
                            xn.AppendChild(narration);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn["DATE"].Name == "DATE")
                        {

                            XmlNode datenew = doc.CreateElement("strDate");

                            string year = xn["DATE"].InnerText.ToString().Substring(0, 4);
                            string month = xn["DATE"].InnerText.ToString().Substring(4, 2);
                            string date1 = xn["DATE"].InnerText.ToString().Substring(6, 2);

                            string dtstring = year + "-" + month + "-" + date1;

                            datenew.InnerText = dtstring;
                            xn.AppendChild(datenew);
                            doc.DocumentElement.AppendChild(xn);


                        }

                        if (xn["AMOUNT"].Name == "AMOUNT")
                        {

                            XmlNode debitamtnew = doc.CreateElement("XAMOUNT");

                            string DebitAmt = xn["AMOUNT"].InnerText.ToString().Remove(0, 1);


                            string debitAmtstring = DebitAmt;

                            debitamtnew.InnerText = debitAmtstring;
                            xn.AppendChild(debitamtnew);
                            doc.DocumentElement.AppendChild(xn);


                        }


                    }

                    if (nodeslIST.Count > 0)
                    {
                        foreach (XmlNode xn in nodeslIST)
                        {
                            if (l > 0)
                            {
                                DataRow dataRow = TallyData.NewRow();
                                dataRow[0] = xn["GUID"].InnerText.ToString();
                                dataRow[1] = xn["VOUCHERTYPENAME"].InnerText.ToString();
                                dataRow[2] = xn["strDate"].InnerText.ToString();
                                dataRow[3] = xn["VOUCHERNUMBER"].InnerText.ToString();// + "-" + xn["GUID"].InnerText.ToString();
                                dataRow[4] = xn["XGUIDPARTY"].InnerText.ToString();
                                dataRow[5] = xn["XAMOUNT"].InnerText.ToString().Insert(0, "+");
                                dataRow[6] = xn["NARRATION"].InnerText.ToString();
                                dataRow[7] = xn["TALLYVOUCHERTYPENAME"].InnerText.ToString();
                                dataRow[8] = xn["XPARENTTYPE"].InnerText.ToString();

                                TallyData.Rows.InsertAt(dataRow, 0);
                            }
                            else
                            {
                                TallyData.Rows.Add(xn["GUID"].InnerText.ToString(),
                                                    xn["VOUCHERTYPENAME"].InnerText.ToString(),
                                                    xn["strDate"].InnerText.ToString(),
                                                    xn["VOUCHERNUMBER"].InnerText.ToString(),// + "-" + xn["GUID"].InnerText.ToString(),
                                                    xn["XGUIDPARTY"].InnerText.ToString(),
                                                    xn["XAMOUNT"].InnerText.ToString().Insert(0, "+"),
                                                    xn["NARRATION"].InnerText.ToString(),
                                                    xn["TALLYVOUCHERTYPENAME"].InnerText.ToString(),
                                                    xn["XPARENTTYPE"].InnerText.ToString()
                              );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionDebitNoteVoucherXMPResponseFromTally;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }


            return TallyData;

        }

        public DataTable JournalInvoiceVoucherXMLResponseFromTally(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();
            try
            {


                DateTime FinacialyearStartdate = Convert.ToDateTime(BOOKBEGINNINGFROM);
                DateTime FinacialyearEndDate = Convert.ToDateTime(LASTVOUCHERENTRYDATE);

                TimeSpan timeSpan = FinacialyearEndDate - FinacialyearStartdate;

                int TotalDays = Int32.Parse(timeSpan.TotalDays.ToString());

                int TotalMonth = TotalDays / 30;

                Boolean IsLeapYear = DateTime.IsLeapYear(FinacialyearStartdate.Year);

                TallyData.Columns.Add("GUID");
                TallyData.Columns.Add("VOUCHERTYPENAME");
                TallyData.Columns.Add("DATE");
                TallyData.Columns.Add("VOUCHERNUMBER");
                TallyData.Columns.Add("PARTYNAME");
                TallyData.Columns.Add("AMOUNT");
                TallyData.Columns.Add("NARRATION");
                TallyData.Columns.Add("DRorCR");
                TallyData.Columns.Add("TALLYVOUCHERTYPENAME");
                TallyData.Columns.Add("PARENTTYPE");


                for (int l = 0; l <= TotalMonth; l++)
                {

                    DateTime Fdate = FinacialyearStartdate.AddMonths(l);
                    DateTime Ldate = Fdate;
                    int getdayinmonth = DateTime.DaysInMonth(Fdate.Year, Fdate.Month);

                    if (getdayinmonth == 30 || getdayinmonth == 31)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }
                    if (Fdate.Month == 2)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }

                    string FDate = Fdate.ToShortDateString();
                    string LDate = Ldate.ToShortDateString();

                    doc.LoadXml(xML.JournalInvoiceVoucherCollectionXMLRequest(CURRENTCOMPANY, FDate, LDate, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS));

                    XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/LEDGERENTRY");

                    foreach (XmlNode xn in nodeslIST)
                    {

                        if (xn.Name != "TALLYVOUCHERTYPENAME")
                        {
                            XmlNode tallyvouchertype = doc.CreateElement("TALLYVOUCHERTYPENAME");
                            tallyvouchertype.InnerText = "Journal";
                            xn.AppendChild(tallyvouchertype);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "XAMOUNT")
                        {
                            XmlNode amount = doc.CreateElement("XAMOUNT");
                            amount.InnerText = "0";
                            xn.AppendChild(amount);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "XVCHNO")
                        {
                            XmlNode vouchernumber = doc.CreateElement("XVCHNO");
                            vouchernumber.InnerText = "0";
                            xn.AppendChild(vouchernumber);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "XVCHDT")
                        {
                            XmlNode date = doc.CreateElement("XVCHDT");
                            date.InnerText = "0";
                            xn.AppendChild(date);
                            doc.DocumentElement.AppendChild(xn);

                        }



                        if (xn.Name != "XVCHNarration")
                        {
                            XmlNode narration = doc.CreateElement("XVCHNarration");
                            narration.InnerText = "0";
                            xn.AppendChild(narration);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn["XVCHDT"].Name == "XVCHDT")
                        {

                            XmlNode datenew = doc.CreateElement("strDate");

                            string year = xn["XVCHDT"].InnerText.ToString().Substring(0, 4);
                            string month = xn["XVCHDT"].InnerText.ToString().Substring(4, 2);
                            string date1 = xn["XVCHDT"].InnerText.ToString().Substring(6, 2);

                            string dtstring = year + "-" + month + "-" + date1;

                            datenew.InnerText = dtstring;
                            xn.AppendChild(datenew);
                            doc.DocumentElement.AppendChild(xn);


                        }

                        if (xn["XAMOUNT"].Name == "XAMOUNT")
                        {

                            XmlNode debitamtnew = doc.CreateElement("XXAMOUNT");

                            string DebitAmt = xn["XAMOUNT"].InnerText.ToString().Remove(0, 1);


                            string debitAmtstring = DebitAmt;

                            debitamtnew.InnerText = debitAmtstring;
                            xn.AppendChild(debitamtnew);
                            doc.DocumentElement.AppendChild(xn);


                        }


                    }

                    if (nodeslIST.Count > 0)
                    {

                        foreach (XmlNode xn in nodeslIST)
                        {
                            if (l > 0)
                            {
                                DataRow dataRow = TallyData.NewRow();
                                dataRow[0] = xn["XGUID"].InnerText.ToString();
                                dataRow[1] = xn["XVCHVTYP"].InnerText.ToString();
                                dataRow[2] = xn["strDate"].InnerText.ToString();
                                dataRow[3] = xn["XVCHNO"].InnerText.ToString();// + "-" + xn["XGUID"].InnerText.ToString();
                                dataRow[4] = xn["XLEDGERNAME"].InnerText.ToString();
                                dataRow[5] = xn["XXAMOUNT"].InnerText.ToString().Insert(0, "+");
                                dataRow[6] = xn["XVCHNarration"].InnerText.ToString();
                                dataRow[7] = xn["XDRORCRFLAG"].InnerText.ToString();
                                dataRow[8] = xn["TALLYVOUCHERTYPENAME"].InnerText.ToString();
                                dataRow[9] = xn["XPARENTTYPE"].InnerText.ToString();
                                TallyData.Rows.InsertAt(dataRow, 0);
                            }
                            else
                            {
                                TallyData.Rows.Add(xn["XGUID"].InnerText.ToString(),
                                                   xn["XVCHVTYP"].InnerText.ToString(),
                                                   xn["strDate"].InnerText.ToString(),
                                                  xn["XVCHNO"].InnerText.ToString(),// + "-" + xn["XGUID"].InnerText.ToString(),
                                                  xn["XLEDGERNAME"].InnerText.ToString(),
                                                  xn["XXAMOUNT"].InnerText.ToString().Insert(0, "+"),
                                                  xn["XVCHNarration"].InnerText.ToString(),
                                                  xn["XDRORCRFLAG"].InnerText.ToString(),
                                                  xn["TALLYVOUCHERTYPENAME"].InnerText.ToString(),
                                                  xn["XPARENTTYPE"].InnerText.ToString()
                                );
                            }


                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionJournalInvoiceVoucherXMLResponseFromTally;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }

            return TallyData;
        }

        public DataTable JournalReceiptVoucherXMLResponseFromTally(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();
            try
            {

                DateTime FinacialyearStartdate = Convert.ToDateTime(BOOKBEGINNINGFROM);
                DateTime FinacialyearEndDate = Convert.ToDateTime(LASTVOUCHERENTRYDATE);

                TimeSpan timeSpan = FinacialyearEndDate - FinacialyearStartdate;

                int TotalDays = Int32.Parse(timeSpan.TotalDays.ToString());

                int TotalMonth = TotalDays / 30;

                Boolean IsLeapYear = DateTime.IsLeapYear(FinacialyearStartdate.Year);

                TallyData.Columns.Add("GUID");
                TallyData.Columns.Add("VOUCHERTYPENAME");
                TallyData.Columns.Add("DATE");
                TallyData.Columns.Add("VOUCHERNUMBER");
                TallyData.Columns.Add("PARTYNAME");
                TallyData.Columns.Add("AMOUNT");
                TallyData.Columns.Add("NARRATION");
                TallyData.Columns.Add("DRorCR");
                TallyData.Columns.Add("TALLYVOUCHERTYPENAME");
                TallyData.Columns.Add("PARENTTYPE");




                for (int l = 0; l <= TotalMonth; l++)
                {

                    DateTime Fdate = FinacialyearStartdate.AddMonths(l);
                    DateTime Ldate = Fdate;
                    int getdayinmonth = DateTime.DaysInMonth(Fdate.Year, Fdate.Month);

                    if (getdayinmonth == 30 || getdayinmonth == 31)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }
                    if (Fdate.Month == 2)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }

                    string FDate = Fdate.ToShortDateString();
                    string LDate = Ldate.ToShortDateString();



                    doc.LoadXml(xML.JournalReceiptVoucherCollectionXMLRequest(CURRENTCOMPANY, FDate, LDate, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS));

                    XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/LEDGERENTRY");

                    foreach (XmlNode xn in nodeslIST)
                    {

                        if (xn.Name != "TALLYVOUCHERTYPENAME")
                        {
                            XmlNode tallyvouchertype = doc.CreateElement("TALLYVOUCHERTYPENAME");
                            tallyvouchertype.InnerText = "Journal";
                            xn.AppendChild(tallyvouchertype);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "XAMOUNT")
                        {
                            XmlNode amount = doc.CreateElement("XAMOUNT");
                            amount.InnerText = "0";
                            xn.AppendChild(amount);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "XVCHNO")
                        {
                            XmlNode vouchernumber = doc.CreateElement("XVCHNO");
                            vouchernumber.InnerText = "0";
                            xn.AppendChild(vouchernumber);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "XVCHDT")
                        {
                            XmlNode date = doc.CreateElement("XVCHDT");
                            date.InnerText = "0";
                            xn.AppendChild(date);
                            doc.DocumentElement.AppendChild(xn);

                        }



                        if (xn.Name != "XVCHNarration")
                        {
                            XmlNode narration = doc.CreateElement("XVCHNarration");
                            narration.InnerText = "0";
                            xn.AppendChild(narration);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn["XVCHDT"].Name == "XVCHDT")
                        {

                            XmlNode datenew = doc.CreateElement("strDate");

                            string year = xn["XVCHDT"].InnerText.ToString().Substring(0, 4);
                            string month = xn["XVCHDT"].InnerText.ToString().Substring(4, 2);
                            string date1 = xn["XVCHDT"].InnerText.ToString().Substring(6, 2);

                            string dtstring = year + "-" + month + "-" + date1;

                            datenew.InnerText = dtstring;
                            xn.AppendChild(datenew);
                            doc.DocumentElement.AppendChild(xn);


                        }
                    }

                    if (nodeslIST.Count > 0)
                    {

                        foreach (XmlNode xn in nodeslIST)
                        {
                            if (l > 0)
                            {
                                DataRow dataRow = TallyData.NewRow();
                                dataRow[0] = xn["XGUID"].InnerText.ToString();
                                dataRow[1] = xn["XVCHVTYP"].InnerText.ToString();
                                dataRow[2] = xn["strDate"].InnerText.ToString();
                                dataRow[3] = xn["XVCHNO"].InnerText.ToString();// + "-" + xn["XGUID"].InnerText.ToString();
                                dataRow[4] = xn["XLEDGERNAME"].InnerText.ToString();
                                dataRow[5] = xn["XAMOUNT"].InnerText.ToString().Insert(0, "-");
                                dataRow[6] = xn["XVCHNarration"].InnerText.ToString();
                                dataRow[7] = xn["XDRORCRFLAG"].InnerText.ToString();
                                dataRow[8] = xn["TALLYVOUCHERTYPENAME"].InnerText.ToString();
                                dataRow[9] = xn["XPARENTTYPE"].InnerText.ToString();
                                TallyData.Rows.InsertAt(dataRow, 0);
                            }
                            else
                            {
                                TallyData.Rows.Add(xn["XGUID"].InnerText.ToString(),
                                                    xn["XVCHVTYP"].InnerText.ToString(),
                                                    xn["strDate"].InnerText.ToString(),
                                                    xn["XVCHNO"].InnerText.ToString(),// + "-" + xn["XGUID"].InnerText.ToString(),
                                                    xn["XLEDGERNAME"].InnerText.ToString(),
                                                    xn["XAMOUNT"].InnerText.ToString().Insert(0, "-"),
                                                    xn["XVCHNarration"].InnerText.ToString(),
                                                    xn["XDRORCRFLAG"].InnerText.ToString(),
                                                    xn["TALLYVOUCHERTYPENAME"].InnerText.ToString(),
                                                    xn["XPARENTTYPE"].InnerText.ToString()
                                );
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionJournalReceiptVoucherXMLResponseFromTally;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }

            return TallyData;

        }

        public DataTable ReceiptVoucherXMPResponseFromTally(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {

            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();
            try
            {
                DateTime FinacialyearStartdate = Convert.ToDateTime(BOOKBEGINNINGFROM);
                DateTime FinacialyearEndDate = Convert.ToDateTime(LASTVOUCHERENTRYDATE);

                TimeSpan timeSpan = FinacialyearEndDate - FinacialyearStartdate;

                int TotalDays = Int32.Parse(timeSpan.TotalDays.ToString());

                int TotalMonth = TotalDays / 30;

                Boolean IsLeapYear = DateTime.IsLeapYear(FinacialyearStartdate.Year);

                TallyData.Columns.Add("GUID");
                TallyData.Columns.Add("VOUCHERTYPENAME");
                TallyData.Columns.Add("DATE");
                TallyData.Columns.Add("VOUCHERNUMBER");
                TallyData.Columns.Add("PARTYNAME");
                TallyData.Columns.Add("AMOUNT");
                TallyData.Columns.Add("NARRATION");
                TallyData.Columns.Add("TALLYVOUCHERTYPENAME");
                TallyData.Columns.Add("PARENTTYPE");



                for (int l = 0; l <= TotalMonth; l++)
                {

                    DateTime Fdate = FinacialyearStartdate.AddMonths(l);
                    DateTime Ldate = Fdate;
                    int getdayinmonth = DateTime.DaysInMonth(Fdate.Year, Fdate.Month);

                    if (getdayinmonth == 30 || getdayinmonth == 31)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }
                    if (Fdate.Month == 2)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }

                    string FDate = Fdate.ToShortDateString();
                    string LDate = Ldate.ToShortDateString();


                    doc.LoadXml(xML.RecipetvoucherCollectionXMLRequest(CURRENTCOMPANY, FDate, LDate, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS));



                    XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/LEDGERENTRY");



                    foreach (XmlNode xn in nodeslIST)
                    {

                        if (xn.Name != "TALLYVOUCHERTYPENAME")
                        {
                            XmlNode tallyvouchertype = doc.CreateElement("TALLYVOUCHERTYPENAME");
                            tallyvouchertype.InnerText = "Receipt";
                            xn.AppendChild(tallyvouchertype);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "XAMOUNT")
                        {
                            XmlNode amount = doc.CreateElement("XAMOUNT");
                            amount.InnerText = "0";
                            xn.AppendChild(amount);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "XVCHNO")
                        {
                            XmlNode vouchernumber = doc.CreateElement("XVCHNO");
                            vouchernumber.InnerText = "0";
                            xn.AppendChild(vouchernumber);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "XVCHDT")
                        {
                            XmlNode date = doc.CreateElement("XVCHDT");
                            date.InnerText = "0";
                            xn.AppendChild(date);
                            doc.DocumentElement.AppendChild(xn);

                        }



                        if (xn.Name != "XLEDGERNAME")
                        {
                            XmlNode narration = doc.CreateElement("XLEDGERNAME");
                            narration.InnerText = "0";
                            xn.AppendChild(narration);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "XVCHNarration")
                        {
                            XmlNode narration = doc.CreateElement("XVCHNarration");
                            narration.InnerText = "Nil";
                            xn.AppendChild(narration);
                            doc.DocumentElement.AppendChild(xn);
                        }

                        if (xn["XVCHDT"].Name == "XVCHDT")
                        {

                            XmlNode datenew = doc.CreateElement("strDate");

                            string year = xn["XVCHDT"].InnerText.ToString().Substring(0, 4);
                            string month = xn["XVCHDT"].InnerText.ToString().Substring(4, 2);
                            string date1 = xn["XVCHDT"].InnerText.ToString().Substring(6, 2);

                            string dtstring = year + "-" + month + "-" + date1;

                            datenew.InnerText = dtstring;
                            xn.AppendChild(datenew);
                            doc.DocumentElement.AppendChild(xn);


                        }

                    }

                    if (nodeslIST.Count > 0)
                    {

                        foreach (XmlNode xn in nodeslIST)
                        {
                            if (l > 0)
                            {
                                DataRow dataRow = TallyData.NewRow();
                                dataRow[0] = xn["XGUID"].InnerText.ToString();
                                dataRow[1] = xn["XVCHVTYP"].InnerText.ToString();
                                dataRow[2] = xn["strDate"].InnerText.ToString();
                                dataRow[3] = xn["XVCHNO"].InnerText.ToString();// + "-" + xn["XGUID"].InnerText.ToString();
                                dataRow[4] = xn["XLEDGERNAME"].InnerText.ToString();
                                dataRow[5] = xn["XAMOUNT"].InnerText.ToString().Insert(0, "-");
                                dataRow[6] = xn["XVCHNarration"].InnerText.ToString();
                                dataRow[7] = xn["TALLYVOUCHERTYPENAME"].InnerText.ToString();
                                dataRow[8] = xn["XPARENTTYPE"].InnerText.ToString();

                                TallyData.Rows.InsertAt(dataRow, 0);
                            }
                            else
                            {
                                TallyData.Rows.Add(xn["XGUID"].InnerText.ToString(),
                                                    xn["XVCHVTYP"].InnerText.ToString(),
                                                    xn["strDate"].InnerText.ToString(),
                                                    xn["XVCHNO"].InnerText.ToString(),// + "-" + xn["XGUID"].InnerText.ToString(),
                                                    xn["XLEDGERNAME"].InnerText.ToString(),
                                                    xn["XAMOUNT"].InnerText.ToString().Insert(0, "-"),
                                                    xn["XVCHNarration"].InnerText.ToString(),
                                                    xn["TALLYVOUCHERTYPENAME"].InnerText.ToString(),
                                                    xn["XPARENTTYPE"].InnerText.ToString()
                                );
                            }


                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionReceiptVoucherXMPResponseFromTally;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }

            return TallyData;
        }

        public DataTable PaymentVoucherXMPResponseFromTally(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {

            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();
            try
            {

                DateTime FinacialyearStartdate = Convert.ToDateTime(BOOKBEGINNINGFROM);
                DateTime FinacialyearEndDate = Convert.ToDateTime(LASTVOUCHERENTRYDATE);

                TimeSpan timeSpan = FinacialyearEndDate - FinacialyearStartdate;

                int TotalDays = Int32.Parse(timeSpan.TotalDays.ToString());

                int TotalMonth = TotalDays / 30;

                Boolean IsLeapYear = DateTime.IsLeapYear(FinacialyearStartdate.Year);

                TallyData.Columns.Add("GUID");
                TallyData.Columns.Add("VOUCHERTYPENAME");
                TallyData.Columns.Add("DATE");
                TallyData.Columns.Add("VOUCHERNUMBER");
                TallyData.Columns.Add("PARTYNAME");
                TallyData.Columns.Add("AMOUNT");
                TallyData.Columns.Add("NARRATION");
                TallyData.Columns.Add("TALLYVOUCHERTYPENAME");
                TallyData.Columns.Add("PARENTTYPE");

                for (int l = 0; l <= TotalMonth; l++)
                {

                    DateTime Fdate = FinacialyearStartdate.AddMonths(l);
                    DateTime Ldate = Fdate;
                    int getdayinmonth = DateTime.DaysInMonth(Fdate.Year, Fdate.Month);

                    if (getdayinmonth == 30 || getdayinmonth == 31)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }
                    if (Fdate.Month == 2)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }

                    string FDate = Fdate.ToShortDateString();
                    string LDate = Ldate.ToShortDateString();


                    doc.LoadXml(xML.PaymentvoucherCollectionXMLRequest(CURRENTCOMPANY, FDate, LDate, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS));



                    XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/LEDGERENTRY");



                    foreach (XmlNode xn in nodeslIST)
                    {


                        if (xn.Name != "TALLYVOUCHERTYPENAME")
                        {
                            XmlNode tallyvouchertype = doc.CreateElement("TALLYVOUCHERTYPENAME");
                            tallyvouchertype.InnerText = "Payment";
                            xn.AppendChild(tallyvouchertype);
                            doc.DocumentElement.AppendChild(xn);


                        }
                        if (xn.Name != "XAMOUNT")
                        {
                            XmlNode amount = doc.CreateElement("XAMOUNT");
                            amount.InnerText = "0";
                            xn.AppendChild(amount);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "XVCHNO")
                        {
                            XmlNode vouchernumber = doc.CreateElement("XVCHNO");
                            vouchernumber.InnerText = "0";
                            xn.AppendChild(vouchernumber);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "XVCHDT")
                        {
                            XmlNode date = doc.CreateElement("XVCHDT");
                            date.InnerText = "0";
                            xn.AppendChild(date);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "XLEDGERNAME")
                        {
                            XmlNode narration = doc.CreateElement("XLEDGERNAME");
                            narration.InnerText = "0";
                            xn.AppendChild(narration);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "XVCHNarration")
                        {
                            XmlNode narration = doc.CreateElement("XVCHNarration");
                            narration.InnerText = "Nil";
                            xn.AppendChild(narration);
                            doc.DocumentElement.AppendChild(xn);
                        }

                        if (xn["XVCHDT"].Name == "XVCHDT")
                        {

                            XmlNode datenew = doc.CreateElement("strDate");

                            string year = xn["XVCHDT"].InnerText.ToString().Substring(0, 4);
                            string month = xn["XVCHDT"].InnerText.ToString().Substring(4, 2);
                            string date1 = xn["XVCHDT"].InnerText.ToString().Substring(6, 2);

                            string dtstring = year + "-" + month + "-" + date1;

                            datenew.InnerText = dtstring;
                            xn.AppendChild(datenew);
                            doc.DocumentElement.AppendChild(xn);


                        }

                        if (xn["XAMOUNT"].Name == "XAMOUNT")
                        {

                            XmlNode debitamtnew = doc.CreateElement("XXAMOUNT");

                            string DebitAmt = xn["XAMOUNT"].InnerText.ToString().Remove(0, 1);


                            string debitAmtstring = DebitAmt;

                            debitamtnew.InnerText = debitAmtstring;
                            xn.AppendChild(debitamtnew);
                            doc.DocumentElement.AppendChild(xn);


                        }


                    }
                    if (nodeslIST.Count > 0)
                    {

                        foreach (XmlNode xn in nodeslIST)
                        {
                            if (l > 0)
                            {
                                DataRow dataRow = TallyData.NewRow();
                                dataRow[0] = xn["XGUID"].InnerText.ToString();
                                dataRow[1] = xn["XVCHVTYP"].InnerText.ToString();
                                dataRow[2] = xn["strDate"].InnerText.ToString();
                                dataRow[3] = xn["XVCHNO"].InnerText.ToString();// + "-" + xn["XGUID"].InnerText.ToString();
                                dataRow[4] = xn["XLEDGERNAME"].InnerText.ToString();
                                dataRow[5] = xn["XXAMOUNT"].InnerText.ToString().Insert(0, "+");
                                dataRow[6] = xn["XVCHNarration"].InnerText.ToString();
                                dataRow[7] = xn["TALLYVOUCHERTYPENAME"].InnerText.ToString();
                                dataRow[8] = xn["XPARENTTYPE"].InnerText.ToString();

                                TallyData.Rows.InsertAt(dataRow, 0);
                            }
                            else
                            {
                                TallyData.Rows.Add(xn["XGUID"].InnerText.ToString(),
                                                    xn["XVCHVTYP"].InnerText.ToString(),
                                                    xn["strDate"].InnerText.ToString(),
                                                    xn["XVCHNO"].InnerText.ToString(),// + "-" + xn["XGUID"].InnerText.ToString(),
                                                    xn["XLEDGERNAME"].InnerText.ToString(),
                                                    xn["XXAMOUNT"].InnerText.ToString().Insert(0, "+"),
                                                    xn["XVCHNarration"].InnerText.ToString(),
                                                    xn["TALLYVOUCHERTYPENAME"].InnerText.ToString(),
                                                    xn["XPARENTTYPE"].InnerText.ToString()

                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionPaymentVoucherXMPResponseFromTally;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }

            return TallyData;
        }

        public DataTable TallyVoucherLedgerClosingBalance(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();

            try
            {
                doc.LoadXml(xML.LedgerCollectionXMLRequest( CURRENTCOMPANY, BOOKBEGINNINGFROM,   LASTVOUCHERENTRYDATE,   LASTMASTERID,   CURRENTLASTMASTERID,   LASTVOUCHERID,   CURRENTLASTVOUCHERID,   COMPANYINITIALS));

                TallyData.Columns.Add("GUID");
                TallyData.Columns.Add("NAME");
                TallyData.Columns.Add("PARENTTYPE");
                TallyData.Columns.Add("CLOSINGBALANCE");
               
                 


                XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/OBJECT");

 
                if (nodeslIST.Count > 0)
                {

                    foreach (XmlNode xn in nodeslIST)
                    {

                        TallyData.Rows.Add(
                            xn["XGUID"].InnerText.ToString(),
                            xn["XPARTY"].InnerText.ToString(),
                            xn["XPARENTTYPE"].InnerText.ToString(),
                            xn["XCL"].InnerText.ToString()
                          ) ;


                    }

                }

            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionTallyVoucherLedgerClosingBalance;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }

            return TallyData;

        }

        public DataTable TallyActiveCompanyXMLResponse()
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();

            try
            {
                doc.LoadXml(xML.TallyActiveCompanyXMLRequest());

                XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/COMPANY");
                TallyData.Columns.Add("REGISTERED");      //1
                TallyData.Columns.Add("COMPANYINITIALS");//2
                TallyData.Columns.Add("NAME");//3
                TallyData.Columns.Add("GUID");//4
                TallyData.Columns.Add("ENDINGAT");//5
                TallyData.Columns.Add("LASTVOUCHERDATE");//6
                TallyData.Columns.Add("STARTINGFROM");//7
                TallyData.Columns.Add("BOOKSFROM");//8
                TallyData.Columns.Add("PINCODE");//9
                TallyData.Columns.Add("STATENAME");//10
                TallyData.Columns.Add("CMPVCHID");//11
                TallyData.Columns.Add("ALTVCHID");//12
                TallyData.Columns.Add("ALTMSTID");//13
                TallyData.Columns.Add("ALTERID");//14
                TallyData.Columns.Add("MASTERID");//15
                TallyData.Columns.Add("COMPANYNUMBER");//16
                TallyData.Columns.Add("LASTSYNC");//16


                foreach (XmlNode xn in nodeslIST)
                {
                    string CurrentGUID = xn["GUID"].InnerText.ToString();

                    string JSONReponse = aPIConnection.CheckGUIDTrackPayoutWebServer(CurrentGUID);
                    JObject CompanyObject = JObject.Parse(JSONReponse);
                    string companyinitialsTag = (string)CompanyObject.SelectToken("company_id");

                    if (xn.Name != "LASTSYNC")
                    {
                        XmlNode lastsync = doc.CreateElement("LASTSYNC");

                        string rawJSONReponse = aPIConnection.getlastDetailTrackPayoutWebServer(companyinitialsTag);
                        JObject o = JObject.Parse(rawJSONReponse);
                        string lastcreateddate = (string)o.SelectToken("lastSyncDate");

                        if (lastcreateddate == "")
                        {
                            lastsync.InnerText = "-";
                        }
                        else
                        {
                            lastsync.InnerText = lastcreateddate;
                        }

                        xn.AppendChild(lastsync);
                        doc.DocumentElement.AppendChild(xn);
                    }

                    if (xn.Name != "CBLOG")
                    {
                        XmlNode CMPLog = doc.CreateElement("CBLOG");

                      
                        string guidRegisteredflag = (string)CompanyObject.SelectToken("status");

                        if (guidRegisteredflag == "200")
                        {
                            CMPLog.InnerText = "Yes";
                        }
                        else
                        {
                            CMPLog.InnerText = "No";
                        }

                        xn.AppendChild(CMPLog);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "COMPANYINITIALS")
                    {
                        XmlNode CMPinitailsLog = doc.CreateElement("COMPANYINITIALS");


                        if (companyinitialsTag == "")
                        {
                            CMPinitailsLog.InnerText = "-";
                        }
                        else
                        {
                            CMPinitailsLog.InnerText = companyinitialsTag;
                        }

                        xn.AppendChild(CMPinitailsLog);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn["ENDINGAT"].Name == "ENDINGAT")
                    {

                        XmlNode datenew = doc.CreateElement("ENDINGATstrDate");

                        string year = xn["ENDINGAT"].InnerText.ToString().Substring(0, 4);
                        string month = xn["ENDINGAT"].InnerText.ToString().Substring(4, 2);
                        string date1 = xn["ENDINGAT"].InnerText.ToString().Substring(6, 2);

                        string dtstring = date1 + "-" + month + "-" + year;

                        datenew.InnerText = dtstring;
                        xn.AppendChild(datenew);
                        doc.DocumentElement.AppendChild(xn);


                    }

                    if (xn["LASTVOUCHERDATE"].Name == "LASTVOUCHERDATE")
                    {

                        XmlNode datenew = doc.CreateElement("LASTVOUCHERDATEstrDate");

                        string year = xn["LASTVOUCHERDATE"].InnerText.ToString().Substring(0, 4);
                        string month = xn["LASTVOUCHERDATE"].InnerText.ToString().Substring(4, 2);
                        string date1 = xn["LASTVOUCHERDATE"].InnerText.ToString().Substring(6, 2);

                        string dtstring = date1 + "-" + month + "-" + year;

                        datenew.InnerText = dtstring;
                        xn.AppendChild(datenew);
                        doc.DocumentElement.AppendChild(xn);


                    }
                    if (xn["STARTINGFROM"].Name == "STARTINGFROM")
                    {

                        XmlNode datenew = doc.CreateElement("STARTINGFROMstrDate");

                        string year = xn["STARTINGFROM"].InnerText.ToString().Substring(0, 4);
                        string month = xn["STARTINGFROM"].InnerText.ToString().Substring(4, 2);
                        string date1 = xn["STARTINGFROM"].InnerText.ToString().Substring(6, 2);

                        string dtstring = date1 + "-" + month + "-" + year;

                        datenew.InnerText = dtstring;
                        xn.AppendChild(datenew);
                        doc.DocumentElement.AppendChild(xn);


                    }
                    if (xn["BOOKSFROM"].Name == "BOOKSFROM")
                    {

                        XmlNode datenew = doc.CreateElement("BOOKSFROMstrDate");

                        string year = xn["BOOKSFROM"].InnerText.ToString().Substring(0, 4);
                        string month = xn["BOOKSFROM"].InnerText.ToString().Substring(4, 2);
                        string date1 = xn["BOOKSFROM"].InnerText.ToString().Substring(6, 2);

                        string dtstring = date1 + "-" + month + "-" + year;

                        datenew.InnerText = dtstring;
                        xn.AppendChild(datenew);
                        doc.DocumentElement.AppendChild(xn);


                    }
                    if (xn.Name != "PINCODE")
                    {
                        XmlNode narration = doc.CreateElement("PINCODE");
                        narration.InnerText = "Nil";
                        xn.AppendChild(narration);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "STATENAME")
                    {
                        XmlNode narration = doc.CreateElement("STATENAME");
                        narration.InnerText = "Nil";
                        xn.AppendChild(narration);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "LASTVOUCHERDATEstrDate")
                    {
                        XmlNode narration = doc.CreateElement("LASTVOUCHERDATEstrDate");
                        narration.InnerText = "Nil";
                        xn.AppendChild(narration);
                        doc.DocumentElement.AppendChild(xn);

                    }
                    if (xn.Name != "CMPVCHID")
                    {
                        XmlNode narration = doc.CreateElement("CMPVCHID");
                        narration.InnerText = "Nil";
                        xn.AppendChild(narration);
                        doc.DocumentElement.AppendChild(xn);

                    }
                    if (xn.Name != "ALTVCHID")
                    {
                        XmlNode narration = doc.CreateElement("ALTVCHID");
                        narration.InnerText = "Nil";
                        xn.AppendChild(narration);
                        doc.DocumentElement.AppendChild(xn);

                    }

                }

                if (nodeslIST.Count > 0)
                {

                    foreach (XmlNode xn in nodeslIST)
                    {

                        TallyData.Rows.Add(xn["CBLOG"].InnerText.ToString(),
                                            xn["COMPANYINITIALS"].InnerText.ToString(),
                                            xn["NAME"].InnerText.ToString(),
                                            xn["NAME"].InnerText.ToString() + "-" + xn["COMPANYNUMBER"].InnerText.ToString() + "-" + xn["GUID"].InnerText.ToString(),
                                            xn["ENDINGATstrDate"].InnerText.ToString(),
                                            xn["LASTVOUCHERDATEstrDate"].InnerText.ToString(),
                                            xn["STARTINGFROMstrDate"].InnerText.ToString(),
                                            xn["BOOKSFROMstrDate"].InnerText.ToString(),
                                            xn["PINCODE"].InnerText.ToString(),
                                            xn["STATENAME"].InnerText.ToString(),
                                            xn["CMPVCHID"].InnerText.ToString(),
                                            xn["ALTVCHID"].InnerText.ToString(),
                                            xn["ALTMSTID"].InnerText.ToString(),
                                            xn["ALTERID"].InnerText.ToString(),
                                            xn["MASTERID"].InnerText.ToString(),
                                            xn["COMPANYNUMBER"].InnerText.ToString(),
                                            xn["LASTSYNC"].InnerText.ToString()
                            );
                    }

                }


            }
            catch (Exception ex )
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionTallyActiveCompanyXMLResponse;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }

            return TallyData;
        }

        public DataTable TallyCustomerMasterXMLResponse(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string COMPANYINITIALS)
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();

            try
            {
                doc.LoadXml(xML.TallyAlterMasterXMLRequest(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, COMPANYINITIALS));

                TallyData.Columns.Add("GUID");
                TallyData.Columns.Add("CUSTOMERNAME");
                TallyData.Columns.Add("PARENTTYPE");
                TallyData.Columns.Add("ADDRESS");
                TallyData.Columns.Add("STATE");
                TallyData.Columns.Add("PINCODE");
                TallyData.Columns.Add("CONTACT_PERSON");
                TallyData.Columns.Add("PHONE_NO");
                TallyData.Columns.Add("MOBILE_NO");
                TallyData.Columns.Add("EMAIL");
                TallyData.Columns.Add("CC_TO");
                TallyData.Columns.Add("DEFAULT_CREDIT_PERIOD");
                TallyData.Columns.Add("OPENING_BALANCE");
                TallyData.Columns.Add("CLOSING_BALANCE");



                XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/LEDGER");


                foreach (XmlNode xn in nodeslIST)
                {

                    if (xn.Name != "PINCODE")
                    {
                        XmlNode pincode = doc.CreateElement("PINCODE");
                        pincode.InnerText = "Nil";
                        xn.AppendChild(pincode);
                        doc.DocumentElement.AppendChild(xn);

                    }
                    if (xn.Name != "ADDRESS.LIST")
                    {
                        XmlNode ADDRESSList = doc.CreateElement("ADDRESS.LIST");
                        XmlNode ADDRESS = doc.CreateElement("ADDRESS");
                        ADDRESS.InnerText = "Nil";
                        ADDRESSList.AppendChild(ADDRESS);
                        xn.AppendChild(ADDRESSList);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "LEDGERCONTACT")
                    {
                        XmlNode ledgerContact = doc.CreateElement("LEDGERCONTACT");
                        ledgerContact.InnerText = "Nil";
                        xn.AppendChild(ledgerContact);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "LEDGERPHONE")
                    {
                        XmlNode ledgerPhone = doc.CreateElement("LEDGERPHONE");
                        ledgerPhone.InnerText = "Nil";
                        xn.AppendChild(ledgerPhone);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "LEDGERMOBILE")
                    {
                        XmlNode ledgerMobile = doc.CreateElement("LEDGERMOBILE");
                        ledgerMobile.InnerText = "Nil";
                        xn.AppendChild(ledgerMobile);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "EMAIL")
                    {
                        XmlNode ledgerEmail = doc.CreateElement("EMAIL");
                        ledgerEmail.InnerText = "Nil";
                        xn.AppendChild(ledgerEmail);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "EMAILCC")
                    {
                        XmlNode ledgerEmailCC = doc.CreateElement("EMAILCC");
                        ledgerEmailCC.InnerText = "Nil";
                        xn.AppendChild(ledgerEmailCC);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "BILLCREDITPERIOD")
                    {
                        XmlNode ledgerBillCreditPeriod = doc.CreateElement("BILLCREDITPERIOD");
                        ledgerBillCreditPeriod.InnerText = "Nil";
                        xn.AppendChild(ledgerBillCreditPeriod);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "OPENINGBALANCE")
                    {
                        XmlNode ledgerOpeningBalance = doc.CreateElement("OPENINGBALANCE");
                        ledgerOpeningBalance.InnerText = "Nil";
                        xn.AppendChild(ledgerOpeningBalance);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "CLOSINGBALANCE")
                    {
                        XmlNode ledgerClosingBalance = doc.CreateElement("CLOSINGBALANCE");
                        ledgerClosingBalance.InnerText = "Nil";
                        xn.AppendChild(ledgerClosingBalance);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "LEDSTATENAME")
                    {
                        XmlNode ledgerState = doc.CreateElement("LEDSTATENAME");
                        ledgerState.InnerText = "Nil";
                        xn.AppendChild(ledgerState);
                        doc.DocumentElement.AppendChild(xn);

                    }


                }

                if (nodeslIST.Count > 0)
                {

                    foreach (XmlNode xn in nodeslIST)
                    {

                        TallyData.Rows.Add(
                            xn["GUID"].InnerText.ToString(),
                            xn["LANGUAGENAME.LIST"].FirstChild.InnerText.ToString(),
                            xn["PARENTTYPE"].InnerText.ToString(),
                            xn["ADDRESS.LIST"].InnerText.ToString(),
                            xn["LEDSTATENAME"].InnerText.ToString(),
                            xn["PINCODE"].InnerText.ToString(),
                            xn["LEDGERCONTACT"].InnerText.ToString(),
                            xn["LEDGERPHONE"].InnerText.ToString(),
                            xn["LEDGERMOBILE"].InnerText.ToString(),
                            xn["EMAIL"].InnerText.ToString(),
                            xn["EMAILCC"].InnerText.ToString(),
                            xn["BILLCREDITPERIOD"].InnerText.ToString(),
                            xn["OPENINGBALANCE"].InnerText.ToString(),
                            xn["CLOSINGBALANCE"].InnerText.ToString()
                          ) ;


                    }

                }


            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionTallyCustomerMasterXMLResponse;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }

            return TallyData;

        }

        public DataTable TallyStatictisReportXMLResponse(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE)
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();

            try
            {
                doc.LoadXml(xML.TallyStatisticsReportXMLRequest(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE));

                TallyData.Columns.Add("TYPE");
                TallyData.Columns.Add("TOTALVOUCHER");
                TallyData.Columns.Add("TOTALCANCELLEDVOUCHER");



                XmlNodeList nodeslIST1 = doc.GetElementsByTagName("STATNAME");
                XmlNodeList nodeslIST2 = doc.GetElementsByTagName("STATDIRECT");
                XmlNodeList nodeslIST3 = doc.GetElementsByTagName("STATCANCELLED");


                for (int i = 0; i < nodeslIST2.Count; i++)
                {
                    TallyData.Rows.Add(nodeslIST1[i].InnerText.ToString(),
                                        nodeslIST2[i].InnerText.ToString(),
                                        nodeslIST3[i].InnerText.ToString()


                        );
                }


            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionTallyStatictisReportXMLResponse;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }
            return TallyData;
        }

        public DataTable TallyBalanceSheetReportXMLResponse(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE)
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();

            try
            {
                doc.LoadXml(xML.TallyBalanceReportXMLRequest(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE));

                TallyData.Columns.Add("ACCOUNTNAME");
                TallyData.Columns.Add("SUBAMOUNT");
                TallyData.Columns.Add("MAINAMOUNT");

                XmlNodeList nodeslIST1 = doc.GetElementsByTagName("DSPDISPNAME");
                XmlNodeList nodeslIST2 = doc.GetElementsByTagName("BSSUBAMT");
                XmlNodeList nodeslIST3 = doc.GetElementsByTagName("BSMAINAMT");


                for (int i = 0; i < nodeslIST2.Count; i++)
                {
                    TallyData.Rows.Add(nodeslIST1[i].InnerText.ToString(),
                                        nodeslIST2[i].InnerText.ToString(),
                                        nodeslIST3[i].InnerText.ToString()


                        );
                }


            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionTallyBalanceSheetReportXMLResponse;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);

            }

            return TallyData;
        }

        public DataTable TallyActiveCompanyXMLResponseforSignup()
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();

            try
            {
                doc.LoadXml(xML.TallyActiveCompanyXMLRequest());

                XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/COMPANY");
                TallyData.Columns.Add("NAME");
                TallyData.Columns.Add("GUID");
                TallyData.Columns.Add("ENDINGAT");
                TallyData.Columns.Add("LASTVOUCHERDATE");
                TallyData.Columns.Add("STARTINGFROM");
                TallyData.Columns.Add("BOOKSFROM");
                TallyData.Columns.Add("PINCODE");
                TallyData.Columns.Add("STATENAME");
                TallyData.Columns.Add("CMPVCHID");
                TallyData.Columns.Add("ALTVCHID");
                TallyData.Columns.Add("ALTMSTID");
                TallyData.Columns.Add("ALTERID");
                TallyData.Columns.Add("MASTERID");
                TallyData.Columns.Add("COMPANYNUMBER");



                foreach (XmlNode xn in nodeslIST)
                {


                    if (xn["ENDINGAT"].Name == "ENDINGAT")
                    {

                        XmlNode datenew = doc.CreateElement("ENDINGATstrDate");

                        string year = xn["ENDINGAT"].InnerText.ToString().Substring(0, 4);
                        string month = xn["ENDINGAT"].InnerText.ToString().Substring(4, 2);
                        string date1 = xn["ENDINGAT"].InnerText.ToString().Substring(6, 2);

                        string dtstring = date1 + "-" + month + "-" + year;

                        datenew.InnerText = dtstring;
                        xn.AppendChild(datenew);
                        doc.DocumentElement.AppendChild(xn);


                    }

                    if (xn["LASTVOUCHERDATE"].Name == "LASTVOUCHERDATE")
                    {

                        XmlNode datenew = doc.CreateElement("LASTVOUCHERDATEstrDate");

                        string year = xn["LASTVOUCHERDATE"].InnerText.ToString().Substring(0, 4);
                        string month = xn["LASTVOUCHERDATE"].InnerText.ToString().Substring(4, 2);
                        string date1 = xn["LASTVOUCHERDATE"].InnerText.ToString().Substring(6, 2);

                        string dtstring = date1 + "-" + month + "-" + year;

                        datenew.InnerText = dtstring;
                        xn.AppendChild(datenew);
                        doc.DocumentElement.AppendChild(xn);


                    }
                    if (xn["STARTINGFROM"].Name == "STARTINGFROM")
                    {

                        XmlNode datenew = doc.CreateElement("STARTINGFROMstrDate");

                        string year = xn["STARTINGFROM"].InnerText.ToString().Substring(0, 4);
                        string month = xn["STARTINGFROM"].InnerText.ToString().Substring(4, 2);
                        string date1 = xn["STARTINGFROM"].InnerText.ToString().Substring(6, 2);

                        string dtstring = date1 + "-" + month + "-" + year;

                        datenew.InnerText = dtstring;
                        xn.AppendChild(datenew);
                        doc.DocumentElement.AppendChild(xn);


                    }
                    if (xn["BOOKSFROM"].Name == "BOOKSFROM")
                    {

                        XmlNode datenew = doc.CreateElement("BOOKSFROMstrDate");

                        string year = xn["BOOKSFROM"].InnerText.ToString().Substring(0, 4);
                        string month = xn["BOOKSFROM"].InnerText.ToString().Substring(4, 2);
                        string date1 = xn["BOOKSFROM"].InnerText.ToString().Substring(6, 2);

                        string dtstring = date1 + "-" + month + "-" + year;

                        datenew.InnerText = dtstring;
                        xn.AppendChild(datenew);
                        doc.DocumentElement.AppendChild(xn);


                    }
                    if (xn.Name != "PINCODE")
                    {
                        XmlNode narration = doc.CreateElement("PINCODE");
                        narration.InnerText = "Nil";
                        xn.AppendChild(narration);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "STATENAME")
                    {
                        XmlNode narration = doc.CreateElement("STATENAME");
                        narration.InnerText = "Nil";
                        xn.AppendChild(narration);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "LASTVOUCHERDATEstrDate")
                    {
                        XmlNode narration = doc.CreateElement("LASTVOUCHERDATEstrDate");
                        narration.InnerText = "Nil";
                        xn.AppendChild(narration);
                        doc.DocumentElement.AppendChild(xn);

                    }
                    if (xn.Name != "CMPVCHID")
                    {
                        XmlNode narration = doc.CreateElement("CMPVCHID");
                        narration.InnerText = "Nil";
                        xn.AppendChild(narration);
                        doc.DocumentElement.AppendChild(xn);

                    }
                    if (xn.Name != "ALTVCHID")
                    {
                        XmlNode narration = doc.CreateElement("ALTVCHID");
                        narration.InnerText = "Nil";
                        xn.AppendChild(narration);
                        doc.DocumentElement.AppendChild(xn);

                    }

                }

                if (nodeslIST.Count > 0)
                {

                    foreach (XmlNode xn in nodeslIST)
                    {

                        TallyData.Rows.Add(xn["NAME"].InnerText.ToString(),
                                           xn["GUID"].InnerText.ToString(),
                                            xn["ENDINGATstrDate"].InnerText.ToString(),
                                            xn["LASTVOUCHERDATEstrDate"].InnerText.ToString(),
                                            xn["STARTINGFROMstrDate"].InnerText.ToString(),
                                            xn["BOOKSFROMstrDate"].InnerText.ToString(),
                                            xn["PINCODE"].InnerText.ToString(),
                                            xn["STATENAME"].InnerText.ToString(),
                                            xn["CMPVCHID"].InnerText.ToString(),
                                            xn["ALTVCHID"].InnerText.ToString(),
                                            xn["ALTMSTID"].InnerText.ToString(),
                                            xn["ALTERID"].InnerText.ToString(),
                                            xn["MASTERID"].InnerText.ToString(),
                                            xn["COMPANYNUMBER"].InnerText.ToString()
                            );
                    }

                }

            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionTallyBalanceSheetReportXMLResponse;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }

            return TallyData;
        }

        public DataTable TallyStatictisXMLReponse(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE)
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();

            try
            {

                /* DateTime FinacialyearStartdate = Convert.ToDateTime(BOOKBEGINNINGFROM);
                 DateTime FinacialyearEndDate = Convert.ToDateTime(LASTVOUCHERENTRYDATE);

                 TimeSpan timeSpan = FinacialyearEndDate - FinacialyearStartdate;

                 int TotalDays = Int32.Parse(timeSpan.TotalDays.ToString());

                 int TotalMonth = TotalDays / 30;

                 Boolean IsLeapYear = DateTime.IsLeapYear(FinacialyearStartdate.Year);
 */
                TallyData.Columns.Add("COMPANY");
                TallyData.Columns.Add("VOUCHERTYPE");
                TallyData.Columns.Add("TOTALAMOUNT");

                /*  for (int l = 0; l <= TotalMonth; l++)
                  {

                      DateTime Fdate = FinacialyearStartdate.AddMonths(l);
                      DateTime Ldate = Fdate;
                      int getdayinmonth = DateTime.DaysInMonth(Fdate.Year, Fdate.Month);

                      if (getdayinmonth == 30 || getdayinmonth == 31)
                      {
                          Ldate = Fdate.AddDays(getdayinmonth - 1);
                      }
                      if (Fdate.Month == 2)
                      {
                          Ldate = Fdate.AddDays(getdayinmonth - 1);
                      }

                      string FDate = Fdate.ToShortDateString();
                      string LDate = Ldate.ToShortDateString();

  */

                doc.LoadXml(xML.TallyStatisticXMLRequest(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE));

                XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/VOUCHERTYPE");



                foreach (XmlNode xn in nodeslIST)
                {

                    if (xn.Name != "TOTALAMOUNT")
                    {
                        XmlNode narration = doc.CreateElement("TOTALAMOUNT");
                        narration.InnerText = "Nil";
                        xn.AppendChild(narration);
                        doc.DocumentElement.AppendChild(xn);

                    }

                }

                if (nodeslIST.Count > 0)
                {

                    foreach (XmlNode xn in nodeslIST)
                    {
                        /*  if (l > 0)
                          {
                              DataRow dataRow = TallyData.NewRow();
                              dataRow[0] = CURRENTCOMPANY;
                              dataRow[1] = xn["XNAME"].InnerText.ToString();
                              dataRow[2] = xn["TOTALAMT"].InnerText.ToString();

                          }
                          else
                          {*/
                        TallyData.Rows.Add(CURRENTCOMPANY,
                                       xn["XNAME"].InnerText.ToString(),
                                        xn["TOTALAMT"].InnerText.ToString()
                        );
                        /* }*/

                    }

                }

                /* }*/
            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionTallyStatictisXMLReponse;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }

            return TallyData;
        }

        public DataTable TallyCompanyVoucherXMLResponse(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTVOUCHERID, string CURRENTLASTVOUCHERID)
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();

            try
            {



                TallyData.Columns.Add("DATE");
                TallyData.Columns.Add("VOUCHERNUMBER");
                TallyData.Columns.Add("VOUCHERTYPENAME");
                TallyData.Columns.Add("PARTYLEDGERNAME");
                TallyData.Columns.Add("AMOUNT");
                TallyData.Columns.Add("COMPANY");




                doc.LoadXml(xML.TallyCompanyVoucherXMLRequest(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTVOUCHERID, CURRENTLASTVOUCHERID));

                XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/VOUCHER");



                foreach (XmlNode xn in nodeslIST)
                {

                    if (xn.Name != "AMOUNT")
                    {
                        XmlNode narration = doc.CreateElement("TOTALAMOUNT");
                        narration.InnerText = "Nil";
                        xn.AppendChild(narration);
                        doc.DocumentElement.AppendChild(xn);

                    }

                }

                if (nodeslIST.Count > 0)
                {
                    foreach (XmlNode xn in nodeslIST)
                    { 
                        TallyData.Rows.Add(xn["DATE"].InnerText.ToString(),
                                        xn["VOUCHERNUMBER"].InnerText.ToString(),
                                        xn["VOUCHERTYPENAME"].InnerText.ToString(),
                                        xn["PARTYLEDGERNAME"].InnerText.ToString(),
                                        xn["AMOUNT"].InnerText.ToString(),
                                        CURRENTCOMPANY
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionTallyCompanyVoucherXMLResponse;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }

            return TallyData;
        }


        /// <summary>
        /// Track payout Response code 
        /// </summary>
        /// <returns></returns>
        /// 
        public DataTable TPO_SalesVoucherXMPResponseFromTally(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();
            try
            {

                DateTime FinacialyearStartdate = Convert.ToDateTime(BOOKBEGINNINGFROM);
                DateTime FinacialyearEndDate = Convert.ToDateTime(LASTVOUCHERENTRYDATE);

                TimeSpan timeSpan = FinacialyearEndDate - FinacialyearStartdate;

                int TotalDays = Int32.Parse(timeSpan.TotalDays.ToString());

                int TotalMonth = TotalDays / 30;

                Boolean IsLeapYear = DateTime.IsLeapYear(FinacialyearStartdate.Year);

                
                TallyData.Columns.Add("COMPANYID");
                TallyData.Columns.Add("CUSTOMERNAME");
                TallyData.Columns.Add("PARTYNAME");
                TallyData.Columns.Add("PONO");
                TallyData.Columns.Add("INVOICENO");
                TallyData.Columns.Add("BILLREFERENCENO");
                TallyData.Columns.Add("INVOICEDATE");
                TallyData.Columns.Add("INVOICEAMOUNT");
                TallyData.Columns.Add("IGSTAMOUNT");
                TallyData.Columns.Add("SGSTAMOUNT");
                TallyData.Columns.Add("CGSTAMOUNT");
                TallyData.Columns.Add("DESCRIPTION");
                TallyData.Columns.Add("PAYMENTTERM");
                TallyData.Columns.Add("PAYMENTDUEDATE");
                TallyData.Columns.Add("GUID");
                TallyData.Columns.Add("PODate");




                for (int l = 0; l <= TotalMonth; l++)
                {

                    DateTime Fdate = FinacialyearStartdate.AddMonths(l);
                    DateTime Ldate = Fdate;
                    int getdayinmonth = DateTime.DaysInMonth(Fdate.Year, Fdate.Month);

                    if (getdayinmonth == 30 || getdayinmonth == 31)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }
                    if (Fdate.Month == 2)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }

                    string FDate = Fdate.ToShortDateString();
                    string LDate = Ldate.ToShortDateString();



                    doc.LoadXml(xML.TPO_SalesVoucherCollectionXMLRequest(CURRENTCOMPANY, FDate, LDate, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS));



                    XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/VOUCHER");

                    foreach (XmlNode xn in nodeslIST)
                    {
                        if (xn["PAYMENTDUEDATE"].Name == "PAYMENTDUEDATE")
                        {

                            XmlNode datenew = doc.CreateElement("strpaymentdueDate");

                            string year = xn["PAYMENTDUEDATE"].InnerText.ToString().Substring(0, 4);
                            string month = xn["PAYMENTDUEDATE"].InnerText.ToString().Substring(4, 2);
                            string date1 = xn["PAYMENTDUEDATE"].InnerText.ToString().Substring(6, 2);

                            string dtstring = year + "-" + month + "-" + date1;

                            datenew.InnerText = dtstring;
                            xn.AppendChild(datenew);
                            doc.DocumentElement.AppendChild(xn);
                        }


                        //if (xn.Name != "PODATE")
                        //{
                        //    XmlNode PODate = doc.CreateElement("PODATE");
                        //    PODate.InnerText = "NA";
                        //    xn.AppendChild(PODate);
                        //    doc.DocumentElement.AppendChild(xn);
                        //}

                        if (xn.Name != "PONUMBER")
                        {
                            XmlNode PONumber = doc.CreateElement("PONUMBER");
                            PONumber.InnerText = "NA";
                            xn.AppendChild(PONumber);
                            doc.DocumentElement.AppendChild(xn);
                        }

                        if (xn.Name != "BILLREFERENCENO")
                        {
                            XmlNode Reference = doc.CreateElement("BILLREFERENCENO");
                            Reference.InnerText = "NA";
                            xn.AppendChild(Reference);
                            doc.DocumentElement.AppendChild(xn);
                        }



                        if (xn.Name != "AMOUNT")
                        {
                            XmlNode amount = doc.CreateElement("AMOUNT");
                            amount.InnerText = "0";
                            xn.AppendChild(amount);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "VOUCHERNUMBER")
                        {
                            XmlNode vouchernumber = doc.CreateElement("VOUCHERNUMBER");
                            vouchernumber.InnerText = "0";
                            xn.AppendChild(vouchernumber);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "DATE")
                        {
                            XmlNode date = doc.CreateElement("DATE");
                            date.InnerText = "0";
                            xn.AppendChild(date);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "NARRATION")
                        {
                            XmlNode narration = doc.CreateElement("NARRATION");
                            narration.InnerText = "0";
                            xn.AppendChild(narration);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn["DATE"].Name == "DATE")
                        {

                            XmlNode datenew = doc.CreateElement("strDate");

                            string year = xn["DATE"].InnerText.ToString().Substring(0, 4);
                            string month = xn["DATE"].InnerText.ToString().Substring(4, 2);
                            string date1 = xn["DATE"].InnerText.ToString().Substring(6, 2);

                            string dtstring = year + "-" + month + "-" + date1;

                            datenew.InnerText = dtstring;
                            xn.AppendChild(datenew);
                            doc.DocumentElement.AppendChild(xn);


                        }

                        if (xn["PODATE"].InnerText != "null")
                        {

                            XmlNode datenew = doc.CreateElement("strPODate");

                            string year = xn["PODATE"].InnerText.ToString().Substring(0, 4);
                            string month = xn["PODATE"].InnerText.ToString().Substring(4, 2);
                            string date1 = xn["PODATE"].InnerText.ToString().Substring(6, 2);

                            string dtstring = year + "-" + month + "-" + date1;

                            datenew.InnerText = dtstring;
                            xn.AppendChild(datenew);
                            doc.DocumentElement.AppendChild(xn);


                        }
                        else
                        {

                            XmlNode datenew = doc.CreateElement("strPODate");

                            string year = DateTime.Now.Year.ToString();
                            string month = DateTime.Now.Month.ToString();
                            string date1 = DateTime.Now.Day.ToString();

                            string dtstring = year + "-" + month + "-" + date1;

                            datenew.InnerText = dtstring;
                            xn.AppendChild(datenew);
                            doc.DocumentElement.AppendChild(xn);


                        }

                    }

                    if (nodeslIST.Count > 0)
                    {
                        foreach (XmlNode xn in nodeslIST)
                        {
                            if (l > 0)
                            {
                                DataRow dataRow = TallyData.NewRow();
                                
                                dataRow[0] = xn["COMPANYID"].InnerText.ToString();
                                dataRow[1] = xn["CUSTOMERID"].InnerText.ToString();
                                dataRow[2] = xn["PARTYLEDGERNAME"].InnerText.ToString();
                                dataRow[3] = xn["PONUMBER"].InnerText.ToString();
                                dataRow[4] = xn["VOUCHERNUMBER"].InnerText.ToString();
                                dataRow[5] = xn["BILLREFERENCENO"].InnerText.ToString();
                                dataRow[6] = xn["strDate"].InnerText.ToString();
                                dataRow[7] = xn["TAXABLEAMT"].InnerText.ToString().Remove(0, 1);
                                dataRow[8] = xn["IGSTAMT"].InnerText.ToString();
                                dataRow[9] = xn["SGSTAMT"].InnerText.ToString();
                                dataRow[10] = xn["CGSTAMT"].InnerText.ToString();
                                dataRow[11] = xn["DESCRIPTION"].InnerText.ToString();
                                dataRow[12] = xn["PAYMENTTERM"].InnerText.TrimStart().ToString();
                                dataRow[13] = xn["strpaymentdueDate"].InnerText.ToString();
                                dataRow[14] = xn["GUID"].InnerText.ToString();
                                dataRow[15] = xn["strPODate"].InnerText.ToString();

                                TallyData.Rows.InsertAt(dataRow, 0);
                            }
                            else
                            {
                                TallyData.Rows.Add(
                                                xn["COMPANYID"].InnerText.ToString(),
                                                xn["CUSTOMERID"].InnerText.ToString(),
                                                xn["PARTYLEDGERNAME"].InnerText.ToString(),
                                                xn["PONUMBER"].InnerText.ToString(),
                                                xn["VOUCHERNUMBER"].InnerText.ToString(),// + "-" + xn["GUID"].InnerText.ToString(),
                                                xn["BILLREFERENCENO"].InnerText.ToString(),
                                                xn["strDate"].InnerText.ToString(),
                                                xn["TAXABLEAMT"].InnerText.ToString().Remove(0, 1),
                                                xn["IGSTAMT"].InnerText.ToString(),
                                                xn["SGSTAMT"].InnerText.ToString(),
                                                xn["CGSTAMT"].InnerText.ToString(),
                                                xn["DESCRIPTION"].InnerText.ToString(),
                                                xn["PAYMENTTERM"].InnerText.TrimStart().ToString(),
                                                xn["strpaymentdueDate"].InnerText.ToString(),
                                                xn["GUID"].InnerText.ToString(),
                                                xn["strPODate"].InnerText.ToString()
                                );
                            }
                        }   
                                  
                    }
                }

            }
            catch (Exception ex)
            {


            }

            return TallyData;
            
        }

        public DataTable TOP_TallyCustomerMasterXMLResponse(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string COMPANYINITIALS)
        {
            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();


            try
            {
                doc.LoadXml(xML.TallyAlterMasterXMLRequest(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, COMPANYINITIALS));

                TallyData.Columns.Add("GUID");
                TallyData.Columns.Add("CUSTOMERNAME");
                TallyData.Columns.Add("PARENTTYPE");
                TallyData.Columns.Add("ADDRESS");
                TallyData.Columns.Add("STATE");
                TallyData.Columns.Add("COUNTRY");
                TallyData.Columns.Add("PINCODE");
                TallyData.Columns.Add("CONTACT_PERSON");
                TallyData.Columns.Add("PHONE_NO");
                TallyData.Columns.Add("MOBILE_NO");
                TallyData.Columns.Add("EMAIL");
                TallyData.Columns.Add("PARTYGSTIN");
                TallyData.Columns.Add("DEFAULT_CREDIT_PERIOD");


                


                XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/LEDGER");


                foreach (XmlNode xn in nodeslIST)
                {

                    if (xn.Name != "PINCODE")
                    {
                        XmlNode pincode = doc.CreateElement("PINCODE");
                        pincode.InnerText = "NA";
                        xn.AppendChild(pincode);
                        doc.DocumentElement.AppendChild(xn);

                    }
                    if (xn.Name != "ADDRESS.LIST")
                    {
                        XmlNode ADDRESSList = doc.CreateElement("ADDRESS.LIST");
                        XmlNode ADDRESS = doc.CreateElement("ADDRESS");
                        ADDRESS.InnerText = "NA";
                        ADDRESSList.AppendChild(ADDRESS);
                        xn.AppendChild(ADDRESSList);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "LEDGERCONTACT")
                    {
                        XmlNode ledgerContact = doc.CreateElement("LEDGERCONTACT");
                        ledgerContact.InnerText = "NA";
                        xn.AppendChild(ledgerContact);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "LEDGERPHONE")
                    {
                        XmlNode ledgerPhone = doc.CreateElement("LEDGERPHONE");
                        ledgerPhone.InnerText = "NA";
                        xn.AppendChild(ledgerPhone);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "LEDGERMOBILE")
                    {
                        XmlNode ledgerMobile = doc.CreateElement("LEDGERMOBILE");
                        ledgerMobile.InnerText = "NA";
                        xn.AppendChild(ledgerMobile);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "EMAIL")
                    {
                        XmlNode ledgerEmail = doc.CreateElement("EMAIL");
                        ledgerEmail.InnerText = "NA";
                        xn.AppendChild(ledgerEmail);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "EMAILCC")
                    {
                        XmlNode ledgerEmailCC = doc.CreateElement("EMAILCC");
                        ledgerEmailCC.InnerText = "NA";
                        xn.AppendChild(ledgerEmailCC);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "PAYMENTTERM")
                    {
                        XmlNode ledgerBillCreditPeriod = doc.CreateElement("PAYMENTTERM");
                        ledgerBillCreditPeriod.InnerText = "NA";
                        xn.AppendChild(ledgerBillCreditPeriod);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "OPENINGBALANCE")
                    {
                        XmlNode ledgerOpeningBalance = doc.CreateElement("OPENINGBALANCE");
                        ledgerOpeningBalance.InnerText = "Nil";
                        xn.AppendChild(ledgerOpeningBalance);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "CLOSINGBALANCE")
                    {
                        XmlNode ledgerClosingBalance = doc.CreateElement("CLOSINGBALANCE");
                        ledgerClosingBalance.InnerText = "NA";
                        xn.AppendChild(ledgerClosingBalance);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "LEDSTATENAME")
                    {
                        XmlNode ledgerState = doc.CreateElement("LEDSTATENAME");
                        ledgerState.InnerText = "NA";
                        xn.AppendChild(ledgerState);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "PARTYGSTIN")
                    {
                        XmlNode ledgerState = doc.CreateElement("PARTYGSTIN");
                        ledgerState.InnerText = "NA";
                        xn.AppendChild(ledgerState);
                        doc.DocumentElement.AppendChild(xn);

                    }

                    if (xn.Name != "COUNTRYNAME")
                    {
                        XmlNode ledgerState = doc.CreateElement("COUNTRYNAME");
                        ledgerState.InnerText = "NA";
                        xn.AppendChild(ledgerState);
                        doc.DocumentElement.AppendChild(xn);

                    }




                }

                if (nodeslIST.Count > 0)
                {

                    foreach (XmlNode xn in nodeslIST)
                    {

                        TallyData.Rows.Add(
                            xn["GUID"].InnerText.ToString(),
                            xn["LANGUAGENAME.LIST"].FirstChild.InnerText.ToString(),
                            xn["PARENTTYPE"].InnerText.ToString(),
                            xn["ADDRESS.LIST"].InnerText.ToString(),
                            xn["LEDSTATENAME"].InnerText.ToString(),
                            xn["COUNTRYNAME"].InnerText.ToString(),
                            xn["PINCODE"].InnerText.ToString(),
                            xn["LEDGERCONTACT"].InnerText.ToString(),
                            xn["LEDGERPHONE"].InnerText.ToString(),
                            xn["LEDGERMOBILE"].InnerText.ToString(),
                            xn["EMAIL"].InnerText.ToString(),
                            xn["PARTYGSTIN"].InnerText.ToString(),
                            xn["PAYMENTTERM"].InnerText.TrimStart().ToString()
                          );

                    }

                }


            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionTallyCustomerMasterXMLResponse;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }

            return TallyData;

        }


        public DataTable TPO_ReceiptVoucherXMPResponseFromTally(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {

            XmlDocument doc = new XmlDocument();
            DataTable TallyData = new DataTable();
            try
            {
                DateTime FinacialyearStartdate = Convert.ToDateTime(BOOKBEGINNINGFROM);
                DateTime FinacialyearEndDate = Convert.ToDateTime(LASTVOUCHERENTRYDATE);

                TimeSpan timeSpan = FinacialyearEndDate - FinacialyearStartdate;

                int TotalDays = Int32.Parse(timeSpan.TotalDays.ToString());

                int TotalMonth = TotalDays / 30;

                Boolean IsLeapYear = DateTime.IsLeapYear(FinacialyearStartdate.Year);

                TallyData.Columns.Add("GUID");
                TallyData.Columns.Add("VOUCHERTYPENAME");
                TallyData.Columns.Add("DATE");
                TallyData.Columns.Add("VOUCHERNUMBER");
                TallyData.Columns.Add("PARTYNAME");
                TallyData.Columns.Add("INVOICENO");
                TallyData.Columns.Add("AMOUNT");
                TallyData.Columns.Add("NARRATION");
                TallyData.Columns.Add("TALLYVOUCHERTYPENAME");
                TallyData.Columns.Add("PARENTTYPE");
                



                for (int l = 0; l <= TotalMonth; l++)
                {

                    DateTime Fdate = FinacialyearStartdate.AddMonths(l);
                    DateTime Ldate = Fdate;
                    int getdayinmonth = DateTime.DaysInMonth(Fdate.Year, Fdate.Month);

                    if (getdayinmonth == 30 || getdayinmonth == 31)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }
                    if (Fdate.Month == 2)
                    {
                        Ldate = Fdate.AddDays(getdayinmonth - 1);
                    }

                    string FDate = Fdate.ToShortDateString();
                    string LDate = Ldate.ToShortDateString();


                    doc.LoadXml(xML.TPO_RecipetvoucherCollectionXMLRequest(CURRENTCOMPANY, FDate, LDate, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS));



                    XmlNodeList nodeslIST = doc.SelectNodes("/ENVELOPE/BODY/DATA/COLLECTION/BILLALLOCATIONS");



                    foreach (XmlNode xn in nodeslIST)
                    {

                        if (xn.Name != "TALLYVOUCHERTYPENAME")
                        {
                            XmlNode tallyvouchertype = doc.CreateElement("TALLYVOUCHERTYPENAME");
                            tallyvouchertype.InnerText = "Receipt";
                            xn.AppendChild(tallyvouchertype);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "XINVOICENO")
                        {
                            XmlNode invoiceNo = doc.CreateElement("XINVOICENO");
                            invoiceNo.InnerText = "NA";
                            xn.AppendChild(invoiceNo);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "XAMOUNT")
                        {
                            XmlNode amount = doc.CreateElement("XAMOUNT");
                            amount.InnerText = "0";
                            xn.AppendChild(amount);
                            doc.DocumentElement.AppendChild(xn);

                        }

                        if (xn.Name != "XVCHNO")
                        {
                            XmlNode vouchernumber = doc.CreateElement("XVCHNO");
                            vouchernumber.InnerText = "0";
                            xn.AppendChild(vouchernumber);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "XVCHDT")
                        {
                            XmlNode date = doc.CreateElement("XVCHDT");
                            date.InnerText = "0";
                            xn.AppendChild(date);
                            doc.DocumentElement.AppendChild(xn);

                        }



                        if (xn.Name != "XLEDGERNAME")
                        {
                            XmlNode narration = doc.CreateElement("XLEDGERNAME");
                            narration.InnerText = "0";
                            xn.AppendChild(narration);
                            doc.DocumentElement.AppendChild(xn);

                        }
                        if (xn.Name != "XVCHNarration")
                        {
                            XmlNode narration = doc.CreateElement("XVCHNarration");
                            narration.InnerText = "NA";
                            xn.AppendChild(narration);
                            doc.DocumentElement.AppendChild(xn);
                        }

                        if (xn["XVCHDT"].Name == "XVCHDT")
                        {

                            XmlNode datenew = doc.CreateElement("strDate");

                            string year = xn["XVCHDT"].InnerText.ToString().Substring(0, 4);
                            string month = xn["XVCHDT"].InnerText.ToString().Substring(4, 2);
                            string date1 = xn["XVCHDT"].InnerText.ToString().Substring(6, 2);

                            string dtstring = year + "-" + month + "-" + date1;

                            datenew.InnerText = dtstring;
                            xn.AppendChild(datenew);
                            doc.DocumentElement.AppendChild(xn);


                        }

                    }

                    if (nodeslIST.Count > 0)
                    {

                        foreach (XmlNode xn in nodeslIST)
                        {
                            if (l > 0)
                            {
                                DataRow dataRow = TallyData.NewRow();
                                dataRow[0] = xn["XGUID"].InnerText.ToString();
                                dataRow[1] = xn["XVCHVTYP"].InnerText.ToString();
                                dataRow[2] = xn["strDate"].InnerText.ToString();
                                dataRow[3] = xn["XVCHNO"].InnerText.ToString();
                                dataRow[4] = xn["XLEDGERNAME"].InnerText.ToString();
                                dataRow[5] = xn["XINVOICENO"].InnerText.ToString();
                                dataRow[6] = xn["XAMOUNT"].InnerText.ToString();
                                dataRow[7] = xn["XVCHNarration"].InnerText.ToString();
                                dataRow[8] = xn["TALLYVOUCHERTYPENAME"].InnerText.ToString();
                                dataRow[9] = xn["XPARENTTYPE"].InnerText.ToString();

                                TallyData.Rows.InsertAt(dataRow, 0);
                            }
                            else
                            {
                                TallyData.Rows.Add(xn["XGUID"].InnerText.ToString(),
                                                    xn["XVCHVTYP"].InnerText.ToString(),
                                                    xn["strDate"].InnerText.ToString(),
                                                    xn["XVCHNO"].InnerText.ToString(),
                                                    xn["XLEDGERNAME"].InnerText.ToString(),
                                                    xn["XINVOICENO"].InnerText.ToString(),
                                                    xn["XAMOUNT"].InnerText.ToString(),
                                                    xn["XVCHNarration"].InnerText.ToString(),
                                                    xn["TALLYVOUCHERTYPENAME"].InnerText.ToString(),
                                                    xn["XPARENTTYPE"].InnerText.ToString()
                                );
                            }


                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorCodeForResponseModelEnum errorCodeFunc = ErrorCodeForResponseModelEnum.FunctionReceiptVoucherXMPResponseFromTally;
                logger.Log(errorCodeFunc + " : " + ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7) + ", " + ex.Message);
            }

            return TallyData;
        }

    }
}
