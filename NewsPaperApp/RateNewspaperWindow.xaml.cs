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

namespace NewsPaperApp
{
    /// <summary>
    /// Interaction logic for RateNewspaperWindow.xaml
    /// </summary>
    public partial class RateNewspaperWindow : Window
    {
        public RateNewspaperWindow()
        {
            InitializeComponent();
            slider_grade.Value = 1.0;
            this.lbl_rate.Content = "Grade 1";
        }

        private void button_rate_Click(object sender, RoutedEventArgs e)
        {
            int grade = (int)slider_grade.Value;
            if(DatabaseConnection.RateNewspaper(ClientData.GetCurrentNewspaper(), grade) == false)
            {
                MessageBox.Show("You can only rate once!", "Rating Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
        }

        private void slider_grade_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int grade = (int)slider_grade.Value;
            if(this.lbl_rate != null)
                this.lbl_rate.Content = "Grade: " + grade.ToString();
        }
    }
}
