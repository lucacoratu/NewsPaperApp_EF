using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewsPaperApp
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Window
    {
        List<NewspaperClass> newsPapers = new List<NewspaperClass>();
        List<NewsPaperUC> userControlNewsPapers = new List<NewsPaperUC>();

        void InitializeListNewsPapers()
        {
            this.newsPapers.Clear();
            this.userControlNewsPapers.Clear();
            this.listbox_newspapers.Items.Clear();

            //this.newsPapers = DatabaseConnection.GetAvailableNewspapers();
            this.newsPapers = EntityFrameworkAPI.GetAvailableNewspapers();

            foreach(var newspaper in newsPapers)
            {
                this.userControlNewsPapers.Add(new NewsPaperUC());
                this.userControlNewsPapers[userControlNewsPapers.Count - 1].lbl_newpaperName.Content = newspaper.GetName();
                this.userControlNewsPapers[userControlNewsPapers.Count - 1].lbl_publishingHouse.Content = newspaper.GetPublishingHouse();
                this.userControlNewsPapers[userControlNewsPapers.Count - 1].lbl_publishingDate.Content = newspaper.GetPublishingDate();
                this.userControlNewsPapers[userControlNewsPapers.Count - 1].lbl_rating.Content = "Rating: " + newspaper.GetRating().ToString();
                this.userControlNewsPapers[userControlNewsPapers.Count - 1].grid_newspaper.MouseLeftButtonDown += this.usercontrol_clicked;

                this.listbox_newspapers.Items.Add(this.userControlNewsPapers[userControlNewsPapers.Count - 1]);
            }
        }

        public MainPage()
        {
            InitializeComponent();

            InitializeListNewsPapers();
            this.data_grid_view_your_articles.Visibility = Visibility.Hidden;
            this.txt_block_content.Visibility = Visibility.Hidden;
            this.lbl_article_content.Visibility = Visibility.Hidden;

            if (DatabaseConnection.CheckAccountTypeForUser(ClientData.GetConnectedAccountUsername()) == "1") {
                //Reader
                this.canvas_write_article.Visibility = Visibility.Hidden;
                this.button_write_new_article.Visibility = Visibility.Hidden;
                this.button_create_new_newspaper.Visibility = Visibility.Hidden;
                this.button_Publish_newspaper.Visibility = Visibility.Hidden;
                this.button_view_your_article.Visibility = Visibility.Hidden;
                this.button_view_newspapers.Visibility = Visibility.Hidden;
            }
            else
            {
                //Writer
                this.listbox_newspapers.Visibility = Visibility.Hidden;
                this.button_delete_article.Visibility = Visibility.Hidden;
            }
        }



        private void elipse_profile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Go to the profile window
            ProfileWindow pWindow = new ProfileWindow();
            this.Hide();
            pWindow.Show();

            
        }

        private void usercontrol_clicked(object sender, MouseButtonEventArgs e)
        {
            //ClientData.SetCurrentNewspaper(this.listbox_newspapers.SelectedItem)
            
            if (this.listbox_newspapers.SelectedIndex != -1)
            {
                ClientData.SetCurrentNewspaper(this.newsPapers[this.listbox_newspapers.SelectedIndex].GetName());
                ClientData.SetCurrentNewspaperDate(this.newsPapers[this.listbox_newspapers.SelectedIndex].GetPublishingDate().ToString());
                ArticleViewer artviewer = new ArticleViewer();
                this.Hide();
                artviewer.Show();
            }
        }


        private void mainpage_window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        //Writer
        // Change visibility for list of components
        public void change_control_visibility(List<Control> controls_list, string option)
        {
            if (option == "Visible")
                foreach (var control in controls_list)
                    control.Visibility = Visibility.Visible;
            else
            if (option == "Hidden")
                foreach (var control in controls_list)
                    control.Visibility = Visibility.Hidden;
            else
                throw new Exception("Only option possible are \"Visible\" and \"Hidden\" in change_control_visibility");
        }

        private void ShowMessage3Sec(string message)
        {
            this.lbl_announcement.Visibility = Visibility.Visible;
            this.lbl_announcement.Content = message;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            timer.Start();
            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                this.canvas_write_article.Visibility = Visibility.Hidden;
            };

        }

        private void UnpublishedNewspaperList()
        {
            List<string> list_of_unpublished_newspaper = new List<string>();
            list_box_newspaper_name.Items.Clear();
            list_of_unpublished_newspaper = DatabaseConnection.GetNameListOfUnpublishedNewspapers();



            if (list_of_unpublished_newspaper.Count == 0)
            {
                ShowMessage3Sec("0 Unpublished Newspapers! Try Later!");
            }
            else
                foreach (string name in list_of_unpublished_newspaper)
                    list_box_newspaper_name.Items.Add(name);
        }
        public void initiate_write_new_article_components()
        {
            canvas_write_article.Visibility = Visibility.Visible;
            List<Control> controls_visible_write_new_article = new List<Control>();
            controls_visible_write_new_article.AddRange(new List<Control>
            {
            lbl_Title,
            txt_box_title,
            lbl_content,
            txt_box_content,
            lbl_category,
            combo_box_category,
            lbl_unpublished_newspapers,
            button_publish,
            list_box_newspaper_name,
            txt_box_photo_path,
            lbl_photo_path
            });



            List<Control> controls_hidden_write_new_article = new List<Control>();
            controls_hidden_write_new_article.AddRange(new List<Control>
            {
            lbl_announcement,
            lbl_date_of_publication,
            calendar_publication_date,
            button_create_newspaper,
            lbl_PublishingHouse,
            txt_box_PublishingHouse,
            button_publish_newspaper,
            data_grid_view_your_articles,
            lbl_article_content,
            button_delete_article
            });
            this.txt_block_content.Visibility = Visibility.Hidden;
            this.image_photo_from_article.Visibility = Visibility.Hidden;

            change_control_visibility(controls_visible_write_new_article, "Visible");
            change_control_visibility(controls_hidden_write_new_article, "Hidden");
        }
        private void button_write_new_article_Click(object sender, RoutedEventArgs e)
        {
            this.image_photo_from_article.Visibility = Visibility.Hidden;
            initiate_write_new_article_components();
            UnpublishedNewspaperList();
        }

        private void initiate_create_new_newspaper_components()
        {
            canvas_write_article.Visibility = Visibility.Visible;

            List<Control> controls_visible_create_new_article = new List<Control>();
            controls_visible_create_new_article.AddRange(new List<Control>
            {
            lbl_Title,
            txt_box_title,
            lbl_date_of_publication,
            calendar_publication_date,
            button_create_newspaper,
            lbl_PublishingHouse,
            txt_box_PublishingHouse,
            });

            List<Control> controls_hidden_create_new_article = new List<Control>();
            controls_hidden_create_new_article.AddRange(new List<Control>
            {
            lbl_content,
            txt_box_content,
            lbl_category,
            combo_box_category,
            lbl_unpublished_newspapers,
            list_box_newspaper_name,
            button_publish,
            lbl_announcement,
            button_publish_newspaper,
            data_grid_view_your_articles,
            lbl_article_content,
            txt_box_photo_path,
            lbl_photo_path,
            button_delete_article
            });
            this.txt_block_content.Visibility = Visibility.Hidden;
            change_control_visibility(controls_visible_create_new_article, "Visible");
            change_control_visibility(controls_hidden_create_new_article, "Hidden");
        }
        private void button_create_new_newspaper_Click(object sender, RoutedEventArgs e)
        {
            this.image_photo_from_article.Visibility = Visibility.Hidden;
            initiate_create_new_newspaper_components();
        }

        private void initiate_publish_newspaper()
        {



            canvas_write_article.Visibility = Visibility.Visible;




            List<Control> controls_visible_publish_newspaper = new List<Control>();
            controls_visible_publish_newspaper.AddRange(new List<Control>
            {
            lbl_unpublished_newspapers,
            list_box_newspaper_name,
            button_publish_newspaper,
            });



            this.txt_block_content.Visibility = Visibility.Hidden;
            List<Control> controls_hidden_publish_newspaper = new List<Control>();
            controls_hidden_publish_newspaper.AddRange(new List<Control>
            {
            lbl_Title,
            txt_box_title,
            lbl_content,
            txt_box_content,
            lbl_category,
            combo_box_category,
            lbl_date_of_publication,
            button_publish,
            lbl_announcement,
            calendar_publication_date,
            button_create_newspaper,
            lbl_PublishingHouse,
            txt_box_PublishingHouse,
            combo_box_category,
            data_grid_view_your_articles,
            lbl_article_content,
            txt_box_photo_path,
            lbl_photo_path,
            button_delete_article
            });



            change_control_visibility(controls_visible_publish_newspaper, "Visible");
            change_control_visibility(controls_hidden_publish_newspaper, "Hidden");
        }
        private void button_Publish_newspaper_Click(object sender, RoutedEventArgs e)
        {
            this.image_photo_from_article.Visibility = Visibility.Hidden;
            initiate_publish_newspaper();
            UnpublishedNewspaperList();
        }

        private void initiate_view_your_articles_components()
        {

            canvas_write_article.Visibility = Visibility.Visible;
            txt_block_content.Visibility = Visibility.Visible;


            List<Control> controls_visible_view_your_articles = new List<Control>();
            controls_visible_view_your_articles.AddRange(new List<Control>
            {
            data_grid_view_your_articles,
            lbl_article_content,
            button_delete_article
            });


            List<Control> controls_hidden_view_your_articles = new List<Control>();
            controls_hidden_view_your_articles.AddRange(new List<Control>
            {
            lbl_Title,
            txt_box_title,
            txt_box_content,
            lbl_content,
            lbl_category,
            combo_box_category,
            lbl_date_of_publication,
            button_publish,
            lbl_announcement,
            calendar_publication_date,
            button_create_newspaper,
            lbl_PublishingHouse,
            txt_box_PublishingHouse,
            combo_box_category,
            list_box_newspaper_name,
            button_publish_newspaper,
            txt_box_photo_path,
            lbl_photo_path
            });
            change_control_visibility(controls_visible_view_your_articles, "Visible");
            change_control_visibility(controls_hidden_view_your_articles, "Hidden");
        }
        private void button_view_your_article_Click(object sender, RoutedEventArgs e)
        {
            initiate_view_your_articles_components();
            string username = ClientData.GetConnectedAccountUsername();
            DatabaseConnection.ShowArticles(username, this.data_grid_view_your_articles);

        }

        private void button_publish_Click(object sender, RoutedEventArgs e)
        {
            bool not_empty_fields = true;
            string username = ClientData.GetConnectedAccountUsername();
            string title = "", content = "", newspaper_name = "", category_selected = "", photo_path = "", category_name = "";



            // Check input for Title
            if (txt_box_title.Text == "")
                not_empty_fields = false;
            else
                title = txt_box_title.Text;

            //Check input for content
            if (txt_box_content.Text == "")
                not_empty_fields = false;
            else
                content = txt_box_content.Text;


            // Check if newspaper name was selected
            if (this.list_box_newspaper_name.SelectedItem == null)
                not_empty_fields = false;
            else
                newspaper_name = this.list_box_newspaper_name.SelectedItem.ToString();


            // Photo path can be empty
            photo_path = txt_box_photo_path.Text;

            // Check if category name was selected
            if (this.combo_box_category.SelectedItem == null)
                not_empty_fields = false;
            else
            {
                // Extract "Family" from "System.Windows.Controls.ComboBoxItem: Family"
                category_selected = this.combo_box_category.SelectedItem.ToString();
                string pattern = @"(\w)+\z";
                Regex rg = new Regex(pattern);
                MatchCollection matchCollection = rg.Matches(category_selected);
                category_name = matchCollection[0].Value.ToString();
            }


            bool insert_success;


            if (not_empty_fields)
                insert_success = DatabaseConnection.WriteArticle(title, content, photo_path, username, newspaper_name, category_name);
            else
                insert_success = false;


            if (insert_success)
                ShowMessage3Sec("Article Published!");
            else
                ShowMessage3Sec("Article NOT Published!");
        }

        private void button_publish_newspaper_Click_1(object sender, RoutedEventArgs e)
        {

            bool insert_success;
            if (list_box_newspaper_name.SelectedItem == null)
                insert_success = false;
            else
            {
                insert_success = DatabaseConnection.PublishExistingNewspaper(list_box_newspaper_name.SelectedItem.ToString());
            }

            if (insert_success)
                ShowMessage3Sec("NewsPaper Published!");
            else
                ShowMessage3Sec("NewsPaper NOT Published!");
        }

        private void button_create_newspaper_Click(object sender, RoutedEventArgs e)
        {
            bool no_empty_fields = true;
            bool insert_success;
            string title = "", publishing_house = "", date = "";

            if (this.txt_box_title.Text == "")
                no_empty_fields = false;
            else
                title = this.txt_box_title.Text;


            if (this.txt_box_PublishingHouse.Text == "")
                no_empty_fields = false;
            else
                publishing_house = this.txt_box_PublishingHouse.Text;

            if (calendar_publication_date.SelectedDate == null)
                no_empty_fields = false;
            else
                date = this.calendar_publication_date.SelectedDate.ToString();

            if (no_empty_fields)
            {
                insert_success = DatabaseConnection.InsertNewNewspaper(title, publishing_house, date);
                bool res = DatabaseConnection.InsertEmptyRating(title);
            }
            else
                insert_success = false;

            if (insert_success)
                ShowMessage3Sec("NewsPaper Added!");
            else
                ShowMessage3Sec("NewsPaper NOT Added");
        }

        private void data_grid_view_your_articles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataGrid dg = (DataGrid)sender;
            DataRowView row_selected = dg.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                string article_title = row_selected["Title"].ToString();
                string newspaper_title = row_selected["Newspaper name"].ToString();
                string publihing_date = row_selected["PublishingDate"].ToString();
                DatabaseConnection.ShowContent(article_title, newspaper_title, publihing_date, txt_block_content);
                DatabaseConnection.ShowPicture(article_title, newspaper_title, publihing_date, this.image_photo_from_article);
            }
        }

        private void button_view_newspapers_Click(object sender, RoutedEventArgs e)
        {
            this.InitializeListNewsPapers();
            this.canvas_write_article.Visibility = Visibility.Hidden;
            this.listbox_newspapers.Visibility = Visibility.Visible;
        }

        private void button_logout_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void button_delete_article_Click(object sender, RoutedEventArgs e)
        {
            DataGrid dg = this.data_grid_view_your_articles;
            DataRowView row_selected = dg.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                string article_title = row_selected["Title"].ToString();
                string newspaper_title = row_selected["Newspaper name"].ToString();
                string publishing_date = row_selected["PublishingDate"].ToString();

                bool res = DatabaseConnection.DeleteArticleFromNewsPaper(article_title, newspaper_title, publishing_date);

                if(res == false)
                {
                    //No need to update the datagrid
                }
                else
                {
                    //Update the datagrid
                    DatabaseConnection.ShowArticles(ClientData.GetConnectedAccountUsername(), this.data_grid_view_your_articles);
                }
            }
        }
    }
}
