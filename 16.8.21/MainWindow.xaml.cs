using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CB_TallyConnector.Configuration;
using CB_TallyConnector.Log;
using CB_TallyConnector.EnDeCryption;
using CB_TallyConnector.Connection;
using CB_TallyConnector.ResponseModel;
using System.Data;
using FluentValidation;
using CB_TallyConnector.ValidationSignUp;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Drawing.Icons;
namespace CB_TallyConnector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        DeCryptFile DecryptFile = new DeCryptFile();
        OtherConnection otherConnection = new OtherConnection();

        ApplicationConfigration applicationConfigration = new ApplicationConfigration();
        APIConnection aPIConnection = new APIConnection();
        XMLResponseModel xMLResponseModel = new XMLResponseModel();

        Logger logger = new Logger();

        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        DispatcherTimer dispatcherTimerforAutoSync = new DispatcherTimer();
        DispatcherTimer dispatcherTimerOTP = new DispatcherTimer();
        DispatcherTimer dispatcherTimerResendOTP = new DispatcherTimer();
        DispatcherTimer dispatcherTimerInvisibleSyncMessage = new DispatcherTimer();


        ValidationSignUP validationsignup = new ValidationSignUP();
        private string companySyncDetailSummary;

        /*  Helper helper = new Helper();*/


        public DataTable CurrentActiveDatatable { get; private set; }
        public string Signuprequest { get; private set; }
        public string OTPNumber { get; private set; }
        public string SignuprequestwithOTP { get; private set; }
        public int OTPVerifycountdown { get; private set; }
        public string CURRENTCOMPANY { get; private set; }
        public string BOOKBEGINNINGFROM { get; private set; }
        public string LASTVOUCHERENTRYDATE { get; private set; }
        public string CURRENTLASTMASTERID { get; private set; }
        public string CURRENTLASTVOUCHERID { get; private set; }
        public string CheckCbRegister { get; private set; }
        public string GUID { get; private set; }
        public string COMPANYINITIALS { get; private set; }
        public string COMPANYNUMBER { get; private set; }
        public string LASTMASTERID { get; private set; }
        public string LASTVOUCHERID { get; private set; }
        public string UpdateEmailrequest { get; private set; }
        public DataTable UpdateCompanyInitailDatatable { get; private set; }
        public string UpdateEmailIdrequestwithOTP { get; private set; }


        private enum ErrorcodeforMainWindow
        {
            FunctiondispatcherTimerforAutoSync_tick,
            FunctionShowConfigureSettingonpannel,
            FunctionSaveConfigurationbutton_Click,
            FunctionWindowsLoad,
            FunctionWindow_Closed,
            FunctionSignUpbutton_Click,
            FunctionSubmitsignupdetailbutton_Click,
            FunctionChoosecompanycombobox_DropDownOpened,
            FunctionverifyOTPbutton_Click,
            FunctionAsyncdatabasecreationonClearBalanceServer,
            FunctionAsyncUpdateEmailonClearBalanceServer,
            FunctionresendOTPbutton_Click,
            FunctionAsyncPostingJSONString,
            FunctionAsyncStartSyncWithCB,
            FunctionUpdateEmailButton_Click,
            FunctionUpdateCompanyinitialComboxbox_DropDownClosed,
            FunctionUpdateEmailIDresendOTPbutton_Click,
            FunctionUpdateEmailIDverifyOTPbutton_Click
        }

        public MainWindow()
        {


            InitializeComponent();

            ShowConfigureSettingonpannel();

            //var obj = Assembly.GetExecutingAssembly().GetName().Version;

            this.versionlabel.Content = "v " + System.Windows.Forms.Application.ProductVersion;
            this.GroupBoxforSignupthecomppany.Visibility = Visibility.Hidden;
            this.Groupboxforotpverification.Visibility = Visibility.Hidden;
            this.GroupBoxforUpdateEmailId.Visibility = Visibility.Hidden;

            this.RectangleforProgressBar.Visibility = Visibility.Hidden;
            this.LoadingDataLabel1.Visibility = Visibility.Hidden;

            this.datagridviewforactivecompany.Visibility = Visibility.Hidden;


            this.Tallyserialnolabel.Visibility = Visibility.Hidden;
            this.TallyserialnoInfoTextBox.Visibility = Visibility.Hidden;

            this.Refreshbutton.IsEnabled = false;

            this.EducationalModeLabel.Visibility = Visibility.Hidden;

            this.SignUpbutton.IsEnabled = false;
            this.UpdateInfobutton.IsEnabled = false;

            this.Refreshbutton.Visibility = Visibility.Hidden;

            this.TallyConfigAlertBox1.Visibility = Visibility.Hidden;
            this.TallyConfigAlertBox2.Visibility = Visibility.Hidden;
            this.TallyConfigAlertBox3.Visibility = Visibility.Hidden;
            this.TallyConfigAlertBox4.Visibility = Visibility.Hidden;
            this.TallyConfigAlertBox5.Visibility = Visibility.Hidden;
            this.TallyConfigAlertBox6.Visibility = Visibility.Hidden;
            this.TallyConfigAlertBox7.Visibility = Visibility.Hidden;

            this.TallyConfigAlterImage.Visibility = Visibility.Hidden;

            this.StackPannelforDisplayingMessageafterSyncProcessComplete.Visibility = Visibility.Hidden;
            this.StackPannelforDisplayingMessageafterSignSuccessFully.Visibility = Visibility.Hidden;

            // InstallMeOnStartUp();
            //InstallCertificate();



        }



        private static void InstallCertificate()
        {
            /*  X509Certificate2 certificate;

              if (String.IsNullOrEmpty(cerPass))
                  certificate = new X509Certificate2(fileName);
              else
                  certificate = new X509Certificate2(cerFileName, cerPass);

              X509Store store = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine);

              store.Open(OpenFlags.ReadWrite);
              store.Add(certificate);
              store.Close();*/

            string file = @"D:\Visual Studio\CB_TallyConnector\cst\Certificate_Clear_Balance.cer";
            X509Store store = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine);

            store.Open(OpenFlags.ReadWrite);
            store.Add(new X509Certificate2(X509Certificate2.CreateFromCertFile(file)));
            store.Close();

        }


        /// <summary>
        ///  Application Startup 
        /// </summary>
        void InstallMeOnStartUp()
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
            }
            catch { }
        }

        private void SaveConfigurationbutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string vTallyIPAddress = "";
                string vTallyPort = "";

                if (!String.IsNullOrWhiteSpace(TallyIPAddress.Text))
                {
                    vTallyIPAddress = TallyIPAddress.Text;
                }
                else
                {
                    vTallyIPAddress = "localhost";
                }

                if (!String.IsNullOrWhiteSpace(TallyPort.Text))
                {
                    vTallyPort = TallyPort.Text;
                }
                else
                {
                    vTallyPort = "9000";
                }



                logger.LogConfiguration(vTallyIPAddress, vTallyPort, "TestCompany", SyncInterval.Text, "192.168.10.177", TallyProduct.Text);

                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Saved Successfully\nTally Connector Restart for the changes  to have effect ?", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information); ;

                if (dialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    System.Windows.Forms.Application.Restart();
                    System.Windows.Application.Current.Shutdown();
                }


            }
            catch (Exception ex)
            {

                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionSaveConfigurationbutton_Click;
                logger.Log(errorCodeFunc + " : " + ex.Message);

            }
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                //// Clear Balance original Logo.jpg    
                //byte[] binaryDataClearBalanceOriginalLogo = Convert.FromBase64String("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");
                //byte[] binaryDataClearBalanceOriginalLogo = Convert.FromBase64String("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");
                byte[] binaryDataClearBalanceOriginalLogo = Convert.FromBase64String("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");

                BitmapImage bitmapImageClearBalanceoriginallogo = new BitmapImage();
                bitmapImageClearBalanceoriginallogo.BeginInit();
                bitmapImageClearBalanceoriginallogo.StreamSource = new MemoryStream(binaryDataClearBalanceOriginalLogo);
                bitmapImageClearBalanceoriginallogo.EndInit();
                ClearBalanceoriginallogo.Source = bitmapImageClearBalanceoriginallogo;


                ////OTP Screen Logo

                byte[] binaryDateOTPSMSgif = Convert.FromBase64String("R0lGODlhAAEAAfcBAAAAAAD/AAEBAQICAgMDAwUFBQYGBgcHBwgICAkJCQoKCgwMDA0NDQ4ODg8PDxAQEBERERMTExQUFBUVFRYWFhcXFxgYGBoaGhsbGxwcHB0dHR4eHh8fHyEhISIiIiMjIyQkJCUlJSYmJigoKCkpKSoqKisrKywsLC0tLS8vLzAwMDExMTIyMjMzMzQ0NDY2Njc3Nzg4ODk5OTo6Ojs7Oz09PT4+Pj8/P0BAQEFBQUJCQkREREVFRUZGRkdHR0hISElJSUtLS0xMTE1NTU5OTk9PT1BQUFJSUlNTU1RUVFVVVVZWVldXV1hYWFpaWltbW1xcXF1dXV5eXl9fX2FhYWJiYmNjY2RkZGVlZWZmZmhoaGlpaWpqamtra2xsbG1tbW9vb3BwcHFxcXJycnNzc3R0dHZ2dnd3d3h4eHl5eXp6ent7e319fX5+fn9/f4CAgIGBgYKCgoSEhIWFhYaGhoeHh4iIiImJiYuLi4yMjI2NjY6Ojo+Pj5CQkJKSkpOTk5SUlJWVlZaWlpeXl5mZmZqampubm5ycnJ2dnZ6enqCgoKGhoaKioqOjo6SkpKWlpaenp6ioqKmpqaqqqqurq6ysrK2tra+vr7CwsLGxsbKysrOzs7S0tLa2tre3t7i4uLm5ubq6uru7u729vb6+vr+/v8DAwMHBwcLCwsTExMXFxcbGxsfHx8jIyMnJycvLy8zMzM3Nzc7Ozs/Pz9DQ0NLS0tPT09TU1NXV1dbW1tfX19nZ2dra2tvb29zc3N3d3d7e3uDg4OHh4eLi4uPj4+Tk5OXl5efn5+jo6Onp6erq6uvr6+zs7O7u7u/v7/Dw8PHx8fLy8vPz8/X19fb29vf39/j4+Pn5+fr6+vz8/P39/f7+/v///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////yH/C05FVFNDQVBFMi4wAwEAAAAh+QQFBAABACwAAAAAAAEAAYcAAAAA/wABAQECAgIDAwMFBQUGBgYHBwcICAgJCQkKCgoMDAwNDQ0ODg4PDw8QEBARERETExMUFBQVFRUWFhYXFxcYGBgaGhobGxscHBwdHR0eHh4fHx8hISEiIiIjIyMkJCQlJSUmJiYoKCgpKSkqKiorKyssLCwtLS0vLy8wMDAxMTEyMjIzMzM0NDQ2NjY3Nzc4ODg5OTk6Ojo7Ozs9PT0+Pj4/Pz9AQEBBQUFCQkJERERFRUVGRkZHR0dISEhJSUlLS0tMTExNTU1OTk5PT09QUFBSUlJTU1NUVFRVVVVWVlZXV1dYWFhaWlpbW1tcXFxdXV1eXl5fX19hYWFiYmJjY2NkZGRlZWVmZmZoaGhpaWlqampra2tsbGxtbW1vb29wcHBxcXFycnJzc3N0dHR2dnZ3d3d4eHh5eXl6enp7e3t9fX1+fn5/f3+AgICBgYGCgoKEhISFhYWGhoaHh4eIiIiJiYmLi4uMjIyNjY2Ojo6Pj4+QkJCSkpKTk5OUlJSVlZWWlpaXl5eZmZmampqbm5ucnJydnZ2enp6goKChoaGioqKjo6OkpKSlpaWnp6eoqKipqamqqqqrq6usrKytra2vr6+wsLCxsbGysrKzs7O0tLS2tra3t7e4uLi5ubm6urq7u7u9vb2+vr6/v7/AwMDBwcHCwsLExMTFxcXGxsbHx8fIyMjJycnLy8vMzMzNzc3Ozs7Pz8/Q0NDS0tLT09PU1NTV1dXW1tbX19fZ2dna2trb29vc3Nzd3d3e3t7g4ODh4eHi4uLj4+Pk5OTl5eXn5+fo6Ojp6enq6urr6+vs7Ozu7u7v7+/w8PDx8fHy8vLz8/P19fX29vb39/f4+Pj5+fn6+vr8/Pz9/f3+/v7///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////8I/wCzcRNIcKDBgggPKkzIcKHDhhAfSoxIcaLFihgvaszIcaPHjty0cdsWcmRJkiJRmkx5siXLlytjqpzpUmZNmjBx2sx5syfPnzuD6hzqU2hRokCRGk3ZsenHp06jQp0qtSrVq1YrLt2qtOvRr0nBchXrNazZsWfLol2LNCvWt27jwp0rty5di2TzptXLlq/av34D7x3cl/BQu4jvJl6M1VowX69Qrar16hcyxZitFt4M2LBgzp/LItu0R4uOEA0ACADAenVrABBQCOECaFSyzqA9/2TM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");
                byte[] binaryDataOTPSMSIcon = Convert.FromBase64String("/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAMCAgMCAgMDAwMEAwMEBQgFBQQEBQoHBwYIDAoMDAsKCwsNDhIQDQ4RDgsLEBYQERMUFRUVDA8XGBYUGBIUFRT/2wBDAQMEBAUEBQkFBQkUDQsNFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBT/wAARCAQABAADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD8qqKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKK1PDvhXWvGGpJp+g6Rf63fv921062e4lb6IgJP5V9JfDj/AIJlftD/ABG8qVfA7+GrKTH+leJLlLLb9YiTN/5DoA+WaK/U74b/APBEOdvKn8f/ABLjj6eZY+G7Itn1xcTEf+iq+kPD3/BJP9nPRbFYLzw/rGvygYN1qGszpIfciAxr/wCO0AfhDRX65ftEf8EZfDl7od5qnwe1u80zWYUaRNB1qYTW1yQCfLjmwGiY9AXLgnGSoyR+T3iPw5qng/Xr/RNbsLjStX0+Zre6srqMpLDIpwysp6EGgDOooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAorb8J+B/EfjzURp/hrQNU8Q35xi10qzkuZef9lFJr6Y+HH/BLf9ob4h+VLL4Sg8J2cnS58R3qW+PrEu+UfilAHyZRX6ufDf8A4Ih2qeVN4/8AiVNN08yx8N2Qjx64nmLZ/wC/Qr6o+G//AATN/Z5+G5ilTwLH4lvEx/pPiS4e93fWJiIf/IdAH4JeGPB+v+NtRGn+HdD1LX789LXS7SS5lP8AwFATX0r8N/8AgmD+0N8RvKlPgxfCtlJj/SvEl0lpt+sQ3TD/AL91+9Ph/wAM6P4R01NP0PSbHRbCP7lrp9skES/REAA/KtOg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Z+K/DuqaJqEfm2GpWstncR/3o5EKMPxDGv5gvGvhW88C+Mte8N6gNt/o9/Pp9wMYxJFI0bfqpr+o+vwH/wCCofw3Pw7/AGyvGDxxeVZeIEg1234xnzk2yn8Z45qAPk+iiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAK6f4XeN7j4afErwr4ttd32jQ9VtdSQKeW8qVX2/jtx+NcxRQB/U/puoW+rafa31pKs9pdRLNDKvR0YAqw9iCKs186/8E9/iR/wtD9j74b6lJL5l3Y6f/Y9xk5YPasYF3e5SNG/4FX0VQAV80/8ABR7wLcfED9jH4k2dpuNxY2cerKF5yttMk8mf+2aPX0tVDxBodp4m0HUtH1CPzrDULaS0uI/70ciFGH4gmgD+Waitzx14Tu/APjfxD4Yvxi+0XUbjTp+MfvIZGjb9VNYdABRRRQAUUUUAFFFFABRRXq37Mv7OfiT9qL4saf4J8ObLcupub/UJhmOxtVZRJMwz82CygKPvMyjIySADymu6+HfwJ+IvxakVPBvgjXvEiMcefp+nySQr/vSAbF+pIr95/gn+wP8ABL4G6VaRaZ4K0/XdXhUeZrfiCBL27kcfxguCsZ9o1UV9CRRJBGkcaLHGgCqijAUDoAKAPw5+G/8AwSB+O/jLypdej0PwPaty39qX4nn2+0duJBn2Zlr6p+HH/BE/wDo/lTeNvHWueJZlwzW+lQx6fAf9k7vNcj3DKfpX6QUUAeD/AA3/AGFvgP8ACnyn0L4Z6LJdR8reatEdQnDf3g9wXKn/AHcV7pb28VrCkMMaQwxjakcahVUDoAB0FSUUAFFcf49+MPgX4W2/neMPGOh+GUxuA1XUIrdn/wB1WYFj7AGvl34kf8FbvgF4G82LSdR1fxvdpwE0TT2WLd7yTmMEe67vxoA+0qK/IX4j/wDBbbxfqIlh8C/D3SdDTlVutcupL6TH94JGIlU+xLD618rfEj/goB8f/ij5qap8StW0+0fj7JobLpsYX+6TAFZh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43U5VlYcgggEEV/SH+yD8RNY+K37Mvw58Va+3mazqGkxtdzc5mkQmMyn3fZuPbLHFfzb1/Rp+wvZ/Yf2P/hJHjG7w/bS8HP313/+zUAe6Vn+IPEGm+FdDv8AWdYvoNM0qxha4ury6kCRQxqMs7MeAABVm9vbfTbOe7u547W0t42lmnmcIkaKMszMeAAASSemK/EX/go1/wAFALj9orXJ/Afge8ltvhpp83724QlG1qZDxIw/54qQCiHqRvbnaFAOX/4KAft3al+1V4tOg+H5Z9P+Gekzk2Vq2UfUZRkfapl+mdiH7oOT8xOPkCivSP2f/wBn/wAX/tJfEax8HeDrH7ReTfvLm7lyLexgBAaaZgPlUZ+pJAAJIFAEXwH+A/i79oz4i2Hg3wbYG71C4O+e4kyILOEEB55nx8qLke5JAAJIB/dj4Q/sP+CPgz+zr4j+GGkxLdXPiTS57LWtduIwJ76WWFo9567UTediA4XryxZj0/7LX7K/g79lH4ex+HfDMH2nULgLJqutToBcahMBjc391Bk7YwcKCepLMfZqAP5YdU0y50XVLzTr2JoLy0me3nibqkiMVZT9CDVWvo3/AIKH/Dj/AIVj+2J8R7BIvKtNQv8A+2bcgYVlulE7Y9hI8i/8Br5yoAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigArvPgL8RH+Enxr8DeMldkj0XWLW8m2/xQrIvmr/wJNy/jXB0UAf1TRyJNGskbK6MAyspyCD0INOrxH9if4kf8LY/ZT+GfiJpfOuX0eKyunzy09vm3lJ9y8TH8a9uoAK+Sv8AgqV8N/8AhYn7G/iyaOLzbzw7Nb65bjHTyn2Sn8IZZj+FfWtYXjvwlaePvBHiHwxfjNjrWn3GnT8Z/dzRtG36MaAP5dKKv+INDu/DOvalo+oR+Tf6fcyWlxH/AHZI3KMPwINUKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAK/pP/ZNthpv7K/wgjc7Nng/SWfcQNpNnEzZ+hJr+bCv0I/a8/b/ADJ8DfBvwR+GWo7LG18OafYeI9ctGx5zLbRq9lCw6IMYkYfe5QcbtwBb/wCCln/BQpvixeX3wr+G+okeCbaTy9X1i2f/AJC8in/VRsP+XdSOv/LQj+6Bu/OyiigDuvgb8G9e/aA+KmgeAvDX2ddX1iVkjkupAkUSIjSSSMepCojtgAk7cAE1/QX+y5+y54Q/ZT+HMPhnwzD9ovptsuqa1MgFxqE4H32/uoMkKgOFB7ksx/Af9mT4kf8ACof2g/h94vaXybbS9Zt5Lp84/wBGZwk4/GJnH41/S3QAUUUUAfkb/wAFuPhv9i8bfDrx7DF8uoWE+jXMijgNA/mxZ9yJ5Pwjr8ya/d7/AIKy/Df/AITz9j7WdSii8y88L6ha6xHtHzbNxgl/AJOzH/c9q/CGgAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigD9kv+CKnxH/ALc+CXjPwZNLvn8P6wt5EpPKwXUfAA9PMglP/A6/RavxA/4I7/Ej/hEf2qJvDc0u218VaPcWixk4BuIcXCH6hI5h/wACr9v6ACiiigD+fL/gpJ8Of+Fa/tkfECCOHyrPV7hNbtzjG/7SgklP/f4zD8K+ZK/UT/gt58N/J174beP4Isi5trjQ7uTHQxsJoB+Pm3H/AHzX5d0AFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABX9Jf7I3xI/4W3+zN8NvFTy+dc3mjQR3Umc7riIeTOf+/sb1/NpX7R/8EYPiR/wkn7PHiLwhNLvufDOtM8aZ+5bXKeYgx7ypcGgD9BaKKKAOW+Knge3+Jnwz8V+Ebrb9n1zSrrTWLdF82Jk3fUFs/hX8w+oWNxpd9c2V3E0F1bStDLE3VHUkMp9wQa/qhr+d7/goJ8N/wDhV37YHxJ0yOLyrO91E6vbYGFKXSichfZXkdf+A0AfPFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAeh/s7/EY/CP46+AvGJfy4NH1m1ubg+sAkAmX8Yy4/Gv6ZFYOoZSGUjII6Gv5WK/pB/Yz+I//C2P2Wvhn4leXz7mfRoba6kzktcQZgmJ9zJEx/GgD2eiiigD5D/4KqfDg/ED9jjxNcxRebeeG7q21uFcc4R/KlP4RTSn8K/BOv6iPiF4PtfiH4D8SeFr7H2LW9NuNNmyM4SaJoyfyav5g9a0e68P6xf6XfR+Te2NxJbTxn+GRGKsPwINAFOiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAr75/4I0/Ej/hF/2ltX8KTS7bbxRo0qRx5+9c25EyH8IvtH518DV6l+y38SP+FRftFfDvxa8vk22m61btdPnGLZ28uf8A8hO9AH9K1FFFABX5Df8ABbb4b/2f8RPh547hi/d6pps2kXDqOA9vJ5iE+7LcMPpH7V+vNfF//BW34bjxx+yDqmqxxeZd+F9StdWTaPm2FjbyD6BZ9x/3PagD8J6KKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAK/Zn/giz8SP+Eg+A/izwbNLvuPDmsi4iXP3Le6jyox/wBdIZz/AMCr8Zq+8f8Agjf8SP8AhFP2oL7wxNLttvFOjTQRx5xuuICJ0P4RpP8AnQB+29FFFABX89f/AAUa+G//AArL9sb4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R2EkQMYP+84r5V+JP/BbrRbbzYPAPw3vr9uQl74ivEt1HuYYt5Yf9tFr8lqKAPd/2kP21vip+1JJHB4w1qO30OF/Mh0DSUNvYo46MU3FpGHZpGYjnGMmvCKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACvV/2UfiQPhJ+0l8OfFby+TbWGtW4upM4xbSN5U/8A5CkevKKKAP6qKK8t/Zb+JB+Ln7Ovw78WvL51zqWi27XT5zm5RBHP/wCRUevUqACiiigAooooAKKKKACiiigAooooAKKKqapq9joVjLe6le2+n2UQzJcXUqxRoPUsxAFAFuivm34kf8FFv2fPhj5sd78RNP1m8TgWvh5X1FmI7b4gYwf95xXyr8SP+C3Wg2vmw+AvhxqGpN0S88Q3iWqj38mLzCw/4GtAH6eVU1TV7HQrGW91K9t9PsohmS4upVijQepZiAK/CT4kf8FXP2g/iB5sVl4g0/wbZycGDw9YIjY/66y+ZID7qwr5g8afEjxZ8R777b4r8T6x4lu85E2rX0tyw+hdjj8KAP3z+JH/AAUW/Z8+GPmx3vxE0/WbxOBa+HlfUWYjtviBjB/3nFfKvxI/4LdaDa+bD4C+HGoak3RLzxDeJaqPfyYvMLD/AIGtfknRQB9g/Er/AIKtftB/EESw2XiCw8GWcnBg8O2Kxtj/AK6ymSQH3VhXy/4y+Inir4i3/wBu8VeJdX8S3mcifVr6W6cfQuxxXPUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQB+13/AARp+JH/AAlH7NOr+FJpd1z4X1qVI48/dtrgCZD+Mv2j8q++q/ET/gkD8b7P4Z/tE3/hPVbpbTTvGtktlA0hwpvomL26k9PmVpkHqzqO9ft3QAUUUUAFFFVtS1Oz0eylvL+7gsbSIbpLi5kWONB6liQBQBZor5w+JX/BQ/8AZ++F3mxah8RdO1e9TIFp4fDai5I/h3Qho1P+8wr5U+JH/Bbrw9Z+bD4C+HOo6o3RLzxBeJaKPfyovMLD23qaAP06qrqeq2Wi2Mt7qN5b2FnEN0lxdSrHGg9SzEAV+EvxI/4KvftBePvNisNe07wZZycGHw/YKrY/66zGSQH3Vlr5f8bfEzxd8Sr77Z4s8Uax4mus5EurX8tyy/Texx+FAH76fEj/AIKJfs+/DHzY7/4jadq94nAtfD4bUWYj+HfCGjU/7zCvlX4kf8FuvD1n5sPgL4c6jqjdEvPEF4loo9/Ki8wsPbepr8kaKAPsT4kf8FXv2gvH3mxWGvad4Ms5ODD4fsFVsf8AXWYySA+6stfL/jb4l+LviVffbfFvijWPE10DkTavfy3LL9N7HH0Fc3RQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQBJa3U1jcxXFvLJb3ELiSOaJirowOQwI5BB5yK/UP9lj/AILFLoeg2Xhz406ZfalLaoIovFOlIsk0qgAD7TCSMt6yIcnjKZyx/LiigD9773/gqx+zVa6d9pi8cXd5NjP2OHQ74S/TLwqn/j1eA/Ej/gtx4bsvNh8BfDrUtWbol5r92logPr5UXmFh7b1NfkdRQB9j/Ej/AIKxftA+PvNi0/W9N8F2cnHk6BYKH2/9dZjI4PupWvl7xv8AE/xh8S737X4t8U6z4muc7hJq1/LclfpvY4+grmaKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigD/2Q==");
                BitmapImage bitmapImageOTPScreenlogo = new BitmapImage();
                bitmapImageOTPScreenlogo.BeginInit();
                bitmapImageOTPScreenlogo.StreamSource = new MemoryStream(binaryDateOTPSMSgif);
                bitmapImageOTPScreenlogo.EndInit();
                outverificationenterLogo.Source = bitmapImageOTPScreenlogo;

                //CloseWindow
                byte[] binaryDataCloseIcon = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAADQAAAA0CAYAAADFeBvrAAAABmJLR0QA/wD/AP+gvaeTAAACCUlEQVRoge2au04CQRSGv93GaAPiU4idNr4CorbGB/CSqPBY3gs1vIQRsKIkNsYCTMQKKixmN9ngZi8zB2Yg/Mnp9kz+L2dnz5nNQLyKwDnQAD6AITC2HMPASwM4CzymygPqwLcDAGnRB2qB51itAQ8OGM0b98BqXGVuHDCnG49MVKrugCnTuAphiszHnkmLPlDwgWOgxPxrAzjygQPbTgR16ANbtl0IquyhGtaKbSdCGnmoDbUw8m0bkNYSyHVJAP0C+0AZaGvkv6O+tFVgIODHqDsPgN3IWkXgNUd+G9UQQ+1gPrWIweSFmoQJtW0IpZ1YjTETqgQ0E3JbJI9bFRtA5QRDSVBpMARrzxwoi7FJqCw568CbDaCkfRBVuKfyPGviySh5jKpAlkpNuzJiQFkrlSSJyogCmUBJwogC6UBJw4gD5YGaBsx44YZTWL5yYjDTgrIKMw0o4wWkGmvaQDsToIUafXSG06zVbNkA0j0+ZIGycnyoaMBkhbJywPthwY7gcVBz/ZMkClVFvfc6n90msAnsBWsZ+Vn+23ZdSyDX5QMj2yYENfKBL9suBPXpAx3bLgTV8YFn2y4E9QSqu/cxb7C2owcUQrKaA4ZM4zJaKg+4dsCUbvy7vATqitadA+byxi0x18tCeahbTfOwp3rARVxl4lQAToEXoIs7VzS7gacTIh+AqP4AWwxn9lKRCf0AAAAASUVORK5CYII=");
                BitmapImage bitmapImageCloselogo = new BitmapImage();
                bitmapImageCloselogo.BeginInit();
                bitmapImageCloselogo.StreamSource = new MemoryStream(binaryDataCloseIcon);
                bitmapImageCloselogo.EndInit();
                CloseWindowlogo.Source = bitmapImageCloselogo;

                //iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAD+SURBVEhL7ZUxDkRQEIZ/WykkWiU3cASHcABuoFNyBafQSBQOQKFwCIlr6OwbO7KetayEQrJf8uLNJOafmWTmKYMAF/Lg72VcLvC1RXVdo21bto7heR7fNgR830dVVTBNkz379H2PpmkghSSBNUQWQxRFbP1G13UUma0XUgWU9QS1SNM02LbNnm3CMISqqrAsS6pAElAUBUEQQNd19vxGkiTI83xs51JAqodMKvMoIvBQluVqi+4/B3+BXe4vcNoc0L/TmXNaBWmaQiQ3njmnCRiGMa6K5fb92EWu645L7ghZlqEoCjiOw543kkAcx3w7Dj0yy+wJSeAK7j4HwBNe81RnVT46TgAAAABJRU5ErkJggg==
                //MinimiseWindow

                string minibase64str = "iVBORw0KGgoAAAANSUhEUgAAADQAAAA0CAYAAADFeBvrAAAABmJLR0QA/wD/AP+gvaeTAAABcklEQVRoge2avU7DMBRGT7wgWFLB2r4APAusiAfgRwLax+KnMABKeZOOFQtiaNibCYYkUgBD7cbi3lQ+0rdF0flk2ZGcC3Z6wBmQAS/AAvgQzqJyyYDTynEpCTAC3hUUWJYcGFbOVraAOwWivhkDm7aVuVYgt2ru+bZSIwVSbXNZl+nRjT2zLDmQGuAI2Kb77ACHBtiXNgnIgQH2pC0CsptQfrA2pE0CUSSUG2ptMNICoYmFtBMLaScW0k4spJ1YSDuxkHZiIe3EQtqJhbQTC3UB16vWDOgL+PWBiYen84MSZWoGf3h9ic+93K8/lv4JJ8+120OxkHZ8CkkfCs64nnIT3xcHYgA8u3rGvw/aMUAhLRGQwgBv0hYBeTXAVNoiIFMDPEpbBOQBysGLHPnBibaZA2ndbKhAqG0umkuVAFcKpFbNj+ElKEe0bhXI+eYGy3hZTUI51dSFPTUHzm0rYyMFToAnYIaeEc1Z5XRM4wBo8gncLmz1KcWRJQAAAABJRU5ErkJggg==";
                byte[] binaryDataMinimiseIcon = Convert.FromBase64String(minibase64str);
                BitmapImage bitmapImageMinimiselogo = new BitmapImage();
                bitmapImageMinimiselogo.BeginInit();
                bitmapImageMinimiselogo.StreamSource = new MemoryStream(binaryDataMinimiseIcon);
                bitmapImageMinimiselogo.EndInit();
                MinimiseWindowlogo.Source = bitmapImageMinimiselogo;

                //RestartWindow
                byte[] binaryDataRestartIcon = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAADQAAAA0CAYAAADFeBvrAAAABmJLR0QA/wD/AP+gvaeTAAAD9UlEQVRogc2az28VVRTHP28e2AI+Ad1gExf+KAgESKz8AWDAhbbuFH/Biv4FJGz40SVhwYaw4A+QH2nihh1YtaDFGOLCgNEqGI02bvjRPiNgWh6LMy3Teefcmbkzd+gnuUlz55xzv3fufWfujzYoRx8wkCjrgO0lY5ZiWQHbtPgB4MWUzYWKdHljdSiPeI0fKtC0F/gMmPMN0AcMAiPIG54COp7lHV8RCTrALWAYaPoEGMkpNk/p8+/HAsl4V4FXfYIczCE2q/xdohNJ0nFnkNEqTNmRqiohWPFPA1HRYGVG6mi5fizgauMssLxoQN+RGizXjwUmM9r5HI9O+YxUnrSehxXAMWDW0daJokF3AY8cAdNlqmQnNN4F2o429+QNtA74xxEoZEJI84ZDy7/AxjxBzhsB6kgIGluAe0a7Y1nOuw3HrOlXVUKweBtZCmltD1lODeCa4XQSd/arYoWQxXGj7UngGc1hyHD4A2jFNlqnQiQEjR7ghqHxA83homH8Xsou3ak6twyDhsZLacOX0X8n15GpmCbZqZEAwi0awPd065wDXkkaHlCMOsB+R/D5ToVOCGk+RNd6OGl0WTGYRr7aLkaoJyEkWYX+wV2Ydj3AfcXgqW+nHZylW+8M0IyAbUCv4vRVbfKK861S1wK2RsB6w2k8nJ7SXDPqt0ZIhtP4M5CYKrhh1L8UAc8rD+aA2+H0lKYN/K/UvxAhWSPNbeS7tJS5q9StipAMkWY2sJgq0F74owjZV6RpKXVLjeeUunaEfEDTPBuXpUoLWKnUT0fA78qDBtAfVFI51qOvMW9FwM+G05vh9JRmwKj/BSTLPaR7KXGmFml+nKNb7wMS0/CKYnAHWectNXqRlJ3W+zU8OVr9QnFcS/1bgzwMAWuU+kUHJv3oG7yrodV58A36fmhD2vA7w/CtupTmwDqVUl/8J4bxdTzOkgOwDPgRXeNHlsNvhsOR8HozsY7RJnHc9H1sOM0CO8LqdbIT+/A+84x7zHC8h+xu62YzsvrXNI2jrxgW8TqyYNUC/BU3UBeb4zY1LW3s3XYX+4wgHeRt1TH9diIfd0vHp0UDnnIEm0VuHEJkv+VIAnBdeJ30CdwERh1BO8jeflcp+YvZjX1+PV9G8bg8nqeH7E51gAngffTjsCx6Y9+JHO2MUsH6sol7+iXLXWQlPIws8bWdbyt+NhzbWpdZ2jTzHhmNvdjZz1VmkCuXqfjvov5tZBUThA3Alx6ifMsYBVKzLw3kBuDXgB2ZpMAtd1U0kUVhnh9z3jKBvCyv/8Kqkn7kfmYcfTtvlYexzyEqOpTJXAd5sBLYhMz915DjsNXxs2nkR34TmVY/Af9V2fhjx5+6J3VnY38AAAAASUVORK5CYII=");
                BitmapImage bitmapImageRestartlogo = new BitmapImage();
                bitmapImageRestartlogo.BeginInit();
                bitmapImageRestartlogo.StreamSource = new MemoryStream(binaryDataRestartIcon);
                bitmapImageRestartlogo.EndInit();
                RestartWindowlogo.Source = bitmapImageRestartlogo;




                DispatcherTimer dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Start();

                this.lable8.Content = "Disconnected";
                this.InternetConnectedImageMark.Background = Brushes.Red;

                this.lable6.Content = "Disonnected";
                this.TallyConnectedImageMark.Background = Brushes.Red;

                bool InternetStatus = await Task.Run(() => otherConnection.IsConnectedToInternet());
                string tallystatus = otherConnection.TallyIsConnected;


                if (InternetStatus)
                {
                    this.lable8.Content = "Connected";
                    this.InternetConnectedImageMark.Background = Brushes.Green;


                    if (applicationConfigration.TallyProductName == "Tally.ERP9")
                    {
                        if (tallystatus == "Tally.ERP 9 Server is Running")
                        {
                            this.lable6.Content = "Connected";
                            this.TallyConnectedImageMark.Background = Brushes.Green;

                            string TallyLicenseNumberInfo = await Task.Run(() => otherConnection.TallyLicenseInfoResponseFromTally());

                            if (TallyLicenseNumberInfo == "Nil")
                            {
                                this.EducationalModeLabel.Visibility = Visibility.Visible;
                                this.Refreshbutton.IsEnabled = false;

                                this.Tallyserialnolabel.Visibility = Visibility.Hidden;
                                this.TallyserialnoInfoTextBox.Visibility = Visibility.Hidden;

                                this.TallyConfigAlertBox1.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox2.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox3.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox4.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox5.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox6.Visibility = Visibility.Hidden;

                                this.TallyConfigAlterImage.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox7.Visibility = Visibility.Hidden;
                                this.SignUpbutton.IsEnabled = false;
                                this.UpdateInfobutton.IsEnabled = false;
                                this.dispatcherTimerforAutoSync.Start();

                            }
                            else
                            if (TallyLicenseNumberInfo == "0")
                            {

                                this.EducationalModeLabel.Visibility = Visibility.Visible;
                                this.Refreshbutton.IsEnabled = false;

                                this.Tallyserialnolabel.Visibility = Visibility.Hidden;
                                this.TallyserialnoInfoTextBox.Visibility = Visibility.Hidden;

                                this.TallyConfigAlertBox1.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox2.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox3.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox4.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox5.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox6.Visibility = Visibility.Hidden;

                                this.TallyConfigAlterImage.Visibility = Visibility.Hidden;

                                this.TallyConfigAlertBox7.Visibility = Visibility.Hidden;

                                this.SignUpbutton.IsEnabled = false;
                                this.UpdateInfobutton.IsEnabled = false;
                                this.dispatcherTimerforAutoSync.Start();
                            }
                            else
                            {
                                this.TallyserialnoInfoTextBox.Text = TallyLicenseNumberInfo;
                                this.EducationalModeLabel.Visibility = Visibility.Hidden;

                                this.Refreshbutton.IsEnabled = true;

                                this.Tallyserialnolabel.Visibility = Visibility.Visible;
                                this.TallyserialnoInfoTextBox.Visibility = Visibility.Visible;


                                this.TallyConfigAlertBox1.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox2.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox3.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox4.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox5.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox6.Visibility = Visibility.Hidden;

                                this.TallyConfigAlterImage.Visibility = Visibility.Hidden;


                                this.GroupBoxforTallyCompany.Visibility = Visibility.Visible;
                                this.SignUpbutton.IsEnabled = true;
                                this.UpdateInfobutton.IsEnabled = true;
                                this.dispatcherTimerforAutoSync.Start();


                            }

                            // this.dispatcherTimerforAutoSync.Start();

                        }
                        else
                        {
                            this.lable6.Content = "Disonnected";
                            this.TallyConnectedImageMark.Background = Brushes.Red;

                            this.EducationalModeLabel.Visibility = Visibility.Hidden;

                            this.TallyConfigAlterImage.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox1.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox2.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox3.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox4.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox5.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox6.Visibility = Visibility.Visible;


                            this.GroupBoxforTallyCompany.Visibility = Visibility.Hidden;

                            this.SignUpbutton.IsEnabled = false;
                            this.UpdateInfobutton.IsEnabled = false;
                            this.dispatcherTimerforAutoSync.Start();
                        }
                    }
                    else
                    {
                        if (tallystatus == "TallyPrime Server is Running")
                        {
                            this.lable6.Content = "Connected";
                            this.TallyConnectedImageMark.Background = Brushes.Green;

                            string TallyLicenseNumberInfo = await Task.Run(() => otherConnection.TallyLicenseInfoResponseFromTally());

                            if (TallyLicenseNumberInfo == "Nil")
                            {
                                this.EducationalModeLabel.Visibility = Visibility.Visible;
                                this.Refreshbutton.IsEnabled = false;

                                this.Tallyserialnolabel.Visibility = Visibility.Hidden;
                                this.TallyserialnoInfoTextBox.Visibility = Visibility.Hidden;

                                this.TallyConfigAlertBox1.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox2.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox3.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox4.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox5.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox6.Visibility = Visibility.Hidden;

                                this.TallyConfigAlterImage.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox7.Visibility = Visibility.Hidden;
                                this.SignUpbutton.IsEnabled = false;
                                this.UpdateInfobutton.IsEnabled = false;
                                this.dispatcherTimerforAutoSync.Start();

                            }
                            else
                            if (TallyLicenseNumberInfo == "0")
                            {

                                this.EducationalModeLabel.Visibility = Visibility.Visible;
                                this.Refreshbutton.IsEnabled = false;

                                this.Tallyserialnolabel.Visibility = Visibility.Hidden;
                                this.TallyserialnoInfoTextBox.Visibility = Visibility.Hidden;

                                this.TallyConfigAlertBox1.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox2.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox3.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox4.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox5.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox6.Visibility = Visibility.Hidden;

                                this.TallyConfigAlterImage.Visibility = Visibility.Hidden;

                                this.TallyConfigAlertBox7.Visibility = Visibility.Hidden;

                                this.SignUpbutton.IsEnabled = false;
                                this.UpdateInfobutton.IsEnabled = false;
                                this.dispatcherTimerforAutoSync.Start();
                            }
                            else
                            {
                                this.TallyserialnoInfoTextBox.Text = TallyLicenseNumberInfo;
                                this.EducationalModeLabel.Visibility = Visibility.Hidden;

                                this.Refreshbutton.IsEnabled = true;

                                this.Tallyserialnolabel.Visibility = Visibility.Visible;
                                this.TallyserialnoInfoTextBox.Visibility = Visibility.Visible;


                                this.TallyConfigAlertBox1.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox2.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox3.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox4.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox5.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox6.Visibility = Visibility.Hidden;

                                this.TallyConfigAlterImage.Visibility = Visibility.Hidden;


                                this.GroupBoxforTallyCompany.Visibility = Visibility.Visible;
                                this.SignUpbutton.IsEnabled = true;
                                this.UpdateInfobutton.IsEnabled = true;
                                this.dispatcherTimerforAutoSync.Start();


                            }

                            // this.dispatcherTimerforAutoSync.Start();

                        }
                        else
                        {
                            this.lable6.Content = "Disonnected";
                            this.TallyConnectedImageMark.Background = Brushes.Red;

                            this.EducationalModeLabel.Visibility = Visibility.Hidden;

                            this.TallyConfigAlterImage.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox1.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox2.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox3.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox4.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox5.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox6.Visibility = Visibility.Visible;


                            this.GroupBoxforTallyCompany.Visibility = Visibility.Hidden;

                            this.SignUpbutton.IsEnabled = false;
                            this.UpdateInfobutton.IsEnabled = false;
                            this.dispatcherTimerforAutoSync.Start();
                        }
                    }

                }
                else
                {
                    this.lable8.Content = "Disconnected";
                    this.InternetConnectedImageMark.Background = Brushes.Red;

                    this.TallyConfigAlertBox7.Visibility = Visibility.Visible;
                    this.TallyConfigAlterImage.Visibility = Visibility.Visible;

                    this.SignUpbutton.IsEnabled = false;
                    this.GroupBoxforTallyCompany.Visibility = Visibility.Hidden;

                    this.dispatcherTimerforAutoSync.Start();

                }
            }
            catch (ApplicationException ex)
            {
                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionWindowsLoad;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.CurrentDateTimelable.Content = DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {

                EnCryptFile enCrypt = new EnCryptFile();

            }
            catch (ApplicationException ex)
            {

                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionWindow_Closed;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }


        }

        private void SignUpbutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.GroupBoxforTallyCompany.Visibility = Visibility.Hidden;
                this.GroupBoxforSignupthecomppany.Visibility = Visibility.Visible;

                this.Refreshbutton.IsEnabled = false;

                this.SaveConfigurationbutton.IsEnabled = false;
                this.SignUpbutton.IsEnabled = false;
                this.UpdateInfobutton.IsEnabled = false;

                this.TallyIPAddress.IsReadOnly = true;
                this.TallyPort.IsReadOnly = true;
                this.TallyProduct.IsEnabled = false;
                this.SyncInterval.IsEnabled = false;

                this.TallyConfigAlertBox1.Visibility = Visibility.Hidden;
                this.TallyConfigAlertBox2.Visibility = Visibility.Hidden;
                this.TallyConfigAlertBox3.Visibility = Visibility.Hidden;
                this.TallyConfigAlertBox4.Visibility = Visibility.Hidden;
                this.TallyConfigAlertBox5.Visibility = Visibility.Hidden;
                this.TallyConfigAlertBox6.Visibility = Visibility.Hidden;

                this.TallyConfigAlterImage.Visibility = Visibility.Hidden;
                this.TallyConfigAlertBox7.Visibility = Visibility.Hidden;

                this.StackPannelforDisplayingMessageafterSyncProcessComplete.Visibility = Visibility.Hidden;

                dispatcherTimerforAutoSync.Stop();
            }
            catch (Exception ex)
            {
                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionSignUpbutton_Click;
                logger.Log(errorCodeFunc + " : " + ex.Message);

            }

        }

        private async void Submitsignupdetailbutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.CloseWindow.IsEnabled = false;

                this.Groupboxforotpverification.Visibility = Visibility.Hidden;
                this.SignUpbutton.IsEnabled = false;
                this.SaveConfigurationbutton.IsEnabled = false;
                this.resendOTPbutton.IsEnabled = false;
                this.Refreshbutton.IsEnabled = false;

                bool InternetStatus = await Task.Run(() => otherConnection.IsConnectedToInternet());
                if (validationsignup.CheckValidCompanyID(Companyinitialtextbox.Text))
                {
                    if (InternetStatus)
                    {
                        this.lable8.Content = "Connected";
                        this.InternetConnectedImageMark.Background = Brushes.Green;


                        Track_Payout.Properties.Settings.Default.v1 = Choosecompanycombobox.Text;
                        Track_Payout.Properties.Settings.Default.v2 = Contactpersontextbox.Text;
                        Track_Payout.Properties.Settings.Default.v3 = Phonenumbertextbox.Text;
                        Track_Payout.Properties.Settings.Default.v4 = Emailaddresstextbox.Text;
                        Track_Payout.Properties.Settings.Default.v5 = Subscriptionstartfromtextbox.Text;
                        Track_Payout.Properties.Settings.Default.v6 = "0";
                        Track_Payout.Properties.Settings.Default.v7 = Validuntiltextbox.Text;
                        Track_Payout.Properties.Settings.Default.v8 = Companyinitialtextbox.Text;
                        Track_Payout.Properties.Settings.Default.v9 = Citytextbox.Text;
                        Track_Payout.Properties.Settings.Default.v10 = CompanyGUIDtextbox.Text;
                        Track_Payout.Properties.Settings.Default.v11 = AlterMasterIDtextbox.Text;
                        Track_Payout.Properties.Settings.Default.v12 = AlterVoucherIDtextbox.Text;
                        Track_Payout.Properties.Settings.Default.v13 = SystemMACAddresstextbox.Text;
                        Track_Payout.Properties.Settings.Default.Save();

                        this.Signuprequest = "{" + '"' + "company_id" + '"' + ":" + '"' + Companyinitialtextbox.Text + '"' + "," +
                                                   '"' + "company_gu_id" + '"' + ":" + '"' + CompanyGUIDtextbox.Text + '"' +
                                                    "}";

                        JObject c = JObject.Parse(aPIConnection.SignUpOnTrackpayoutWebServer(Signuprequest));


                        var responseMsg = (string)c.SelectToken("message");

                        var Signupstatus = (string)c.SelectToken("status");

                        var Signupotp = "";// (string)c.SelectToken("result.otp");

                        //Clear_Balance.Properties.Settings.Default.v14 = Signupotp;
                        //Clear_Balance.Properties.Settings.Default.Save();

                        if (Signupstatus == "200")
                        {

                            string currentdateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                            //string currentdateTime = "2021-06-07 00:00:00";// DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString(); ;
                            string vlastvoucheralterId = "-1";
                            string vlastmasteralterId = "-1";
                            string setlastdetails = "{" + '"' + "company_id" + '"' + ":" + '"' + Companyinitialtextbox.Text + '"' + "," +
                                                            '"' + "last_master_alter_id" + '"' + ":" + '"' + vlastmasteralterId + '"' + "," +
                                                   '"' + "last_voucher_alter_id" + '"' + ":" + '"' + vlastvoucheralterId + '"' + "," +
                                                  '"' + "last_sync_date" + '"' + ":" + '"' + currentdateTime + '"' +
                                                    "}";

                            string setinitailstatus = aPIConnection.SetLastDetailOnTrackpayoutWebServer(setlastdetails);

                            StackPannelforDisplayingMessageafterSignSuccessFully.Visibility = Visibility.Visible;
                            MessageafterSignUpComplete1.Content = responseMsg;
                            MessageafterSignUpComplete2.Visibility = Visibility.Hidden;
                            MessageafterSignUpComplete3.Visibility = Visibility.Hidden;
                            MessageafterSignUpComplete4.Visibility = Visibility.Hidden;

                            this.OkButtonafterSignUpComplete.Visibility = Visibility.Visible;

                            this.OkButtonifOTPmismatch.Visibility = Visibility.Hidden;


                            byte[] binarySignupIcon = Convert.FromBase64String("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");
                            BitmapImage bitmapImageSignuplogo = new BitmapImage();
                            bitmapImageSignuplogo.BeginInit();
                            bitmapImageSignuplogo.StreamSource = new MemoryStream(binarySignupIcon);
                            bitmapImageSignuplogo.EndInit();
                            ImageforSignupSuccesfully.Source = bitmapImageSignuplogo;

                            //Clear_Balance.Properties.Settings.Default.v8
                            //System.Windows.Forms.MessageBox.Show(responseMsg, "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);



                        }
                        else
                        {


                            StackPannelforDisplayingMessageafterSignSuccessFully.Visibility = Visibility.Visible;

                            byte[] binarySignupIcon = Convert.FromBase64String("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AACYXL51ev75iC1bygKD4S5/WCzZF+Cee5rjWAzHsWMRl12mySowMQEAMBl3/Q9XlvnfdlvE8eNlAVgYeQtLNgpUlTUcBw40xwGyIgBgTK4BBCaTDahs/ocl3zBde21z7ZjNPyymxx+PuPjipnqHYdi61fWAwMQEAMD4brkl4vbby4RBePDB5j7/3DwAiy2P8Nx6a1M67vz4MGRlRwbvAGNyBAAYz44dEU89pdnUUGSTv5tv1mkahip/V+dtAR//eHNzAIstf19nYAvQkgAAaM91f8ORHf0/97mIO+/U5A9qsGFDxAMPRGzfXhZYSPm7+6qrIvbuLQsAoxEAAO3kVVPf/nbzyWLLO/3f977mE6hLXheYtwWsWVMWWDiuBwTGoAcAMLosIf3KV2z+F12+6b/jjubB0eYf6vTww02/j6efLgssnKzCy1t4VOMBLQgAgNHddZe7pRddnvHPh/57721KSIF6nTwZ8e53N40CXS23mPJI3pe+VCYAy3MEABhNNv175pkyYeFkN/APf7h56wfwRhs3NhVeW7aUBRZKhjiufARGIAAAlrd+fXPuf/XqssDCyW7R2TUa4ELymNenPtVc7+qGl8WSFV153ePBg2UB4PwEAMDS8rqob3zDW6EhyG7/WQUAsJRt25pqgAx/WRzHjjW9XfJoB8AF6AEALO13f9fmfyh+8zebf58AS9m/P+KSSyIeeaQssBAysMngBmAJP/HbEa/+ATiPvCbqd36nTBiErVuboxzPPlsWAM4jbwt54omI7363af6a1WD034YNEW96U9PwFeA8HAEAzi87C7/wguuFhspxAGBUuanMMCC/F+i/7Adw1VURe/eWBYDXCACAH5eb/mz6lw99DJcQABhVfi/kdXM33FAW6LXsA5DHOI4fLwsADT0AgB/30EM2/zXQEwAY1enTETfeGHHHHc0bZvptzZqIxx5zmwPwYwQAwI/KTeE115QJgycEANq4996IK6+MOHGiLNBb2fPlnnvKBKDhCADwmrz66fnnvTGokeMAQBvr1jV9AdwS03/XXx+xe3eZALUTAACN7Ayf5/7d+1wvIQDQRt4M8IUvRLz//WWBXjp1qukHcOxYWQBq5ggA0MgycJv/ujkOALSRVwXefHPzoy9Af2XAn719AF4lAACaM/955z8IAYC2Hnww4vLL9QXos+3bI26/vUyAmjkC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AAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACACggAAAAAoAICAAAAAKiAAAAAAAAqIAAAAACAwYv4/wFDXoWXPHg6xQAAAABJRU5ErkJggg==");
                            BitmapImage bitmapImageSignuplogo = new BitmapImage();
                            bitmapImageSignuplogo.BeginInit();
                            bitmapImageSignuplogo.StreamSource = new MemoryStream(binarySignupIcon);
                            bitmapImageSignuplogo.EndInit();
                            ImageforSignupSuccesfully.Source = bitmapImageSignuplogo;

                            this.MessageafterSignUpComplete1.Foreground = Brushes.Red;
                            MessageafterSignUpComplete1.Content = responseMsg;
                            MessageafterSignUpComplete2.Visibility = Visibility.Hidden;
                            MessageafterSignUpComplete3.Visibility = Visibility.Hidden;
                            MessageafterSignUpComplete4.Visibility = Visibility.Hidden;

                            this.OkButtonafterSignUpComplete.Visibility = Visibility.Hidden;

                            this.OkButtonifOTPmismatch.Visibility = Visibility.Visible;

                            //System.Windows.Forms.MessageBox.Show(responseMsg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        }

                        //if (Signupstatus == "2021N")
                        //{
                        //    ClearSignUptextbox();
                        //    this.GroupBoxforSignupthecomppany.Visibility = Visibility.Hidden;
                        //    this.Groupboxforotpverification.Visibility = Visibility.Visible;
                        //    this.otpverificationMessagelabel4.Content = Signupotp;
                        //    this.UpdateEmailIDverifyOTPbutton.Visibility = Visibility.Hidden;
                        //    this.UpdateEmailIDresendOTPbutton.Visibility = Visibility.Hidden;
                        //    this.OTPVerifycountdown = 60;

                        //    dispatcherTimerOTP.Tick += new EventHandler(dispatcherTimerOTP_Tick);
                        //    dispatcherTimerOTP.Interval = new TimeSpan(0, 0, 1);
                        //    dispatcherTimerOTP.Start();
                        //    otpverificationMessagelabel3.Content = "OTP will be invalid in " + this.OTPVerifycountdown.ToString() + " sec";

                        //}


                    }
                    else
                    {
                        this.lable8.Content = "Disconnected";
                        this.InternetConnectedImageMark.Background = Brushes.Red;
                    }
                }




            }
            catch (Exception ex)
            {
                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionSubmitsignupdetailbutton_Click;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }
        }


        private void dispatcherTimerOTP_Tick(object sender, EventArgs e)
        {
            OTPVerifycountdown--;

            if (OTPVerifycountdown == 0)
            {

                dispatcherTimerOTP.Stop();
                this.resendOTPbutton.IsEnabled = true;
                this.UpdateEmailIDresendOTPbutton.IsEnabled = true;

                this.otpverificationMessagelabel5.Visibility = Visibility.Hidden;

                this.verifyOTPbutton.IsEnabled = false;
                this.UpdateEmailIDverifyOTPbutton.IsEnabled = false;

                otpverificationMessagelabel3.Content = "if you didn't received OTP please click on resend?";

            }
            else
            {
                otpverificationMessagelabel3.Content = "OTP will be invalid in " + OTPVerifycountdown.ToString() + " sec";
            }
        }

        public void ClearSignUptextbox()
        {
            Choosecompanycombobox.Text = "";
            Contactpersontextbox.Text = "";
            Phonenumbertextbox.Text = "";
            Emailaddresstextbox.Text = "";
            Subscriptionstartfromtextbox.Text = "";
            Validuntiltextbox.Text = "";
            Companyinitialtextbox.Text = "";
            Citytextbox.Text = "";
            CompanyGUIDtextbox.Text = "";
            SystemMACAddresstextbox.Text = "";
            AlterMasterIDtextbox.Text = "";
            AlterVoucherIDtextbox.Text = "";


        }



        private void Choosecompanycombobox_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                this.CurrentActiveDatatable = xMLResponseModel.TallyActiveCompanyXMLResponseforSignup();
                this.Choosecompanycombobox.ItemsSource = CurrentActiveDatatable.DefaultView;
                this.Choosecompanycombobox.DisplayMemberPath = "NAME";

            }
            catch (Exception ex)
            {
                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionChoosecompanycombobox_DropDownOpened;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }
        }

        private void Choosecompanycombobox_DropDownClosed(object sender, EventArgs e)
        {
            try
            {

                DateTime Sdate = DateTime.Today;
                DateTime Edate = DateTime.Today.AddDays(15);

                int SYear = Sdate.Year;
                int SMonth = Sdate.Month;
                int SDate = Sdate.Day;

                int EYear = Edate.Year;
                int EMonth = Edate.Month;
                int EDate = Edate.Day;

                this.Subscriptionstartfromtextbox.Text = SMonth + "/" + SDate + "/" + SYear;
                this.Subscriptionstartfromtextbox.IsReadOnly = true;

                this.Validuntiltextbox.Text = EMonth + "/" + EDate + "/" + EYear;
                this.Validuntiltextbox.IsReadOnly = true;

                this.SystemMACAddresstextbox.Text = otherConnection.MacAddress;
                this.AlterMasterIDtextbox.Text = "-1";
                this.AlterVoucherIDtextbox.Text = "-1";

                this.CompanyGUIDtextbox.Text = CurrentActiveDatatable.Rows[this.Choosecompanycombobox.SelectedIndex][1].ToString();



            }
            catch (Exception ex)
            {
                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionChoosecompanycombobox_DropDownOpened;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }
        }

        private void Choosecompanycombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GoBackorCancelbutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.GroupBoxforTallyCompany.Visibility = Visibility.Visible;
                this.GroupBoxforSignupthecomppany.Visibility = Visibility.Hidden;
                this.SignUpbutton.IsEnabled = true;
                this.SaveConfigurationbutton.IsEnabled = true;
                this.Refreshbutton.IsEnabled = true;

                this.TallyIPAddress.IsReadOnly = false;
                this.TallyPort.IsReadOnly = false;
                this.TallyProduct.IsEnabled = true;
                this.SyncInterval.IsEnabled = true;
                this.CloseWindow.IsEnabled = true;
                this.UpdateInfobutton.IsEnabled = true;
                ClearSignUptextbox();

                dispatcherTimerforAutoSync.Start();

            }
            catch { }
        }

        private async void verifyOTPbutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.OTPNumber = Otpnumber1textbox.Text + Otpnumber2textbox.Text + Otpnumber3textbox.Text + Otpnumber4textbox.Text + Otpnumber5textbox.Text + Otpnumber6textbox.Text;

                if (Track_Payout.Properties.Settings.Default.v14 == OTPNumber)
                {
                    // string page = "signup";

                    this.SignuprequestwithOTP = "{" + '"' + "company_name" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v1 + '"' + "," +
                                            '"' + "contact_person" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v2 + '"' + "," +
                                           '"' + "phone_no" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v3 + '"' + "," +
                                          '"' + "email_address" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v4 + '"' + "," +
                                          '"' + "start_date" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v5 + '"' + "," +
                                          '"' + "amount_paid" + '"' + ":" + '"' + "0" + '"' + "," +
                                          '"' + "valid_until" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v7 + '"' + "," +
                                          '"' + "subdomain_name" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v8 + '"' + "," +
                                          '"' + "city" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v9 + '"' + "," +
                                          '"' + "guid" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v10 + '"' + "," +
                                          '"' + "master_id" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v11 + '"' + "," +
                                          '"' + "alter_id" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v12 + '"' + "," +
                                          '"' + "mac_address" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v13 + '"' +
                                            "}";

                    this.otpverificationMessagelabel4.Content = "Please Wait.....";

                    this.verifyOTPbutton.IsEnabled = false;
                    this.resendOTPbutton.IsEnabled = false;
                    dispatcherTimerOTP.Stop();
                    dispatcherTimerResendOTP.Stop();

                    this.otpverificationMessagelabel3.Content = "";

                    JObject d = await Task.Run(() => AsyncdatabasecreationonClearBalanceServer());



                    this.otpverificationMessagelabel4.Visibility = Visibility.Hidden;

                    string verifyMessagestr = (string)d.SelectToken("result.message");
                    string verifyMessagestr1 = (string)d.SelectToken("result.message1");
                    string verifyMessagestr2 = (string)d.SelectToken("result.message2");
                    string verifyMessagestr3 = (string)d.SelectToken("result.message3");

                    string verifycustomeremailstr = (string)d.SelectToken("result.cust_email");
                    string verifystartdatestr = (string)d.SelectToken("result.start_date");
                    string verifyenddatestr = (string)d.SelectToken("result.end_date");

                    string verifystaus = (string)d.SelectToken("result.status");

                    if (verifystaus == "True")
                    {

                        this.Groupboxforotpverification.Visibility = Visibility.Hidden;
                        this.StackPannelforDisplayingMessageafterSignSuccessFully.Visibility = Visibility.Visible;
                        this.OkButtonifOTPmismatch.Visibility = Visibility.Hidden;

                        byte[] binarySignupIcon = Convert.FromBase64String("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");
                        BitmapImage bitmapImageSignuplogo = new BitmapImage();
                        bitmapImageSignuplogo.BeginInit();
                        bitmapImageSignuplogo.StreamSource = new MemoryStream(binarySignupIcon);
                        bitmapImageSignuplogo.EndInit();
                        ImageforSignupSuccesfully.Source = bitmapImageSignuplogo;


                        this.MessageafterSignUpComplete1.Content = verifyMessagestr;
                        this.MessageafterSignUpComplete2.Content = verifyMessagestr1;
                        this.MessageafterSignUpComplete3.Content = verifyMessagestr2;
                        this.MessageafterSignUpComplete4.Content = verifyMessagestr3;

                        this.OkButtonafterSignUpComplete.Visibility = Visibility.Visible;

                        clearOTPTextBox();


                        //DialogResult result = System.Windows.Forms.MessageBox.Show(verifyMessagestr, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //if (result == System.Windows.Forms.DialogResult.OK)
                        //{
                        //    System.Windows.Forms.Application.Restart();
                        //    System.Windows.Application.Current.Shutdown();
                        //}

                    }
                    else
                    {
                        this.Groupboxforotpverification.Visibility = Visibility.Hidden;
                        this.StackPannelforDisplayingMessageafterSignSuccessFully.Visibility = Visibility.Visible;


                        this.MessageafterSignUpComplete1.Content = verifyMessagestr;
                        this.MessageafterSignUpComplete2.Content = "";
                        this.MessageafterSignUpComplete3.Content = "";
                        this.MessageafterSignUpComplete4.Content = "";

                        this.OkButtonafterSignUpComplete.Visibility = Visibility.Visible;


                        clearOTPTextBox();

                        //DialogResult result = System.Windows.Forms.MessageBox.Show("Registration Failed \n Please try again", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //if (result == System.Windows.Forms.DialogResult.OK)
                        //{
                        //    System.Windows.Forms.Application.Restart();
                        //    System.Windows.Application.Current.Shutdown();
                        //}

                    }
                }
                else
                {
                    clearOTPTextBox();

                    this.StackPannelforDisplayingMessageafterSignSuccessFully.Visibility = Visibility.Visible;


                    byte[] binaryAlertlogo = Convert.FromBase64String("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");
                    BitmapImage bitmapImageAlertlogo = new BitmapImage();
                    bitmapImageAlertlogo.BeginInit();
                    bitmapImageAlertlogo.StreamSource = new MemoryStream(binaryAlertlogo);
                    bitmapImageAlertlogo.EndInit();


                    /*   byte[] binaryEmailreceivedlogo = Convert.FromBase64String("data:image/png;base64,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");
                       BitmapImage bitmapEmailreceivedlogo = new BitmapImage();
                       bitmapEmailreceivedlogo.BeginInit();
                       bitmapEmailreceivedlogo.StreamSource = new MemoryStream(binaryEmailreceivedlogo);
                       bitmapEmailreceivedlogo.EndInit();
   */



                    this.ImageforSignupSuccesfully.Source = bitmapImageAlertlogo;
                    this.MessageafterSignUpComplete2.Content = "Please enter valid OTP";

                    this.MessageafterSignUpComplete1.Content = "";

                    this.MessageafterSignUpComplete3.Content = "";
                    this.MessageafterSignUpComplete4.Content = "";

                    this.OkButtonafterSignUpComplete.Visibility = Visibility.Hidden;
                    this.OkButtonifOTPmismatch.Visibility = Visibility.Visible;

                    //System.Windows.Forms.MessageBox.Show("OTP is invalid\nPlease Check and try again or Click on Resend OTP", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionverifyOTPbutton_Click;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

        }

        public void clearOTPTextBox()
        {
            Otpnumber1textbox.Text = "";
            Otpnumber2textbox.Text = "";
            Otpnumber3textbox.Text = "";
            Otpnumber4textbox.Text = "";
            Otpnumber5textbox.Text = "";
            Otpnumber6textbox.Text = "";
        }

        private JObject AsyncdatabasecreationonClearBalanceServer()
        {
            try
            {
                JObject d = JObject.Parse(aPIConnection.SignUpOnClearBalanceWebServerWithOTP(this.SignuprequestwithOTP, this.OTPNumber));

                return d;
            }
            catch (Exception ex)
            {

                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionAsyncdatabasecreationonClearBalanceServer;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }
            return null;
        }

        private JObject AsyncUpdateEmailonClearBalanceServer()
        {
            try
            {
                JObject d = JObject.Parse(aPIConnection.UpdateEmailOnClearBalanceWebServer(this.UpdateEmailIdrequestwithOTP));

                return d;
            }
            catch (Exception ex)
            {
                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionAsyncUpdateEmailonClearBalanceServer;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }
            return null;
        }


        private void resendOTPbutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // string page = "signup";

                this.SignuprequestwithOTP = "{" + '"' + "company_name" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v1 + '"' + "," +
                                           '"' + "contact_person" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v2 + '"' + "," +
                                          '"' + "phone_no" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v3 + '"' + "," +
                                         '"' + "email_address" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v4 + '"' + "," +
                                         '"' + "start_date" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v5 + '"' + "," +
                                         '"' + "amount_paid" + '"' + ":" + '"' + "0" + '"' + "," +
                                         '"' + "valid_until" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v7 + '"' + "," +
                                         '"' + "subdomain_name" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v8 + '"' + "," +
                                         '"' + "city" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v9 + '"' + "," +
                                         '"' + "guid" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v10 + '"' + "," +
                                         '"' + "master_id" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v11 + '"' + "," +
                                         '"' + "alter_id" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v12 + '"' + "," +
                                         '"' + "mac_address" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v13 + '"' +
                                           "}";


                JObject c = JObject.Parse(aPIConnection.SignUpOnClearBalanceWebServer(SignuprequestwithOTP));

                var ResendresponseMsg = (string)c.SelectToken("result.message");

                var ResendSignupstatus = (string)c.SelectToken("result.status");

                var ResendSignupotp = (string)c.SelectToken("result.otp");

                Track_Payout.Properties.Settings.Default.v14 = ResendSignupotp;
                Track_Payout.Properties.Settings.Default.Save();

                if (ResendSignupstatus == "True")
                {

                    this.OTPVerifycountdown = 60;

                    dispatcherTimerResendOTP.Tick += new EventHandler(dispatcherTimerResendOTP_Tick);
                    dispatcherTimerResendOTP.Interval = new TimeSpan(0, 0, 1);
                    dispatcherTimerResendOTP.Start();
                    otpverificationMessagelabel3.Content = "OTP will be invalid in " + this.OTPVerifycountdown.ToString() + " sec";

                    this.resendOTPbutton.IsEnabled = false;

                    this.verifyOTPbutton.IsEnabled = true;
                    this.otpverificationMessagelabel3.Visibility = Visibility.Visible;

                    this.otpverificationMessagelabel4.Content = Track_Payout.Properties.Settings.Default.v14;

                    clearOTPTextBox();

                }
            }
            catch (Exception ex)
            {

                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionresendOTPbutton_Click;
                logger.Log(errorCodeFunc + " : " + ex.Message);

            }

        }

        private void dispatcherTimerResendOTP_Tick(object sender, EventArgs e)
        {
            OTPVerifycountdown--;

            if (OTPVerifycountdown == 0)
            {
                dispatcherTimerResendOTP.Stop();
                this.resendOTPbutton.IsEnabled = true;
                this.otpverificationMessagelabel5.Visibility = Visibility.Hidden;
                this.verifyOTPbutton.IsEnabled = false;
                otpverificationMessagelabel3.Content = "Didn't received the verification OTP? Click Resend";


            }
            else
            {
                otpverificationMessagelabel3.Content = "OTP will be invalid in " + OTPVerifycountdown.ToString() + " sec";
            }
        }




        private void Refreshbutton_Click(object sender, RoutedEventArgs e)
        {
            /*try
            {

                DataTable dt1 = xMLResponseModel.CompanyXMLResponsefromTallyService();
                datagridviewforactivecompany.ItemsSource = dt1.AsDataView();

               this.datagridviewforactivecompany.Visibility = Visibility.Visible;

                this.Refreshbutton.IsEnabled = false;
                this.SaveConfigurationbutton.IsEnabled = false;
                this.SignUpbutton.IsEnabled = false;


                if (dt1.Rows.Count > 0)
                {
                    this.DataLoadingProgressbar.Visibility = Visibility.Visible;
                    this.LoadingDataLabel.Visibility = Visibility.Visible;
                }


                await AsyncPostingJSONString();

                this.DataLoadingProgressbar.Visibility = Visibility.Hidden;
                this.LoadingDataLabel.Visibility = Visibility.Hidden;

                this.SaveConfigurationbutton.IsEnabled = true;
               // this.SignUpbutton.IsEnabled = true;
                this.Refreshbutton.IsEnabled = true;


                DataTable dt2 = xMLResponseModel.CompanyXMLResponsefromTallyService();
                datagridviewforactivecompany.ItemsSource = dt2.AsDataView();



            }
            catch (Exception ex)
            {
                logger.Log("Error code Refreshbutton_Click :" + ex.Message);
            } */
        }


        public void ShowConfigureSettingonpannel()
        {
            try
            {
                applicationConfigration.GetConfigDetail();
                TallyIPAddress.Text = applicationConfigration.TallyIPAddress;
                TallyPort.Text = applicationConfigration.TallyPort;

                SyncInterval.Text = applicationConfigration.SyncInterval;
                TallyProduct.Text = applicationConfigration.TallyProductName;

                //Int32.Parse(applicationConfigration.timerValue)                

                this.dispatcherTimerforAutoSync.Interval = new TimeSpan(0, 0, Int32.Parse(applicationConfigration.timerValue));
                dispatcherTimerforAutoSync.Tick += new EventHandler(dispatcherTimerforAutoSync_Tick);


            }
            catch (Exception ex)
            {
                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionShowConfigureSettingonpannel;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

        }
        //dispatcherTimerInvisibleSyncMessage

        private async void dispatcherTimerforAutoSync_Tick(object sender, EventArgs e)
        {

            try
            {

                this.dispatcherTimerInvisibleSyncMessage.Interval = new TimeSpan(0, 0, 10);
                this.dispatcherTimerInvisibleSyncMessage.Tick += new EventHandler(dispatcherTimerInvisibleSyncMessage_Tick);

                DataTable dt1 = xMLResponseModel.CompanyXMLResponsefromTallyService();
                this.datagridviewforactivecompany.ItemsSource = dt1.AsDataView();

                this.datagridviewforactivecompany.Visibility = Visibility.Visible;

                this.StackPannelforDisplayingMessageafterSyncProcessComplete.Visibility = Visibility.Hidden;
                this.Refreshbutton.IsEnabled = false;
                this.SaveConfigurationbutton.IsEnabled = false;
                this.SignUpbutton.IsEnabled = false;
                this.CloseWindow.IsEnabled = false;

                this.TallyIPAddress.IsReadOnly = true;
                this.TallyPort.IsReadOnly = true;
                this.TallyProduct.IsEnabled = false;
                this.SyncInterval.IsEnabled = false;
                this.UpdateInfobutton.IsEnabled = false;

                if (dt1.Rows.Count > 0)
                {

                    this.RectangleforProgressBar.Visibility = Visibility.Visible;
                    this.LoadingDataLabel1.Visibility = Visibility.Visible;

                }
                var watch = System.Diagnostics.Stopwatch.StartNew();

                await AsyncPostingJSONString();

                this.dispatcherTimerInvisibleSyncMessage.Start();
                this.dispatcherTimerInvisibleSyncMessage.Interval = new TimeSpan(0, 0, 10);
                this.dispatcherTimerInvisibleSyncMessage.Tick += new EventHandler(dispatcherTimerInvisibleSyncMessage_Tick);


                watch.Stop();

                double mins = Double.Parse(watch.Elapsed.TotalMinutes.ToString());

                EstimateTimeInfoTextBox.Text = String.Format("{0:0.00}", mins) + " Mins";

                this.RectangleforProgressBar.Visibility = Visibility.Hidden;
                this.LoadingDataLabel1.Visibility = Visibility.Hidden;

                this.SaveConfigurationbutton.IsEnabled = true;
                //this.SignUpbutton.IsEnabled = true;
                this.Refreshbutton.IsEnabled = true;
                this.CloseWindow.IsEnabled = true;

                this.TallyIPAddress.IsReadOnly = false;
                this.TallyPort.IsReadOnly = false;
                this.TallyProduct.IsEnabled = true;
                this.SyncInterval.IsEnabled = true;


                DataTable dt3 = xMLResponseModel.CompanyXMLResponsefromTallyService();
                datagridviewforactivecompany.ItemsSource = dt3.AsDataView();

            }
            catch (Exception ex)
            {
                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctiondispatcherTimerforAutoSync_tick;
                logger.Log(errorCodeFunc + " : " + ex.Message);

            }

        }

        private void dispatcherTimerInvisibleSyncMessage_Tick(object sender, EventArgs e)
        {
            this.StackPannelforDisplayingMessageafterSyncProcessComplete.Visibility = Visibility.Hidden;
            this.dispatcherTimerInvisibleSyncMessage.Stop();
        }

        public async Task AsyncPostingJSONString()
        {
            try
            {

                dispatcherTimerforAutoSync.Stop();

                bool status = false;
                bool InternetStatus = await Task.Run(() => otherConnection.IsConnectedToInternet());
                string TallyStatus = await Task.Run(() => otherConnection.IsTallyRunning());

                if (InternetStatus)
                {
                    this.lable8.Content = "Connected";
                    this.InternetConnectedImageMark.Background = Brushes.Green;

                    // this.SignUpbutton.IsEnabled = true;

                    this.TallyConfigAlertBox7.Visibility = Visibility.Hidden;
                    this.TallyConfigAlterImage.Visibility = Visibility.Hidden;

                    if (applicationConfigration.TallyProductName == "Tally.ERP9")
                    {
                        //string TallyStatus = await Task.Run(() => otherConnection.IsTallyRunning());

                        if ((TallyStatus == "Tally.ERP 9 Server is Running"))
                        {
                            this.lable6.Content = "Connected";
                            this.TallyConnectedImageMark.Background = Brushes.Green;



                            string TallyLicenseNumberInfo = await Task.Run(() => otherConnection.TallyLicenseInfoResponseFromTally());

                            if (TallyLicenseNumberInfo == "Nil")
                            {
                                this.EducationalModeLabel.Visibility = Visibility.Visible;
                                this.Refreshbutton.IsEnabled = false;

                                this.Tallyserialnolabel.Visibility = Visibility.Hidden;
                                this.TallyserialnoInfoTextBox.Visibility = Visibility.Hidden;

                                this.TallyConfigAlertBox1.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox2.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox3.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox4.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox5.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox6.Visibility = Visibility.Hidden;

                                this.TallyConfigAlterImage.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox7.Visibility = Visibility.Hidden;
                                this.SignUpbutton.IsEnabled = false;
                                this.UpdateInfobutton.IsEnabled = false;
                            }
                            else
                            if (TallyLicenseNumberInfo == "0")
                            {

                                this.EducationalModeLabel.Visibility = Visibility.Visible;
                                this.Refreshbutton.IsEnabled = false;

                                this.Tallyserialnolabel.Visibility = Visibility.Hidden;
                                this.TallyserialnoInfoTextBox.Visibility = Visibility.Hidden;

                                this.TallyConfigAlertBox1.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox2.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox3.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox4.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox5.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox6.Visibility = Visibility.Hidden;

                                this.TallyConfigAlterImage.Visibility = Visibility.Hidden;

                                this.TallyConfigAlertBox7.Visibility = Visibility.Hidden;

                                this.SignUpbutton.IsEnabled = false;
                                this.UpdateInfobutton.IsEnabled = false;

                            }
                            else
                            {
                                this.TallyserialnoInfoTextBox.Text = TallyLicenseNumberInfo;
                                this.EducationalModeLabel.Visibility = Visibility.Hidden;

                                this.Refreshbutton.IsEnabled = true;

                                this.Tallyserialnolabel.Visibility = Visibility.Visible;
                                this.TallyserialnoInfoTextBox.Visibility = Visibility.Visible;

                                this.TallyConfigAlertBox1.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox2.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox3.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox4.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox5.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox6.Visibility = Visibility.Hidden;

                                this.TallyConfigAlterImage.Visibility = Visibility.Hidden;


                                this.GroupBoxforTallyCompany.Visibility = Visibility.Visible;
                                this.SignUpbutton.IsEnabled = false;
                                this.UpdateInfobutton.IsEnabled = false;

                                DataTable dt1 = xMLResponseModel.CompanyXMLResponsefromTallyService();
                                datagridviewforactivecompany.ItemsSource = dt1.AsDataView();


                                if (dt1.Rows.Count == 0)
                                {
                                    this.SignUpbutton.IsEnabled = true;
                                    this.UpdateInfobutton.IsEnabled = true;
                                    this.StackPannelforDisplayingMessageafterSyncProcessComplete.Visibility = Visibility.Visible;
                                    this.MessageafterSyncProcessComplete1.Content = "None of the company is registered with Track Payout";
                                    this.MessageafterSyncProcessComplete2.Content = "Please click on sign up to registered with us.";
                                }
                                else
                                {
                                    status = await Task.Run(() => AsyncStartSyncWithTrackpayout());

                                    if (status)
                                    {
                                        this.SignUpbutton.IsEnabled = true;
                                        this.UpdateInfobutton.IsEnabled = true;
                                        this.StackPannelforDisplayingMessageafterSyncProcessComplete.Visibility = Visibility.Visible;
                                        this.MessageafterSyncProcessComplete1.Content = "Sync between tally and trackpayout successfully done.";
                                        this.MessageafterSyncProcessComplete2.Content = this.companySyncDetailSummary;
                                    }
                                    else
                                    {
                                        this.SignUpbutton.IsEnabled = true;
                                        this.UpdateInfobutton.IsEnabled = true;
                                        this.StackPannelforDisplayingMessageafterSyncProcessComplete.Visibility = Visibility.Visible;
                                        this.MessageafterSyncProcessComplete1.Content = "Sync between the tally and trackpayout failed due to some internal error";
                                        this.MessageafterSyncProcessComplete2.Content = "Check the log file";

                                    }

                                }



                            }
                        }
                        else
                        {
                            // System.Windows.Forms.MessageBox.Show("Failed to connected with " + applicationConfigration.TallyProductName + ". Please Check the configure Product Name, Server Name/ IP and Port.\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            this.lable6.Content = "Disonnected";
                            this.TallyConnectedImageMark.Background = Brushes.Red;

                            this.EducationalModeLabel.Visibility = Visibility.Hidden;
                            this.Tallyserialnolabel.Visibility = Visibility.Hidden;
                            this.TallyserialnoInfoTextBox.Visibility = Visibility.Hidden;

                            this.TallyConfigAlertBox1.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox2.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox3.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox4.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox5.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox6.Visibility = Visibility.Visible;

                            this.TallyConfigAlterImage.Visibility = Visibility.Visible;

                            this.SignUpbutton.IsEnabled = false;
                            this.GroupBoxforTallyCompany.Visibility = Visibility.Hidden;

                        }
                    }
                    else
                    {

                        if ((TallyStatus == "TallyPrime Server is Running"))
                        {
                            this.lable6.Content = "Connected";
                            this.TallyConnectedImageMark.Background = Brushes.Green;

                            string TallyLicenseNumberInfo = await Task.Run(() => otherConnection.TallyLicenseInfoResponseFromTally());

                            if (TallyLicenseNumberInfo == "Nil")
                            {
                                this.EducationalModeLabel.Visibility = Visibility.Visible;
                                this.Refreshbutton.IsEnabled = false;

                                this.Tallyserialnolabel.Visibility = Visibility.Hidden;
                                this.TallyserialnoInfoTextBox.Visibility = Visibility.Hidden;

                                this.TallyConfigAlertBox1.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox2.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox3.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox4.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox5.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox6.Visibility = Visibility.Hidden;

                                this.TallyConfigAlterImage.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox7.Visibility = Visibility.Hidden;
                                this.SignUpbutton.IsEnabled = false;
                                this.UpdateInfobutton.IsEnabled = false;
                            }
                            else
                            if (TallyLicenseNumberInfo == "0")
                            {

                                this.EducationalModeLabel.Visibility = Visibility.Visible;
                                this.Refreshbutton.IsEnabled = false;

                                this.Tallyserialnolabel.Visibility = Visibility.Hidden;
                                this.TallyserialnoInfoTextBox.Visibility = Visibility.Hidden;

                                this.TallyConfigAlertBox1.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox2.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox3.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox4.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox5.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox6.Visibility = Visibility.Hidden;

                                this.TallyConfigAlterImage.Visibility = Visibility.Hidden;

                                this.TallyConfigAlertBox7.Visibility = Visibility.Hidden;

                                this.SignUpbutton.IsEnabled = false;
                                this.UpdateInfobutton.IsEnabled = false;

                            }
                            else
                            {
                                this.TallyserialnoInfoTextBox.Text = TallyLicenseNumberInfo;
                                this.EducationalModeLabel.Visibility = Visibility.Hidden;

                                this.Refreshbutton.IsEnabled = true;

                                this.Tallyserialnolabel.Visibility = Visibility.Visible;
                                this.TallyserialnoInfoTextBox.Visibility = Visibility.Visible;

                                this.TallyConfigAlertBox1.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox2.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox3.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox4.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox5.Visibility = Visibility.Hidden;
                                this.TallyConfigAlertBox6.Visibility = Visibility.Hidden;

                                this.TallyConfigAlterImage.Visibility = Visibility.Hidden;


                                this.GroupBoxforTallyCompany.Visibility = Visibility.Visible;
                                this.SignUpbutton.IsEnabled = false;
                                this.UpdateInfobutton.IsEnabled = false;

                                DataTable dt1 = xMLResponseModel.CompanyXMLResponsefromTallyService();
                                datagridviewforactivecompany.ItemsSource = dt1.AsDataView();


                                if (dt1.Rows.Count == 0)
                                {
                                    this.SignUpbutton.IsEnabled = true;
                                    this.UpdateInfobutton.IsEnabled = true;
                                    this.StackPannelforDisplayingMessageafterSyncProcessComplete.Visibility = Visibility.Visible;
                                    this.MessageafterSyncProcessComplete1.Content = "None of the company is registered with Track Payout";
                                    this.MessageafterSyncProcessComplete2.Content = "Please click on sign up to registered with us.";
                                }
                                else
                                {
                                    status = await Task.Run(() => AsyncStartSyncWithTrackpayout());


                                    if (status)
                                    {
                                        this.SignUpbutton.IsEnabled = true;
                                        this.UpdateInfobutton.IsEnabled = true;
                                        this.StackPannelforDisplayingMessageafterSyncProcessComplete.Visibility = Visibility.Visible;
                                        this.MessageafterSyncProcessComplete1.Content = "Sync between tally and trackpayout successfully done.";
                                        this.MessageafterSyncProcessComplete2.Content = this.companySyncDetailSummary;
                                    }
                                    else
                                    {
                                        this.SignUpbutton.IsEnabled = true;
                                        this.UpdateInfobutton.IsEnabled = true;
                                        this.StackPannelforDisplayingMessageafterSyncProcessComplete.Visibility = Visibility.Visible;
                                        this.MessageafterSyncProcessComplete1.Content = "Sync between the tally and trackpayout failed due to some internal error";
                                        this.MessageafterSyncProcessComplete2.Content = "Check the log file";

                                    }

                                }

                            }

                        }
                        else
                        {
                            // System.Windows.Forms.MessageBox.Show("Failed to connected with " + applicationConfigration.TallyProductName + ". Please Check the configure Product Name, Server Name/ IP and Port.\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            this.lable6.Content = "Disonnected";
                            this.TallyConnectedImageMark.Background = Brushes.Red;

                            this.EducationalModeLabel.Visibility = Visibility.Hidden;
                            this.Tallyserialnolabel.Visibility = Visibility.Hidden;
                            this.TallyserialnoInfoTextBox.Visibility = Visibility.Hidden;

                            this.TallyConfigAlertBox1.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox2.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox3.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox4.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox5.Visibility = Visibility.Visible;
                            this.TallyConfigAlertBox6.Visibility = Visibility.Visible;

                            this.TallyConfigAlterImage.Visibility = Visibility.Visible;

                            this.SignUpbutton.IsEnabled = false;
                            this.GroupBoxforTallyCompany.Visibility = Visibility.Hidden;

                        }
                    }
                }
                else
                {

                    // System.Windows.Forms.MessageBox.Show("Please check your internet connection", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.lable8.Content = "Disconnected";
                    this.InternetConnectedImageMark.Background = Brushes.Red;

                    this.TallyConfigAlertBox7.Visibility = Visibility.Visible;
                    this.TallyConfigAlterImage.Visibility = Visibility.Visible;

                    this.GroupBoxforTallyCompany.Visibility = Visibility.Hidden;
                    this.SignUpbutton.IsEnabled = false;



                }


                dispatcherTimerforAutoSync.Start();
                dispatcherTimerInvisibleSyncMessage.Start();

            }
            catch (Exception ex)
            {
                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionAsyncPostingJSONString;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

        }

        /// <summary>
        ///  AutoSync With Clear Balance
        /// </summary>
        /// <returns></returns>
        private bool AsyncStartSyncWithCB()
        {
            try
            {
                //   this.Closing += new System.ComponentModel.CancelEventHandler(Window_ClosingStart);

                string JSONReponseAltMstID, lastmasterid, lastvoucherid;
                string JSONReponseforUpdateguidCBdb;
                string updateFlag;


                string JSONResponseForClearBalanceFlag, JSONResponseForClearBalanceFlagVoucher, JSONResponseForClearBalanceFlagLedgerClosing;
                string JSONResponseForClearBalancestr, JSONResponseForClearBalanceVoucher, JSONResponseForClearBalanceLedgerCLosingstr;



                DataTable Cmpdt = xMLResponseModel.TallyActiveCompanyXMLResponse();

                if (Cmpdt.Rows.Count > 0)
                {
                    for (int i = 0; i < Cmpdt.Rows.Count; i++)
                    {

                        this.CURRENTCOMPANY = Cmpdt.Rows[i][2].ToString();
                        this.BOOKBEGINNINGFROM = Cmpdt.Rows[i][7].ToString();
                        this.LASTVOUCHERENTRYDATE = Cmpdt.Rows[i][5].ToString();
                        this.CURRENTLASTMASTERID = Cmpdt.Rows[i][12].ToString();
                        this.CURRENTLASTVOUCHERID = Cmpdt.Rows[i][11].ToString();
                        this.CheckCbRegister = Cmpdt.Rows[i][0].ToString();
                        this.GUID = Cmpdt.Rows[i][3].ToString();
                        this.COMPANYINITIALS = Cmpdt.Rows[i][1].ToString();

                        this.COMPANYNUMBER = Cmpdt.Rows[i][15].ToString();

                        JSONReponseAltMstID = aPIConnection.GetguidCBdatabasewithlastId(this.GUID, "fetch");
                        JObject o = JObject.Parse(JSONReponseAltMstID);

                        lastmasterid = (string)o.SelectToken("AltMstID");
                        lastvoucherid = (string)o.SelectToken("AltVchID");


                        this.LASTMASTERID = lastmasterid;
                        this.LASTVOUCHERID = lastvoucherid;

                        string JSONReponse = aPIConnection.CheckGUIDClearBalanceWebServer(GUID, "other");


                        JObject r = JObject.Parse(JSONReponse);
                        string macaddresstag = (string)r.SelectToken("mac_address");

                        if (this.CheckCbRegister == "Yes")
                        {

                            if (macaddresstag != otherConnection.MacAddress)
                            {
                                DialogResult result = System.Windows.Forms.MessageBox.Show("You are trying to access ClearBalance application in a different system for " + this.CURRENTCOMPANY + ".\n Can we log you out from the other system and register this.?", "Warning.", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                                if (result == System.Windows.Forms.DialogResult.Yes)
                                {

                                    string JSONReponseMacaddress = aPIConnection.UpdateMACClearBalanceWebServer(otherConnection.MacAddress, COMPANYINITIALS, GUID);
                                    JObject MA = JObject.Parse(JSONReponseMacaddress);
                                    string macaddressupdateresult = (string)MA.SelectToken("status");

                                    if (macaddressupdateresult == "True")
                                    {
                                        JSONResponseForClearBalancestr = aPIConnection.JSONstringResponseforWebServerAgainstPostedJSONStringForLedger(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);
                                        JSONResponseForClearBalanceLedgerCLosingstr = aPIConnection.JSONstringResponseforWebServerAgainstPostedJSONStringForLedgerClosingBalance(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);
                                        JSONResponseForClearBalanceVoucher = aPIConnection.JSONstringResponseforWebServerAgainstPostedJSONStringForVoucher(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);

                                        JObject p = JObject.Parse(JSONResponseForClearBalancestr);
                                        JObject q = JObject.Parse(JSONResponseForClearBalanceLedgerCLosingstr);
                                        JObject s = JObject.Parse(JSONResponseForClearBalanceVoucher);

                                        JSONResponseForClearBalanceFlag = (string)p.SelectToken("status");
                                        JSONResponseForClearBalanceFlagVoucher = (string)q.SelectToken("status");
                                        JSONResponseForClearBalanceFlagLedgerClosing = (string)s.SelectToken("status");

                                        if (JSONResponseForClearBalanceFlag == "True" && JSONResponseForClearBalanceFlagVoucher == "True" && JSONResponseForClearBalanceFlagLedgerClosing == "True")
                                        {
                                            //logger.CreateLastSyncCompanydetails(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE,
                                            //                           aPIConnection.newledgerCount.ToString(), aPIConnection.updatedledgerCount.ToString(),
                                            //                           aPIConnection.salesvoucherCount.ToString(), aPIConnection.receiptvouchercount.ToString(), aPIConnection.creditnoteCount.ToString(),
                                            //                           aPIConnection.purchasevoucherCount.ToString(), aPIConnection.paymentvoucherCount.ToString(), aPIConnection.debitnoteCount.ToString());

                                            do
                                            {
                                                JSONReponseforUpdateguidCBdb = aPIConnection.UpdateguidCBdatabasewithcurrentId(GUID, CURRENTLASTVOUCHERID, CURRENTLASTMASTERID, "post");
                                                JObject m = JObject.Parse(JSONReponseforUpdateguidCBdb);
                                                updateFlag = (string)m.SelectToken("status");

                                            } while (updateFlag == "False");


                                        }
                                    }
                                    else
                                    {
                                        System.Windows.Forms.MessageBox.Show("Failed to register the new system.\n Click on refresh button", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }


                                }


                            }
                            else
                            {
                                JSONResponseForClearBalancestr = aPIConnection.JSONstringResponseforWebServerAgainstPostedJSONStringForLedger(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);
                                JSONResponseForClearBalanceLedgerCLosingstr = aPIConnection.JSONstringResponseforWebServerAgainstPostedJSONStringForLedgerClosingBalance(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);
                                JSONResponseForClearBalanceVoucher = aPIConnection.JSONstringResponseforWebServerAgainstPostedJSONStringForVoucher(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);

                                JObject p = JObject.Parse(JSONResponseForClearBalancestr);
                                JObject q = JObject.Parse(JSONResponseForClearBalanceLedgerCLosingstr);
                                JObject s = JObject.Parse(JSONResponseForClearBalanceVoucher);

                                JSONResponseForClearBalanceFlag = (string)p.SelectToken("status");
                                JSONResponseForClearBalanceFlagVoucher = (string)q.SelectToken("status");
                                JSONResponseForClearBalanceFlagLedgerClosing = (string)s.SelectToken("status");

                                if (JSONResponseForClearBalanceFlag == "True" && JSONResponseForClearBalanceFlagVoucher == "True" && JSONResponseForClearBalanceFlagLedgerClosing == "True")
                                {

                                    //logger.CreateLastSyncCompanydetails(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE,
                                    //                              aPIConnection.newledgerCount.ToString(), aPIConnection.updatedledgerCount.ToString(),
                                    //                              aPIConnection.salesvoucherCount.ToString(), aPIConnection.receiptvouchercount.ToString(), aPIConnection.creditnoteCount.ToString(),
                                    //                              aPIConnection.purchasevoucherCount.ToString(), aPIConnection.paymentvoucherCount.ToString(), aPIConnection.debitnoteCount.ToString());

                                    do
                                    {
                                        JSONReponseforUpdateguidCBdb = aPIConnection.UpdateguidCBdatabasewithcurrentId(GUID, CURRENTLASTVOUCHERID, CURRENTLASTMASTERID, "post");
                                        JObject m = JObject.Parse(JSONReponseforUpdateguidCBdb);
                                        updateFlag = (string)m.SelectToken("status");

                                    } while (updateFlag == "False");


                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (ApplicationException ex)
            {
                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionAsyncStartSyncWithCB;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return false;
        }

        private bool AsyncStartSyncWithTrackpayout()
        {
            try
            {
                //   this.Closing += new System.ComponentModel.CancelEventHandler(Window_ClosingStart);

                string JSONReponseAltMstID, lastmasterid, lastvoucherid;
                string JSONReponseforUpdateguidCBdb;
                string updateFlag;

                string JSONResponseForClearBalanceFlag, JSONResponseForClearBalanceFlagVoucher, JSONResponseForClearBalanceFlagLedgerClosing;
                string JSONResponseForClearBalancestr, JSONResponseForClearBalanceLedgerCLosingstr;

                bool JSONResponseForClearBalanceVoucher = false;

                this.companySyncDetailSummary = "";

                DataTable Cmpdt = xMLResponseModel.TallyActiveCompanyXMLResponse();

                if (Cmpdt.Rows.Count > 0)
                {
                    for (int i = 0; i < Cmpdt.Rows.Count; i++)
                    {

                        this.CURRENTCOMPANY = Cmpdt.Rows[i][2].ToString();
                        this.BOOKBEGINNINGFROM = Cmpdt.Rows[i][7].ToString();
                        this.LASTVOUCHERENTRYDATE = Cmpdt.Rows[i][5].ToString();
                        this.CURRENTLASTMASTERID = Cmpdt.Rows[i][12].ToString();
                        this.CURRENTLASTVOUCHERID = Cmpdt.Rows[i][11].ToString();
                        this.CheckCbRegister = Cmpdt.Rows[i][0].ToString();
                        this.GUID = Cmpdt.Rows[i][3].ToString();
                        this.COMPANYINITIALS = Cmpdt.Rows[i][1].ToString();
                        this.COMPANYNUMBER = Cmpdt.Rows[i][15].ToString();


                        JSONReponseAltMstID = aPIConnection.getlastDetailTrackPayoutWebServer(this.COMPANYINITIALS);

                        JObject o = JObject.Parse(JSONReponseAltMstID);


                        var getstatusmsg = (string)o.SelectToken("status");


                        if (getstatusmsg == "200")
                        {
                            lastvoucherid = (string)o.SelectToken("lastVoucherAlterId");
                            lastmasterid = (string)o.SelectToken("lastMasterAlterId");
                        }
                        else
                        {
                            lastvoucherid = "-1";
                            lastmasterid = "-1";
                        }

                        this.LASTMASTERID = lastmasterid;
                        this.LASTVOUCHERID = lastvoucherid;

                        string JSONReponse = aPIConnection.checkcompanyValidityTrackPayoutWebServer(this.COMPANYINITIALS);


                        JObject r = JObject.Parse(JSONReponse);
                        string IsValidCompany = (string)r.SelectToken("status");

                        if (this.CheckCbRegister == "Yes" && IsValidCompany == "True")
                        {


                            JSONResponseForClearBalanceVoucher = aPIConnection.TPO_JSONstringResponseforWebServerAgainstPostedJSONString(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE, LASTMASTERID, CURRENTLASTMASTERID, LASTVOUCHERID, CURRENTLASTVOUCHERID, COMPANYINITIALS);
                            //Console.WriteLine("Hi");

                            //JObject s = JObject.Parse(JSONResponseForClearBalanceVoucher);

                            //JSONResponseForClearBalanceFlagVoucher = (string)s.SelectToken("status");

                            if (JSONResponseForClearBalanceVoucher == false)
                                return false;

                            if (JSONResponseForClearBalanceVoucher)
                            {
                                //logger.TPO_CreateLastSyncCompanydetails(CURRENTCOMPANY, BOOKBEGINNINGFROM, LASTVOUCHERENTRYDATE,
                                //                              aPIConnection.newledgerCount.ToString(),
                                //                              aPIConnection.salesvoucherCount.ToString(), aPIConnection.receiptvouchercount.ToString());

                                this.companySyncDetailSummary = this.CURRENTCOMPANY + "\n" + aPIConnection.customerPosttag + ": Total Count : " + aPIConnection.newledgerCount + ", " + aPIConnection.invoicePostTag + " : Total Count : " + aPIConnection.salesvoucherCount + " \n";

                                do
                                {
                                    JSONReponseforUpdateguidCBdb = aPIConnection.setlastDetailTrackPayoutWebServer(COMPANYINITIALS, CURRENTLASTVOUCHERID, CURRENTLASTMASTERID);
                                    JObject m = JObject.Parse(JSONReponseforUpdateguidCBdb);
                                    updateFlag = (string)m.SelectToken("status");
                                } while (updateFlag != "200");

                                return true;

                            }



                        }
                    }
                }


            }
            catch (ApplicationException ex)
            {
                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionAsyncStartSyncWithCB;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }

            return false;
        }

        private void Phonenumbertextbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);

        }

        private bool IsDelOrBackspaceOrTabKey(Key inKey)
        {
            return inKey == Key.Delete || inKey == Key.Back || inKey == Key.Tab;
        }

        private bool IsNumberKey(Key inKey)
        {
            if (inKey < Key.D0 || inKey > Key.D9)
            {
                if (inKey < Key.NumPad0 || inKey > Key.NumPad9)
                {
                    return false;
                }
            }
            return true;
        }

        private void Contactpersontextbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key < Key.A || e.Key > Key.Z)
            {
                e.Handled = true && !IsDelOrBackspaceOrTabKey(e.Key);
            }

        }

        private void Companyinitialtextbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //if (e.Key < Key.A || e.Key > Key.Z)
            //{
            //    e.Handled = true && !IsDelOrBackspaceOrTabKey(e.Key);
            //}
            e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);



        }

        private void Citytextbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key < Key.A || e.Key > Key.Z)
            {
                e.Handled = true && !IsDelOrBackspaceOrTabKey(e.Key);
            }
        }

        private void Otpnumber1textbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);
        }

        private void Otpnumber2textbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);
        }

        private void Otpnumber3textbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);
        }

        private void Otpnumber4textbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);
        }

        private void Otpnumber5textbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);
        }

        private void Otpnumber6textbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);
        }

        private void TallyIPAddress_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key < Key.A || e.Key > Key.Z)
            {
                e.Handled = true && !IsDelOrBackspaceOrTabKey(e.Key);
            }

        }

        private void TallyPort_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);

        }

        private void Window_ClosingStart(object sender, System.ComponentModel.CancelEventArgs e)
        {


        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }



        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //MessageBoxResult result = System.Windows.MessageBox.Show("if you want to quit the appliction choose 'Yes' or if you want to run application in the background choose 'No' ?","Alert",MessageBoxButton.YesNo,MessageBoxImage.Exclamation);

            //if (result == MessageBoxResult.Yes)
            //{
            //    System.Windows.Application.Current.Shutdown();
            //}
            //else
            //{
            this.WindowState = WindowState.Minimized;

            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
            //}


        }

        private void RestartWindowlogo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            MessageBoxResult result = System.Windows.MessageBox.Show("Do you really want to restart ?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (result == MessageBoxResult.Yes)
            {
                System.Windows.Forms.Application.Restart();
                System.Windows.Application.Current.Shutdown();
            }


        }

        private void MinimiseWindowlogo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;

            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
        }

        //private void notifyIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    Show();
        //    this.WindowState = WindowState.Normal;
        //    NotifyIcon notifyIcon = new NotifyIcon();
        //    notifyIcon.Visible = false;
        //}



        private void OkButtonafterSignUpComplete_Click(object sender, RoutedEventArgs e)
        {
            this.StackPannelforDisplayingMessageafterSignSuccessFully.Visibility = Visibility.Hidden;

            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();

        }

        private void OkButtonifOTPmismatch_Click(object sender, RoutedEventArgs e)
        {
            this.StackPannelforDisplayingMessageafterSignSuccessFully.Visibility = Visibility.Hidden;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }


        }

        private async void UpdateEmailButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var key = "getotp";
                this.CloseWindow.IsEnabled = false;

                this.Groupboxforotpverification.Visibility = Visibility.Hidden;
                this.SignUpbutton.IsEnabled = false;
                this.SaveConfigurationbutton.IsEnabled = false;
                this.resendOTPbutton.IsEnabled = false;

                byte[] binaryEmailreceivedlogo = Convert.FromBase64String("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");
                BitmapImage bitmapEmailreceivedlogo = new BitmapImage();
                bitmapEmailreceivedlogo.BeginInit();
                bitmapEmailreceivedlogo.StreamSource = new MemoryStream(binaryEmailreceivedlogo);
                bitmapEmailreceivedlogo.EndInit();
                outverificationenterLogo.Source = bitmapEmailreceivedlogo;


                bool InternetStatus = await Task.Run(() => otherConnection.IsConnectedToInternet());

                if (InternetStatus)
                {
                    this.lable8.Content = "Connected";
                    this.InternetConnectedImageMark.Background = Brushes.Green;

                    if (validationsignup.CheckValidEmail(UpdateEmailIdTextBox.Text))
                    {
                        Track_Payout.Properties.Settings.Default.v4 = UpdateEmailIdTextBox.Text;
                        Track_Payout.Properties.Settings.Default.v8 = UpdateCompanyinitialComboxbox.Text;

                        this.UpdateEmailrequest = "{" + '"' + "action" + '"' + ":" + '"' + key + '"' + "," +
                                                    '"' + "email" + '"' + ":" + '"' + UpdateEmailIdTextBox.Text + '"' + "," +
                                                   '"' + "subdomain_name" + '"' + ":" + '"' + UpdateCompanyinitialComboxbox.Text + '"' +
                                                    "}";


                        JObject c = JObject.Parse(aPIConnection.UpdateEmailOnClearBalanceWebServer(UpdateEmailrequest));


                        var responseMsg = (string)c.SelectToken("result.message");

                        var UpdateEmailstatus = (string)c.SelectToken("result.status");

                        var UpdateEmailpotp = (string)c.SelectToken("result.otp");

                        Track_Payout.Properties.Settings.Default.v14 = UpdateEmailpotp;
                        Track_Payout.Properties.Settings.Default.Save();

                        if (UpdateEmailstatus == "False")
                        {
                            System.Windows.Forms.MessageBox.Show(responseMsg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        if (UpdateEmailstatus == "True")
                        {
                            ClearUpdateEmailtextbox();

                            this.GroupBoxforSignupthecomppany.Visibility = Visibility.Hidden;
                            this.Groupboxforotpverification.Visibility = Visibility.Visible;
                            this.GroupBoxforUpdateEmailId.Visibility = Visibility.Hidden;

                            this.otpverificationMessagelabel4.Content = UpdateEmailpotp;

                            this.verifyOTPbutton.Visibility = Visibility.Hidden;
                            this.resendOTPbutton.Visibility = Visibility.Hidden;

                            this.OTPVerifycountdown = 60;

                            dispatcherTimerOTP.Tick += new EventHandler(dispatcherTimerOTP_Tick);
                            dispatcherTimerOTP.Interval = new TimeSpan(0, 0, 1);
                            dispatcherTimerOTP.Start();
                            otpverificationMessagelabel3.Content = "OTP will be invalid in " + this.OTPVerifycountdown.ToString() + " sec";

                        }

                    }
                }
                else
                {
                    this.lable8.Content = "Disconnected";
                    this.InternetConnectedImageMark.Background = Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionUpdateEmailButton_Click;
                logger.Log(errorCodeFunc + " : " + ex.Message);

            }
        }


        public void ClearUpdateEmailtextbox()
        {
            UpdateCompanyinitialComboxbox.Text = "";
            UpdateEmailIdTextBox.Text = "";
        }

        private void UpdateInfobutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.GroupBoxforTallyCompany.Visibility = Visibility.Hidden;
                this.GroupBoxforSignupthecomppany.Visibility = Visibility.Hidden;
                this.GroupBoxforUpdateEmailId.Visibility = Visibility.Visible;
                this.otpverificationMessagelabel2.Content = "we have sent the OTP on your email";
                this.Refreshbutton.IsEnabled = false;

                this.SaveConfigurationbutton.IsEnabled = false;
                this.SignUpbutton.IsEnabled = false;
                this.UpdateInfobutton.IsEnabled = false;

                this.UpdateEmailIDresendOTPbutton.IsEnabled = false;


                this.TallyIPAddress.IsReadOnly = true;
                this.TallyPort.IsReadOnly = true;
                this.TallyProduct.IsEnabled = false;
                this.SyncInterval.IsEnabled = false;

                this.TallyConfigAlertBox1.Visibility = Visibility.Hidden;
                this.TallyConfigAlertBox2.Visibility = Visibility.Hidden;
                this.TallyConfigAlertBox3.Visibility = Visibility.Hidden;
                this.TallyConfigAlertBox4.Visibility = Visibility.Hidden;
                this.TallyConfigAlertBox5.Visibility = Visibility.Hidden;
                this.TallyConfigAlertBox6.Visibility = Visibility.Hidden;

                this.TallyConfigAlterImage.Visibility = Visibility.Hidden;
                this.TallyConfigAlertBox7.Visibility = Visibility.Hidden;

                this.StackPannelforDisplayingMessageafterSyncProcessComplete.Visibility = Visibility.Hidden;

                dispatcherTimerforAutoSync.Stop();
            }
            catch
            {

            }
        }

        private void UpdateCompanyinitialComboxbox_DropDownOpened(object sender, EventArgs e)
        {

            this.UpdateCompanyInitailDatatable = xMLResponseModel.CompanyXMLResponsefromTallyService();
            this.UpdateCompanyinitialComboxbox.ItemsSource = UpdateCompanyInitailDatatable.DefaultView;
            this.UpdateCompanyinitialComboxbox.DisplayMemberPath = "Company Initials";

        }

        private void UpdateCompanyinitialComboxbox_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (UpdateCompanyinitialComboxbox.Text == "")
                {
                    System.Windows.MessageBox.Show("Please select company initials", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    this.UpdateCompanyinitialComboxbox.Text = UpdateCompanyInitailDatatable.Rows[this.UpdateCompanyinitialComboxbox.SelectedIndex][4].ToString();
                }

            }
            catch (Exception ex)
            {
                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionUpdateCompanyinitialComboxbox_DropDownClosed;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }


        }

        private void UpdateEmailCancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.GroupBoxforTallyCompany.Visibility = Visibility.Visible;
            this.GroupBoxforSignupthecomppany.Visibility = Visibility.Hidden;
            this.GroupBoxforUpdateEmailId.Visibility = Visibility.Hidden;

            this.SignUpbutton.IsEnabled = true;
            this.SaveConfigurationbutton.IsEnabled = true;
            this.Refreshbutton.IsEnabled = true;

            this.TallyIPAddress.IsReadOnly = false;
            this.TallyPort.IsReadOnly = false;
            this.TallyProduct.IsEnabled = true;
            this.SyncInterval.IsEnabled = true;
            this.CloseWindow.IsEnabled = true;
            this.UpdateInfobutton.IsEnabled = true;

            ClearUpdateEmailtextbox();

            dispatcherTimerforAutoSync.Start();
        }

        private async void UpdateEmailIDverifyOTPbutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.OTPNumber = Otpnumber1textbox.Text + Otpnumber2textbox.Text + Otpnumber3textbox.Text + Otpnumber4textbox.Text + Otpnumber5textbox.Text + Otpnumber6textbox.Text;

                if (Track_Payout.Properties.Settings.Default.v14 == OTPNumber)
                {
                    string page = "update";

                    this.UpdateEmailIdrequestwithOTP = "{" + '"' + "action" + '"' + ":" + '"' + page + '"' + "," +
                                            '"' + "email" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v4 + '"' + "," +
                                          '"' + "subdomain_name" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v8 + '"' +

                                            "}";

                    this.otpverificationMessagelabel4.Content = "Please Wait.....";

                    this.UpdateEmailIDverifyOTPbutton.IsEnabled = false;
                    this.UpdateEmailIDresendOTPbutton.IsEnabled = false;
                    dispatcherTimerOTP.Stop();
                    dispatcherTimerResendOTP.Stop();

                    this.otpverificationMessagelabel3.Content = "";

                    JObject d = await Task.Run(() => AsyncUpdateEmailonClearBalanceServer());



                    this.otpverificationMessagelabel4.Visibility = Visibility.Hidden;

                    string verifyMessagestr = (string)d.SelectToken("result.message");
                    string verifyMessagestr1 = (string)d.SelectToken("result.message1");
                    string verifyMessagestr2 = (string)d.SelectToken("result.message2");
                    string verifyMessagestr3 = (string)d.SelectToken("result.message3");

                    string verifycustomeremailstr = (string)d.SelectToken("result.cust_email");
                    string verifystartdatestr = (string)d.SelectToken("result.start_date");
                    string verifyenddatestr = (string)d.SelectToken("result.end_date");

                    string verifystaus = (string)d.SelectToken("result.status");

                    if (verifystaus == "True")
                    {

                        this.Groupboxforotpverification.Visibility = Visibility.Hidden;
                        this.StackPannelforDisplayingMessageafterSignSuccessFully.Visibility = Visibility.Visible;
                        this.OkButtonifOTPmismatch.Visibility = Visibility.Hidden;

                        byte[] binarySignupIcon = Convert.FromBase64String("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");
                        BitmapImage bitmapImageSignuplogo = new BitmapImage();
                        bitmapImageSignuplogo.BeginInit();
                        bitmapImageSignuplogo.StreamSource = new MemoryStream(binarySignupIcon);
                        bitmapImageSignuplogo.EndInit();
                        ImageforSignupSuccesfully.Source = bitmapImageSignuplogo;


                        this.MessageafterSignUpComplete1.Content = verifyMessagestr;
                        this.MessageafterSignUpComplete2.Content = verifyMessagestr1;
                        this.MessageafterSignUpComplete3.Content = verifyMessagestr2;
                        this.MessageafterSignUpComplete4.Content = verifyMessagestr3;

                        this.OkButtonafterSignUpComplete.Visibility = Visibility.Visible;

                        clearOTPTextBox();


                        //DialogResult result = System.Windows.Forms.MessageBox.Show(verifyMessagestr, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //if (result == System.Windows.Forms.DialogResult.OK)
                        //{
                        //    System.Windows.Forms.Application.Restart();
                        //    System.Windows.Application.Current.Shutdown();
                        //}

                    }
                    else
                    {
                        this.Groupboxforotpverification.Visibility = Visibility.Hidden;
                        this.StackPannelforDisplayingMessageafterSignSuccessFully.Visibility = Visibility.Visible;


                        this.MessageafterSignUpComplete1.Content = verifyMessagestr;
                        this.MessageafterSignUpComplete2.Content = "";
                        this.MessageafterSignUpComplete3.Content = "";
                        this.MessageafterSignUpComplete4.Content = "";

                        this.OkButtonafterSignUpComplete.Visibility = Visibility.Visible;


                        clearOTPTextBox();


                    }
                }
                else
                {
                    clearOTPTextBox();

                    this.StackPannelforDisplayingMessageafterSignSuccessFully.Visibility = Visibility.Visible;


                    byte[] binaryAlertlogo = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAABAAAAAQACAYAAAB/HSuDAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAE5MSURBVHhe7d1/iJ/XfS/4c575zsT1ilrRCjZ/qMaz+mFdSEBhG1AhZWykcbysvKPJldkWerHF1hCDw7W4DVvjmN7gGLcki3WJQQEXZNMuLVj1aBot1/aMsIcGriGBeLEhsn7sGF9d1n94VTl4XXXmO8/Z7zNzmus4sa0f8+M83+/rBclz3sdpqOMZzZzP9/M5TwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAWGcxPwGAPnXmwNjWODSyrarqL/R+9G+rev9q9lNK78aQLqY6vjv0ucWLo8/Pvbv8fwAA9CUFAADoM+fuuWtHHFr6gxiriZTCF2MMN+W/9Fm6IaUzIVYnu3X6u9unZ17P+wBAH1AAAIA+MH/v2BeWFob/IFTxX/d+uH81b9+g9HpK6e+GUuevR6dfejtvAgAtpQAAAC12dvLubVXqPhVjPNiLnZXd1ZdSmB2q4jdHX3j5TN4CAFpGAQAAWur8xPj9oQpP9X6Yb85bayqlcCXF9NjFS4tH75yb6+ZtAKAlFAAAoGWWP/UPS8d6P8QP5K31lcJrVRUP6wYAgHap8hMAaIHmU//e4f+NDTv8N2LYu1Snn52b3P8neQcAaAEdAADQEucO7n+oivEHOZYhhe9vPznzrZwAgIIpAABAC1yYHD/Ue/xN719rdtHf9Up1OrJjevZojgBAoRQAAKBwFw7u/2rvR/Z/7P3U3pS3StNcCPiH26dmTqxEAKBECgAAULD5r9+1u07pH3rLrSs7ZWreEBBDGt9+cvbHeQsAKIwCAAAU6syBsa2d4eGfxBBvy1tFSyFcTt34lZ0/evl83gIACuItAABQqE5n+FhbDv+NGMLmaij91StjY8XdUwAAKAAAQJGaS/9ijM3Ff+0Sw95tW4YfzgkAKIgRAAAoTNP6Pzw88vPesui5/0/S3AcwVMUvj77w8pm8BQAUQAcAABSmaf3vPVp5+G/EGG6q63TcKAAAlEUBAAAK0trW/48zCgAAxTECAACFmJ8Y27wUR34eY/hC3mq15VGAVP2r0emX3s5bAMAG0gEAAIWoq+Gn+uXw31geBajq4zkCABtMAQAACnDh4L4DvSPz/Tn2kzvOHdz/UF4DABvICAAAbLCm9b+Ow2+EGLflrf6SwgdVqr5kFAAANpYOAADYYE3rf98e/hsxbDIKAAAbTwEAADbQucl9d/dp6//H3XH+4P5v5DUAsAGMAADABnnz3rFNN3WH34gh3pa3+loK4XIdhr60a+rFi3kLAFhHOgAAYIPc3B15clAO/40YwuYqLB3LEQBYZzoAAGADnJvYd0dVVa/kOFBSHQ7vmJ55NkcAYJ0oAADAOhu01v+PMwoAABvDCAAArLNBa/3/OKMAALAxdAAAwDo6O7l/71CI/ynHgZZC+MMdUzN/myMAsMYUAABgnczfN3bT0vvDP4sx7s5bg+69xcWFf7X71Nx7OQMAa8gIAACsk/r9kccd/n/F1k5n2CgAAKwTHQAAsA5y6/8/9JadlR0+4t7tUzMn8hoAWCMKAACwxrT+fyajAACwDowAAMAaq3/xuUcd/j/V1pHh4afyGgBYIzoAAGANvTUxvqdThZ/0llr/P0uq79l+8vSpnACAVaYDAADWyCtjY51OlY73lg7/VyUem58Y25wDALDKFAAAYI3cumXk271D7Z4c+SwxbqsrowAAsFaMAADAGtD6fwOMAgDAmtABAACrTOv/jTIKAABrQQEAAFbZti3DD2v9vwExbkvVyOM5AQCrxAgAAKyi+a/ftXupTj+LMdyUt0rUDSlc6f0WsCnnItV1fefO6dOv5ggA3CAdAACwSprW/7pOxws//Ife4f9oHdIjORUrVvH4m/eOFV2kAIA2UQAAgFWy3Pofw94ci5RSOlPdsvDYzpOzT/di0Z+uxxBvu7k78mSOAMANUgAAgFXQtP7HFEufW+/WMRwefW7uShOqujocUvhg+a8UKoXw0LmJfXfkCADcAAUAAFgFSyk904bW/11Ts6/lFEanX3q7DaMAVRWfmb9vrOz/bQGgBRQAAOAGnTu4/6EYwldzLNK/tP7n+EttGAUIIe6o3/dWAAC4UQoAAHAD5ie+dlsVYvFz6imlB/+l9f/jqhgfTM1bAUoWw8NnJ/cXfb8CAJROAQAAbkBd1cdLf51eDOHpT3ud3ugLL59JMf1ad0BhOlUKx40CAMD1UwAAgOvUtP73HkVfUJdCevvDzsJnzvlfvLR4tPcf/uX9ACWKMe42CgAA1y/mJwBwDZrW/zrWb5T+6X9d13d+2qf/H9W8yWCpTj8r/DLD7lJIv//RywwBgKujAwAArsNSVf+g7a3/H9eWUYChEI69MjbWyRkAuEoKAABwjc5PjN/fO1wfyLFMKV28mtb/j1seBQjhpyupVHHPrVtGvp0DAHCVjAAAwDU4O3n3tiosvdH7Abo5b5Up1fdsP3n6VE7X5K2J8T2dKvyktyz5U/Zutw5fuX165vWcAYDPoAMAAK5B7/B/rPjDf0jPXu/hv5EP1U+spGJ1OlU6bhQAAK6eAgAAXKW2tP5X9eKRnK7bO5cWvtv7Lyv803WjAABwLYwAAMBVOHNgbGtneORcP7f+f1xbRgGqGL/UXGCYMwDwCXQAAMBV6HSG+771/+PaMgpQ10YBAOBqKAAAwGe4MDl+KMZ4KMdSvbe4uPitvF411W8v/HlKqexP12PYu23L8MM5AQCfwAgAAHyKpvV/eHjk573l1pWdYt27fWrmRF6vqrOT+/cOhfgPvWWxn7KnFK4MVfHLRgEA4JPpAACAT9G0/vceRR/+U0on1urw39g1NftaSOFojkWKMdxkFAAAPp0CAAB8gra0/ne7iw/m9Zqpbll4zCgAALSbEQAA+A3mJ8Y213H4jRDjtrxVqjVr/f+4NowChBQ+qFL1pdHpl97OOwBApgMAAH6Duhp+qvTDfwrh1Hod/hvNKEAM4Yc5limGTXVVH88JAPgIBQAA+JgLB/cd6J0k78+xSL3D/+U6DK156//HfdhZeCSFVPqn63ecO7j/obwGADIjAADwEW1p/U91OLxjeubZHNfVuYl9d1RV9UqOZTIKAAC/RgcAAHxEW1r/N+rw39g5ffrVGMLTOZbJKAAA/BoFAADImk+2tf5fnbaMApyfGC/6nycArCcFAADoefPesU2xisV/Ytw7dH9r19SLF3PcMF98fu6DGNMDOZarCk+dnby79Dc5AMC6UAAAgJ6buyNPxhBvy7FUr+6cmv3LvN5w2184PRtC2rBRhKsRQ9hchaVjOQLAQFMAAGDgNa3/KYSyb41vLrWrq8M5FaOqF4+ElDa8I+HTxBAOGAUAAAUAAAZcW1r/65AeKfFG+9HpucshpA2/k+AzGQUAAAUAAAbbby2NPN6K1v+Ts8Xeur/95OlTbRgFGArdH+QIAANJAQCAgXV2cv/ekLT+r4Y2jAKEEA9emBw/lAMADBwFAAAG0vx9YzdVKTSt/52VnTLVMX2nxNb/j1seBYjxSI4lO3bmwNjWvAaAgaIAAMBAqt8feTzGuDvHMqXw2sVLi0dzKt72qZkTKaUTOZZqa6cz7K0AAAwkBQAABs5y638MD+dYpJTClaqKh++cm+vmrVbodhebCwHfW0llijEeMgoAwCBSAABgoLSl9T/F9NjoCy+fybE1dp+aaw7/5b8VwCgAAANIAQCAgVL/YuRPtf6vrbaMAgwPD38vrwFgICgAADAw3poY39N7PLqSitVtY+v/xzWjACmEyzkWKt5/4eC+AzkAQN9TAABgILwyNtbpVKn41v+eJ9rY+v9xy6MAdWjBWwHisfmJsc05AEBfUwAAYCDcumXk273DXtMBULD0+juXFr6bQ+vtmJ55NoVwKscyxbitroafygkA+poCAAB9ry2t/926/a3/H1eHIaMAAFAIBQAA+tpK6394prcsvvX/9umZ1/O6b+yaevFiW0YB3rx3bFMOANCXFAAA6Gvbtgw37/v/3ZVUqJTe7KfW/49rRgFCCi/mWKYYt93cHXkyJwDoSwoAAPSt+a/ftTum+HiOpeouxfBAv7X+f1yVqgdDCh/kWKQUwkPnJvbdkSMA9B0FAAD6UtP6X9fpeIzhprxVphSO7pqafS2nvjU6/dLbdUiP5FisWMXjRgEA6FcKAAD0peXW/xj25liklNKZ6paFx3LseztPzj7de7y6ksoUQ7zNKAAA/UoBAIC+05bW/zqGw6PPzV3JeSBUdXXYKAAAbAwFAAD6Tp3SMa3/ZWrNKECMx+bvGyv7awgArpECAAB95dzB/Q/1HkV/eptCenuQWv8/bnkUIIWiix8xxt31+yOld5EAwDVRAACgb8xPfO22KsTi57dTnQau9f/jqioeTimU/b9BDA+fndxf9D0SAHAtFAAA6Bt1VR/vHdqKvsE9hvD0zunTRV+Etx5GX3j5TIqp9C6ITpXCcaMAAPQLBQAA+kJbWv8/7CwUP/++Xi5eWjxqFAAA1k/MTwBorbOTd28bSks/L/3T/7qu7/Tp/69q3tiwVKefFX5pYzctpd/b8fezP80ZAFpJBwAArVeFpWOlH/57fujw/+uaUYAY0ndyLFUnDoVnXhkb6+QMAK2kAABAq52fGL8/hnAgxzKldLGqtf5/knf+cfH7vf+RXs+xUHHPrVtGvp0DALSSEQAAWqtp/a/C0hu9H2ab81aZUn3P9pOnT+XEb/DWxPieThV+0luW/Cl7t1uHr9w+PVN4sQIAfjMdAAC0VtP6X/zhP6RnHf4/Wz5UP7GSitXpVOm4UQAA2koBAIBWak/r/+KRnPgM71xa+K5RAABYO0YAAGidMwfGtg4Pj/y8t9y6slMorf/XrA2jACmFK0NV/HJzgWHeAoBW0AEAQOt0OsPHeo/CD//pbx3+r10zCpBC+H6ORWpeWVjXRgEAaB8FAABa5cLk+KEY46EcS/XeYnfxm3nNNRr67YXvpJTK/nQ9hr3btgw/nBMAtIIRAABaozWt/yHcu31q5kRecx3OTu7fOxTiP/SWRgEAYJXoAACgNdrQ+p9SOuHwf+N2Tc2+FlI4mmORjAIA0DYKAAC0woWD+w60ofW/2118MK+5QdUtC4+1YhTg88PfyAkAimYEAIDizU+Mba7j8Bshxm15q0hLKfybXSdn/jpHVsGFg/u/2vvn3owClCuFD6pUfWl0+qW38w4AFEkHAADFq6vhp0o//KcQTjn8r77tJ2d/HEN4OscyxbCprurjOQFAsRQAACha0/rfO2Hdn2OReof/y3UY0vq/Rj7sLDySQir90/U7zh3c/1BeA0CRjAAAUKy2tP6nOhzeMT3zbI6sgXMT++6oquqVHMtkFACAwukAAKBYKY58rw2t/w7/a2/n9OlX2zAKsBTrZ3ICgOIoAABQpOYT3xTDH+dYJK3/66sNowAxhv3nJ8aLHlkBYHApAABQnDfvHdsUq1j8pWoppcd2Tb14MUfW2Befn/sgpvTNHMtVhafOTt5ddOcKAINJAQCA4tzcHXkyhnhbjqV6defJ2bJb0vvQ9pOnT4WQih65iCFsrsLSsRwBoBgKAAAUZbn1P4Syb1NvLnurq8M5sc6qevFISKnozosYwgGjAACURgEAgGK0pfW/DukRN71vnNHpucshpPLvXjAKAEBhFAAAKMZvLY78mdZ/rkZrRgFS96kcAWDDKQAAUISzk/v39k5MD+dYpJTClbobH8iRDbY8ChDCeyupTDHGQxcmxw/lCAAbSgEAgA03f9/YTVUKTet/Z2WnTCmmx3b+6OXzObLBVkYBQhtew3jszIGxrXkNABtGAQCADVe/P/J4jHF3jmVK4bWLlxaP5kQhtk/NnEgpncixVFs7nWFvBQBgwykAALCh2tL6X1Xx8J1zc928RUG63cWmC8AoAAB8BgUAADbMK2Njnba0/o++8PKZHCnM7lNzzeG/FaMA8xNjm/MaANadAgAAG+bWLSPf1vrPamjLKEBdDXsrAAAbRgEAgA3x1sT4nt7j0ZVUrG43hQe1/rdDHTtHUgjNxYAFi/dfOLjvQA4AsK4UAABYd03rf6dKxbf+9zxx+/TM63lN4XZNvXgx1KF5NWDholEAADaEAgAA665p/e8dgpoOgIKl19+5tPDdHGiJHdMzz6YQTuVYphi3GQUAYCMoAACwrlrT+l+79b+t6jD0oFEAAPh1CgAArJvl1v8Ymveha/1nzbRlFCDF+IM37x3blCMArDkFAADWzbYtww+HGPbmWKSU0pnqtxf+PEdaqhkF6D1eXUlliiHednN35MkcAWDNKQAAsC7mv37X7pji4zmWqlvHcHj0ubkrOdNiVV0dDil8kGORUggPnZvYd0eOALCmFAAAWHNN639dp+MxhpvyVplSOLprava1nGi50emX3q5DeiTHYsUqHjcKAMB6UAAAYM21pvX/loXHcqRP7Dw5+3TvYRQAAHoUAABYU+fuuWuH1n82UltGAS4c3P/VHAFgTSgAALCmqk56pvjW/xie1vrfv5pRgJRS8d0dKYRn5u8bK/t7BYBWUwAAYM2cO7j/od6j6AvOUkhv/9OQ1v9+958vLz7d+4dddJEnxri7fn+k9G4ZAFos5icArKr5ia/dVsf6jd5PmqIvN6vr+s6d06eLnhFndTRvoliq088K70jpLoX0+zpSAFgLOgAAWBN1VR8v/fAfQ3ja4X9wjL7w8pkUix8F6FQpHDcKAMBaUAAAYNW1pfX/w85C8a+IY3VdvLR41CgAAIPKCAAAq+rs5N3bqrD0Ru8HzOa8VSSt/4OrLaMA3Tp85fbpmddzBoAbpgMAgFXVO/wfK/3wH0J61uF/cDWjAL3D/1/kWKpOp0rHXxkb6+QMADdMBwAAq+b8xPj9sQrHcyxTShertPil0em5y3lnIJw5MLZ1uDO8O8W4LdTpC6kKnRjixVTX73ZGumdGn597N/9HB0JzsL51y/BPer8K7clbpfrO9qmZf5/XAHBDFAAAWBVtaf0Pqb5n+8nTp3Lqa+cnx7/Y+/s9GGM10Yu/u7L7CVJ6M8TqZLdOfzcobedvTYzv6VThJ71lyZ+yGwUAYNUoAACwKnqHzR/1fqgcyLFQ6dntU7OHc+hbzcE/hvR478f8wbx1TVIKs0spfGsQDp0XJsebT9f/bCWVKr3+zqXFr9w5N9fNGwBwXdwBAMAN6x04/6D4w3/T+l8vHsmpL71579im3oG2uYPhZ9d7+G/EGPZ3qvCzCwf3/03z35m3+9I7lxa+2xywcyxU3HPr54f/JAcAuG46AAC4Icuz5cMjP+8tt67sFOve7VMzJ/K678zfO/aFenFkqveTfW/eWhUppTNpqbpn549ePp+3+s7Zyf17h0L8h96y2FGAlMKVoSp+ubnAMG8BwDXTAQDADel0ho/1HkUf/nuH2BP9fPg/d89dO5a6w/9ptQ//jead9FUn/afmkJy3+s6uqdnXQgpHcyxS88rCuvZWAABujAIAANftwuT4od4B8VCOpXqv2118MK/7TtOiH4fqH8UQb8tba2FrleJUc9Fjzn2numXhsabbIccyxbB325bhh3MCgGumAADAdWla/3uP5tP/0j24+9Tce3ndV5pPg39rceT55lP6vLVmYgxfGArdH/XrnQCjz81dqWNoLogs+qK9mOLj81+/a83/eQPQnxQAALguw53hH/QeWv830O9sHn4oxHB3jusg7vmtpZHHc+g7bRoFyBEArokCAADX7MLBfQd6J5E/yLFIKYTL/dz6Pz8xtjlUcf1fX5fCQ82dAzn1nWYUoPc3WfaFhzHsPXdw/0M5AcBVUwAA4JosHzxDLL/1vw5H+rX1v1FXIw/HEHr/LNZdp+rUj+Z131keBajTAzkWqwrxyfmJr63lvQ8A9CEFAACuSV0NPxViLPoyuBTCqR3TM8/m2Hea2f/e3+O/zXEDxD/Kd0D0pZ3Tp1+NITydY5li2FRXtVEAAK6JAgAAV2259T/E+3MsUtP6X4ehvm39b9z633bu2KBP//9FpzM00vta6F8fdhYeSSG9nWOp7jAKAMC1UAAA4Kq0qfV/19SLF3PqU9X/lBcbJlZpIi/70hefn/sg1al5K0DRjAIAcC0UAAC4KnU18qTW/zL0DqbrePP/J0hxf171rdaMAsS6Da/jBKAACgAAfKZzE/vu6D2+sZIKlcIHQ3X1zZz6W4xfyKuN0zt4rnSF9LdYLzwWUiq7oySGu89PjBc9mgNAGRQAAPhUb947tilWsfjLxuqQHhmdfqn0me0btvzPY2Pn/39pqRopuiNkNYxOz10OIZV/p0QVnjo7eXff//MA4MYoAADwqW7ujjwZQyx9xvjVnSdny27VXiX/zdLnijnkpbru2zcBfNT2k6dP9f5uix4taYpCVVgyCgDAp1IAAOATNa3/KYSybxlP4YOqroq/rG21dJeWNr79fwBV9eKR0kcBYggHjAIA8GkUAAD4jebvG7tJ6z+saNMowPy9Y4pEAPxGCgAA/Eb1+yOPl976n0L48aC0/rPxmlGAGNJf51ikZhRgaXH4BzkCwK9QAADg15yd3L+3d5J4OMcipRSuDMX4QI6wLhYWF4/0Hu+tpDLFGA9dmBw/lCMA/JICAAC/omn9r1JoWv87KztlSjE9NvrCy2dyhHWx+9Rcc/gvfxQghGNnDowNxCWNAFw9BQAAfsVy63+Mu3MsUwqvXby0eDQnWFfbp2ZOpJRO5FiqrZ3OsLcCAPArFAAA+KW2tP5XVTx859xcN2/Buut2F5suAKMAALSKAgAAy14ZG+sMpfBMb1l0638M6Tta/9lozShACuGbORYrpfCD+YmxzTkCMOAUAABYduuWkW+HGL+YY6HS6+/84+L3c4ANtWNq5m9TCKdyLFKM4Qt1NfxUjgAMOAUAAMJbE+N7eo9HV1Kxut1a6z9lqcPQgymEyzkWKt5/4eC+AzkAMMAUAAAGXNP636lS8bf+9zxx+/TM63kNRdg19eLFUIfm1YCFi8eMAgCgAAAw4JZb/0NsOgAKll5/59LCd3OAouyYnnm29FGAEOM2owAAKAAADLDzk+PNzL/Wf7hBbRkFODe57+4cABhACgAAA6pp/Y8tuPW/d6j6vtZ/Src8CpDSIzkWK4Z47M17xzblCMCAUQAAGFDbtgw/3DsN7M2xSCmlM0O/vfCdHKFoO07O/rD3eHUllSmGeNvN3ZEncwRgwCgAAAyg+a/ftTum+HiOperWMRwefW7uSs5QvKquDocUPsixSCmEh85N7LsjRwAGiAIAwIBpWv/rOh2PMdyUt8qUwtFdU7Ov5QStMDr90tt1aMEoQBWPGwUAGDwKAAADpi2t/9UtC4/lCK2y8+Ts072HUQAAiqMAADBA5ie+dluV4p/lWCyt/7RdMwqQUij6aziF8I2zk/uLLgYCsLoUAAAGSF3Vx0MMRbf9xhCe1vpP2zWjACmm0rtYOlUKx+fvGyt7HAiAVaMAADAgzh3c/1DvUfTFXymktz/sLBQ/Pw1X4+KlxaO9L+qii1kxxt31+yOlXwgKwCpRAAAYAMut/yEWP++b6nT4i8/PFX2DOlytO+fmulUVix8FCDE8bBQAYDAoAAAMgLa0/u+cPl30xWlwrUZfePmMUQAASqEAANDnzk3u/+PeQ+s/bJDWjAL84nOP5ghAn1IAAOhjZyfv3hZD/F6OxUohPaj1n37VjAKkGB7oLbsrO6VKf/rWxPieHADoQwoAAH2sCkvHYgibcyxUenbn1OkXc4C+tGNq5s3e44mVVKxOp0rHXxkb6+QMQJ9RAADoU+cnxu/vHf4P5FimlC5W9eKRnKCvvXNp4bu9L/rXcyxU3HPrlpFv5wBAn1EAAOhDTet/70/4p3IsWHpwdHrucg7Q15pRgG4dD/eWhY8ChEeNAgD0JwUAgD7Ultb/7SdPn8oBBsLt0zNNB4BRAAA2hAIAQJ+5MDl+qPjW/xDe0/rPoGpGAVJKZ3IsVNyzbcvwwzkA0CcUAAD6yJkDY1t7j2MrqWha/xlYzShAHUPxowAxxcfnv37X7hwB6AMKAAB9pNMZbg7/TRGgWCmlE9unZk7kCANp19TsayGFozkWKcZwU10bBQDoJwoAAH1iufU/xkM5luq9bnfxwbyGgVbdsvBY8aMAMew1CgDQPxQAAPpAm1r/d5+aey+vYaCNPjd3xSgAAOtJAQCgD4wMDzev/NP6Dy3TllGApZSeyRGAFlMAAGi5Cwf3HUgh/lGORUohXK5jx63/8Bv80/DCd1JIb+dYpBjCV88d3P9QjgC0lAIAQIvNT4xt7v1qXn7rfx2O7Jp68WJOwEd88fm5D1KdmlGAolUhPjk/8bXbcgSghRQAAFqsroafCjFuy7FIKYRTO6Znns0R+A12Tp9+NYbwdI5limFTXdXHcwKghRQAAFqqaf3v/UZ+f45FWm79D0Nu/Yer8GFn4ZHSRwF67jAKANBeCgAALaT1H/qPUQAA1poCAEALpWrk8eJb/1OY1foP12Z5FCCFv8yxTDFsWqrqH+QEQIsoAAC0zLmJfXekEMpuwU3hg6FUPZATcA1iWvhWSKnozpkYwoHzE+NFjyAB8OsUAABa5M17xzbFKhZ/CVcd0iOj0y+VPssMRRqdnrscQir/7owqPHV28u6iO5EA+FUKAAAtcnN35MkYYumzt6/uPDlb9m3mULjtJ0+fCiEVPUITQ9hchaXy7yIB4JcUAABaoi2t/1VdFX+JGbRBVS8eMQoAwGpSAABogfn7xm6qqvhMjsXS+g+rp02jAGcOjG3NCYCCKQAAtED9/sjjIcQdOZYphdcu/uPiD3MCVkEzCpBSOpFjkZpRgE5n2CgAQAsoAAAU7uzk/r2937AfzrFIKYUrVRUP3zk3181bwCrpdhebLoD3VlKZYoyHLkyOH8oRgEIpAAAUbLn1P4Xm1v/Oyk6ZUkyPjb7w8pkcgVW0+9Rcc/gvfxQghGNGAQDKpgAAULCm9T/GuDvHMjWt/5cWj+YErIHtUzMnSh8F6NlqFACgbAoAAIXS+g98lFEAAG6UAgBAgV4ZG+sMhdB8klZ063+M4S+0/sP6aEYBUh2+lWO5UnpqfmJsc04AFEQBAKBAt24Z+XbveL0nx0Kl19+5tPDdHIB1sGN65tkUwqkcyxTjtroafionAAqiAABQmLcmxpuD/6MrqVjdbq31HzZCHYYeTCFczrFQ8f4LB/cdyAGAQigAABSkaf3vVKn4W/97nrh9eub1vAbW0a6pFy+GOhzJsWDxmFEAgLIoAAAUROs/cDWMAgBwPRQAAAox//W7dqcU/rccS9VNS+EBrf+w8ZpRgJDCBzkWKt5/bmLfHTkAsMEUAAAK0LT+13U6HmO4KW+VKYWjO/5+9qc5ARuoGQWoQ3okx2LFKh5/896xTTkCsIEUAAAKsG3L8MMhhr05FimldKa6ZeGxHIEC7Dw5+3Tv8epKKlMM8babuyNP5gjABlIAANhgTet/TPHxHEvVrWM4PPrc3JWcgUJUdXW49FGAFMJDRgEANp4CAMAGalPr/66p2ddyAgoyOv3S20YBALgaCgAAG+h3Ng8/pPUfuFFtGQX4raWR0rudAPqaAgDABpmf+NptMRbf+t8UAB7U+g/lq2J8MKVQ9vdqCg+dndxfdNEToJ8pAABskLqqj4cYim6HjSE8vXP6dNGfKgIrRl94+UyKqfRunU6VwvH5+8bKHnsC6FMKAAAb4NzB/Q/1HkVfiJVCevvDzkLxc8XAf3Xx0uLR3jdv0fd1xBh31+8bBQDYCAoAAOusaf2vQiz+lVipToe/+Pxc0TeLA7/qzrm5blXFw8WPAsTwsFEAgPWnAACwzrT+A2vJKAAAn0QBAGAdnZ8Yv7/3KPtd2CldjLVb/6HNlkcBQno9xyItjwL8YuRPcwRgHSgAAKyTs5N3b+v9qftUjgVLD45Oz13OAWihZhSgW8fDvWV3ZadYj741Mb4nrwFYYwoAAOukCkvHYgibcyxUenb7ydOncgBa7PbpmaYD4ImVVKxOp0rHXxkb6+QMwBpSAABYB03rf+/wfyDHMqV0saoXj+QE9IF3Li18t/RRgBDinlu3jHw7BwDWkAIAwBrT+g9sFKMAAHyUAgDAGhsK3R+U3vofQ/prrf/Qn1ZGAeKf51iqTqcKzxgFAFhbCgAAa+jC5Pih3i/eB3Ms1XsLi1r/oZ9Vv/3PT6SUzuRYqt/dtmX44bwGYA0oAACskTMHxrb2HsdWUtEe3H1q7r28BvrQ6HNzV+oYih8FiCk+Pv/1u3bnCMAqUwAAWCOdznBz+G+KAMVKKZ3YPjVzIkegj+2amn0tpHA0xyLFGG6qa28FAFgrCgAAa6Bp/Y8xHsqxVO91u4sP5jUwAKpbFh4rfhQghr1GAQDWhgIAwCrT+g+UyigAwGBTAABYZcPDw9/rPcpu/Q/hlNZ/GEzNKEAM4Yc5Fml5FCClNhRSAVpFAQBgFV04uO9A71fX+3MsUu/wf7kOQ1r/YYB92Fl4JIX0do6luuPcwf0P5TUAq0ABAGCVzE+Mbe4d/sv/xKoOR3ZNvXgxJ2AAffH5uQ9SnZpRgKJVIT45P/G123IE4AYpAACskroafirEuC3HIjWt/zumZ57NERhgO6dPvxpDeDrHMsWwqa7q4zkBcIMUAABWgdZ/oI2MAgAMFgUAgBv05r1jm9rQ+t/7Jf9bWv+Bj2pGAWJMD+RYrGYU4Ozk3UV3WAG0gQIAwA26uTvyZOmt/z2v7pya/cu8Bvil7S+cng0hlT0aFMOmKix5KwDADVIAALgB5yb23ZFCKLs1NYUPqroq/rIvYONU9eKRkFLRHUIxhAPnJ8aLHrUCKJ0CAMB1alr/YxWLv5yqDumR0emXSp/xBTbQ6PTc5RBS+XeEVOEpowAA108BAOA6Na3/McTSX0/16s6Ts2Xf8g0UYfvJ06dKHwWIIWw2CgBw/RQAAK5DG1r/UwpXtP4D16IZBej92fFujkUyCgBw/RQAAK7R/H1jN8XYglv/Y3pM6z9wLZpRgBjDN3MsVqzC984cGNuaIwBXSQEA4BrV7488HmPcnWOZUnjt4qXFozkBXLXtUzMnUkoncizV1k5n2CgAwDVSAAC4Bmcn9+8NMTycY5GWW/+rePjOublu3gK4Jt3uYnMh4HsrqUwxxkMXJscP5QjAVVAAALhKTet/lUJz639nZadMy63/L7x8JkeAa7b71Fxz+C//rQAhHDMKAHD1FAAArpLWf2CQGAUA6D8KAABX4a2J8T2lt/73dFMMD2j9B1bL0PDiN1MIl3Ms0vIowMF9B3IE4FMoAAB8hlfGxjqdKhXf+t/zxI6pmTfzGuCGjT4/926ow5EcCxaPzU+Mbc4BgE+gAADwGW7dMvLt3i+Xe3IsVHr9nUsL380BYNXsmJ55NoVwKscyxbitroafygmAT6AAAPApllv/Q3h0JRWr263d+g+snToMPVj6KEAI8X6jAACfTgEA4BO0qfX/9umZ1/MaYNXtmnrxolEAgPZTAAD4BL/z+c6fFt/6n9KbWv+B9dCMAoQUXsyxTDFuS3HkezkB8DEKAAC/wfzX79rd+yOy+Nb/Jbf+A+uoStWDIYUPcixSiuGPz03suyNHAD5CAQDgY5rW/7pOx2MMN+WtMqVwdNfU7Gs5Aay50emX3q5DeiTHYsUqHn/z3rFNOQKQKQAAfMy2LcMPhxj25liklNKZ6paFx3IEWDc7T84+3Xu8upLKFEO87ebuyJM5ApApAAB8RNP6H1N8PMdSdesYDo8+N3clZ4B1VdXV4eJHAUJ4yCgAwK9SAADItP4DXB2jAADtpAAAkG37/PA3im/9D+ltrf9ACZZHAVIouhjZjAL81uLIn+UIMPAUAAB65ie+dlsVYvHzoqlOWv+BYlRVPJxSKPvPpBgePju5v+jiLsB6UQAA6Kmr+njvl8Si20RjCE/vnD5d9MVbwGAZfeHlMymm0ruSOlUKx+fvGyt7vAtgHSgAAAPv3MH9D/UeRV8U1bT+f9hZKH7eFhg8Fy8tHi1+FCDG3fX7I6Vf8Aqw5hQAgIHWptb/Lz4/V/SN28BgunNurmsUAKAdFACAgbYU62eKb/1P4S+1/gMla0YBQqifyLFUy6MAzRtfcgYYOAoAwMA6PzF+f4xhf45lSuliTAvfygmgWP/5H7t/3vtD6/Uci9SMAty6ZeTbOQIMHAUAYCCdnbx7W+9PwKdyLFh6cHR67nIOAMVqRgG6dTzcW3ZXdor16FsT43vyGmCgKAAAA6kKS8diCJtzLFR6dvvJ06dyACje7dMzTQdA8aMAnSoZBQAGkgIAMHCWW/9DOJBjmVK6WNWLR3ICaI13Li18t/RRgBDiHqMAwCBSAAAGitZ/gLVlFACgXAoAwECpUvep0lv/U0ontP4DbbY8CpDC0RxL1enEcMwoADBIFACAgXFhcvxQjPFQjqV6r9tdfDCvAVqrumXhsZTSmRzLFMPebVuGH84JoO8pAAAD4cyBsa29x7GVVLQHd5+aey+vAVpr9Lm5K3UMxY8CxBQfn//6XbtzBOhrCgDAQOh0hpvDf1MEKNZy6//UzIkcAVpv19Tsa6WPAsQYbqprbwUABoMCAND3tP4DbByjAADlUAAA+tr8xFhz4V/xrf9LKRzR+g/0o2YUIIbwQI7FakYBzt1z144cAfqSAgDQ1+pquHnlX9mt/yGc2nVy5q9zhE8Vq0qhiNbZfnL2xzGEp3MsUjMKUHXSMzkC9CUFAKBvXTi470DvV7r7cyxS7/B/uQ5DWv+5akP1wsW83HB1qC7nJXymDzsLj6SQ3s6xVHecO7j/obwG6DsKAEBfWmn9j+Xf+l+HI7umXizmQEf5RqfnLocUPshxQ6WlcooRlO+Lz899kOrUvBWgaFWIT85PfO22HAH6igIA0JeWW/9j3JZjkZrW/x3TM8/mCFcvpnfzasOkFK64t4JrtXP69KuljwKEGDbVVX08J4C+ogAA9J1zk/vu1vpPP4spvpqXG+nH+QnXpC2jAOcP7v9GXgP0DQUAoK+8ee/YptiC1v+U0mNa/7leKdTTebmB0t/lBVyTZhQgpvTNHMsV45NnJ+8uupMM4FopAAB95ebuyJMxxNJnN1/deXK27BZYilbd0p3d6HsA6tg5lZdwzbafPN37+klFj0DFEDZXYan8u2QAroECANA3zk3suyOFUPbtzb1DW1VXxV+CRdmW36sewwYentJJHSzcqKpePBJSKvrrKIZw4PzEeNEjZQDXQgEA6AvLrf9VLP7SpjqkR0anXyp99pUWiJ2FJ5qL+HJcV906ficv4botv9EipPLvQqnCU0YBgH6hAAD0Ba3/DJrR5+fejTH8RY7rKD17+/TM6znADTEKALC+FACA1js7uX9v6a3/zSe1VYxu/WdV/VNn4fu9x09X0tprbm6vOouP5AirYnFx8Vu9R9GvlFweBZgc/4McAVpLAQBotfn7xm6qUii+9T/F9NjoCy+fyRFWRXObetVZuCel8G7eWjvLlw7Ge5rOg7wDq2L3qbnm8F98gTSG8IMzB8a25gjQSgoAQKvV7488HmPcnWOZUnjt4qXFoznBqlo+kNfpnhTC5by16lbuGqj/cMfUzJt5C1bV9qmZEymlEzmWamunM2wUAGg1BQCgtZrW/xDDwzkWabn1v4qH75yb6+YtWHU7/n72p0Mx/l7vK+583lo1TXdBHdOdK7PasHa63cWmC6DsUYAYD12YHD+UI0DrKAAArfSR1v/Oyk6ZtP6zXpqvs6qz+PshhRfz1o1L4bU6Dn1l19Tsa3kH1kxbRgF6jhkFANpKAQBopaVfjPxZ8a3/IfxU6z/rqRkH2H5y5n+sQ/0/9k7v131Tf0rpTF3Hyd5/1+953z/rqRkF6H0FnsyxVFuHO8M/yGuAVon5CdAab02M7+lU4Se9Zcmf/ne7dfiK16WxkS4c3HcghmoixXCwFz/1E8vmDoEqpFNLdfV3Fy//8yljK2yU5p37VVh6o3n9Xt4qU6rvMRoDtI0CANAqr4yNdW7dMtw7/Mc9eatU39k+NfPv8xo23Pn/ef/v1lXcPRTStlDF/y6m0Ekp/ZeU4rt1lc78l0uLP3XopxTnJ8bvj1Xhb3hJ6WKVFr80Oj23ZhdwAqw2BQCgVS5MjjeH6j9bSaVKr79zafErDlMA1+/85PiPer+oHsixUOnZ7VOzh3MAKJ47AIDWaFr/e49HV1Kxut3arf8AN6oOQw+u5estV0e8vxm1yQGgeAoAQCs0rf+dKhV/63/PE+b+AW7c8gWUdTiSY8HisfmJsbLvKwDIFACAVrj188N/Uvrcf3NzevXbC3+eIwA3aMf0zLO9x6srqVAxbqurkSdzAiiaAgBQvPmv37U7hVj43H/o1jEcHn1u7krOAKyCqq4OhxQ+yLFU3zg3se+OvAYolgIAULSm9b+u0/EYw015q0wpHN01NftaTgCsktHpl96uQ3okx2LFKh5/896xTTkCFEkBACjati3DD4cY9uZYpOXW/1sWHssRgFW28+Ts071H0aMAMcTbbu4aBQDKpgAAFKtp/Y8pPp5jqbT+A6yDNowCpBAeMgoAlEwBAChWK1r/Y3ha6z/A2mtGAVJKxXdbNaMA8/eNlf2zCxhYCgBAkc4d3P9Q8a3/Ib39T0Na/wHWy3++vPh07w/foouuzShA/f5I6d1rwICK+QlQjPmJr91Wx/qN3p9QRV+mVNf1nTunT5f9eiqAPtOMhy3V6WeFd4h1l0L6fR1iQGl0AADFqav6eOmH/xjC0w7/AOtv9IWXz6RY/ChAp0rBKABQHAUAoCjLrf8hFH2BUtP6/2FnofhXUgH0q4uXFo8WPwoQ426jAEBpjAAAxWhL63+I9fj2F07P5gStcubA2NY4NLJtKKXO0OcWL44+P/du/kvQKs0oQJ3SG71lZ2WnSEYBgKIoAADFuHBw/D/2/lS6O8dCpWe3T80ezgGKd+Hg/q+GWH0tpPpgCnHHb5qbTimdqWKcTbGefuf/7b5659xcN/8lKNqFyfF/33v82UoqVEpvvvOPi1/2fQWUQAEAKML5ifH7YxWO51imlC5WafFLo9Nzl/MOFOmVsbHO73x++I97P+QfDTFuy9tX672U6v8wdEv3+6PPzV3Je1Ck5mv91i3DP+n9Srsnb5XqO9unZppiBcCGUgAANtzZybu3VWHpjd4fSJvzVplSfc/2k6dP5QRFOj85/gcxpMd7P+J35K3rklJ4t/fv39lxcvaHeQuK9NbE+J5OFX7SWxY9CtCtw1dun555PWeADeESQGDD9Q7/x4o//Det/w7/FKz5JLR3+H+y9730Nzd6+G/EGL4QYzz2f0/u/ys3mVOyfKh+YiUVq9Op0vHm+zRngA2hAABsqOXW/xAO5FimpvW/XjySExTnzXvHNv3OlpGp3vfSn+atVZNC/KP6/ZFX5u8d+0LeguK8c2nhu72v1sI/XY97bt0y8u0cADaEAgCwYZYPFFV4KsdyxXjE3D8l+63u8F+taSEthr314siUTgBK1VywtxTCg71l6RftPXp+cvyLeQ2w7hQAgA2ztDj8g9Jb/1NKJ7ZPzZzIEYpzYfKuZt7/YI5rJ4a96RfDz+QExVl+1V4KR3MsVSem8IxRAGCjKAAAG+LC5PihGOOhHEv1Xre72HyiBEW6cHDfgRDSurUUN+MA5w7ufyhHKE51y8JjzWstcyxTDHu3bRl+OCeAdRXzE2DdnDkwtnV4eOTnveXWlZ1i3evTf0qVX/X3Roxxd95aL+9V9cJOYzGU6uzk/r1DIf5Db1nsp+wphStDVfzy6Asvl12sAPqODgBg3XU6w8d6j6IP/1r/Kd3vbB75ow04/De21tWITy8pVhtGAWIMN9W1twIA608BAFhXbWj9TyFc1vpP6WKVHs3Lddf7Hvm3LgSkZM0oQO8r9XyOZTIKAGwABQBg3cxPjG0OKZV/638djuw+NfdeTlCctybG9/RODzf8rv/r1VzeWb/f2Z8jFGf0ubkrdZ0eyLFYVYp/Nj/xtdtyBFhzCgDAuqmr4adCjNtyLFIK4dSO6Zlnc4QiDcV67V75d5ViqCbyEoq0c/r0qzGEp3MsUwyb6qo+nhPAmlMAANbFym3l8f4ci9S0/tdhSOs/xYtx4w/fdQgbXoSAz/JhZ+GRFNLbOZbqDm/XANaLAgCw5pZb/0NsLv4rWx2O7Jp68WJOUKyUwoZ30sQYvuAeAEr3xefnPkh1OpxjsaoQnzQKAKwHBQBgzbWh9T+k8KLWf9qguTW8OXznuKG6lz5X9vc19DSjAL3HD1dSoYwCAOtEAQBYUxe+vm9/6a3/vcP/B1WqtP7TCrdtvqmcQ3e1pABAK1T1wiMhpdI7vO44N7n/j/MaYE0oAABr5s17xzalFJ/JsVh1SI+MTr9U+owoANdpdHrucgip+EJvDPF7ZyfvVlgD1owCALBmbu6OPNn7Zab0mcZXd56cLfuWaPiIty9fKedTzHrInRm0xvaTp0+FkIoe9WpesVmFpfLvzAFaSwEAWBPnJvbdkUIo+1bjpvW/roq/HAo+6s65uW7v8d5K2lj/fNM/v5uX0ApVvXik9FGAGMKB8xPjZY/OAa2lAACsuqb1P1ax+MuMtP7TYiV83b7X3LCe19AKbRkF6P2G/pRRAGAtKAAAq64Vrf8pvKb1n/aKL+bFBkqn8gJaZXkUIKW/zbFIRgGAtaIAAKyqs5P796YQvpFjkVIKV6oqav2ntdJSPZ2XG6auqw3//wGu12J38Zu9RxGjNJ+kGQW4MDl+KEeAVaEAAKya+fvGbqpSaFr/Oys7ZUoxPTb6wstncoTW2fH3sz/dyDnmpoj2zyP/PJsjtM7uU3PN4b8Nr389dubA2Na8BrhhCgDAqqnfH3k8xrg7xzKl8NrFS4tHc4LWqkP4i7xcdzGEp83/03bbp2ZOpJRO5FiqrZ3OsFEAYNUoAACromn9750KHs6xSP/S+p9vUYdWu/iPiz/sfVWfz3HdpBAuV2nhiRyh1brdxaYLoOxRgBgPGQUAVosCAHDD2tL6H0P6jtZ/+kVTyEohPpbjuokp/cXKTerQfs0oQAqhuQ+gdEYBgFWhAADcsPoXn3u0+Nb/kF5/5x8Xv58D9IUdUzPNTeY/XElrr3dQOuX7iH7TfB81X9s5lmrryPDwU3kNcN0UAIAb8tbE+J7eseBPcyxVt1tr/ac/vXNp4ZsphbW/kC+lN690Fv7Q9xH9qA5DDzbjLTkWKYX4RxcO7juQI8B1UQAArtsrY2OdTpWKb/3veeL26ZnX8xr6SnMgvzK8MNlbvrqyswZ6h/9qeHHcxX/0q11TL14MdTiSY8HisfmJsc05AFwzBQDgut26ZeTbvV9G9uRYqPT6O5cWvpsD9KXmYN77Oh+PKfxl3lo1TWv0Pw0v/t7o83Pv5i3oSzumZ54tfhQgxm11ZRQAuH4xPwGuSdP636nCT3rLkj/973br8BWf/jNIzk3u/+OY4uMxhi/krevStEM3F/41M//a/hkUZyfv3laFpTd6vyCX/Sl7qu/ZfvJ06fcWAAVSAACuWdP6f+uW4d7hv+xP/3sHmD/fMTXzSI4wMJo3cyy93/mTEKt/d60HmeZ1mc17/ptX/bntn0F0/uD+b8QYy373fkoXq7T4Jd+jwLVSAACu2bnJ/X9Shfi9HIuUUjozdMvil0efm7uSt2DgNIWA7i86dwyl6l+nkO5u2ofzX/oVzaf9VUinUojT/9RZeNGsP4PuwuT4K73HHSupTE2h7r+fmmnDKwyBgigAANdk/ut37V6q089iDDflrRJ1l0L6/V1Ts6/lDPQsFwQufW5bqJaWCwGdoaF3/7+hf77owA+/an7ia7fVsX6j95vyprxVpLqu79w5fXrtLgAF+o4CAHDVllv/Pz/yD70/OfbmrTKl8P3tJ2e+lRMAXLNzB/c/VMX4gxyLlEJ6+0pn8UuKeMDV8hYA4Kpt2zL8cOmH/6b1v7pl4bEcAeC67Dw5+3TvUfSn6zHE227ujjyZI8BnUgAArkrT+t/cLJ5jsWIID5j7B2A11N34QHMxZo5FSiE8dG5iX9H3FQDlUAAArspSSs8UPvffHP6f3n5y9sc5AsAN2fmjl8+nmIrvKquq+Exzx0eOAJ9IAQD4TM0cZO9w/dUci9TMQX7YWfDKPwBW1cVLi0d7P2QKv1Q27qjfHym+Sw/YeC4BBD6Vm5ABGHTegAP0Cx0AwKeqq/p46Yf/pvXf4R+AtTL6wstnWjAK0KlSOG4UAPg0CgDAJ2pa/3uPoi8W0voPwHpowyhAjHG3UQDg0xgBAH6js5N3bxtKSz8v/dP/kOp7tp88fSonAFgzb02M7+lU4Se9ZWdlp0jdtJR+b8ffz/40Z4Bf0gEA/EZVWDpW/OE/pGcd/gFYL7dPz7zeezyxkorViUPhmVfGxkouUgAbRAEA+DXnJ8bvjyEcyLFMKV2s6sUjOQHAunjn0sJ3ez+EmkJAweKeW7eMfDsHgF8yAgD8iqb1vwpLb/T+cNict8qk9R+ADdKWUYBuHb6SuxYAlukAAH5F0/pf/OFf6z8AG6gtowCdKh03CgB8lAIA8EutaP0P4T2t/wBstGYUIKV0JsdCGQUAfpURAGDZmQNjW4eHR37eW25d2SnWvdunZk7kNQBsmLOT+/cOhfgPvWWxn7KnFK4MVfHLoy+8XHixAlgPOgCAZZ3O8LHeo+jDf0rphMM/AKXYNTX7WkjhaI5FijHcVNdGAYAVCgBAuDA5fijGeCjHUr3X7S4+mNcAUITqloXHih8FiGHvti3DD+cEDDAjADDgtP4DwI0xCgC0hQ4AGHBa/wHgxhgFANpCAQAG2LmJuw6W3vqfQrhcx45b/wEo2j8NL3wnhfR2jmWKYe/vbB5+KCdgABkBgAE1PzG2uY7Db4QYt+WtIqU6HN4xPfNsjgBQrHMT++6oquqVHMuUwgdVqr40Ov1S2cUKYE3oAIABVVfDTxV/+A/hlMM/AG2xc/r0qzGEp3MsUwyb6qo+nhMwYBQAYABdOLjvQO83gPtzLNJy638Ycus/AK3yYWfhkeJHAUK449zB/UYBYAAZAYABo/UfANaWUQCgVDoAYMC0ofW/51WHfwDaqhkF6J2wy/45ZhQABpICAAyQ5hOJ0lv/lz+RqKvDOQFAK1X14pGQ0sUcS3XH+Ynxsn8vAFaVAgAMiDfvHdsUq1h8pb8O6RHtiAC03ej03OUQUvl32VThqbOTd5feGQisEgUAGBA3d0eejCHelmOpXt15crbs25MB4CptP3n6VOmjADGEzVVYOpYj0OcUAGAANK3/KYSyb/vV+g9AH2rDKEAM4YBRABgMCgDQ59rS+p9SekzrPwD9phkFqFP1zRzLZRQABoICAPS531oaebz41v8UXvvPlxe1/gPQl3ZOv3wypXQixyI1owBDofuDHIE+pQAAfezs5P69vcN10a3/KYUrVRUP3zk3181bANB3ut3F5kLA91ZSqeLBC5Pjh3IA+pACAPSp+fvGbqpSaFr/Oys7ZUoxPTb6wstncgSAvrT71Fxz+C//rQAhHDtzYGxrXgN9RgEA+lT9/sjjMcbdOZYphdcuXlo8mhMA9LXtUzMnSh8F6Nna6Qx7KwD0KQUA6EPLrf8xPJxjkbT+AzCI2jAKEGM8ZBQA+pMCAPSZtrT+xxj+Qus/AIOmGQVIdfhWjiUzCgB9SAEA+kz9i5E/Lb71P6TX37m08N0cAGCg7JieeTaFcCrHUm0dHh7+Xl4DfUIBAPrIWxPje3qPR1dSsbrdWus/AIOtDkMPphAu51ioeP+Fg/sO5AD0AQUA6BOvjI11OlUqvvW/54nbp2dez2sAGEi7pl68GOpwJMeCxWPzE2ObcwBaTgEA+sStW0a+3fsh3XQAFEzrPwD8i1aMAsS4ra6Gn8oJaDkFAOgDbWn9T0vhAa3/APBfNaMAIYUPciyUUQDoFwoA0HIrrf/hmd6y7Nb/FI7u+PvZn+YEAPQ0owB1SI/kWLB47M17xzblALSUAgC03LYtw837/n93JZUppXSmumXhsRwBgI/YeXL26d7j1ZVUqBi33dwdeTInoKUUAKDF5r9+1+6Y4uM5lqpbx3B49Lm5KzkDAB9T1dXh0kcBUggPnZvYd0eOQAspAECL1SkdizHclGOZUji6a2r2tZwAgN9gdPqlt9swClBV8Zlm/DBHoGUUAKClLkyOH+o9Cq/Cp/Na/wHg6jSjACmEH+dYqLgjjx8CLaQAAC00f9/YTb3DdfFzeHWdHtD6DwBXbyjGB1IKRf/sjCE+eubA2NYcgRZRAIAWWnp/+P6mAp9jkWIIT++cPl32hUYAUJjRF14+k2Iqunuu9zN+c2d45N/lCLSIAgC0UIzxf83LIqWQ3v6ws9CCVxoBQHkuXlo82vthWvb9OSnc7y4AaB8FAGiZtybG9/QeZb/2r06Hv/j8XNE3GQNAqe6cm+tWVTxc8ihAjOEL2zZ/7kCOQEsoAEDLdIbCfXlZJK3/AHDj2jAKUFV10b+TAL+u97s60CbnD+7/eYxxd47FSSm8G2Jy8R8A3KCYQifEuC3H8qTwwTv/uPD5pmMh7wCFUwCAFpm/d+wLdXfk/8kRAGBDLYX0e7umZsu+rwD4JSMA0CJL3ZHC3/sPAAySoRT8bgItogAALRJT+B/yEgBgw6XgdxNoEwUAaJEU0m15CQCw4WKMfjeBFlEAgBbxQxYAKIzfTaBFFACgTVL6Ql4BAJRga34CLaAAAC2SYvCaHQAA4LooAAAAAMAAUAAAAACAAaAAAC0SU3w3LwEANl4KH+QV0AIKANAiMYQ38xIAYOPFcCavgBZQAIAWWYrprbwEANh4KZ3PK6AFFACgRaqUVNkBgJL8X/kJtIACALRIlbo/7j28ChAAKEXzuwnQEgoA0CKj03OXUwiv5QgAsGFSCu9uPzmrAAAtogAALRNT+j/zEgBgw/QOEqfyEmgJBQBomSoN/W3vYQwAANhQS6n+P/ISaAkFAGiZ0emX3g4hqbgDABsovb5z+vSrOQAtoQAALZSWwhN5CQCw7lId/0NeAi2iAAAttOPvZ3/ae6i6AwDrL6WLQ5sXmpFEoGUUAKClqhgf7D3cBQAArK8Yj4w+N3clJ6BFFACgpUZfePlMCuH7OQIArL0UXtw+NXMiJ6BlFACgxYZ+e+E7KaS3cwQAWDspfFClqulABFpKAQBabKX9Lt7T/EDOWwAAa6JO8d+svI0IaCsFAGi5HVMzb9axvre3dB8AALA2Unpk5/TLJ3MCWkoBAPrAzqnTL9YpHckRAGD1xHB0+8nZP88JaLGYn0AfOD85/ge9b+q/6i07KzsAADfkO9unZv59XgMtpwAAfebC1/ftD3U11fvu3pS3AACuVTfV4YEd0zPP5gz0AQUA6EPn7rlrR9VJf9Nb/u7KDgDA1Urn01L4wx1/P/vTvAH0CXcAQB/a+aOXz79zaeH3Ugjm9QCAqxZT+Mt/6ix+2eEf+pMOAOhzK90A9aO9b/c/6kV3AwAAv0E62a3jd26fnnk9bwB9SAEABkQeC/h3veWh3r+2Lm8CAIMrhQ9iTCcX6/i/O/jDYFAAgAHzythY59bPd+6OMf4vdQhfjSHelv8SANDnUgrv9v79xyHGvxv67YWTo8/NXcl/CRgACgAw4M4cGNs6NPS5r8a49MUYhz4XQr0t/yUAoPWqi70D/1Jv8eZSGHpt19SLvQwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALAeQvj/AbdLDxKVSCiJAAAAAElFTkSuQmCC");
                    BitmapImage bitmapImageAlertlogo = new BitmapImage();
                    bitmapImageAlertlogo.BeginInit();
                    bitmapImageAlertlogo.StreamSource = new MemoryStream(binaryAlertlogo);
                    bitmapImageAlertlogo.EndInit();

                    this.ImageforSignupSuccesfully.Source = bitmapImageAlertlogo;
                    this.MessageafterSignUpComplete2.Content = "Please enter valid OTP";

                    this.MessageafterSignUpComplete1.Content = "";

                    this.MessageafterSignUpComplete3.Content = "";
                    this.MessageafterSignUpComplete4.Content = "";

                    this.OkButtonafterSignUpComplete.Visibility = Visibility.Hidden;
                    this.OkButtonifOTPmismatch.Visibility = Visibility.Visible;

                    //System.Windows.Forms.MessageBox.Show("OTP is invalid\nPlease Check and try again or Click on Resend OTP", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionUpdateEmailIDverifyOTPbutton_Click;
                logger.Log(errorCodeFunc + " : " + ex.Message);
            }


        }

        private void UpdateEmailIDresendOTPbutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string page = "getotp";

                this.UpdateEmailIdrequestwithOTP = "{" + '"' + "action" + '"' + ":" + '"' + page + '"' + "," +
                                          '"' + "email" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v4 + '"' + "," +
                                        '"' + "subdomain_name" + '"' + ":" + '"' + Track_Payout.Properties.Settings.Default.v8 + '"' +

                                          "}";



                JObject c = JObject.Parse(aPIConnection.UpdateEmailOnClearBalanceWebServer(UpdateEmailIdrequestwithOTP));


                var ResendresponseMsg = (string)c.SelectToken("result.message");
                var ResendSignupstatus = (string)c.SelectToken("result.status");


                var ResendSignupotp = (string)c.SelectToken("result.otp");

                Track_Payout.Properties.Settings.Default.v14 = ResendSignupotp;
                Track_Payout.Properties.Settings.Default.Save();

                if (ResendSignupstatus == "True")
                {

                    this.OTPVerifycountdown = 60;

                    dispatcherTimerResendOTP.Tick += new EventHandler(dispatcherTimerResendOTP_Tick);
                    dispatcherTimerResendOTP.Interval = new TimeSpan(0, 0, 1);
                    dispatcherTimerResendOTP.Start();
                    otpverificationMessagelabel3.Content = "OTP will be invalid in " + this.OTPVerifycountdown.ToString() + " sec";

                    this.UpdateEmailIDresendOTPbutton.IsEnabled = false;

                    this.UpdateEmailIDverifyOTPbutton.IsEnabled = true;
                    this.otpverificationMessagelabel3.Visibility = Visibility.Visible;

                    this.otpverificationMessagelabel4.Content = Track_Payout.Properties.Settings.Default.v14;

                    clearOTPTextBox();

                }
            }
            catch (Exception ex)
            {

                ErrorcodeforMainWindow errorCodeFunc = ErrorcodeforMainWindow.FunctionUpdateEmailIDresendOTPbutton_Click;
                logger.Log(errorCodeFunc + " : " + ex.Message);


            }
        }

        private void Companyinitialtextbox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }
    }
}

