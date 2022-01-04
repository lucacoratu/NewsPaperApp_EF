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
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        public static string ByteArrayToHexString(byte[] Bytes)
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

        private void btn_register_Click(object sender, RoutedEventArgs e)
        {
            /*
             * Check if the text boxes are filled correctly and that the password and confirm password password boxes have the same content
             */

            if(this.pwdbox_password.Password != this.pwdbox_confirm.Password)
            {
                this.lbl_output.Content = "Password mismatch, check the input and try again!";
                return;
            }

            if (DatabaseConnection.AccountExists(this.txtbox_username.Text))
            {
                this.lbl_output.Content = "An account with this username already exists!";
                return;
            }

            string password = this.pwdbox_password.Password;
            SHA256 sha256 = SHA256.Create();
            Byte[] hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            string HashedPassword = ByteArrayToHexString(hashedPassword);
            string type = this.combox_accountType.Text;

            DatabaseConnection.RegisterAccount(this.txtbox_username.Text, HashedPassword, this.txtbox_email.Text, type);

            this.lbl_output.Content = "Your account has been registered, you can go back to the login";
        }

        private void register_window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void button_back_Click(object sender, RoutedEventArgs e){
            this.Hide();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
