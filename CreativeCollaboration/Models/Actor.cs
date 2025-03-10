namespace CreativeCollaboration.Models
{
    public class Actor
    {
        public int ActorId { get; set; }
        public string ActorName { get; set; }
        public string ActorDOB { get; set; }

        public string ActorBirthPlace {  get; set; }

        public string ActorGender { get; set; }

        public string ActorNationality { get; set; }

        public string ActorRole {  get; set; }

        public int ActorAwardWon { get; set; }

        public int ActorDebutYear {  get; set; }

        public int ActorNetWorth { get; set; }

        // An actor can act in many movies
        public ICollection<Movie> Movies { get; set; }
    }

    public class ActorDto
    {

        public int ActorId { get; set; }

        public string ActorName { get; set; }

        public string ActorDOB { get; set; }

        public string ActorBirthPlace { get; set; }

        public string ActorGender { get; set; }

        public string ActorNationality { get; set; }

        public string ActorRole { get; set; }

        public int ActorAwardWon { get; set; }

        public int ActorDebutYear { get; set; }

        public int ActorNetWorth { get; set; }
    }
}
