using System;
using System.ComponentModel.DataAnnotations;

namespace PlantCareAssistant.Core.Models
{
    public class Plant
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Название должно быть от 1 до 100 символов")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Species { get; set; }

        [Required]
        public string Location { get; set; } = "Комната";

        [Required]
        public string CareType { get; set; } = "Декоративно-лиственный";

        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [Required]
        public string Status { get; set; } = "Здоровое";

        [Range(1, 365, ErrorMessage = "Частота полива должна быть от 1 до 365 дней")]
        public int WateringFrequencyDays { get; set; } = 7;

        [Range(1, 52, ErrorMessage = "Частота удобрения должна быть от 1 до 52 недель")]
        public int FertilizerFrequencyWeeks { get; set; } = 4;

        public bool NeedsSpraying { get; set; } = false;

        [Required]
        public string Lighting { get; set; } = "Полутень";

        public string? Notes { get; set; }

        public DateTime? LastWateringDate { get; set; }
        public DateTime? LastFertilizerDate { get; set; }
        public DateTime? LastSprayingDate { get; set; }

        public DateTime NextWateringDate =>
            LastWateringDate?.AddDays(WateringFrequencyDays) ?? DateTime.Now;

        public DateTime NextFertilizerDate =>
            LastFertilizerDate?.AddWeeks(FertilizerFrequencyWeeks) ?? DateTime.Now;

        public bool RequiresWatering => DateTime.Now >= NextWateringDate;
        public bool RequiresFertilizer => DateTime.Now >= NextFertilizerDate;

        public string CareActionNeeded
        {
            get
            {
                if (RequiresWatering && RequiresFertilizer)
                    return "Полив и удобрение";
                if (RequiresWatering)
                    return "Требуется полив";
                if (RequiresFertilizer)
                    return "Требуется удобрение";
                return "Уход не требуется";
            }
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime AddWeeks(this DateTime date, int weeks)
        {
            return date.AddDays(weeks * 7);
        }
    }
}