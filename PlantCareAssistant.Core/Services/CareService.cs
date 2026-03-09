using System;
using System.Collections.Generic;
using PlantCareAssistant.Core.Interfaces;
using PlantCareAssistant.Core.Models;

namespace PlantCareAssistant.Core.Services
{
    public class CareService : ICareService
    {
        public DateTime CalculateNextWateringDate(DateTime lastWateringDate, int frequencyDays)
        {
            return lastWateringDate.AddDays(frequencyDays);
        }

        public DateTime CalculateNextFertilizerDate(DateTime lastFertilizerDate, int frequencyWeeks)
        {
            return lastFertilizerDate.AddDays(frequencyWeeks * 7);
        }

        public bool IsPlantRequiringCare(Plant plant, DateTime currentDate)
        {
            var nextWatering = plant.LastWateringDate.HasValue
                ? CalculateNextWateringDate(plant.LastWateringDate.Value, plant.WateringFrequencyDays)
                : currentDate;

            var nextFertilizer = plant.LastFertilizerDate.HasValue
                ? CalculateNextFertilizerDate(plant.LastFertilizerDate.Value, plant.FertilizerFrequencyWeeks)
                : currentDate;

            return currentDate >= nextWatering || currentDate >= nextFertilizer;
        }

        public string GetCareRecommendation(Plant plant)
        {
            var recommendations = new List<string>();

            switch (plant.CareType.ToLower())
            {
                case "кактус":
                    recommendations.Add("Минимальный полив, раз в 2-3 недели");
                    recommendations.Add("Яркое солнечное освещение");
                    recommendations.Add("Не требует частого удобрения");
                    break;

                case "декоративно-лиственный":
                    recommendations.Add("Регулярный полив, раз в 5-7 дней");
                    recommendations.Add("Полутень или рассеянный свет");
                    recommendations.Add("Удобрение раз в месяц в период роста");
                    break;

                case "цветущий":
                    recommendations.Add("Обильный полив во время цветения");
                    recommendations.Add("Яркое освещение");
                    recommendations.Add("Регулярное удобрение для цветения");
                    break;

                case "овощи":
                    recommendations.Add("Ежедневный или через день полив");
                    recommendations.Add("Максимальное освещение");
                    recommendations.Add("Частое удобрение раз в 1-2 недели");
                    break;

                case "травы":
                    recommendations.Add("Регулярный полив");
                    recommendations.Add("Хорошее освещение");
                    recommendations.Add("Минимальное удобрение");
                    break;

                default:
                    recommendations.Add("Стандартный уход");
                    break;
            }

            if (plant.NeedsSpraying)
                recommendations.Add("Требуется регулярное опрыскивание");

            return string.Join("; ", recommendations);
        }

        public string GetStatusBasedOnCare(Plant plant, DateTime currentDate)
        {
            if (plant.Status == "Погибло")
                return "Погибло";

            var daysSinceWatering = plant.LastWateringDate.HasValue
                ? (currentDate - plant.LastWateringDate.Value).Days
                : 999;

            var maxAllowedDays = plant.WateringFrequencyDays * 2;

            if (daysSinceWatering > maxAllowedDays)
                return "Болеет";
            else if (daysSinceWatering > plant.WateringFrequencyDays)
                return "Требует внимания";
            else
                return "Здоровое";
        }

        public List<string> GetSmartRecommendations(Plant plant)
        {
            var recommendations = new List<string>();

            // Рекомендации на основе типа ухода
            recommendations.Add(GetCareRecommendation(plant));

            // Рекомендации на основе статуса
            if (plant.Status == "Требует внимания")
                recommendations.Add("⚠️ Растение требует срочного ухода!");

            if (plant.Status == "Болеет")
                recommendations.Add("🚨 Растение болеет! Проверьте условия содержания.");

            // Рекомендации на основе освещенности
            switch (plant.Lighting.ToLower())
            {
                case "тень":
                    recommendations.Add("Подходит для тенелюбивых растений");
                    break;
                case "солнечно":
                    recommendations.Add("Требуется яркое освещение");
                    break;
            }

            // Сезонные рекомендации
            var currentMonth = DateTime.Now.Month;
            if (currentMonth >= 3 && currentMonth <= 5)
                recommendations.Add("🌱 Весна - период активного роста, увеличьте полив и удобрение");
            else if (currentMonth >= 12 || currentMonth <= 2)
                recommendations.Add("❄️ Зима - период покоя, уменьшите полив");

            return recommendations;
        }
    }
}