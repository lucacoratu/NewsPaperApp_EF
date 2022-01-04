using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Data.Common;
using System.Windows.Media.Imaging;

namespace NewsPaperApp
{
    static class DatabaseConnection
    {
        private static string connectionString = "Server=.;Database=Newspaper;Trusted_Connection=true";
        private static SqlConnection sqlConnection = new SqlConnection(connectionString);

        private static void FillError(object sender, FillErrorEventArgs args)
        {
            //Handler for when an error occured during the fill method of the sqlDataAdapter
            /*The DataAdapter issues the FillError event when an error occurs during a Fill operation.
             * This type of error commonly occurs when the data in the row being added could not be 
             * converted to a .NET Framework type without some loss of precision.
             * If an error occurs during a Fill operation, the current row is not added to the DataTable. 
             * The FillError event enables you to resolve the error and add the row, or to ignore the 
             * excluded row and continue the Fill operation.
             * ^- Taken from https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/handling-dataadapter-events
             */

            if (args.Errors.GetType() == typeof(System.OverflowException))
            {
                MessageBox.Show("Error encountered during the filling of " + args.DataTable.TableName, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Continue = false;
            }
        }

        public static ref SqlConnection GetSqlConnection()
        {
            return ref sqlConnection;
        }

        public static bool CheckLoginCredentials(string username, string hashedPassword)
        {
            /*
             * Checks if the credetials the user set before pressing the login button can be found in the Accounts table in the database
             * Returns true if found else false
             */
            string query = "select * from Accounts where Username = \'" + username + "\' and Password = \'" + hashedPassword + "\'";
            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return ((dt.Rows.Count == 1) ? true : false);
        }

        public static bool AccountExists(string username)
        {
            /*
             *  Checks if an account with the username passed as a parameter exists in the database
             *  Returns true if it exists else it returns false
             */
            string query = "select * from Accounts where Username = \'" + username + "\'";
            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return ((dt.Rows.Count != 0) ? true : false);
        }

        public static bool RegisterAccount(string username, string hashedPassword, string email, string type)
        {
            /*
             * Inserts an account into the database
             */
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;

            try
            {
                sqlConnection.Open();
                if (email != "")
                {
                    cmd.CommandText = "insert into Accounts (Username, Password, Email, CreationDate, Type) values ( \'" + username + "\', \'"
                        + hashedPassword + "\', \'" + email + "\', GETDATE(), (select top(1) ID from AccountTypes where Nume = \'" + type + "\'))";
                }
                else
                {
                    cmd.CommandText = "insert into Accounts (Username, Password, Email, CreationDate, Type) values ( \'" + username + "\', \'"
                        + hashedPassword + "\', NULL, GETDATE(), ( select top(1) ID from AccountTypes where Nume = '\'" + type + "\' ));";
                }
                string test = cmd.CommandText;

                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch(SqlException sqlException)
            {
                MessageBox.Show(sqlException.Message, "Error " + sqlException.ErrorCode.ToString(),MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch(InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }


            return true;
        }

        public static List<string> GetProfileDetails(string username)
        {
            List<string> details = new List<string>();

            string query = "select CreationDate, Email, AccountTypes.Nume from Accounts inner join AccountTypes on Accounts.Type = AccountTypes.ID where Username = \'" + username + "\'";

            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for(int i = 0; i < dt.Rows.Count; i++)
            {
                details.Add(dt.Rows[i].Field<DateTime>("CreationDate").ToString());
                details.Add(dt.Rows[i].Field<string>("Email"));
                details.Add(dt.Rows[i].Field<string>("Nume"));
            }

            return details;
        }

        public static bool ChangePassword(string username, string newHashedPassword)
        {
            //A check to be sure but it should not get to the stage where the user can change the password if he is not logged in
            if(DatabaseConnection.AccountExists(username) == false)
            {
                return false;
            }

            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConnection;

                //Open the connection
                sqlConnection.Open();

                //Query for updating the password
                cmd.CommandText = "update Accounts set Password = \'" + newHashedPassword + "\' where Username = \'" + username + "\'";

                //Execute the query
                cmd.ExecuteNonQuery();

                //Close the connection
                sqlConnection.Close();
            }
            catch (SqlException sqlException)
            {
                MessageBox.Show(sqlException.Message, "Error " + sqlException.ErrorCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            //Return the status of the execution
            return true;
        }

        public static bool ComparePasswords(string username, string newHashedPassword)
        {
            string query = "select * from Accounts where Username = \'" + username + "\' and Password = \'" + newHashedPassword + "\'";
            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return ((dt.Rows.Count == 0) ? true : false);
        }

        public static List<NewspaperClass> GetAvailableNewspapers()
        {
            List<NewspaperClass> newspapers = new List<NewspaperClass>();

            string query = "select Name, PublishingHouse, PublishingDate, Rating from NewsPaper inner join Ratings on Ratings.NewsPaperID = NewsPaper.ID where Published = 1";
            //string query = "select Name, PublishingHouse, PublishingDate from NewsPaper where Published = 1";

            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                newspapers.Add(new NewspaperClass(dt.Rows[i].Field<string>("Name"), dt.Rows[i].Field<string>("PublishingHouse"), dt.Rows[i].Field<DateTime>("PublishingDate"), dt.Rows[i].Field<double>("Rating")));
            }

            return newspapers;
        }

        public static List<ArticleClass> GetNewspaperArticles(string name)
        {
            List<ArticleClass> articles = new List<ArticleClass>();
            string query = @"select Title, Accounts.Username, Content, ArticleCategories.Name from Article
                            inner join Accounts
                            on Accounts.ID = Article.PublisherID
                            inner join ArticleCategories
                            on ArticleCategories.ID = Article.CategoryID
                            inner join NewsPaper
                            on NewsPaper.ID = Article.NewsPaperID
                            where NewsPaper.Name =" + "\'" + name + "\'";


            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                articles.Add(new ArticleClass(dt.Rows[i].Field<string>("Title"), dt.Rows[i].Field<string>("Username"), dt.Rows[i].Field<string>("Content"), dt.Rows[i].Field<string>("Name")));
            }

            return articles;
        }

        
        public static bool CreateNewspaper(string name, string publishingHouse, DateTime publishingDate)
        {
            /*
             * Creates a new newspaper with 0 articles 
             */
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;

            try
            {
                sqlConnection.Open();
                cmd.CommandText = "insert into NewsPaper (Name, PublishingHouse, PublishingDate) values ( \'" + name + "\', \'"
                    + publishingHouse + "\', " + publishingDate.ToString() + ")";
                string test = cmd.CommandText;

                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (SqlException sqlException)
            {
                MessageBox.Show(sqlException.Message, "Error " + sqlException.ErrorCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public static bool CreateArticle(string newspaperName, string articleCategory, string publisher, string title, string content, string photoPath)
        {
            /*
             * Creates an article which will be contained in a newspaper
             * If the insertion was successful then this function will return true
             * Else it will return false
             */

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;

            try
            {
                sqlConnection.Open();
                cmd.CommandText = "insert into Article (Title, Content, PhotoPath, PublisherID, NewsPaperID, CategoryID) " +
                    "values ( \'" + title + "\', \'" + content + "\', \'" + photoPath + "\', (select ID from Accounts where Username = \'" + publisher + "\'), (select ID from NewsPaper where Name = \'" + newspaperName + "\' )"
                    + "(select ID from ArticleCategories where Name = \'" + articleCategory + "\'))";
                string test = cmd.CommandText;

                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (SqlException sqlException)
            {
                MessageBox.Show(sqlException.Message, "Error " + sqlException.ErrorCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public static List<ArticleClass> GetArticlesOfCategory(string newspaperName, string categoryName)
        {
            List<ArticleClass> articles = new List<ArticleClass>();
            string query = @"select Title, Accounts.Username, Content, ArticleCategories.Name from Article
                            inner join Accounts
                            on Accounts.ID = Article.PublisherID
                            inner join ArticleCategories
                            on ArticleCategoies.ID = Article.CategoryID
                            inner join NewsPaper
                            on NewsPaper.ID = Article.NewsPaperID
                            where NewsPaper.Name =" + "\'" + newspaperName + "\' and ArticleCategories.Name = \'" + categoryName + "\'";


            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                articles.Add(new ArticleClass(dt.Rows[i].Field<string>("Title"), dt.Rows[i].Field<string>("Username"), dt.Rows[i].Field<string>("Content"), dt.Rows[i].Field<string>("Name")));
            }

            return articles;
        }

        public static List<CommentClass> GetNewspaperComments(string name)
        {
            List<CommentClass> comments = new List<CommentClass>();
            string query = @"select Content, Comments.PublishingDate, Accounts.Username from Comments
                             inner join NewsPaper
                             on NewsPaper.ID = Comments.NewsPaperID
                             inner join Accounts
                             on Accounts.ID = Comments.UserID
                             where NewsPaper.Name = \'" + name + "\'";


            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                comments.Add(new CommentClass(dt.Rows[i].Field<string>("Username"), dt.Rows[i].Field<string>("Content"), dt.Rows[i].Field<DateTime>("PublishingDate")));
            }

            return comments;
        }

        public static bool AddCommentToNewsPaper(string newspaperName, string publisherName, string content)
        {
            if (content == null || content.Length == 0)
                return false;

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            try
            {
                sqlConnection.Open();
                cmd.CommandText = @"insert into Comments values(UserID, Content, NewsPaperID, PublishingDate)
                                    values ((select ID from Accounts where Name = \'" + publisherName + "\'), \'" + content + "\', (select ID from NewsPaper where Name = \'" + newspaperName + "\'), GETDATE())";

                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (SqlException sqlException)
            {
                MessageBox.Show(sqlException.Message, "Error " + sqlException.ErrorCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public static float GetNewspaperRating(string name)
        {
            /*
             * Returns the rating of a newspaper 
             */
            string query = @"select Rating from Ratings
                             inner join NewsPaper
                             on NewsPaper.ID = Ratings.NewsPaperID
                             where NewsPaper.Name = \'" + name + "\'";

            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt.Rows[0].Field<float>("Rating");
        }

        public static bool RateNewspaper(string name, int grade)
        {
            /*
            * Adds a grade to the rating then it recalculates the rating 
            * Returns true if the operation was successful
            * Else it returns false
            */

            if (grade >= 10 || grade < 0)
            {
                return false; 
            }

            //TO DO...Check if the account hasn't already rated the newspaper
            string query = @"select * from AccountRatings where NewsPaperID = (select ID from NewsPaper where Name =" + " \'" + name + "\') " +
                            "and AccountID = (select ID from Accounts where Username = \'" + ClientData.GetConnectedAccountUsername() + "\')";
            SqlDataAdapter dadap = new SqlDataAdapter(query, connectionString);
            dadap.FillError += new FillErrorEventHandler(FillError);
            DataTable dt1 = new DataTable();
            dadap.Fill(dt1);

            if(dt1.Rows.Count != 0)
            {
                return false;
            }

            //First select the current rating and the ratings given
            string query1 = @"select Rating, RatingsGiven from Ratings
                             inner join Newspaper
                             on Newspaper.ID = Ratings.NewsPaperID
                             where Newspaper.Name =" + "\'" + name + "\'";

            SqlDataAdapter da = new SqlDataAdapter(query1, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            double rating = dt.Rows[0].Field<double>("Rating");
            int ratingsGiven = dt.Rows[0].Field<int>("RatingsGiven");

            //Calculate the new rating
            double newRating = rating * ratingsGiven;
            newRating += grade;
            ratingsGiven++;
            newRating = newRating / ratingsGiven;

            //Update the database with the new value
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;

            try
            {
                sqlConnection.Open();
                cmd.CommandText = @"update R
                                    set Rating = " + newRating.ToString() + ", RatingsGiven = " + ratingsGiven.ToString() +
                                    " from Ratings as R " +
                                    "inner join NewsPaper " +
                                    "on NewsPaper.ID = R.NewsPaperID " +
                                    "where NewsPaper.Name = \'" + name + "\'";

                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (SqlException sqlException)
            {
                MessageBox.Show(sqlException.Message, "Error " + sqlException.ErrorCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            try
            {
                int newspaperID = DatabaseConnection.GetNewspaperID(name);
                sqlConnection.Open();
                cmd.CommandText = @"insert into AccountRatings(NewsPaperID, AccountID, Grade) 
                                    values( " + newspaperID.ToString() + ", (select ID from Accounts where Username =" + "\'" + ClientData.GetConnectedAccountUsername() + "\'), " + grade.ToString() + ")";

                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (SqlException sqlException)
            {
                MessageBox.Show(sqlException.Message, "Error " + sqlException.ErrorCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }


        //Writer
        public static string CheckAccountTypeForUser(string username)
        {
            /*
            * Check if user is writer(return true) or reader(return false)
            */

            var cmd = sqlConnection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select Type from Accounts where Username = \'" + username + "\'";

            sqlConnection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            string type = "";
            while (reader.Read())
                type = reader[0].ToString();
            sqlConnection.Close();



            return type;
        }
        public static List<string> GetNameListOfUnpublishedNewspapers()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;



            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select Name from NewsPaper where Published=0";



            sqlConnection.Open();
            SqlDataReader reader = cmd.ExecuteReader();



            List<string> list_of_unpublished_newspaper = new List<string>();



            if (reader.HasRows)
            {
                while (reader.Read())
                    list_of_unpublished_newspaper.Add(reader.GetValue(0).ToString());

            }

            reader.Close();
            sqlConnection.Close();
            return list_of_unpublished_newspaper;
        }

        public static void ShowArticles(string publisher_username, DataGrid data_grid_view_your_articles)
        {
            //Fill DataGrid object with articles with no order option

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            cmd.CommandType = CommandType.Text;

            sqlConnection.Open();

            cmd.CommandText = @"select Title,N.Name as 'Newspaper name',N.PublishingHouse,AC.Name as 'Category',N.PublishingDate
                                from Article
                                inner join Accounts
                                on Article.PublisherID = Accounts.ID
                                inner
                                join NewsPaper as N
                                on Article.NewsPaperID = N.ID
                                inner
                                join ArticleCategories as AC
                                on Article.CategoryID = AC.ID where Accounts.Username = '";
            cmd.CommandText += publisher_username + "\'";

            cmd.ExecuteNonQuery();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("Article");
            da.Fill(dt);
            data_grid_view_your_articles.ItemsSource = dt.DefaultView;
            da.Update(dt);
            sqlConnection.Close();
        }

        public static int GetPublisherID(string username)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;

            sqlConnection.Open();
            cmd.CommandText = "select ID from Accounts where Username = \'" + username + "\'";
            int publisherID = (int)cmd.ExecuteScalar();

            sqlConnection.Close();

            return publisherID;
        }

        public static int GetNewspaperID(string newspaper_name)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;

            sqlConnection.Open();

            cmd.CommandText = "select ID from NewsPaper where Name = \'" + newspaper_name + "\'";
            int newspaperID = (int)cmd.ExecuteScalar();

            sqlConnection.Close();

            return newspaperID;
        }

        public static int GetCategoryID(string category_name)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;

            sqlConnection.Open();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select ID from ArticleCategories where Name=\'" + category_name + "\'";


            int categoryID = (int)cmd.ExecuteScalar();

            sqlConnection.Close();
            return categoryID;
        }
        public static bool WriteArticle(string title, string content, string photo_path, string username, string newspaper_name, string category)
        {
            int publisherID = GetPublisherID(username);
            int newspaperID = GetNewspaperID(newspaper_name);
            int categoryID = GetCategoryID(category);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;

            sqlConnection.Open();
            cmd.CommandText = "insert into Article values ( \'" + title + "\', \'" + content + "\',\'" + photo_path + "\', " + publisherID.ToString() + ", " + newspaperID.ToString() + ", " + categoryID.ToString() + ")";

            int nr_rows_affected = cmd.ExecuteNonQuery();
            sqlConnection.Close();

            if (nr_rows_affected != 0)
                return true; // success
            return false; // error
        }

        public static bool PublishExistingNewspaper(string newspaper)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;


            cmd.CommandType = CommandType.Text;
            sqlConnection.Open();


            cmd.CommandText = "update NewsPaper set Published=1 where Name=\'" + newspaper + "\'";

            int nr_rows_affected = cmd.ExecuteNonQuery();

            if (nr_rows_affected == 0)
            {
                sqlConnection.Close();
                return false;
            }
            else
            {
                cmd.CommandText = "update NewsPaper set PublishingDate=GETDATE() where name=\'" + newspaper + "\'";
                cmd.ExecuteNonQuery();
                sqlConnection.Close();
                return true;
            }

        }

        public static bool InsertNewNewspaper(string title, string publishing_house, string date)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;

            DateTime time_now = DateTime.Now;
            DateTime date_hour = DateTime.Parse(date);

            int comp = DateTime.Compare(date_hour, time_now);
            if (comp < 0)
                return false;

            cmd.CommandType = CommandType.Text;
            sqlConnection.Open();
            cmd.CommandText = "insert into Newspaper values (\'" + date_hour + "\',\'"
            + publishing_house + "\',\'"
            + title + "\',"
            + "0)";
            int nr_rows_affected = cmd.ExecuteNonQuery();
            sqlConnection.Close();

            if (nr_rows_affected == 0)
                return false;
            else return true;

        }

        public static void ShowContent(string article_name, string newspaper_name, string publishing_date, TextBlock txt_block)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            cmd.CommandType = CommandType.Text;



            sqlConnection.Open();



            cmd.CommandText = @"select A.Content
                                from Article as A
                                inner join Accounts
                                on a.PublisherID=Accounts.ID
                                inner join NewsPaper as N
                                on N.ID=A.NewsPaperID
                                where Accounts.Username = '";
            cmd.CommandText += ClientData.GetConnectedAccountUsername() + "\'";



            cmd.CommandText += @"AND CAST(A.Title as VARCHAR(8000))='";
            cmd.CommandText += article_name;
            cmd.CommandText += "' AND N.Name='";
            cmd.CommandText += newspaper_name;
            cmd.CommandText += "'AND N.PublishingDate='";
            cmd.CommandText += publishing_date + "'";



            DbDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                txt_block.Text = reader.GetString(0);

            sqlConnection.Close();
        }

        public static void ShowPicture(string article_name, string newspaper_name, string publishing_date, Image image_from_path)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            cmd.CommandType = CommandType.Text;



            sqlConnection.Open();



            cmd.CommandText = @"select A.PhotoPath
                                from Article as A
                                inner join Accounts
                                on a.PublisherID=Accounts.ID
                                inner join NewsPaper as N
                                on N.ID=A.NewsPaperID
                                where Accounts.Username = '";
            cmd.CommandText += ClientData.GetConnectedAccountUsername() + "\'";
            cmd.CommandText += @"AND CAST(A.Title as VARCHAR(8000))='";
            cmd.CommandText += article_name;
            cmd.CommandText += "' AND N.Name='";
            cmd.CommandText += newspaper_name;
            cmd.CommandText += "'AND N.PublishingDate='";
            cmd.CommandText += publishing_date + "'";

            DbDataReader reader = cmd.ExecuteReader();

            string path = "";

            while (reader.Read())
                if (reader.GetString(0) == null)
                    path = "";
                else
                    path = reader.GetString(0);

            if (path != "")
            {
                image_from_path.Visibility = System.Windows.Visibility.Visible;
                image_from_path.Source = new BitmapImage(new Uri(path));

            }
            else
                image_from_path.Visibility = System.Windows.Visibility.Hidden;

            sqlConnection.Close();
        }

        public static bool DeleteArticleFromNewsPaper(string article_title, string newspaper_title, string publishing_date)
        {
            string query = @"delete A 
                            from Article A
                            inner join NewsPaper as N
                            on A.NewsPaperID = N.ID
                            where N.Name = " + "\'" + newspaper_title + "\' and CAST(A.Title as VARCHAR(8000)) = \'" + article_title + "\' and N.PublishingDate = \'" + publishing_date + "\'";

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            try
            {
                sqlConnection.Open();
                cmd.CommandText = query;

                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (SqlException sqlException)
            {
                MessageBox.Show(sqlException.Message, "Error " + sqlException.ErrorCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public static bool InsertEmptyRating(string newspaper_name)
        {
            int newspaper_id = DatabaseConnection.GetNewspaperID(newspaper_name);
            string query = "insert into Ratings(Rating, RatingsGiven, NewsPaperID) values( 0.0, 0, " + newspaper_id.ToString() + ")";

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            try
            {
                sqlConnection.Open();
                cmd.CommandText = query;

                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (SqlException sqlException)
            {
                MessageBox.Show(sqlException.Message, "Error " + sqlException.ErrorCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public static void NewspaperToday()
        {
            // if publishing date for a newspaper is today and Published=0 then update Published=1
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;

            sqlConnection.Open();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            Update NewsPaper 
            set Published=1
            where CONVERT(VARCHAR,NewsPaper.PublishingDate,23)=CONVERT(VARCHAR,GETDATE(),23)";

            cmd.ExecuteNonQuery();

            sqlConnection.Close();

        }
    }
}
