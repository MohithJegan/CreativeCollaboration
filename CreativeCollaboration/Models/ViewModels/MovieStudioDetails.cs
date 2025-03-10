namespace CreativeCollaboration.Models.ViewModels
{
    public class MovieStudioDetails
    {
        public required MovieDto Movie { get; set; }

        public IEnumerable<StudioDto>? Studios { get; set; }
    }
}
