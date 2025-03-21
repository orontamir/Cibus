using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CibusServer.DAL.SQL.Entities
{
    [Table("Messages")]
    public class MessageEntity
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // DB generated -- unique in all DB

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("message")]
        public string Message { get; set; }

        [Column("vote")]
        public int Vote { get; set; }
    }
}
