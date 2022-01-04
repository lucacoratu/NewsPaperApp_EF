using System;
using System.Collections.Generic;
using System.Text;

namespace NewsPaperApp
{
    public class ArticleClass
    {
        private string Title;
        private string PublisherUsername;
        private string Content;
        private string Category;

        public ArticleClass(string title, string publisherUsername, string content, string category)
        {
            this.Title = title;
            this.PublisherUsername = publisherUsername;
            this.Content = content;
            this.Category = category;
        }

        public string GetTitle()
        {
            return this.Title;
        }

        public string GetPublisherUsername()
        {
            return this.PublisherUsername;
        }

        public string GetContent()
        {
            return this.Content;
        }

        public string GetCategory()
        {
            return this.Category;
        }
    }
}
