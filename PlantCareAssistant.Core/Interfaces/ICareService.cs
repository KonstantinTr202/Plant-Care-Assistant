using System.Collections.Generic;
using PlantCareAssistant.Core.Models;

namespace PlantCareAssistant.Core.Interfaces
{
    public interface ICareService
    {
        DateTime CalculateNextWateringDate(DateTime lastWateringDate, int frequencyDays);
        DateTime CalculateNextFertilizerDate(DateTime lastFertilizerDate, int frequencyWeeks);
        bool IsPlantRequiringCare(Plant plant, DateTime currentDate);
        string GetCareRecommendation(Plant plant);
        string GetStatusBasedOnCare(Plant plant, DateTime currentDate);
        List<string> GetSmartRecommendations(Plant plant);
    }
}