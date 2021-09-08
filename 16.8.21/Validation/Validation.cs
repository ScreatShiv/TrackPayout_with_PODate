using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CB_TallyConnector.ValidationSignUp
{
    public class ValidationSignUP
    {

        public bool CheckValidContactPerson(string ContactPersonName)
        {
            if (ContactPersonName == "")
            {
                MessageBox.Show("Please Enter Contact Person Name", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                return true;
            }
        }

      
        public bool CheckValidNumber(string ContactNumber)
        {
            if (ContactNumber != "")
            {                
                if (ContactNumber.Length < 10)
                {
                    MessageBox.Show("Enter Correct Phone Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else if (ContactNumber.Length > 10)
                {
                    MessageBox.Show("Enter Correct Phone Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;

                }
                else if (ContactNumber.StartsWith("0"))
                {
                    MessageBox.Show("Dont start With Mobile Number 0", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                MessageBox.Show("Please Enter Valid Contact Number", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

        }

        public bool CheckValidEmail(string ContactEmailID)
        {
            try
            {
                if (ContactEmailID.Length <= 0)
                {
                    MessageBox.Show("Please Enter Email ID", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {

                    MailAddress m = new MailAddress(ContactEmailID);
                    return m.Address == ContactEmailID;
                }

            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid Email ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool CheckValidCompanyID(string CompanyID)
        {
            try
            {
                if(CompanyID == "")
                {
                    MessageBox.Show("Please Enter Track Payout Company ID", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
