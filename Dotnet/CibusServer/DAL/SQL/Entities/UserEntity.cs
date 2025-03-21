using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CibusServer.DAL.SQL.Entities
{
    [Table("Users")]
    public class UserEntity
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // DB generated -- unique in all DB

        [Column("name")]
        public string UserName { get; set; }

        [Column("password")]
        public string Password { get; set; }


    }
}
