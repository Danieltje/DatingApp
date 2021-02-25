using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    // when EF creates this table it will call it Photos
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }

        // the URL of the image
        public string Url { get; set; }

        // is this their main photo; yes or no
        public bool IsMain { get; set; }

        // the Photo storage we're gonna use, we will use this property for that
        public string PublicId { get; set; }

        // we need to fully define the relationship with our AppUser class
        public AppUser AppUser { get; set; }

        // part of fully defining the relationship
        public int AppUserId { get; set; }
    }
}