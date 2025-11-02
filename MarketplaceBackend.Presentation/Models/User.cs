using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceBackend.Presentation.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Annoucement> Annoucements { get; set; }
    }
}
