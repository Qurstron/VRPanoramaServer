using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VRPanoramaServer.Database
{
    public class User : IdentityUser
    {
        [JsonIgnore]
        [InverseProperty("User")]
        public virtual ICollection<Project>? projects { get; set; }
    }
}
