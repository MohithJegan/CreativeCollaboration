namespace CreativeCollaboration.Models.ViewModels
{
    public class StudioDetails
    {
        //A studio page must have a studio
        public required StudioDto Studio { get; set; }

        public IEnumerable<MovieDto>? Movies { get; set; }
    }
}
