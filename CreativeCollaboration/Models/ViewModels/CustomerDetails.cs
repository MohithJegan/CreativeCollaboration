using System.ComponentModel.DataAnnotations;

namespace CreativeCollaboration.Models.ViewModels
{
    public class CustomerDetails
    {
        public CustomerDto Customer { get; set; }
        public List<OrderDto> Order { get; set; }

        public IEnumerable<MovieDto>? CustomerMovies { get; set; }

        public IEnumerable<MovieDto>? AllMovies { get; set; }

    }
}
