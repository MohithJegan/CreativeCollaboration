using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreativeCollaboration.Models
{
    public class Movie
    {
        [Key]
        public int MovieID { get; set; }
        public string MovieTitle { get; set; }
        public string MovieReleaseDate { get; set; }
        public int MovieDuration { get; set; }
        public string MovieDescription { get; set; }
        public Double MovieBudget { get; set; }
        public Double MovieBoxOfficeCollection { get; set; }
        public Double MovieRating { get; set; }
        public int MovieAwardNomination { get; set; }
        public int MovieAwardWin { get; set; }

        // A movie can be produced by one studio
        [ForeignKey("Studio")]
        public int StudioID { get; set; }
        public virtual Studio Studio { get; set; }

        // A movie can have many actors
        public ICollection<Actor> Actors { get; set; }

        // A movie can be watched by many customers
        public ICollection<Customer> Customers { get; set; }

    }


    public class MovieDto
    {
        [Key]
        public int MovieID { get; set; }
        public string MovieTitle { get; set; }
        public string MovieReleaseDate { get; set; }
        public int MovieDuration { get; set; }
        public string MovieDescription { get; set; }
        public Double MovieBudget { get; set; }
        public Double MovieBoxOfficeCollection { get; set; }
        public Double MovieRating { get; set; }
        public int MovieAwardNomination { get; set; }
        public int MovieAwardWin { get; set; }
        public string MovieStudioName { get; set; }
        public int StudioID { get; set; }
        


    }
}
 