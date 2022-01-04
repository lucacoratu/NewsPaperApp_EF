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
    
    public partial class NewsPaper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NewsPaper()
        {
            this.AccountRatings = new ObservableCollection<AccountRating>();
            this.Articles = new ObservableCollection<Article>();
            this.Comments = new ObservableCollection<Comment>();
            this.Ratings = new ObservableCollection<Rating>();
        }
    
        public int ID { get; set; }
        public System.DateTime PublishingDate { get; set; }
        public string PublishingHouse { get; set; }
        public string Name { get; set; }
        public int Published { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<AccountRating> AccountRatings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<Article> Articles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<Comment> Comments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<Rating> Ratings { get; set; }
    }
}
