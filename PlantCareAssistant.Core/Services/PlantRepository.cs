using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantCareAssistant.Core.Data;
using PlantCareAssistant.Core.Interfaces;
using PlantCareAssistant.Core.Models;

namespace PlantCareAssistant.Core.Services
{
    public class PlantRepository : IPlantRepository
    {
        private readonly AppDbContext _context;

        public PlantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Plant>> GetAllAsync()
        {
            return await _context.Plants.ToListAsync();
        }

        public async Task<Plant?> GetByIdAsync(int id)
        {
            return await _context.Plants.FindAsync(id);
        }

        public async Task<List<Plant>> GetByLocationAsync(string location)
        {
            return await _context.Plants
                .Where(p => p.Location == location)
                .ToListAsync();
        }

        public async Task<List<Plant>> GetByCareTypeAsync(string careType)
        {
            return await _context.Plants
                .Where(p => p.CareType == careType)
                .ToListAsync();
        }

        public async Task<List<Plant>> GetByStatusAsync(string status)
        {
            return await _context.Plants
                .Where(p => p.Status == status)
                .ToListAsync();
        }

        public async Task<List<Plant>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower();
            return await _context.Plants
                .Where(p => p.Name.ToLower().Contains(searchTerm) ||
                           (p.Species != null && p.Species.ToLower().Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<List<Plant>> GetPlantsRequiringCareAsync(DateTime date)
        {
            var plants = await _context.Plants.ToListAsync();
            return plants.Where(p =>
            {
                var nextWatering = p.LastWateringDate?.AddDays(p.WateringFrequencyDays) ?? date;
                var nextFertilizer = p.LastFertilizerDate?.AddWeeks(p.FertilizerFrequencyWeeks) ?? date;
                return date >= nextWatering || date >= nextFertilizer;
            }).ToList();
        }

        public async Task AddAsync(Plant plant)
        {
            await _context.Plants.AddAsync(plant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Plant plant)
        {
            _context.Plants.Update(plant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var plant = await GetByIdAsync(id);
            if (plant != null)
            {
                _context.Plants.Remove(plant);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddCareRecordAsync(CareRecord record)
        {
            await _context.CareRecords.AddAsync(record);

            // Обновляем даты последнего ухода у растения
            var plant = await GetByIdAsync(record.PlantId);
            if (plant != null)
            {
                switch (record.Type.ToLower())
                {
                    case "полив":
                        plant.LastWateringDate = record.Date;
                        break;
                    case "удобрение":
                        plant.LastFertilizerDate = record.Date;
                        break;
                    case "опрыскивание":
                        plant.LastSprayingDate = record.Date;
                        break;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<CareRecord>> GetCareHistoryAsync(int plantId)
        {
            return await _context.CareRecords
                .Where(r => r.PlantId == plantId)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }
    }
}