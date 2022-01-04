using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace NewsPaperApp
{

    public static class EntityFrameworkAPI
    {
        private static NewspaperEntities context = new NewspaperEntities();
        public static bool CheckLoginCredentials(string username, string hashedPassword)
        {
            /*
             * Checks if the credetials the user set before pressing the login button can be found in the Accounts table in the database
             * Returns true if found else false
             */

            var acc = (from a in context.Accounts
                       where a.Username == username && a.Password == hashedPassword
                       select a).FirstOrDefault();

            return (acc != null) ? true : false;
        }

        public static bool AccountExists(string username)
        {
            /*
             *  Checks if an account with the username passed as a parameter exists in the database
             *  Returns true if it exists else it returns false
             */

            var acc = (from a in context.Accounts
                       where a.Username == username
                       select a).FirstOrDefault();

            return (acc == null) ? false : true;
        }

        public static bool RegisterAccount(string username, string hashedPassword, string email, string type)
        {
            /*
             * Inserts an account into the database
             */

            var acc_type = (from at in context.AccountTypes
                            where at.Nume == type
                            select at).FirstOrDefault();

            if (acc_type == null)
                throw new Exception("Inexistent account type");


            Account acc = new Account()
            {
                Username = username,
                Password = hashedPassword,
                Email = email,
                AccountType = acc_type,
                CreationDate = System.DateTime.Now
            };

            context.Accounts.Add(acc);
            int res = context.SaveChanges();

            return true;
        }

        public static List<string> GetProfileDetails(string username)
        {
            List<string> details = new List<string>();

            var acc = (from a in context.Accounts
                            where a.Username == username
                            select a).FirstOrDefault();

            if (acc == null)
                throw new Exception("Inexistent account!");

            details.Add(acc.CreationDate.ToString());
            details.Add(acc.Email);
            details.Add(acc.AccountType.Nume);

            return details;
        }

        public static bool ChangePassword(string username, string newHashedPassword)
        {
            //A check to be sure but it should not get to the stage where the user can change the password if he is not logged in
            if (EntityFrameworkAPI.AccountExists(username) == false)
            {
                return false;
            }

            var acc = (from a in context.Accounts
                       where a.Username == username
                       select a).FirstOrDefault();

            if (acc == null)
                return false;

            if(acc.Password != newHashedPassword)
            {
                acc.Password = newHashedPassword;
                int res = context.SaveChanges();
                //TO DO...Check the return value of save changes
                return true;
            }


            //Return the status of the execution
            return false;
        }

        public static bool ComparePasswords(string username, string newHashedPassword)
        {
            //Compares the current password of the user and the new wants it wants to set
            var acc = (from a in context.Accounts
                       where a.Username == username
                       select a).FirstOrDefault();

            if (acc == null)
                throw new Exception("Account does not exist!");

            return (acc.Password == newHashedPassword) ? false : true; 
        }

        public static List<NewspaperClass> GetAvailableNewspapers()
        {
            List<NewspaperClass> newspapers = new List<NewspaperClass>();

            var npapers = (from n in context.NewsPapers
                           where n.Published == 1
                           select n
                           ).ToList();


            if (npapers == null)
                throw new Exception("Something went wrong with the database!");

            foreach(var n in npapers)
            {
                var rating = (from r in context.Ratings
                              where r.NewsPaperID == n.ID
                              select r).FirstOrDefault();

                if(rating != null)
                    newspapers.Add(new NewspaperClass(n.Name, n.PublishingHouse, n.PublishingDate, rating.Rating1));
            }

            return newspapers;
        }

        public static List<ArticleClass> GetNewspaperArticles(string name)
        {
            List<ArticleClass> articles = new List<ArticleClass>();

            var arts = (from art in context.Articles
                            where art.NewsPaper.Name == name
                            select art).ToList();

            

            foreach(var art in arts)
            {
                var publisher = (from a in context.Accounts
                                 where a.ID == art.PublisherID
                                 select a).FirstOrDefault();

                var category = (from c in context.ArticleCategories
                                where c.ID == art.CategoryID
                                select c).FirstOrDefault();

                if (publisher != null && category != null)
                    articles.Add(new ArticleClass(art.Title, publisher.Username, art.Content, category.Name));
            }


            return articles;
        }
    }
}
