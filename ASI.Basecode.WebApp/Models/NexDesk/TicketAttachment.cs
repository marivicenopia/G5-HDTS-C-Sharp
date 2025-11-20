using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASI.Basecode.WebApp.Models.NexDesk
{
    [Table("TicketAttachments")]
    public class TicketAttachment
    {
        [Key]
        [Column("Id")]
        [StringLength(50)]
        public string Id { get; set; }

        [Column("TicketId")]
        [StringLength(20)]
        [Required]
        public string TicketId { get; set; }

        [Column("Name")]
        [StringLength(255)]
        [Required]
        public string Name { get; set; }

        [Column("Size")]
        public int? Size { get; set; }

        [Column("Type")]
        [StringLength(255)]
        public string Type { get; set; }

        [Column("UploadDate")]
        public DateTime UploadDate { get; set; }

        [Column("Url")]
        [StringLength(2083)]
        public string Url { get; set; }
    }
}