using System;
using System.Collections.Generic;
using System.Text;

namespace NewsPaperApp
{
    static class ClientData
    {
        private static string ConnectedAccountUsername;
        private static string CurrentNewspaper;
        private static string CurrentNewspaperDate;
        
        //Modifiers
        public static void SetConnectedAccountUsername(string username)
        {
            ConnectedAccountUsername = username;
        }

        public static void SetCurrentNewspaper(string currentNewspaper)
        {
            CurrentNewspaper = currentNewspaper;
        }
        public static void SetCurrentNewspaperDate(string date)
        {
            CurrentNewspaperDate = date;
        }
        //Accessors
        public static string GetConnectedAccountUsername()
        {
            return ConnectedAccountUsername;
        }

        public static string GetCurrentNewspaper()
        {
            return CurrentNewspaper;
        }

        public static string GetCurrentNewspaperDate()
        {
            return CurrentNewspaperDate;
        }
    }
}
