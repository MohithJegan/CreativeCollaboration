namespace CreativeCollaboration.Models.ViewModels
{
    public class ActorDetails
    {
        // A Actor page must have an actor
        public required ActorDto Actor { get; set; }

        // An actor may have movies associated to it
        public IEnumerable<MovieDto>? ActorMovies { get; set; }

        public IEnumerable<MovieDto>? AllMovies { get; set; }

    }
}
