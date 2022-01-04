using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace NewsPaperApp
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private bool ShowStatus = false;
        private string ByteArrayToHexString(byte[] Bytes)
        {
            StringBuilder Result = new StringBuilder(Bytes.Length * 2);
            string HexAlphabet = "0123456789ABCDEF";

            foreach (byte B in Bytes)
            {
                Result.Append(HexAlphabet[(int)(B >> 4)]);
                Result.Append(HexAlphabet[(int)(B & 0xF)]);
            }

            return Result.ToString();
        }
        private void PopulateLabels()
        {
            this.lbl_usernameText.Content = ClientData.GetConnectedAccountUsername();
            //List<string> AccountDetails = DatabaseConnection.GetProfileDetails(ClientData.GetConnectedAccountUsername());
            List<string> AccountDetails = EntityFrameworkAPI.GetProfileDetails(ClientData.GetConnectedAccountUsername());

            this.lbl_accountSinceText.Content = AccountDetails[0];
            this.lbl_emailText.Content = AccountDetails[1];
            this.lbl_accountTypeText.Content = AccountDetails[2];
        }

        private void HideChangePasswordUI()
        {
            this.lbl_newPassword.Visibility = Visibility.Hidden;
            this.lbl_confirmNewPassword.Visibility = Visibility.Hidden;
            this.pwdBox_confirmNewPassword.Visibility = Visibility.Hidden;
            this.pwdBox_newPassword.Visibility = Visibility.Hidden;
            this.btn_confirmChange.Visibility = Visibility.Hidden;
        }

        private void ShowChangePasswordUI()
        {
            this.lbl_newPassword.Visibility = Visibility.Visible;
            this.lbl_confirmNewPassword.Visibility = Visibility.Visible;
            this.pwdBox_confirmNewPassword.Visibility = Visibility.Visible;
            this.pwdBox_newPassword.Visibility = Visibility.Visible;
            this.btn_confirmChange.Visibility = Visibility.Visible;
        }

        public ProfileWindow()
        {
            InitializeComponent();

            HideChangePasswordUI();

            PopulateLabels();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ShowStatus == false)
            {
                ShowChangePasswordUI();
                this.ShowStatus = true;
            }
            else
            {
                HideChangePasswordUI();
                this.ShowStatus = false;
            }
        }

        private void btn_confirmChange_Click(object sender, RoutedEventArgs e)
        {
            //Verify that the password change can be made
            
            //The password inputed in the new password pwd Box should be the same as the one in the confirm new password

            if(this.pwdBox_newPassword.Password != this.pwdBox_confirmNewPassword.Password)
            {
                MessageBox.Show("Passwords do not match, try again", "Failed", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            //Calculate the hash of the password
            string password = this.pwdBox_newPassword.Password;
            SHA256 sha256 = SHA256.Create();
            Byte[] hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            string HashedPassword = ByteArrayToHexString(hashedPassword);

            //Check if the password is the same as the one in the database
            //If it is then don't change the password and show a message to the user

            if(DatabaseConnection.ComparePasswords(ClientData.GetConnectedAccountUsername(), HashedPassword) == false)
            {
                MessageBox.Show("Could not the change the password to the same one", "Failed", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            //Update the database with the new 
            DatabaseConnection.ChangePassword(ClientData.GetConnectedAccountUsername(), HashedPassword);

            MessageBox.Show("Account password has been changed", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            
            this.HideChangePasswordUI();

        }

        private void profile_window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void button_back_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainPage mainPage = new MainPage();
            mainPage.Show();
        }
    }
}
