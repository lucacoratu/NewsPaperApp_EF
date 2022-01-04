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
    /// Interaction logic for ArticleViewer.xaml
    /// </summary>
    public partial class ArticleViewer : Window
    {
        List<ArticleClass> articles = new List<ArticleClass>();
        List<ArticleUserControl> articleUserControls = new List<ArticleUserControl>();
        int selectedArticle = 0;
        void InitializeArticleListBox()
        {
            //this.articles = DatabaseConnection.GetNewspaperArticles(ClientData.GetCurrentNewspaper());
            this.articles = EntityFrameworkAPI.GetNewspaperArticles(ClientData.GetCurrentNewspaper());

            foreach (var article in articles) 
            {
                this.articleUserControls.Add(new ArticleUserControl());
                this.articleUserControls[this.articleUserControls.Count - 1].lbl_author.Content = article.GetPublisherUsername();
                this.articleUserControls[this.articleUserControls.Count - 1].lbl_title.Content = article.GetTitle();
                this.articleUserControls[this.articleUserControls.Count - 1].txtblock_content.Text = article.GetContent();
                //this.lstbox_articles.Items.Add(this.articleUserControls[this.articleUserControls.Count - 1]);
            }

            if (articles.Count != 0)
            {
                this.lbl_title.Content = articles[0].GetTitle();
                this.txtblock_content.Text = articles[0].GetContent();

                this.lbl_articleCounter.Content = "Article 1 / " + this.articles.Count.ToString();
                DatabaseConnection.ShowPicture(this.articles[0].GetTitle(), ClientData.GetCurrentNewspaper(), ClientData.GetCurrentNewspaperDate(), this.img_article);
            }
            else
            {
                this.lbl_articleCounter.Content = "Article 0 / " + this.articles.Count.ToString();
            }

        }

        public ArticleViewer()
        {
            InitializeComponent();

            InitializeArticleListBox();
        }

        private void articleviewer_window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btn_decrementArticle_Click(object sender, RoutedEventArgs e)
        {
            if (this.selectedArticle > 0)
                this.selectedArticle--;
            else
                this.selectedArticle = this.articles.Count - 1;

            int aux = this.selectedArticle + 1;

            this.lbl_articleCounter.Content = "Article " + aux.ToString() + " / " + this.articles.Count.ToString();
            this.lbl_title.Content = this.articles[this.selectedArticle].GetTitle();
            this.txtblock_content.Text = this.articles[this.selectedArticle].GetContent();

            DatabaseConnection.ShowPicture(this.articles[this.selectedArticle].GetTitle(), ClientData.GetCurrentNewspaper(), ClientData.GetCurrentNewspaperDate(), this.img_article);
        }

        private void btn_incrementArticle_Click(object sender, RoutedEventArgs e)
        {
            if (this.selectedArticle < this.articles.Count - 1)
                this.selectedArticle++;
            else
                this.selectedArticle = 0;

            int aux = this.selectedArticle + 1;
            this.lbl_articleCounter.Content = "Article " + aux.ToString() + " / " + this.articles.Count.ToString();
            this.lbl_title.Content = this.articles[this.selectedArticle].GetTitle();
            this.txtblock_content.Text = this.articles[this.selectedArticle].GetContent();

            DatabaseConnection.ShowPicture(this.articles[this.selectedArticle].GetTitle(), ClientData.GetCurrentNewspaper(), ClientData.GetCurrentNewspaperDate(), this.img_article);
        }

        private void button_back_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainPage mainPage = new MainPage();
            mainPage.Show();
        }

        private void button_rate_newspaper_Click(object sender, RoutedEventArgs e)
        {
            RateNewspaperWindow rateWindow = new RateNewspaperWindow();
            rateWindow.Show();
        }
    }
}
