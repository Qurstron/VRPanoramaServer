using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VRPanoramaServer.Database
{
    public class Project
    {
        [Key]
        public Guid ProjectID { get; set; }
        [ForeignKey("UserId")]
        public virtual required User User { get; set; }

        public required string Name { get; set; }
        public required string InternalName { get; set; }
        public required DateTime UploadTime { get; set; }
        public required DateTime LastUpdateTime { get; set; }
    }
}
