namespace CreativeCollaboration.Models.ViewModels
{
    public class MovieDetails
    {
        // A movie page must have a movie
        public required MovieDto Movie { get; set; }

        // A movie may have actors associated to it
        public IEnumerable<ActorDto>? MovieActors { get; set; }

        public IEnumerable<ActorDto>? AllActors { get; set; }

        public IEnumerable<StudioDto>? Studio { get; set; }

        public IEnumerable<CustomerDto>? MovieCustomers { get; set; }
        public IEnumerable<CustomerDto>? AllCustomers { get; set; }

    }
}
