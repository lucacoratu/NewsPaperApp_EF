//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewsPaperApp
{
    using System;
    using System.Collections.ObjectModel;
    
    public partial class Comment
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Content { get; set; }
        public int NewsPaperID { get; set; }
        public System.DateTime PublishingDate { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual NewsPaper NewsPaper { get; set; }
    }
}