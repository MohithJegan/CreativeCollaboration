using System.ComponentModel.DataAnnotations;

namespace CreativeCollaboration.Models
{
    public class Studio
    {
        [Key]
        public int StudioID { get; set; }
        public string StudioName { get; set; }
        public string StudioCountry {  get; set; }
        public int StudioEstablishedYear {  get; set; }
        public string StudioCEO { get; set; }
        public string StudioHeadquarter {  get; set; }

        public bool HasPic { get; set; } = false;

        // images stored in /wwwroot/images/studios/{StudioId}.{PicExtension}
        public string? PicExtension { get; set; }

    }

    public class StudioDto
    {
        public int StudioID { get; set; }
        public string StudioName { get; set; }
        public string StudioCountry { get; set; }
        public int StudioEstablishedYear { get; set; }
        public string StudioCEO { get; set; }
        public string StudioHeadquarter { get; set; }

        // bool if there is a pic, false otherwise
        public bool HasStudioPic { get; set; }

        // string to represent the path to the studio picture
        public string? StudioImagePath { get; set; }
    }
}
