using CB_TallyConnector.Connection;
using CB_TallyConnector.Log;
using System;

namespace CB_TallyConnector.XMLRequestConnection
{
    public class XMLRequest
    {
        APIConnection aPI = new APIConnection();

        Logger logger = new Logger();

        enum ErrorCodeForRequestModelEnum
        {
            FunctionListofCompanyCollectionXMLRequest,
            FunctionLedgerCollectionXMLRequest,
            FunctionSalesVoucherCollectionXMLRequest,
            FunctionPurchaseVoucherCollectionXMLRequest,
            FunctionDebitNoteVoucherCollectionXMLRequest,
            FunctionCreditNoteVoucherCollectionXMLRequest,
            FunctionJournalInvoiceVoucherCollectionXMLRequest,
            FunctionJournalReceiptVoucherCollectionXMLRequest,
            FunctionRecipetvoucherCollectionXMLRequest,
            FunctionPaymentvoucherCollectionXMLRequest,
            FunctionTallyLicenseInfoXMLRequest,
            FunctionTallyActiveCompanyXMLRequest,
            FunctionTallyAlterMasterXMLRequest,
            FunctionTallyStatisticsReportXMLRequest,
            FunctionTallyBalanceReportXMLRequest,
            FunctionTallyStatisticXMLRequest,
            FunctionTallyCompanyVoucherXMLRequest,
            FunctionTallyTrialBalanceXMLRequest,
            Functiontpo_RecipetvoucherCollectionXMLRequest

        }


        /// <summary>
        /// Clear Balance Request for Tally ERP 9
        /// </summary>
        /// <returns></returns>
        public string ListofCompanyCollectionXMLRequest()
        {
            String xmlstc = "";
            String lVocuherResponse = "";
            String lVocuherResponseAfterUDFRemove = "";

            try
            {
                xmlstc = "<ENVELOPE>\r\n";
                xmlstc = xmlstc + "<HEADER>\r\n";
                xmlstc = xmlstc + "<VERSION>1</VERSION>\r\n";
                xmlstc = xmlstc + "<TALLYREQUEST>Export</TALLYREQUEST>\r\n";
                xmlstc = xmlstc + "<TYPE>Collection</TYPE>\r\n";
                xmlstc = xmlstc + "<ID>CollectionofCompany</ID>\r\n";
                xmlstc = xmlstc + "</HEADER>\r\n";
                xmlstc = xmlstc + "<BODY>\r\n";
                xmlstc = xmlstc + "<DESC>\r\n";
                xmlstc = xmlstc + "<TDL>\r\n";
                xmlstc = xmlstc + "<TDLMESSAGE>\r\n";
                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofCompany' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<TYPE>Company</TYPE>\r\n";
                xmlstc = xmlstc + "<Fetch>NAME,COMPANYNUMBER,STARTINGFROM, BOOKSFROM,GUID</Fetch>\r\n";
                xmlstc = xmlstc + "<Compute>CurrentDt : $$MachineDate</Compute>\r\n";
                xmlstc = xmlstc + "<Compute>CurrentCompany : $GUID:Company:##SVCurrentCompany</Compute>\r\n";
                xmlstc = xmlstc + "<FILTER>IsPrimaryCompanyFilterNew</FILTER>\r\n";
                xmlstc = xmlstc + "<FETCH>Name,StateName,PinCode,PhoneNumber,Email</FETCH>\r\n";  /////
                xmlstc = xmlstc + "</COLLECTION>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'IsPrimaryCompanyFilterNew'>$$IsPrimaryCompany</SYSTEM>\r\n";
                xmlstc = xmlstc + "</TDLMESSAGE>\r\n";
                xmlstc = xmlstc + "</TDL>\r\n";
                xmlstc = xmlstc + "</DESC>\r\n";
                xmlstc = xmlstc + "</BODY>\r\n";
                xmlstc = xmlstc + "</ENVELOPE>\r\n";

                String xml = xmlstc;
                lVocuherResponse = aPI.TallySendReqst(xml);
                lVocuherResponseAfterUDFRemove = lVocuherResponse.Replace("UDF:", "");

            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionListofCompanyCollectionXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }
               
          
            return lVocuherResponseAfterUDFRemove;

        }

        public string LedgerCollectionXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {
             
            String lVocuherResponseAfterUDFRemove = "";

            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;
            string lastVchAlterID = LASTVOUCHERID;
            string currentlastVchAlterID = CURRENTLASTVOUCHERID;

            try
            {
                String xml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>EXPORT</TALLYREQUEST><TYPE>Collection</TYPE><ID>COllectionofBothNonAcctAndAcct</ID></HEADER><BODY><Desc><STATICVARIABLES><SVFROMDATE>" + SVFromDate + "</SVFROMDATE><SVTODATE>" + SVToDate + "</SVTODATE><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY> " + SVCURRENTCOMPANY + " </SVCURRENTCOMPANY></STATICVARIABLES><TDL><TDLMESSAGE><COLLECTION NAME = 'CollectionofLedgerForVouchersrc' ISMODIFY='NO'><TYPE>VOUCHERS</TYPE><FETCH>AlterID, VoucherNumber, PartyLedgerName, Amount, Date</FETCH><FILTER>GetFiltertheReport1</FILTER></COLLECTION><COLLECTION Name = 'CollectionofLedgerForVoucherAcct' ISMODIFY ='No'><SourceCollection>CollectionofLedgerForVouchersrc</SourceCollection><WALK>LEDGERENTRIES</WALK><BY>XParty		: $LedgerName </BY><BY>XGUID		: $GUID:Ledger:$LedgerName </BY>	<COMPUTE>XPARENTTYPE: IF ($$IsObjectBelongsTo:Group:($Parent:Ledger:$LedgerName):$$GroupSundryCreditors) THEN 'CREDITORS' ELSE 'DEBTORS'</COMPUTE><COMPUTE>XCL	: $CLOSINGBALANCE:LEDGER:$LedgerName</COMPUTE><Filter>FilterBothSCSDLed</Filter></COLLECTION><COLLECTION Name = 'CollectionofLedgerForVoucherNonAcct' ISMODIFY ='No'><SourceCollection>CollectionofLedgerForVouchersrc</SourceCollection><BY>XParty		: $$Owner:$PartyLedgerName </BY><BY>XGUID		: $GUID:Ledger:$PartyLedgerName </BY>	<COMPUTE>XPARENTTYPE: IF ($$IsObjectBelongsTo:Group:($Parent:Ledger:$LedgerName):$$GroupSundryCreditors) THEN 'CREDITORS' ELSE 'DEBTORS'</COMPUTE><COMPUTE>XCL	: $CLOSINGBALANCE:LEDGER:$PartyLedgerName</COMPUTE><Filter>FilterBothSCSDLed</Filter></COLLECTION><COLLECTION Name = 'COllectionofBothNonAcctAndAcct' ISMODIFY='No'><COLLECTION>CollectionofLedgerForVoucherAcct, CollectionofLedgerForVoucherNonAcct</COLLECTION></COLLECTION><SYSTEM TYPE =  'Formulae'  Name = 'FilterBothSCSDLed'> @@FilterSCVchLed or @@FilterSDVchLed</SYSTEM><SYSTEM TYPE =  'Formulae'  Name = 'FilterSCVchLed'> ($$IsObjectBelongsTo:Group:($Parent:Ledger:$XParty):$$GroupSundryCreditors)</SYSTEM><SYSTEM TYPE =  'Formulae'  Name = 'FilterSDVchLed'> ($$IsObjectBelongsTo:Group:($Parent:Ledger:$XParty):$$GroupSundryDebtors)</SYSTEM><SYSTEM TYPE =  'Formulae'  Name = 'GetFiltertheReport1'> Not ($IsOptional or $IsCancelled) And ( $Date &gt;= $$Date:'" + SVFromDate + "' and $Date  &lt;= $$Date:'" + SVToDate + "') And $alterid &gt; " + lastVchAlterID + " AND $alterid &lt;= " + currentlastVchAlterID + "</SYSTEM></TDLMESSAGE></TDL></Desc></BODY></ENVELOPE>";
                String lVocuherResponse = aPI.TallySendReqst(xml);
                lVocuherResponseAfterUDFRemove = lVocuherResponse.Replace("UDF:", "");
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionLedgerCollectionXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lVocuherResponseAfterUDFRemove;
        }

        public string SalesVoucherCollectionXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {

            string xmlstc = "";
            string lVocuherResponseAfterUDFRemove = "";

            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;
            string lastVchAlterID = LASTVOUCHERID;
            string currentlastVchAlterID = CURRENTLASTVOUCHERID;



            try
            {
                if (lastVchAlterID == "0")
                {
                    lastVchAlterID = "-1";
                }

                xmlstc = "<ENVELOPE>\r\n";
                xmlstc = xmlstc + "<HEADER>\r\n";
                xmlstc = xmlstc + "<VERSION>1</VERSION>\r\n";
                xmlstc = xmlstc + "<TALLYREQUEST>Export</TALLYREQUEST>\r\n";
                xmlstc = xmlstc + "<TYPE>Collection</TYPE>\r\n";
                xmlstc = xmlstc + "<ID>CollectionofSalesVoucher</ID>\r\n";
                xmlstc = xmlstc + "</HEADER>\r\n";
                xmlstc = xmlstc + "<BODY>\r\n";
                xmlstc = xmlstc + "<DESC>\r\n";
                xmlstc = xmlstc + "<STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<SVFROMDATE>"+ SVFromDate + "</SVFROMDATE>\r\n";
                xmlstc = xmlstc + "<SVTODATE>"+SVToDate + "</SVTODATE>\r\n";
                xmlstc = xmlstc + "<SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT>\r\n";
                xmlstc = xmlstc + "<SVCURRENTCOMPANY> "+ SVCURRENTCOMPANY + " </SVCURRENTCOMPANY>\r\n";
                xmlstc = xmlstc + "</STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<TDL>\r\n";
                xmlstc = xmlstc + "<TDLMESSAGE>\r\n";

                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofSalesVoucherDEBTORS' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<TYPE>Vouchers: VoucherType</TYPE>\r\n";
                xmlstc = xmlstc + "<ChildOf>$$VchTypeSales</ChildOf>\r\n";
                xmlstc = xmlstc + "<BelongsTo>Yes</BelongsTo>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryDebtorParty,GetFiltertheReport1</FILTER>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,IsOptional,IsCancelled</FETCH>\r\n";
                xmlstc = xmlstc + "<Compute> XGUIDParty	: $GUID:Ledger:$PartyLedgerName</Compute> \r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  'DEBTORS'</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";

                /*    xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofSalesVoucherCREDITORS' ISMODIFY='No'>\r\n";
                    xmlstc = xmlstc + "<TYPE>Vouchers: VoucherType</TYPE>\r\n";
                    xmlstc = xmlstc + "<ChildOf>$$VchTypeSales</ChildOf>\r\n";
                    xmlstc = xmlstc + "<BelongsTo>Yes</BelongsTo>\r\n";
                    xmlstc = xmlstc + "<FILTER>FilterSundryCreditorParty,GetFiltertheReport1</FILTER>\r\n";
                    xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,IsOptional,IsCancelled</FETCH>\r\n";
                    xmlstc = xmlstc + "<Compute> XGUIDParty	: $GUID:Ledger:$PartyLedgerName</Compute> \r\n";
                    xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  'CREDITORS'</COMPUTE>\r\n";
                    xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                    xmlstc = xmlstc + "</COLLECTION>\r\n";

                    CollectionofSalesVoucherCREDITORS,
    */
                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofSalesVoucher' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<COLLECTION>CollectionofSalesVoucherDEBTORS </COLLECTION>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,IsOptional,IsCancelled</FETCH>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";

              //  xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryCreditorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$PartyLedgerName):$$GroupSundryCreditors) </SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryDebtorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$PartyLedgerName):$$GroupSundryDebtors) </SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'GetFiltertheReport1'> Not ($IsOptional or $IsCancelled) And ( $Date &gt;= $$Date:'" + SVFromDate + "' and $Date  &lt;= $$Date:'" + SVToDate + "') And $alterid &gt; " + lastVchAlterID + " AND $alterid &lt;= " + currentlastVchAlterID + "</SYSTEM>\r\n";
                xmlstc = xmlstc + "</TDLMESSAGE>\r\n";
                xmlstc = xmlstc + "</TDL>\r\n";
                xmlstc = xmlstc + "</DESC>\r\n";
                xmlstc = xmlstc + "</BODY>\r\n";
                xmlstc = xmlstc + "</ENVELOPE>\r\n";
                String xml = xmlstc;
                String lVocuherResponse = aPI.TallySendReqst(xml);
                lVocuherResponseAfterUDFRemove = lVocuherResponse.Replace("UDF:", "");
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionSalesVoucherCollectionXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }
           
            return lVocuherResponseAfterUDFRemove;
        }

        public string PurchaseVoucherCollectionXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {

            string xmlstc = "";
            string lVocuherResponseAfterUDFRemove = "";

            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;
            string lastVchAlterID = LASTVOUCHERID;
            string currentlastVchAlterID = CURRENTLASTVOUCHERID;

            try
            {
                if (lastVchAlterID == "0")
                {
                    lastVchAlterID = "-1";
                }

                xmlstc = "<ENVELOPE>\r\n";
                xmlstc = xmlstc + "<HEADER>\r\n";
                xmlstc = xmlstc + "<VERSION>1</VERSION>\r\n";
                xmlstc = xmlstc + "<TALLYREQUEST>Export</TALLYREQUEST>\r\n";
                xmlstc = xmlstc + "<TYPE>Collection</TYPE>\r\n";
                xmlstc = xmlstc + "<ID>CollectionofSalesVoucher</ID>\r\n";
                xmlstc = xmlstc + "</HEADER>\r\n";
                xmlstc = xmlstc + "<BODY>\r\n";
                xmlstc = xmlstc + "<DESC>\r\n";
                xmlstc = xmlstc + "<STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<SVFROMDATE>" + SVFromDate + "</SVFROMDATE>\r\n";
                xmlstc = xmlstc + "<SVTODATE>" + SVToDate + "</SVTODATE>\r\n";
                xmlstc = xmlstc + "<SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT>\r\n";
                xmlstc = xmlstc + "<SVCURRENTCOMPANY> " + SVCURRENTCOMPANY + " </SVCURRENTCOMPANY>\r\n";
                xmlstc = xmlstc + "</STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<TDL>\r\n";
                xmlstc = xmlstc + "<TDLMESSAGE>\r\n";

                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofSalesVoucherDEBTORS' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<TYPE>Vouchers: VoucherType</TYPE>\r\n";
                xmlstc = xmlstc + "<ChildOf>$$VchTypePurchase</ChildOf>\r\n";
                xmlstc = xmlstc + "<BelongsTo>Yes</BelongsTo>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryDebtorParty,GetFiltertheReport1</FILTER>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,IsOptional,IsCancelled</FETCH>\r\n";
                xmlstc = xmlstc + "<Compute> XGUIDParty	: $GUID:Ledger:$PartyLedgerName</Compute> \r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  'DEBTORS'</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";

                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofSalesVoucherCREDITORS' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<TYPE>Vouchers: VoucherType</TYPE>\r\n";
                xmlstc = xmlstc + "<ChildOf>$$VchTypePurchase</ChildOf>\r\n";
                xmlstc = xmlstc + "<BelongsTo>Yes</BelongsTo>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryCreditorParty,GetFiltertheReport1</FILTER>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,IsOptional,IsCancelled</FETCH>\r\n";
                xmlstc = xmlstc + "<Compute> XGUIDParty	: $GUID:Ledger:$PartyLedgerName</Compute> \r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  'CREDITORS'</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n"; 

                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofSalesVoucher' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<COLLECTION>CollectionofSalesVoucherCREDITORS,CollectionofSalesVoucherDEBTORS </COLLECTION>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,IsOptional,IsCancelled</FETCH>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";

                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryCreditorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$PartyLedgerName):$$GroupSundryCreditors) </SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryDebtorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$PartyLedgerName):$$GroupSundryDebtors) </SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'GetFiltertheReport1'>Not ($IsOptional or $IsCancelled) And ($Date &gt;= $$Date:'" + SVFromDate + "' and $Date  &lt;= $$Date:'" + SVToDate + "') And $alterid &gt; " + lastVchAlterID + " AND $alterid &lt;= " + currentlastVchAlterID + "</SYSTEM>\r\n";

                xmlstc = xmlstc + "</TDLMESSAGE>\r\n";
                xmlstc = xmlstc + "</TDL>\r\n";
                xmlstc = xmlstc + "</DESC>\r\n";
                xmlstc = xmlstc + "</BODY>\r\n";
                xmlstc = xmlstc + "</ENVELOPE>\r\n";

                String xml = xmlstc;
                String lVocuherResponse = aPI.TallySendReqst(xml);
                lVocuherResponseAfterUDFRemove = lVocuherResponse.Replace("UDF:", "");
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionPurchaseVoucherCollectionXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lVocuherResponseAfterUDFRemove;
        }

        public string DebitNoteVoucherCollectionXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {

            string xmlstc = "";
            string lVocuherResponseAfterUDFRemove = "";

            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;
            string lastVchAlterID = LASTVOUCHERID;
            string currentlastVchAlterID = CURRENTLASTVOUCHERID;

            try
            {
                if (lastVchAlterID == "0")
                {
                    lastVchAlterID = "-1";
                }

                xmlstc = "<ENVELOPE>\r\n";
                xmlstc = xmlstc + "<HEADER>\r\n";
                xmlstc = xmlstc + "<VERSION>1</VERSION>\r\n";
                xmlstc = xmlstc + "<TALLYREQUEST>Export</TALLYREQUEST>\r\n";
                xmlstc = xmlstc + "<TYPE>Collection</TYPE>\r\n";
                xmlstc = xmlstc + "<ID>CollectionofSalesVoucher</ID>\r\n";
                xmlstc = xmlstc + "</HEADER>\r\n";
                xmlstc = xmlstc + "<BODY>\r\n";
                xmlstc = xmlstc + "<DESC>\r\n";
                xmlstc = xmlstc + "<STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<SVFROMDATE>" + SVFromDate + "</SVFROMDATE>\r\n";
                xmlstc = xmlstc + "<SVTODATE>" + SVToDate + "</SVTODATE>\r\n";
                xmlstc = xmlstc + "<SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT>\r\n";
                xmlstc = xmlstc + "<SVCURRENTCOMPANY> " + SVCURRENTCOMPANY + " </SVCURRENTCOMPANY>\r\n";
                xmlstc = xmlstc + "</STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<TDL>\r\n";
                xmlstc = xmlstc + "<TDLMESSAGE>\r\n";

                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofSalesVoucherDEBTORS' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<TYPE>Vouchers: VoucherType</TYPE>\r\n";
                xmlstc = xmlstc + "<ChildOf>$$VchTypeDebitNote</ChildOf>\r\n";
                xmlstc = xmlstc + "<BelongsTo>Yes</BelongsTo>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryDebtorParty,GetFiltertheReport1</FILTER>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,IsOptional,IsCancelled</FETCH>\r\n";
                xmlstc = xmlstc + "<Compute> XGUIDParty	: $GUID:Ledger:$PartyLedgerName</Compute> \r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  'DEBTORS'</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";

                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofSalesVoucherCREDITORS' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<TYPE>Vouchers: VoucherType</TYPE>\r\n";
                xmlstc = xmlstc + "<ChildOf>$$VchTypeDebitNote</ChildOf>\r\n";
                xmlstc = xmlstc + "<BelongsTo>Yes</BelongsTo>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryCreditorParty,GetFiltertheReport1</FILTER>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,IsOptional,IsCancelled</FETCH>\r\n";
                xmlstc = xmlstc + "<Compute> XGUIDParty	: $GUID:Ledger:$PartyLedgerName</Compute> \r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  'CREDITORS'</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";
                
 
                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofSalesVoucher' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<COLLECTION>CollectionofSalesVoucherCREDITORS, CollectionofSalesVoucherDEBTORS </COLLECTION>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,IsOptional,IsCancelled</FETCH>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";

               xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryCreditorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$PartyLedgerName):$$GroupSundryCreditors) </SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryDebtorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$PartyLedgerName):$$GroupSundryDebtors) </SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'GetFiltertheReport1'> Not ($IsOptional or $IsCancelled) And ( $Date &gt;= $$Date:'" + SVFromDate + "' and $Date  &lt;= $$Date:'" + SVToDate + "') And $alterid &gt; " + lastVchAlterID + " AND $alterid &lt;= " + currentlastVchAlterID + "</SYSTEM>\r\n";


                xmlstc = xmlstc + "</TDLMESSAGE>\r\n";
                xmlstc = xmlstc + "</TDL>\r\n";
                xmlstc = xmlstc + "</DESC>\r\n";
                xmlstc = xmlstc + "</BODY>\r\n";
                xmlstc = xmlstc + "</ENVELOPE>\r\n";

                String xml = xmlstc;
                String lVocuherResponse = aPI.TallySendReqst(xml);
                lVocuherResponseAfterUDFRemove = lVocuherResponse.Replace("UDF:", "");
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionDebitNoteVoucherCollectionXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lVocuherResponseAfterUDFRemove;
        }
       
        public string CreditNoteVoucherCollectionXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {

            string xmlstc = "";
            string lVocuherResponseAfterUDFRemove = "";
            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;
            string lastVchAlterID = LASTVOUCHERID;
            string currentlastVchAlterID = CURRENTLASTVOUCHERID;

            try
            {
                if (lastVchAlterID == "0")
                {
                    lastVchAlterID = "-1";
                }

                xmlstc = "<ENVELOPE>\r\n";
                xmlstc = xmlstc + "<HEADER>\r\n";
                xmlstc = xmlstc + "<VERSION>1</VERSION>\r\n";
                xmlstc = xmlstc + "<TALLYREQUEST>Export</TALLYREQUEST>\r\n";
                xmlstc = xmlstc + "<TYPE>Collection</TYPE>\r\n";
                xmlstc = xmlstc + "<ID>CollectionofSalesVoucher</ID>\r\n";
                xmlstc = xmlstc + "</HEADER>\r\n";
                xmlstc = xmlstc + "<BODY>\r\n";
                xmlstc = xmlstc + "<DESC>\r\n";
                xmlstc = xmlstc + "<STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<SVFROMDATE>" + SVFromDate + "</SVFROMDATE>\r\n";
                xmlstc = xmlstc + "<SVTODATE>" + SVToDate + "</SVTODATE>\r\n";
                xmlstc = xmlstc + "<SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT>\r\n";
                xmlstc = xmlstc + "<SVCURRENTCOMPANY> " + SVCURRENTCOMPANY + " </SVCURRENTCOMPANY>\r\n";
                xmlstc = xmlstc + "</STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<TDL>\r\n";
                xmlstc = xmlstc + "<TDLMESSAGE>\r\n";

                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofSalesVoucherDEBTORS' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<TYPE>Vouchers: VoucherType</TYPE>\r\n";
                xmlstc = xmlstc + "<ChildOf>$$VchTypeCreditNote</ChildOf>\r\n";
                xmlstc = xmlstc + "<BelongsTo>Yes</BelongsTo>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryDebtorParty,GetFiltertheReport1</FILTER>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,IsOptional,IsCancelled</FETCH>\r\n";
                xmlstc = xmlstc + "<Compute> XGUIDParty	: $GUID:Ledger:$PartyLedgerName</Compute> \r\n";

                 xmlstc = xmlstc + "<Compute> XPARENTTYPE	: 'DEBTORS'</Compute> \r\n";

                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";

                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofSalesVoucherCREDITORS' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<TYPE>Vouchers: VoucherType</TYPE>\r\n";
                xmlstc = xmlstc + "<ChildOf>$$VchTypeCreditNote</ChildOf>\r\n";
                xmlstc = xmlstc + "<BelongsTo>Yes</BelongsTo>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryCreditorParty,GetFiltertheReport1</FILTER>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,IsOptional,IsCancelled</FETCH>\r\n";
                xmlstc = xmlstc + "<Compute> XGUIDParty	: $GUID:Ledger:$PartyLedgerName</Compute> \r\n";

                xmlstc = xmlstc + "<Compute> XPARENTTYPE	: 'CREDITORS'</Compute> \r\n";

                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";
            

                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofSalesVoucher' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<COLLECTION> CollectionofSalesVoucherCREDITORS, CollectionofSalesVoucherDEBTORS </COLLECTION>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,IsOptional,IsCancelled</FETCH>\r\n";
  
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";

                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryCreditorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$PartyLedgerName):$$GroupSundryCreditors) </SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryDebtorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$PartyLedgerName):$$GroupSundryDebtors) </SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'GetFiltertheReport1'> Not ($IsOptional or $IsCancelled) And ( $Date &gt;= $$Date:'" + SVFromDate + "' and $Date  &lt;= $$Date:'" + SVToDate + "' ) And $alterid &gt; " + lastVchAlterID + " AND $alterid &lt;= " + currentlastVchAlterID + "</SYSTEM>\r\n";


                xmlstc = xmlstc + "</TDLMESSAGE>\r\n";
                xmlstc = xmlstc + "</TDL>\r\n";
                xmlstc = xmlstc + "</DESC>\r\n";
                xmlstc = xmlstc + "</BODY>\r\n";
                xmlstc = xmlstc + "</ENVELOPE>\r\n";

                String xml = xmlstc;
                String lVocuherResponse = aPI.TallySendReqst(xml);
                lVocuherResponseAfterUDFRemove = lVocuherResponse.Replace("UDF:", "");
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionCreditNoteVoucherCollectionXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lVocuherResponseAfterUDFRemove;
        }

        public string JournalInvoiceVoucherCollectionXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {

            string xmlstc = "";
            string lVocuherResponseAfterUDFRemove = "";

            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;
            string lastVchAlterID = LASTVOUCHERID;
            string currentlastVchAlterID = CURRENTLASTVOUCHERID;

            try
            {
                if (lastVchAlterID == "0")
                {
                    lastVchAlterID = "-1";
                }

                xmlstc = "<ENVELOPE>\r\n";
                xmlstc = xmlstc + "<HEADER>\r\n";
                xmlstc = xmlstc + "<VERSION>1</VERSION>\r\n";
                xmlstc = xmlstc + "<TALLYREQUEST>Export</TALLYREQUEST>\r\n";
                xmlstc = xmlstc + "<TYPE>Collection</TYPE>\r\n";
                xmlstc = xmlstc + "<ID>COLLECTIONOFDEBITJOURNALVOUCHERSUMM</ID>\r\n";
                xmlstc = xmlstc + "</HEADER>\r\n";
                xmlstc = xmlstc + "<BODY>\r\n";
                xmlstc = xmlstc + "<DESC>\r\n";
                xmlstc = xmlstc + "<STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<SVFROMDATE>" + SVFromDate + "</SVFROMDATE>\r\n";
                xmlstc = xmlstc + "<SVTODATE>" + SVToDate + "</SVTODATE>\r\n";
                xmlstc = xmlstc + "<SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT>\r\n";
                xmlstc = xmlstc + "<SVCURRENTCOMPANY> " + SVCURRENTCOMPANY + " </SVCURRENTCOMPANY>\r\n";
                xmlstc = xmlstc + "</STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<TDL>\r\n";
                xmlstc = xmlstc + "<TDLMESSAGE>\r\n";
               
                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofSalesVoucher' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<TYPE>Vouchers: VoucherType</TYPE>\r\n";
                xmlstc = xmlstc + "<ChildOf>$$VchTypeJournal</ChildOf>\r\n";
                xmlstc = xmlstc + "<BelongsTo>Yes</BelongsTo>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,IsOptional,IsCancelled, MasterID, AlterID</FETCH>\r\n";
                xmlstc = xmlstc + "<FILTER>GetFiltertheReport2</FILTER>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";
              
                xmlstc = xmlstc + "<COLLECTION Name = 'COLLECTIONOFCREDITRECEIPTVOUCHERDEBTORSSUMM' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<SourceCollection>CollectionofSalesVoucher</SourceCollection>\r\n";
                xmlstc = xmlstc + "<Walk>ALLLedgerEntries</Walk>\r\n";
                xmlstc = xmlstc + "<Compute> XGUID			: $$Owner:$GUID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNO		: $$Owner:$VOUCHERNUMBER</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHVTYP		: $$Owner:$VOUCHERTYPENAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHDT		: $$Owner:$DATE</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XLEDGERNAME	: $GUID:Ledger:$LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XXLEDGERNAME	:  $LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XAMOUNT		: $AMOUNT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XDRORCRFLAG	: $IsDeemedPositive</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHALTERID	: $$Owner:$ALTERID </Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNarration	: $$Owner:$Narration </Compute>\r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  'DEBTORS'</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryDebtorParty,FilterAlterID</FILTER>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";
             
                 xmlstc = xmlstc + "<COLLECTION Name = 'COLLECTIONOFCREDITRECEIPTVOUCHERCREDITORSSUMM' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<SourceCollection>CollectionofSalesVoucher</SourceCollection>\r\n";
                xmlstc = xmlstc + "<Walk>ALLLedgerEntries</Walk>\r\n";
                xmlstc = xmlstc + "<Compute> XGUID			: $$Owner:$GUID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNO		: $$Owner:$VOUCHERNUMBER</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHVTYP		: $$Owner:$VOUCHERTYPENAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHDT		: $$Owner:$DATE</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XLEDGERNAME	: $GUID:Ledger:$LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XXLEDGERNAME	:  $LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XAMOUNT		: $AMOUNT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XDRORCRFLAG	: $IsDeemedPositive</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHALTERID	: $$Owner:$ALTERID </Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNarration	: $$Owner:$Narration </Compute>\r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  'CREDITORS'</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryCreditorParty,FilterAlterID</FILTER>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";
                
 
                xmlstc = xmlstc + "<COLLECTION Name = 'COLLECTIONOFDEBITJOURNALVOUCHERSUMM' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<Collection>COLLECTIONOFCREDITRECEIPTVOUCHERCREDITORSSUMM, COLLECTIONOFCREDITRECEIPTVOUCHERDEBTORSSUMM </Collection>\r\n";
                xmlstc = xmlstc + "<Compute> XGUID			: $XGUID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNO		: $XVCHNO</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHVTYP		: $XVCHVTYP</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHDT		: $XVCHDT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XLEDGERNAME	: $XLEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XXLEDGERNAME	:  $XXLEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XAMOUNT		: $XAMOUNT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XDRORCRFLAG	: $XDRORCRFLAG</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHALTERID	: $XVCHALTERID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNarration	: $XVCHNarration </Compute>\r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  $XPARENTTYPE</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";

                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryCreditorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$XXLEDGERNAME):$$GroupSundryCreditors) And $XDRORCRFLAG = 'Yes'</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryDebtorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$XXLEDGERNAME):$$GroupSundryDebtors) And $XDRORCRFLAG = 'Yes'</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'GetFiltertheReport2'>Not ($IsOptional or $IsCancelled) And ($Date &gt;= $$Date:'" + SVFromDate + "' and $Date &lt;=$$Date:'" + SVToDate + "')</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterAlterID'> $XVCHALTERID &gt; " + lastVchAlterID + " AND $XVCHALTERID &lt;= " + currentlastVchAlterID + "</SYSTEM>\r\n";

                xmlstc = xmlstc + "</TDLMESSAGE>\r\n";
                xmlstc = xmlstc + "</TDL>\r\n";
                xmlstc = xmlstc + "</DESC>\r\n";
                xmlstc = xmlstc + "</BODY>\r\n";
                xmlstc = xmlstc + "</ENVELOPE>\r\n";
              
                String xml = xmlstc;
                String lVocuherResponse = aPI.TallySendReqst(xml);
                lVocuherResponseAfterUDFRemove = lVocuherResponse.Replace("UDF:", "");
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionJournalInvoiceVoucherCollectionXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lVocuherResponseAfterUDFRemove;
        }

        public string JournalReceiptVoucherCollectionXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {

            string xmlstc = "";
            string lVocuherResponseAfterUDFRemove = "";

            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;
            string lastVchAlterID = LASTVOUCHERID;
            string currentlastVchAlterID = CURRENTLASTVOUCHERID;

            try
            {
                if (lastVchAlterID == "0")
                {
                    lastVchAlterID = "-1";
                }

                xmlstc = "<ENVELOPE>\r\n";
                xmlstc = xmlstc + "<HEADER>\r\n";
                xmlstc = xmlstc + "<VERSION>1</VERSION>\r\n";
                xmlstc = xmlstc + "<TALLYREQUEST>Export</TALLYREQUEST>\r\n";
                xmlstc = xmlstc + "<TYPE>Collection</TYPE>\r\n";
                xmlstc = xmlstc + "<ID>COLLECTIONOFDEBITJOURNALVOUCHERSUMM</ID>\r\n";
                xmlstc = xmlstc + "</HEADER>\r\n";
                xmlstc = xmlstc + "<BODY>\r\n";
                xmlstc = xmlstc + "<DESC>\r\n";
                xmlstc = xmlstc + "<STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<SVFROMDATE>" + SVFromDate + "</SVFROMDATE>\r\n";
                xmlstc = xmlstc + "<SVTODATE>" + SVToDate + "</SVTODATE>\r\n";
                xmlstc = xmlstc + "<SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT>\r\n";
                xmlstc = xmlstc + "<SVCURRENTCOMPANY> " + SVCURRENTCOMPANY + " </SVCURRENTCOMPANY>\r\n";
                xmlstc = xmlstc + "</STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<TDL>\r\n";
                xmlstc = xmlstc + "<TDLMESSAGE>\r\n";
               
                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofSalesVoucher' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<TYPE>Vouchers: VoucherType</TYPE>\r\n";
                xmlstc = xmlstc + "<ChildOf>$$VchTypeJournal</ChildOf>\r\n";
                xmlstc = xmlstc + "<BelongsTo>Yes</BelongsTo>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,IsOptional,IsCancelled, MasterID, AlterID</FETCH>\r\n";
                xmlstc = xmlstc + "<FILTER>GetFiltertheReport2</FILTER>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";
              
                xmlstc = xmlstc + "<COLLECTION Name = 'COLLECTIONOFCREDITRECEIPTVOUCHERDEBTORSSUMM' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<SourceCollection>CollectionofSalesVoucher</SourceCollection>\r\n";
                xmlstc = xmlstc + "<Walk>ALLLedgerEntries</Walk>\r\n";
                xmlstc = xmlstc + "<Compute> XGUID			: $$Owner:$GUID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNO		: $$Owner:$VOUCHERNUMBER</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHVTYP		: $$Owner:$VOUCHERTYPENAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHDT		: $$Owner:$DATE</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XLEDGERNAME	: $GUID:Ledger:$LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XXLEDGERNAME	:  $LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XAMOUNT		: $AMOUNT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XDRORCRFLAG	: $IsDeemedPositive</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHALTERID	: $$Owner:$ALTERID </Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNarration	: $$Owner:$Narration </Compute>\r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  'DEBTORS'</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryDebtorParty,FilterAlterID</FILTER>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";
               
                 xmlstc = xmlstc + "<COLLECTION Name = 'COLLECTIONOFCREDITRECEIPTVOUCHERCREDITORSSUMM' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<SourceCollection>CollectionofSalesVoucher</SourceCollection>\r\n";
                xmlstc = xmlstc + "<Walk>ALLLedgerEntries</Walk>\r\n";
                xmlstc = xmlstc + "<Compute> XGUID			: $$Owner:$GUID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNO		: $$Owner:$VOUCHERNUMBER</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHVTYP		: $$Owner:$VOUCHERTYPENAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHDT		: $$Owner:$DATE</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XLEDGERNAME	: $GUID:Ledger:$LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XXLEDGERNAME	:  $LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XAMOUNT		: $AMOUNT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XDRORCRFLAG	: $IsDeemedPositive</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHALTERID	: $$Owner:$ALTERID </Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNarration	: $$Owner:$Narration </Compute>\r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  'CREDITORS'</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryCreditorParty,FilterAlterID</FILTER>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";
                
 
                xmlstc = xmlstc + "<COLLECTION Name = 'COLLECTIONOFDEBITJOURNALVOUCHERSUMM' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<Collection>COLLECTIONOFCREDITRECEIPTVOUCHERCREDITORSSUMM, COLLECTIONOFCREDITRECEIPTVOUCHERDEBTORSSUMM </Collection>\r\n";
                xmlstc = xmlstc + "<Compute> XGUID			: $XGUID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNO		: $XVCHNO</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHVTYP		: $XVCHVTYP</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHDT		: $XVCHDT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XLEDGERNAME	: $XLEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XXLEDGERNAME	:  $XXLEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XAMOUNT		: $XAMOUNT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XDRORCRFLAG	: $XDRORCRFLAG</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHALTERID	: $XVCHALTERID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNarration	: $XVCHNarration </Compute>\r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  $XPARENTTYPE</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";
                
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryCreditorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$XXLEDGERNAME):$$GroupSundryCreditors) And $XDRORCRFLAG = 'No'</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryDebtorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$XXLEDGERNAME):$$GroupSundryDebtors) And $XDRORCRFLAG = 'No'</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'GetFiltertheReport2'>Not ($IsOptional or $IsCancelled) And ( $Date &gt;= $$Date:'" + SVFromDate + "' and $Date &lt;=$$Date:'" + SVToDate + "')</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterAlterID'> $XVCHALTERID &gt; " + lastVchAlterID + " AND $XVCHALTERID &lt;= " + currentlastVchAlterID + "</SYSTEM>\r\n";
                xmlstc = xmlstc + "</TDLMESSAGE>\r\n";
                xmlstc = xmlstc + "</TDL>\r\n";
                xmlstc = xmlstc + "</DESC>\r\n";
                xmlstc = xmlstc + "</BODY>\r\n";
                xmlstc = xmlstc + "</ENVELOPE>\r\n";

                String xml = xmlstc;
                String lVocuherResponse = aPI.TallySendReqst(xml);
                lVocuherResponseAfterUDFRemove = lVocuherResponse.Replace("UDF:", "");
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionJournalReceiptVoucherCollectionXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lVocuherResponseAfterUDFRemove;
        }
 
        public string RecipetvoucherCollectionXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {
            String xmlstc = "";
            String lVocuherResponseAfterUDFRemove = "";

            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;
            string lastVchAlterID = LASTVOUCHERID;
            string currentlastVchAlterID = CURRENTLASTVOUCHERID;


            try
            {
                if (lastVchAlterID == "0")
                {
                    lastVchAlterID = "-1";
                }

                xmlstc = "<ENVELOPE>\r\n";
                xmlstc = xmlstc + "<HEADER>\r\n";
                xmlstc = xmlstc + "<VERSION>1</VERSION>\r\n";
                xmlstc = xmlstc + "<TALLYREQUEST>Export</TALLYREQUEST>\r\n";
                xmlstc = xmlstc + "<TYPE>Collection</TYPE>\r\n";
                xmlstc = xmlstc + "<ID>COLLECTIONOFCREDITRECEIPTVOUCHERSUMM</ID>\r\n";
                xmlstc = xmlstc + "</HEADER>\r\n";
                xmlstc = xmlstc + "<BODY>\r\n";
                xmlstc = xmlstc + "<DESC>\r\n";
                xmlstc = xmlstc + "<STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<SVFROMDATE>" + SVFromDate + "</SVFROMDATE>\r\n";
                xmlstc = xmlstc + "<SVTODATE>" + SVToDate + "</SVTODATE>\r\n";
                xmlstc = xmlstc + "<SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT>\r\n";
                xmlstc = xmlstc + "<SVCURRENTCOMPANY> " + SVCURRENTCOMPANY + " </SVCURRENTCOMPANY>\r\n";
                xmlstc = xmlstc + "</STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<TDL>\r\n";
                xmlstc = xmlstc + "<TDLMESSAGE>\r\n";
                
                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofReceiptVoucher' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<TYPE>Vouchers: VoucherType</TYPE>\r\n";
                xmlstc = xmlstc + "<ChildOf>$$VchTypeReceipt</ChildOf>\r\n";
                xmlstc = xmlstc + "<BelongsTo>Yes</BelongsTo>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,,IsOptional,IsCancelled, MasterID, AlterID</FETCH>\r\n";
                xmlstc = xmlstc + "<FILTER>GetFiltertheReport2</FILTER>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";

                xmlstc = xmlstc + "<COLLECTION Name = 'COLLECTIONOFCREDITRECEIPTVOUCHERDEBTORSSUMM' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<SourceCollection>CollectionofReceiptVoucher</SourceCollection>\r\n";
                xmlstc = xmlstc + "<Walk>ALLLedgerEntries</Walk>\r\n";
                xmlstc = xmlstc + "<Compute> XGUID			: $$Owner:$GUID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNO		: $$Owner:$VOUCHERNUMBER</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHVTYP		: $$Owner:$VOUCHERTYPENAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHDT		: $$Owner:$DATE</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XLEDGERNAME	: $GUID:Ledger:$LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XXLEDGERNAME	:  $LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XAMOUNT		: $AMOUNT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XDRORCRFLAG	: $IsDeemedPositive</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHALTERID	: $$Owner:$ALTERID </Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNarration	: $$Owner:$Narration </Compute>\r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  'DEBTORS'</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryDebtorParty,FilterAlterID</FILTER>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";
                
                 xmlstc = xmlstc + "<COLLECTION Name = 'COLLECTIONOFCREDITRECEIPTVOUCHERCREDITORSSUMM' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<SourceCollection>CollectionofReceiptVoucher</SourceCollection>\r\n";
                xmlstc = xmlstc + "<Walk>ALLLedgerEntries</Walk>\r\n";
                xmlstc = xmlstc + "<Compute> XGUID			: $$Owner:$GUID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNO		: $$Owner:$VOUCHERNUMBER</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHVTYP		: $$Owner:$VOUCHERTYPENAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHDT		: $$Owner:$DATE</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XLEDGERNAME	: $GUID:Ledger:$LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XXLEDGERNAME	:  $LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XAMOUNT		: $AMOUNT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XDRORCRFLAG	: $IsDeemedPositive</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHALTERID	: $$Owner:$ALTERID </Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNarration	: $$Owner:$Narration </Compute>\r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  'CREDITORS'</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryCreditorParty,FilterAlterID</FILTER>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";
                
 
                xmlstc = xmlstc + "<COLLECTION Name = 'COLLECTIONOFCREDITRECEIPTVOUCHERSUMM' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<Collection>COLLECTIONOFCREDITRECEIPTVOUCHERCREDITORSSUMM, COLLECTIONOFCREDITRECEIPTVOUCHERDEBTORSSUMM </Collection>\r\n";
                xmlstc = xmlstc + "<Compute> XGUID			: $XGUID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNO		: $XVCHNO</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHVTYP		: $XVCHVTYP</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHDT		: $XVCHDT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XLEDGERNAME	: $XLEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XXLEDGERNAME	:  $XXLEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XAMOUNT		: $XAMOUNT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XDRORCRFLAG	: $XDRORCRFLAG</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHALTERID	: $XVCHALTERID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNarration	: $XVCHNarration </Compute>\r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  $XPARENTTYPE</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";
                
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryCreditorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$XXLEDGERNAME):$$GroupSundryCreditors) And $XDRORCRFLAG = 'No'</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryDebtorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$XXLEDGERNAME):$$GroupSundryDebtors) And $XDRORCRFLAG = 'No'</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'GetFiltertheReport2'>Not ($IsOptional or $IsCancelled) And ($Date &gt;= $$Date:'" + SVFromDate + "' and $Date &lt;=$$Date:'" + SVToDate + "')</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterAlterID'> $XVCHALTERID &gt; " + lastVchAlterID + " AND $XVCHALTERID &lt;= " + currentlastVchAlterID + "</SYSTEM>\r\n";
                xmlstc = xmlstc + "</TDLMESSAGE>\r\n";
                xmlstc = xmlstc + "</TDL>\r\n";
                xmlstc = xmlstc + "</DESC>\r\n";
                xmlstc = xmlstc + "</BODY>\r\n";
                xmlstc = xmlstc + "</ENVELOPE>\r\n";

                String xml = xmlstc;
                String lVocuherResponse = aPI.TallySendReqst(xml);
                lVocuherResponseAfterUDFRemove = lVocuherResponse.Replace("UDF:", "");
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionRecipetvoucherCollectionXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }
        
            return lVocuherResponseAfterUDFRemove;
          
        }

        public string PaymentvoucherCollectionXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {
            String xmlstc = "";
            String lVocuherResponseAfterUDFRemove = "";

            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;
            string lastVchAlterID = LASTVOUCHERID;
            string currentlastVchAlterID = CURRENTLASTVOUCHERID;



            try
            {
                if (lastVchAlterID == "0")
                {
                    lastVchAlterID = "-1";
                }

                xmlstc = "<ENVELOPE>\r\n";
                xmlstc = xmlstc + "<HEADER>\r\n";
                xmlstc = xmlstc + "<VERSION>1</VERSION>\r\n";
                xmlstc = xmlstc + "<TALLYREQUEST>Export</TALLYREQUEST>\r\n";
                xmlstc = xmlstc + "<TYPE>Collection</TYPE>\r\n";
                xmlstc = xmlstc + "<ID>COLLECTIONOFCREDITRECEIPTVOUCHERSUMM</ID>\r\n";
                xmlstc = xmlstc + "</HEADER>\r\n";
                xmlstc = xmlstc + "<BODY>\r\n";
                xmlstc = xmlstc + "<DESC>\r\n";
                xmlstc = xmlstc + "<STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<SVFROMDATE>" + SVFromDate + "</SVFROMDATE>\r\n";
                xmlstc = xmlstc + "<SVTODATE>" + SVToDate + "</SVTODATE>\r\n";
                xmlstc = xmlstc + "<SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT>\r\n";
                xmlstc = xmlstc + "<SVCURRENTCOMPANY> " + SVCURRENTCOMPANY + " </SVCURRENTCOMPANY>\r\n";
                xmlstc = xmlstc + "</STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<TDL>\r\n";
                xmlstc = xmlstc + "<TDLMESSAGE>\r\n";
                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofReceiptVoucher' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<TYPE>Vouchers: VoucherType</TYPE>\r\n";
                xmlstc = xmlstc + "<ChildOf>$$VchTypePayment</ChildOf>\r\n";
                xmlstc = xmlstc + "<BelongsTo>Yes</BelongsTo>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,,IsOptional,IsCancelled, MasterID, AlterID</FETCH>\r\n";
                xmlstc = xmlstc + "<FILTER>GetFiltertheReport2</FILTER>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";
                xmlstc = xmlstc + "<COLLECTION Name = 'COLLECTIONOFCREDITRECEIPTVOUCHERDEBTORSSUMM' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<SourceCollection>CollectionofReceiptVoucher</SourceCollection>\r\n";
                xmlstc = xmlstc + "<Walk>ALLLedgerEntries</Walk>\r\n";
                xmlstc = xmlstc + "<Compute> XGUID			: $$Owner:$GUID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNO		: $$Owner:$VOUCHERNUMBER</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHVTYP		: $$Owner:$VOUCHERTYPENAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHDT		: $$Owner:$DATE</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XLEDGERNAME	: $GUID:Ledger:$LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XXLEDGERNAME	:  $LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XAMOUNT		: $AMOUNT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XDRORCRFLAG	: $IsDeemedPositive</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHALTERID	: $$Owner:$ALTERID </Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNarration	: $$Owner:$Narration </Compute>\r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  'DEBTORS'</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryDebtorParty,FilterAlterID</FILTER>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";

               xmlstc = xmlstc + "<COLLECTION Name = 'COLLECTIONOFCREDITRECEIPTVOUCHERCREDITORSSUMM' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<SourceCollection>CollectionofReceiptVoucher</SourceCollection>\r\n";
                xmlstc = xmlstc + "<Walk>ALLLedgerEntries</Walk>\r\n";
                xmlstc = xmlstc + "<Compute> XGUID			: $$Owner:$GUID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNO		    : $$Owner:$VOUCHERNUMBER</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHVTYP		: $$Owner:$VOUCHERTYPENAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHDT		    : $$Owner:$DATE</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XLEDGERNAME	: $GUID:Ledger:$LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XXLEDGERNAME	: $LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XAMOUNT		: $AMOUNT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XDRORCRFLAG	: $IsDeemedPositive</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHALTERID	: $$Owner:$ALTERID </Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNarration	: $$Owner:$Narration </Compute>\r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  'CREDITORS'</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryCreditorParty,FilterAlterID</FILTER>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";
                


                xmlstc = xmlstc + "<COLLECTION Name = 'COLLECTIONOFCREDITRECEIPTVOUCHERSUMM' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<Collection>COLLECTIONOFCREDITRECEIPTVOUCHERCREDITORSSUMM,COLLECTIONOFCREDITRECEIPTVOUCHERDEBTORSSUMM </Collection>\r\n";
                xmlstc = xmlstc + "<Compute> XGUID			: $XGUID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNO		    : $XVCHNO</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHVTYP		: $XVCHVTYP</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHDT		    : $XVCHDT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XLEDGERNAME	: $XLEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XXLEDGERNAME	: $XXLEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XAMOUNT		: $XAMOUNT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XDRORCRFLAG	: $XDRORCRFLAG</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHALTERID	: $XVCHALTERID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNarration	: $XVCHNarration </Compute>\r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  $XPARENTTYPE</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";

                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryCreditorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$XXLEDGERNAME):$$GroupSundryCreditors) And $XDRORCRFLAG = 'Yes'</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryDebtorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$XXLEDGERNAME):$$GroupSundryDebtors) And $XDRORCRFLAG = 'Yes'</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'GetFiltertheReport2'>Not ($IsOptional or $IsCancelled) And ( $Date &gt;= $$Date:'" + SVFromDate + "' and $Date &lt;=$$Date:'" + SVToDate + "') </SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterAlterID'> $XVCHALTERID &gt; " + lastVchAlterID + " AND $XVCHALTERID &lt;= " + currentlastVchAlterID + "</SYSTEM>\r\n";
                xmlstc = xmlstc + "</TDLMESSAGE>\r\n";
                xmlstc = xmlstc + "</TDL>\r\n";
                xmlstc = xmlstc + "</DESC>\r\n";
                xmlstc = xmlstc + "</BODY>\r\n";
                xmlstc = xmlstc + "</ENVELOPE>\r\n";

                String xml = xmlstc;
                String lVocuherResponse = aPI.TallySendReqst(xml);
                lVocuherResponseAfterUDFRemove = lVocuherResponse.Replace("UDF:", "");
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionPaymentvoucherCollectionXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lVocuherResponseAfterUDFRemove;

        }

        public string TallyLicenseInfoXMLRequest()
        {
            String lVocuherResponse = "";


            try
            {
                String xml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Function</TYPE><ID>$$LicenseInfo</ID></HEADER><BODY><DESC><FUNCPARAMLIST><PARAM>Serial Number</PARAM></FUNCPARAMLIST></DESC></BODY></ENVELOPE>";
                lVocuherResponse = aPI.TallySendReqst(xml);
                
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionTallyLicenseInfoXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }
           
            return lVocuherResponse;


        }
        
        public string TallyActiveCompanyXMLRequest()
        {
            String lVocuherResponse = "";

            try
            {
                String xml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Collection</TYPE><ID>Collection of Ledgers</ID></HEADER><BODY><DESC><STATICVARIABLES><SVFROMDATE TYPE='Date'>01-Jan-1970</SVFROMDATE><SVTODATE TYPE='Date'>01-Jan-1970</SVTODATE><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT></STATICVARIABLES><TDL><TDLMESSAGE><COLLECTION NAME='Collection of Ledgers' ISMODIFY='NO'><TYPE>Company</TYPE><FETCH>COMPANYNUMBER,GUID,ALTERID,MASTERID,GUID,NAME,STATE,STARTINGFROM,BOOKSFROM,ENDINGAT,LASTVOUCHERDATE,CMPVCHID,ALTVCHID,ALTMSTID,ISAGGREGATE,BASICCOMPANYFORMALNAME,COMPANYCHEQUENAME,COMPANYCONTACTPERSON,COMPANYCONTACTNUMBER,EMAIL,WEBSITE,PHONENUMBER,CMPMOBNO,MOBILENO,_ADDRESS1,_ADDRESS2,_ADDRESS3,_ADDRESS4,_ADDRESS5,STATENAME,PINCODE,COUNTRYNAME,VATTINNUMBER,UDF:CORPORATEIDENTITYNO,INTERSTATESTNUMBER,TANUMBER,STREGNUMBER,CMPPFCODE,INCOMETAXNUMBER,GSTREGISTRATIONTYPE,GSTAPPLICABLEDATE,ISGSTCESSON,GSTDETAILS.LIST,DESTINATION</FETCH><FILTERS>GroupFilter</FILTERS></COLLECTION><SYSTEM TYPE='FORMULAE' NAME='GroupFilter'>$isaggregate = 'No'</SYSTEM></TDLMESSAGE></TDL></DESC></BODY></ENVELOPE>";
                lVocuherResponse = aPI.TallySendReqst(xml);
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionTallyActiveCompanyXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lVocuherResponse;
        }

        public string TallyAlterMasterXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTMASTERID, string CURRENTLASTMASTERID, string COMPANYINITIALS)
        {
            String xmlstc = "";
            String lVocuherResponseAfterUDFRemove = "";

            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;
            string lastmasterAlterID = LASTMASTERID;
            string currentlastmasterAlterID = CURRENTLASTMASTERID;
           
            try
            {
                if (lastmasterAlterID == "0")
                {
                    lastmasterAlterID = "-1";
                }

                xmlstc = "<ENVELOPE>\r\n";
                xmlstc = xmlstc + "<HEADER>\r\n";
                xmlstc = xmlstc + "<VERSION>1</VERSION>\r\n";
                xmlstc = xmlstc + "<TALLYREQUEST>Export</TALLYREQUEST>\r\n";
                xmlstc = xmlstc + "<TYPE>Collection</TYPE>\r\n";
                xmlstc = xmlstc + "<ID>Collection of SUNDRYDEBTORSLEDGERS</ID>\r\n";
                xmlstc = xmlstc + "</HEADER>\r\n";
                xmlstc = xmlstc + "<BODY>\r\n";
                xmlstc = xmlstc + "<DESC>\r\n";
                xmlstc = xmlstc + "<STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<SVCURRENTCOMPANY>" + SVCURRENTCOMPANY + "</SVCURRENTCOMPANY>\r\n";
                xmlstc = xmlstc + "<SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT>\r\n";
                xmlstc = xmlstc + "</STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<TDL>\r\n";
                xmlstc = xmlstc + "<TDLMESSAGE>\r\n";
                
                //xmlstc = xmlstc + "<COLLECTION NAME='CollectionofLedgers' ISMODIFY='No'>\r\n";
                //xmlstc = xmlstc + "<COLLECTION>Collection of SUNDRYDEBTORSLEDGERS, Collection of SUNDRYCREDITORSLEDGERS</COLLECTION>\r\n";
                //xmlstc = xmlstc + "<FETCH>GUID,ALTERID,MASTERID,NAME,PARENT,PARENTTYPE,LANGUAGENAME.LIST,ADDRESS.LIST,STATENAME,COUNTRYNAME,PINCODE,EMAIL,EMAILCC,LEDGERPHONE,LEDGERMOBILE,LEDGERCONTACT,LEDSTATENAME,OPENINGBALANCE,CLOSINGBALANCE,BILLALLOCATIONS.LIST,VATTINNUMBER,INTERSTATESTNUMBER,EXCISEREGNO,PARTYGSTIN,GSTDUTYHEAD,GSTDETAILS.LIST,GSTAPPROPRIATETO,TAXTYPE,GSTAPPLICABLE,RATEOFTAXCALCULATION,ISBILLWISEON,BILLCREDITPERIOD,CREDITLIMIT,PERFORMANCE,BANKDETAILS,BANKBRANCHNAME,BANKACCHOLDERNAME,BANKBSRCODE,IFSCODE,BRANCHCODE,BANKINGCONFIGBANK,DESCRIPTION,PRICELEVEL</FETCH>\r\n";
              
                //xmlstc = xmlstc + "</COLLECTION>\r\n";

                xmlstc = xmlstc + "<COLLECTION NAME='Collection of SUNDRYDEBTORSLEDGERS' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<TYPE>Ledger</TYPE>\r\n";
                xmlstc = xmlstc + "<CHILDOF>$$GroupSundryDebtors</CHILDOF>\r\n";
                xmlstc = xmlstc + "<BELONGSTO>YES</BELONGSTO>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,ALTERID,MASTERID,NAME,PARENT,LANGUAGENAME.LIST,ADDRESS.LIST,STATENAME,COUNTRYNAME,PINCODE,EMAIL,EMAILCC,LEDGERPHONE,LEDGERMOBILE,LEDGERCONTACT,LEDSTATENAME,OPENINGBALANCE,CLOSINGBALANCE,BILLALLOCATIONS.LIST,VATTINNUMBER,INTERSTATESTNUMBER,EXCISEREGNO,PARTYGSTIN,GSTDUTYHEAD,GSTDETAILS.LIST,GSTAPPROPRIATETO,TAXTYPE,GSTAPPLICABLE,RATEOFTAXCALCULATION,ISBILLWISEON,BILLCREDITPERIOD,CREDITLIMIT,PERFORMANCE,BANKDETAILS,BANKBRANCHNAME,BANKACCHOLDERNAME,BANKBSRCODE,IFSCODE,BRANCHCODE,BANKINGCONFIGBANK,DESCRIPTION,PRICELEVEL</FETCH>\r\n";
                xmlstc = xmlstc + " <COMPUTE>PARENTTYPE: 'DEBTORS'</COMPUTE>\r\n";
                xmlstc = xmlstc + " <COMPUTE>PAYMENTTERM: $$NUmber:$BILLCREDITPERIOD</COMPUTE>\r\n";
                xmlstc = xmlstc + "<FILTERS>AlterId</FILTERS>\r\n";

                xmlstc = xmlstc + "</COLLECTION>\r\n";
                
                //xmlstc = xmlstc + "<COLLECTION NAME='Collection of SUNDRYCREDITORSLEDGERS' ISMODIFY='No'>\r\n";
                //xmlstc = xmlstc + "<TYPE>Ledger</TYPE>\r\n";
                //xmlstc = xmlstc + "<CHILDOF>$$GroupSundryCreditors</CHILDOF>\r\n";
                //xmlstc = xmlstc + "<BELONGSTO>YES</BELONGSTO>\r\n";
                //xmlstc = xmlstc + "<FETCH>GUID,ALTERID,MASTERID,NAME,PARENT,LANGUAGENAME.LIST,ADDRESS.LIST,STATENAME,COUNTRYNAME,PINCODE,EMAIL,EMAILCC,LEDGERPHONE,LEDGERMOBILE,LEDGERCONTACT,LEDSTATENAME,OPENINGBALANCE,CLOSINGBALANCE,BILLALLOCATIONS.LIST,VATTINNUMBER,INTERSTATESTNUMBER,EXCISEREGNO,PARTYGSTIN,GSTDUTYHEAD,GSTDETAILS.LIST,GSTAPPROPRIATETO,TAXTYPE,GSTAPPLICABLE,RATEOFTAXCALCULATION,ISBILLWISEON,BILLCREDITPERIOD,CREDITLIMIT,PERFORMANCE,BANKDETAILS,BANKBRANCHNAME,BANKACCHOLDERNAME,BANKBSRCODE,IFSCODE,BRANCHCODE,BANKINGCONFIGBANK,DESCRIPTION,PRICELEVEL</FETCH>\r\n";
                //xmlstc = xmlstc + " <COMPUTE>PARENTTYPE: 'CREDITORS'</COMPUTE>\r\n";
                //xmlstc = xmlstc + "<FILTERS>AlterId</FILTERS>\r\n";
                //xmlstc = xmlstc + "</COLLECTION>\r\n";
                
                xmlstc = xmlstc + "<SYSTEM TYPE='FORMULAE' NAME='AlterId'>$alterid &gt; "+lastmasterAlterID+" AND $alterid &lt;= "+currentlastmasterAlterID+"</SYSTEM>\r\n";
                
                xmlstc = xmlstc + "</TDLMESSAGE>\r\n";
                xmlstc = xmlstc + "</TDL>\r\n";
                xmlstc = xmlstc + "</DESC>\r\n";
                xmlstc = xmlstc + "</BODY>\r\n";
                xmlstc = xmlstc + "</ENVELOPE>\r\n";


                String xml = xmlstc;
                String lVocuherResponse = aPI.TallySendReqst(xml);
                lVocuherResponseAfterUDFRemove = lVocuherResponse.Replace("UDF:", "");
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionTallyAlterMasterXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lVocuherResponseAfterUDFRemove;

        }

        public string TallyStatisticsReportXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE)
        {
            String lVocuherResponse = "";
            
            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;

            try
            {
                String xml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>Statistics</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>"+ SVCURRENTCOMPANY + "</SVCURRENTCOMPANY></STATICVARIABLES><REPEATVARIABLES><REPEATSET><SVFROMDATE>"+ SVFromDate + "</SVFROMDATE><SVTODATE>"+ SVToDate + "</SVTODATE></REPEATSET></REPEATVARIABLES></DESC></BODY></ENVELOPE>";
                lVocuherResponse = aPI.TallySendReqst(xml);
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionTallyStatisticsReportXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lVocuherResponse;
        }

        public string TallyBalanceReportXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE)
        {
            String lVocuherResponse = "";

            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;

            try
            {
                String xml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>BalanceSheet</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>" + SVCURRENTCOMPANY + "</SVCURRENTCOMPANY></STATICVARIABLES><REPEATVARIABLES><REPEATSET><SVFROMDATE>" + SVFromDate + "</SVFROMDATE><SVTODATE>" + SVToDate + "</SVTODATE></REPEATSET></REPEATVARIABLES></DESC></BODY></ENVELOPE>";
                lVocuherResponse = aPI.TallySendReqst(xml);
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionTallyBalanceReportXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lVocuherResponse;
        }

        public string TallyTrialBalanceXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE)
        {
            String lVocuherResponse = "";

            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;

            try
            {
                String xml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>Trial Balance</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>"+SVCURRENTCOMPANY+"</SVCURRENTCOMPANY><IsLedgerWise>Yes</IsLedgerWise><EXPLODEFLAG>No</EXPLODEFLAG></STATICVARIABLES><REPEATVARIABLES><REPEATSET><SVFROMDATE>"+SVFromDate+"</SVFROMDATE><SVTODATE>"+ SVToDate + "</SVTODATE></REPEATSET></REPEATVARIABLES><TDL><TDLMESSAGE><REPORT ISINTERNAL= ISOPTION='No' ISINITIALIZE='No' ISFIXED='No' ISMODIFY='Yes' isledgerwise='Yes' NAME='Trial Balance'><SET>ExplodeFlag:Yes</SET><SET>IsLedgerWise:Yes</SET></REPORT></TDLMESSAGE></TDL></DESC></BODY></ENVELOPE>";
                lVocuherResponse = aPI.TallySendReqst(xml);
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionTallyTrialBalanceXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lVocuherResponse;
        }


        public string TallyStatisticXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE)
        {

            String lVocuherResponse = "";

            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;



            try
            {
                String xml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>EXPORT</TALLYREQUEST><TYPE>Collection</TYPE><ID>CollectionForVoucherTypeWiseTotalValue</ID></HEADER><BODY><Desc><STATICVARIABLES><SVCURRENTCOMPANY>" + SVCURRENTCOMPANY + "</SVCURRENTCOMPANY><SVFROMDATE>" + SVFromDate + "</SVFROMDATE><SVTODATE>" + SVToDate + "</SVTODATE><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT></STATICVARIABLES> <TDL><TDLMESSAGE><COLLECTION Name = 'CollectionForVoucherTypeWiseTotalValuesrc' ISMODIFY='No'><TYPE>VoucherType</TYPE></COLLECTION><COLLECTION NAME = 'CollectionForVoucherTypeWiseTotalValue' ISMODIFY = 'NO'><SourceCollection>CollectionForVoucherTypeWiseTotalValuesrc</SourceCollection><Compute> XName			: $Name</Compute><Compute> TOtalAmt		: $$FilterAmtTotal:CollectionOfAllVoucher:IsFiltValue:$Amount</Compute><KeepSource>()..</KeepSource></COLLECTION><Collection Name = 'CollectionOfAllVoucher' ISMODIFY = 'No'><TYPE>Vouchers</TYPE><Fetch>Date,VoucherTypeName, partyLedgerName, Amount</Fetch></Collection><SYSTEM TYPE =  'Formulae'  Name = 'IsFiltValue'>$VoucherTypeName =   $$ReqObject:$Name </SYSTEM></TDLMESSAGE></TDL></Desc></BODY></ENVELOPE>";
                lVocuherResponse = aPI.TallySendReqst(xml);
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionTallyStatisticXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lVocuherResponse;
        }


        public string TallyCompanyVoucherXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTVOUCHERID, string CURRENTLASTVOUCHERID)
        {
            String lVocuherResponse = "";

            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;
            string lastVchAlterID = LASTVOUCHERID;
            string currentlastVchAlterID = CURRENTLASTVOUCHERID;


            try
            {
                String xml = "<ENVELOPE>\r\n";
                xml += "<HEADER><VERSION>1</VERSION><TALLYREQUEST>EXPORT</TALLYREQUEST><TYPE>Collection</TYPE><ID>CollectionOfAllVoucherSrc</ID></HEADER>\r\n";
                xml += "<BODY>\r\n";
                xml += "< Desc>\r\n";
                xml+= "<STATICVARIABLES>\r\n";
                xml += "< SVCURRENTCOMPANY>" + SVCURRENTCOMPANY + "</SVCURRENTCOMPANY>\r\n";
                xml += "< SVFROMDATE>" + SVFromDate + "</SVFROMDATE>\r\n";
                xml += "< SVTODATE>" + SVToDate + "</SVTODATE>\r\n";
                xml += "< SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT>\r\n";
                xml += "</STATICVARIABLES>\r\n";
                xml += "<TDL>\r\n";
                xml += "<TDLMESSAGE>\r\n";
                xml += "<Collection Name = 'CollectionOfAllVoucherSrc' ISMODIFY = 'No'>\r\n";
                xml += "<TYPE>Vouchers</TYPE>\r\n";
                xml += "<Fetch>Date,VoucherTypeName,alterid,partyLedgerName, Amount</Fetch>\r\n";
                xml += "<Filter>GetFiltertheReport</Filter>\r\n";
                xml += "</Collection><SYSTEM TYPE =  'Formulae'  Name = 'GetFiltertheReport'>$Date &gt;= $$Date:'" + SVFromDate + "' and $Date  &lt;= $$Date:'" + SVToDate + "' And $alterid &gt; " + lastVchAlterID + " AND $alterid &lt;= " + currentlastVchAlterID + "</SYSTEM>\r\n";
                xml += "</TDLMESSAGE>\r\n";
                xml += "</TDL>\r\n";
                xml += "</Desc>\r\n";
                xml += "</BODY>\r\n";
                xml += "</ENVELOPE>\r\n";
                lVocuherResponse = aPI.TallySendReqst(xml);
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.FunctionTallyCompanyVoucherXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lVocuherResponse;
        }



        // Trackpayout Request for Tally ERP 9 or Tally PRIME

        public string TPO_SalesVoucherCollectionXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {

            string xmlstc = "";
            string lVocuherResponseAfterUDFRemove = "";

            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;
            string lastVchAlterID = LASTVOUCHERID;
            string currentlastVchAlterID = CURRENTLASTVOUCHERID;



            try
            {
                //if (lastVchAlterID == "0")
                //{
                //    lastVchAlterID = "-1";
                //}

                xmlstc = "<ENVELOPE>\r\n";
                xmlstc = xmlstc + "<HEADER>\r\n";
                xmlstc = xmlstc + "<VERSION>1</VERSION>\r\n";
                xmlstc = xmlstc + "<TALLYREQUEST>Export</TALLYREQUEST>\r\n";
                xmlstc = xmlstc + "<TYPE>Collection</TYPE>\r\n";
                xmlstc = xmlstc + "<ID>CollectionofSalesVoucher</ID>\r\n";
                xmlstc = xmlstc + "</HEADER>\r\n";
                xmlstc = xmlstc + "<BODY>\r\n";
                xmlstc = xmlstc + "<DESC>\r\n";
                xmlstc = xmlstc + "<STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<SVFROMDATE>" + SVFromDate + "</SVFROMDATE>\r\n";
                xmlstc = xmlstc + "<SVTODATE>" + SVToDate + "</SVTODATE>\r\n";
                xmlstc = xmlstc + "<SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT>\r\n";
                xmlstc = xmlstc + "<SVCURRENTCOMPANY> " + SVCURRENTCOMPANY + " </SVCURRENTCOMPANY>\r\n";
                xmlstc = xmlstc + "</STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<TDL>\r\n";
                xmlstc = xmlstc + "<TDLMESSAGE>\r\n";
                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofSalesVoucher' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<TYPE>Vouchers: VoucherType</TYPE>\r\n";
                xmlstc = xmlstc + "<ChildOf>$$VchTypeSales</ChildOf>\r\n";
                xmlstc = xmlstc + "<BelongsTo>Yes</BelongsTo>\r\n";
                xmlstc = xmlstc + "<Fetch>GUID,Reference,BasicDueDateOfPymt,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,InventoryEntries.*,InventoryEntries.Amount,LedgerEntries.*, IsCancelled, IsOptional</Fetch>\r\n";
                xmlstc = xmlstc + "<Fetch>LedgerEntries.BillAllocations.*,InvoiceOrderList.*</Fetch>\r\n";

                xmlstc = xmlstc + "<Compute>billReferenceNo: if $$IsEmpty:$Reference then 'NA' else $Reference</Compute>\r\n";
                xmlstc = xmlstc + "<Compute>companyId: $CompanyNumber:Company:$$CurrentCompany</Compute>\r\n";
                xmlstc = xmlstc + "<Compute>customerId: $GUID:Ledger:$PartyLedgerName</Compute>\r\n";
                xmlstc = xmlstc + "<Compute>IGSTAmt   : if $$IsEmpty:$$FilterAmtTotal:LedgerEntries:IsIGSTLedger:$Amount then '0' else  $$FilterAmtTotal:LedgerEntries:IsIGSTLedger:$Amount</Compute>\r\n";
                xmlstc = xmlstc + "<Compute>SGSTAmt   : if $$IsEmpty:$$FilterAmtTotal:LedgerEntries:IsSGSTLedger:$Amount then '0' else  $$FilterAmtTotal:LedgerEntries:IsSGSTLedger:$Amount</Compute>\r\n";
                xmlstc = xmlstc + "<Compute>CGSTAmt   : if $$IsEmpty:$$FilterAmtTotal:LedgerEntries:IsSGSTLedger:$Amount then '0' else  $$FilterAmtTotal:LedgerEntries:IsCGSTLedger:$Amount</Compute>\r\n";
                //xmlstc = xmlstc + "<Compute>TAXABLEAmt   :  if $$IsEmpty:$$CollAmtTotal:InventoryEntries:$Amount then '0' else  $$CollAmtTotal:InventoryEntries:$Amount</Compute>\r\n";
                xmlstc = xmlstc + "<Compute>TAXABLEAmt   :  if $$IsEmpty:$Amount then '-0' else $Amount</Compute>\r\n";
                xmlstc = xmlstc + "<Compute>PONumber  :  if $$IsEmpty:$$FullList:InvoiceOrderList:$BasicPurchaseOrderNo then 'NA' else $$FullList:InvoiceOrderList:$BasicPurchaseOrderNo</Compute>\r\n";
                xmlstc = xmlstc + "<Compute>PODate  :  if $$IsEmpty:$$FullList:InvoiceOrderList:$BasicOrderDate then 'NA' else $$FullList:InvoiceOrderList:$BasicOrderDate</Compute>\r\n";
                xmlstc = xmlstc + "<Compute>Paymentterm  : if $$IsEmpty:$BasicDueDateOfPymt then 'NA' else $$Number:$BasicDueDateOfPymt</Compute>\r\n";
                xmlstc = xmlstc + "<Compute>Description  : if $$IsEmpty:$$FullList:InventoryEntries:$StockItemName then 'NA' else $$FullListEx:',':InventoryEntries:$StockitemName:'Qty:':$BilledQty:'Rate:':$Rate:'Item Value:':$Amount:'; '</Compute>\r\n"; //
                xmlstc = xmlstc + "<Compute>PaymentDueDate  : if $$IsEmpty:$LedgerEntries.BillAllocations.BillCreditPeriod then $Date + $$Number:30 else (if $$String:$LedgerEntries.BillAllocations.BillCreditPeriod contains 'Days' then $Date + $$Number:$LedgerEntries.BillAllocations.BillCreditPeriod else $$Date:($$String:($LedgerEntries.BillAllocations.BillCreditPeriod):'Short Date'))</Compute>\r\n";
                xmlstc = xmlstc + "<FILTER>GetFiltertheReport</FILTER>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'GetFiltertheReport'> Not ($IsOptional or $IsCancelled) And ($$IsObjectBelongsTo:Group:($Parent:Ledger:$PartyLedgerName):$$GroupSundryDebtors) And $Date &gt;= $$Date:'" + SVFromDate + "' and $Date  &lt;= $$Date:'" + SVToDate + "' And $alterid &gt; " + lastVchAlterID + " AND $alterid &lt;= " + currentlastVchAlterID + "</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'IsIGSTLedger'>$GSTDutyHead:Ledger:$LedgerName = 'Integrated Tax'</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'IsSGSTLedger'>$GSTDutyHead:Ledger:$LedgerName = 'State Tax'</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'IsCGSTLedger'>$GSTDutyHead:Ledger:$LedgerName = 'Central Tax'</SYSTEM>\r\n";
                xmlstc = xmlstc + "</TDLMESSAGE>\r\n";
                xmlstc = xmlstc + "</TDL>\r\n";
                xmlstc = xmlstc + "</DESC>\r\n";
                xmlstc = xmlstc + "</BODY>\r\n";
                xmlstc = xmlstc + "</ENVELOPE>\r\n";

            
                String xml = xmlstc;
                String lVocuherResponse = aPI.TallySendReqst(xml);
                lVocuherResponseAfterUDFRemove = lVocuherResponse.Replace("UDF:", "");
            }
            catch (Exception ex)
            {

            }

            return lVocuherResponseAfterUDFRemove;
        }

        public string TPO_RecipetvoucherCollectionXMLRequest(string CURRENTCOMPANY, string BOOKBEGINNINGFROM, string LASTVOUCHERENTRYDATE, string LASTVOUCHERID, string CURRENTLASTVOUCHERID, string COMPANYINITIALS)
        {
            String xmlstc = "";
            String lVocuherResponseAfterUDFRemove = "";

            string SVCURRENTCOMPANY = CURRENTCOMPANY;
            string SVFromDate = BOOKBEGINNINGFROM;
            string SVToDate = LASTVOUCHERENTRYDATE;
            string lastVchAlterID = LASTVOUCHERID;
            string currentlastVchAlterID = CURRENTLASTVOUCHERID;


            try
            {
                if (lastVchAlterID == "0")
                {
                    lastVchAlterID = "-1";
                }

                xmlstc = "<ENVELOPE>\r\n";
                xmlstc = xmlstc + "<HEADER>\r\n";
                xmlstc = xmlstc + "<VERSION>1</VERSION>\r\n";
                xmlstc = xmlstc + "<TALLYREQUEST>Export</TALLYREQUEST>\r\n";
                xmlstc = xmlstc + "<TYPE>Collection</TYPE>\r\n";
                xmlstc = xmlstc + "<ID>COLLECTIONOFCREDITRECEIPTVOUCHERDEBTORSSUMM</ID>\r\n";
                xmlstc = xmlstc + "</HEADER>\r\n";
                xmlstc = xmlstc + "<BODY>\r\n";
                xmlstc = xmlstc + "<DESC>\r\n";
                xmlstc = xmlstc + "<STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<SVFROMDATE>" + SVFromDate + "</SVFROMDATE>\r\n";
                xmlstc = xmlstc + "<SVTODATE>" + SVToDate + "</SVTODATE>\r\n";
                xmlstc = xmlstc + "<SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT>\r\n";
                xmlstc = xmlstc + "<SVCURRENTCOMPANY> " + SVCURRENTCOMPANY + " </SVCURRENTCOMPANY>\r\n";
                xmlstc = xmlstc + "</STATICVARIABLES>\r\n";
                xmlstc = xmlstc + "<TDL>\r\n";
                xmlstc = xmlstc + "<TDLMESSAGE>\r\n";

                xmlstc = xmlstc + "<COLLECTION Name = 'CollectionofReceiptVoucher' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<TYPE>Vouchers: VoucherType</TYPE>\r\n";
                xmlstc = xmlstc + "<ChildOf>$$VchTypeReceipt</ChildOf>\r\n";
                xmlstc = xmlstc + "<BelongsTo>Yes</BelongsTo>\r\n";
                xmlstc = xmlstc + "<FETCH>GUID,VoucherTypeName,VoucherNumber,Date,PartyLedgerName,Amount,Narration,,IsOptional,IsCancelled, MasterID, AlterID</FETCH>\r\n";
                xmlstc = xmlstc + "<FILTER>GetFiltertheReport2</FILTER>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";

                xmlstc = xmlstc + "<COLLECTION Name = 'COLLECTIONOFCREDITRECEIPTVOUCHERDEBTORSSUMM' ISMODIFY='No'>\r\n";
                xmlstc = xmlstc + "<SourceCollection>CollectionofReceiptVoucher</SourceCollection>\r\n";
                xmlstc = xmlstc + "<Walk>ALLLedgerEntries, BillAllocations</Walk>\r\n";
                xmlstc = xmlstc + "<Compute> XGUID			: $$Owner:$GUID</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNO		: $$Owner:$$Owner:$VoucherNumber</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHVTYP		: $$Owner:$VOUCHERTYPENAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHDT		: $$Owner:$$Owner:$Date</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XLEDGERNAME	: $GUID:Ledger:($$Owner:$LEDGERNAME)</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XXLEDGERNAME	:  $$Owner:$LEDGERNAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XINVOICENO		: $NAME</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XAMOUNT		: $AMOUNT</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XDRORCRFLAG	: $IsDeemedPositive</Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHALTERID	: $$Owner:$ALTERID </Compute>\r\n";
                xmlstc = xmlstc + "<Compute> XVCHNarration	: $$Owner:$Narration </Compute>\r\n";
                xmlstc = xmlstc + "	<COMPUTE>XPARENTTYPE:  'DEBTORS'</COMPUTE>\r\n";
                xmlstc = xmlstc + "<KeepSource>()..</KeepSource>\r\n";
                xmlstc = xmlstc + "<FILTER>FilterSundryDebtorParty,FilterAlterID</FILTER>\r\n";
                xmlstc = xmlstc + "</COLLECTION>\r\n";

                //xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryCreditorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$XXLEDGERNAME):$$GroupSundryCreditors) And $XDRORCRFLAG = 'No'</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterSundryDebtorParty'>($$IsObjectBelongsTo:Group:($Parent:Ledger:$XXLEDGERNAME):$$GroupSundryDebtors) And $XDRORCRFLAG = 'No'</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'GetFiltertheReport2'>Not ($IsOptional or $IsCancelled) And ($Date &gt;= $$Date:'" + SVFromDate + "' and $Date &lt;=$$Date:'" + SVToDate + "')</SYSTEM>\r\n";
                xmlstc = xmlstc + "<SYSTEM TYPE =  'Formulae'  Name = 'FilterAlterID'> $XVCHALTERID &gt; " + lastVchAlterID + " AND $XVCHALTERID &lt;= " + currentlastVchAlterID + "</SYSTEM>\r\n";
                
                xmlstc = xmlstc + "</TDLMESSAGE>\r\n";
                xmlstc = xmlstc + "</TDL>\r\n";
                xmlstc = xmlstc + "</DESC>\r\n";
                xmlstc = xmlstc + "</BODY>\r\n";
                xmlstc = xmlstc + "</ENVELOPE>\r\n";

                String xml = xmlstc;
                String lVocuherResponse = aPI.TallySendReqst(xml);
                lVocuherResponseAfterUDFRemove = lVocuherResponse.Replace("UDF:", "");
            }
            catch (Exception ex)
            {
                ErrorCodeForRequestModelEnum errorCodeFunc = ErrorCodeForRequestModelEnum.Functiontpo_RecipetvoucherCollectionXMLRequest;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return lVocuherResponseAfterUDFRemove;

        }

    }
}