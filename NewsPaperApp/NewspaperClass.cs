using System;
using System.Collections.Generic;
using System.Text;

namespace NewsPaperApp
{
    /*
     * Class that contains the information of a newspapers
     * The information will be extracted from the database
     */
    public class NewspaperClass
    {
        //Member variables
        private string name;
        private string publishingHouse;
        private DateTime publishingDate;
        private double rating;

        //Constructor
        public NewspaperClass(string name, string publishingHouse, DateTime publishingDate, double rating)
        {
            this.name = name;
            this.publishingHouse = publishingHouse;
            this.publishingDate = publishingDate;
            this.rating = rating;
        }
        

        //Accessors
        public string GetName()
        {
            return this.name;
        }

        public string GetPublishingHouse()
        {
            return this.publishingHouse;
        }

        public DateTime GetPublishingDate()
        {
            return this.publishingDate;
        }

        public double GetRating()
        {
            return this.rating;
        }
    }
}
