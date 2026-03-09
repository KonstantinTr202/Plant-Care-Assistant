using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantCareAssistant.Core.Data;
using PlantCareAssistant.Core.Models;
using PlantCareAssistant.Core.Services;
using Xunit;

namespace PlantCareAssistant.Tests.Services
{
    public class PlantRepositoryTests
    {
        private AppDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_PlantIsAdded_Successfully()
        {
            var context = CreateInMemoryContext("AddTest");
            var repository = new PlantRepository(context);
            var plant = new Plant
            {
                Name = "Тестовое растение",
                Location = "Комната",
                CareType = "Кактус",
                Status = "Здоровое",
                WateringFrequencyDays = 14,
                FertilizerFrequencyWeeks = 4
            };

            await repository.AddAsync(plant);

            var plants = await repository.GetAllAsync();
            Assert.Single(plants);
            Assert.Equal("Тестовое растение", plants[0].Name);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllPlants()
        {
            var context = CreateInMemoryContext("GetAllTest");
            var repository = new PlantRepository(context);
            await repository.AddAsync(new Plant { Name = "Plant1", Location = "Комната", CareType = "Кактус", Status = "Здоровое", WateringFrequencyDays = 7, FertilizerFrequencyWeeks = 4 });
            await repository.AddAsync(new Plant { Name = "Plant2", Location = "Кухня", CareType = "Цветущий", Status = "Здоровое", WateringFrequencyDays = 5, FertilizerFrequencyWeeks = 4 });

            var plants = await repository.GetAllAsync();
            Assert.Equal(2, plants.Count);
        }

        [Fact]
        public async Task GetByLocationAsync_ReturnsOnlyMatchingPlants()
        {
            var context = CreateInMemoryContext("LocationTest");
            var repository = new PlantRepository(context);
            await repository.AddAsync(new Plant { Name = "Plant1", Location = "Комната", CareType = "Кактус", Status = "Здоровое", WateringFrequencyDays = 7, FertilizerFrequencyWeeks = 4 });
            await repository.AddAsync(new Plant { Name = "Plant2", Location = "Кухня", CareType = "Цветущий", Status = "Здоровое", WateringFrequencyDays = 5, FertilizerFrequencyWeeks = 4 });

            var plants = await repository.GetByLocationAsync("Комната");
            Assert.Single(plants);
            Assert.Equal("Plant1", plants[0].Name);
        }

        [Fact]
        public async Task GetPlantsRequiringCareAsync_ReturnsPlantsWithOverdueCare()
        {
            var context = CreateInMemoryContext("CareTest");
            var repository = new PlantRepository(context);

            var plant1 = new Plant
            {
                Name = "Plant1",
                Location = "Комната",
                CareType = "Кактус",
                Status = "Здоровое",
                WateringFrequencyDays = 7,
                FertilizerFrequencyWeeks = 4,
                LastWateringDate = DateTime.Now.AddDays(-10)
            };

            var plant2 = new Plant
            {
                Name = "Plant2",
                Location = "Кухня",
                CareType = "Цветущий",
                Status = "Здоровое",
                WateringFrequencyDays = 5,
                FertilizerFrequencyWeeks = 4,
                LastWateringDate = DateTime.Now,
                LastFertilizerDate = DateTime.Now
            };

            await repository.AddAsync(plant1);
            await repository.AddAsync(plant2);

            var plants = await repository.GetPlantsRequiringCareAsync(DateTime.Now);

            Assert.Single(plants);
            Assert.Equal("Plant1", plants[0].Name);
        }

        [Fact]
        public async Task UpdateAsync_PlantIsUpdated_Successfully()
        {
            var context = CreateInMemoryContext("UpdateTest");
            var repository = new PlantRepository(context);
            var plant = new Plant { Name = "Original", Location = "Комната", CareType = "Кактус", Status = "Здоровое", WateringFrequencyDays = 7, FertilizerFrequencyWeeks = 4 };
            await repository.AddAsync(plant);

            plant.Name = "Updated";
            await repository.UpdateAsync(plant);

            var updated = await repository.GetByIdAsync(plant.Id);
            Assert.Equal("Updated", updated.Name);
        }

        [Fact]
        public async Task DeleteAsync_PlantIsRemoved()
        {
            var context = CreateInMemoryContext("DeleteTest");
            var repository = new PlantRepository(context);
            var plant = new Plant { Name = "ToDelete", Location = "Комната", CareType = "Кактус", Status = "Здоровое", WateringFrequencyDays = 7, FertilizerFrequencyWeeks = 4 };
            await repository.AddAsync(plant);

            await repository.DeleteAsync(plant.Id);

            var plants = await repository.GetAllAsync();
            Assert.Empty(plants);
        }

        [Fact]
        public async Task AddCareRecordAsync_UpdatesPlantLastWateringDate()
        {
            var context = CreateInMemoryContext("CareRecordTest");
            var repository = new PlantRepository(context);
            var plant = new Plant { Name = "Plant1", Location = "Комната", CareType = "Кактус", Status = "Здоровое", WateringFrequencyDays = 7, FertilizerFrequencyWeeks = 4 };
            await repository.AddAsync(plant);

            var record = new CareRecord
            {
                PlantId = plant.Id,
                Date = DateTime.Now,
                Type = "Полив"
            };
            await repository.AddCareRecordAsync(record);

            var updatedPlant = await repository.GetByIdAsync(plant.Id);
            Assert.NotNull(updatedPlant.LastWateringDate);
        }
    }
}