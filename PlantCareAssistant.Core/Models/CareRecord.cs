using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantCareAssistant.Core.Models
{
    public class CareRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PlantId { get; set; }

        [ForeignKey(nameof(PlantId))]
        public Plant? Plant { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty; // "Полив", "Удобрение", "Опрыскивание"

        public string? Notes { get; set; }
    }
}