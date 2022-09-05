using System.ComponentModel.DataAnnotations;

namespace EmailListFilter.Entity
{
    public abstract class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
