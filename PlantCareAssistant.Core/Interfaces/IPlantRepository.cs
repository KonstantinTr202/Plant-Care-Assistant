using System.Collections.Generic;
using System.Threading.Tasks;
using PlantCareAssistant.Core.Models;

namespace PlantCareAssistant.Core.Interfaces
{
    public interface IPlantRepository
    {
        Task<List<Plant>> GetAllAsync();
        Task<Plant?> GetByIdAsync(int id);
        Task<List<Plant>> GetByLocationAsync(string location);
        Task<List<Plant>> GetByCareTypeAsync(string careType);
        Task<List<Plant>> GetByStatusAsync(string status);
        Task<List<Plant>> SearchAsync(string searchTerm);
        Task<List<Plant>> GetPlantsRequiringCareAsync(DateTime date);
        Task AddAsync(Plant plant);
        Task UpdateAsync(Plant plant);
        Task DeleteAsync(int id);
        Task AddCareRecordAsync(CareRecord record);
        Task<List<CareRecord>> GetCareHistoryAsync(int plantId);
    }
}