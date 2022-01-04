using System;
using System.Collections.Generic;
using System.Text;

namespace NewsPaperApp
{
    class CommentClass
    {
        private string Username;
        private string Content;
        private DateTime PublishingDate;

        public CommentClass(string username, string content, DateTime publishingDate)
        {
            this.Username = username;
            this.Content = content;
            this.PublishingDate = publishingDate;
        }

        public string GetUsername()
        {
            return this.Username;
        }

        public string GetContent()
        {
            return this.Content;
        }

        public DateTime GetPublishingDate()
        {
            return this.PublishingDate;
        }
    }
}
