using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASI.Basecode.WebApp.Models.NexDesk
{
    [Table("Articles")]
    public class Article
    {
        [Key]
        [Column("Id")]
        [StringLength(20)]
        public string Id { get; set; }

        [Column("Title")]
        [StringLength(200)]
        [Required]
        public string Title { get; set; }

        [Column("Category")]
        [StringLength(100)]
        public string Category { get; set; }

        [Column("Author")]
        [StringLength(100)]
        public string Author { get; set; }

        [Column("Content")]
        public string Content { get; set; }
    }
}