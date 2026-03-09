using System;
using System.Collections.Generic;
using PlantCareAssistant.Core.Models;
using PlantCareAssistant.Core.Services;
using Xunit;

namespace PlantCareAssistant.Tests.Services
{
    public class CareServiceTests
    {
        private readonly CareService _careService;

        public CareServiceTests()
        {
            _careService = new CareService();
        }

        [Fact]
        public void CalculateNextWateringDate_ReturnsCorrectDate()
        {
            // Arrange
            var lastWatering = new DateTime(2025, 1, 1);
            int frequency = 7;

            // Act
            var nextDate = _careService.CalculateNextWateringDate(lastWatering, frequency);

            // Assert
            Assert.Equal(new DateTime(2025, 1, 8), nextDate);
        }

        [Fact]
        public void CalculateNextFertilizerDate_ReturnsCorrectDate()
        {
            // Arrange
            var lastFertilizer = new DateTime(2025, 1, 1);
            int frequency = 2; // 2 недели

            // Act
            var nextDate = _careService.CalculateNextFertilizerDate(lastFertilizer, frequency);

            // Assert
            Assert.Equal(new DateTime(2025, 1, 15), nextDate);
        }

        [Fact]
        public void IsPlantRequiringCare_ReturnsTrue_WhenWateringOverdue()
        {
            // Arrange
            var plant = new Plant
            {
                Name = "Test",
                Location = "Комната",
                CareType = "Кактус",
                Status = "Здоровое",
                WateringFrequencyDays = 7,
                FertilizerFrequencyWeeks = 4,
                LastWateringDate = DateTime.Now.AddDays(-10)
            };

            // Act
            var requiresCare = _careService.IsPlantRequiringCare(plant, DateTime.Now);

            // Assert
            Assert.True(requiresCare);
        }

        [Fact]
        public void IsPlantRequiringCare_ReturnsFalse_WhenCareIsUpToDate()
        {
            // Arrange
            var plant = new Plant
            {
                Name = "Test",
                Location = "Комната",
                CareType = "Кактус",
                Status = "Здоровое",
                WateringFrequencyDays = 7,
                FertilizerFrequencyWeeks = 4,
                LastWateringDate = DateTime.Now.AddDays(-2), // Полили 2 дня назад
                LastFertilizerDate = DateTime.Now.AddDays(-10) // Удобрили 10 дней назад (частота 4 недели)
            };

            // Act
            var requiresCare = _careService.IsPlantRequiringCare(plant, DateTime.Now);

            // Assert
            Assert.False(requiresCare);
        }

        [Fact]
        public void GetStatusBasedOnCare_ReturnsHealthy_WhenRecentlyWatered()
        {
            // Arrange
            var plant = new Plant
            {
                Name = "Test",
                Location = "Комната",
                CareType = "Кактус",
                Status = "Здоровое",
                WateringFrequencyDays = 7,
                FertilizerFrequencyWeeks = 4,
                LastWateringDate = DateTime.Now.AddDays(-3)
            };

            // Act
            var status = _careService.GetStatusBasedOnCare(plant, DateTime.Now);

            // Assert
            Assert.Equal("Здоровое", status);
        }

        [Fact]
        public void GetStatusBasedOnCare_ReturnsRequiresAttention_WhenWateringOverdue()
        {
            // Arrange
            var plant = new Plant
            {
                Name = "Test",
                Location = "Комната",
                CareType = "Кактус",
                Status = "Здоровое",
                WateringFrequencyDays = 7,
                FertilizerFrequencyWeeks = 4,
                LastWateringDate = DateTime.Now.AddDays(-10)
            };

            // Act
            var status = _careService.GetStatusBasedOnCare(plant, DateTime.Now);

            // Assert
            Assert.Equal("Требует внимания", status);
        }

        [Fact]
        public void GetStatusBasedOnCare_ReturnsSick_WhenWateringVeryOverdue()
        {
            // Arrange
            var plant = new Plant
            {
                Name = "Test",
                Location = "Комната",
                CareType = "Кактус",
                Status = "Здоровое",
                WateringFrequencyDays = 7,
                FertilizerFrequencyWeeks = 4,
                LastWateringDate = DateTime.Now.AddDays(-20)
            };

            // Act
            var status = _careService.GetStatusBasedOnCare(plant, DateTime.Now);

            // Assert
            Assert.Equal("Болеет", status);
        }

        [Fact]
        public void GetCareRecommendation_ReturnsSpecificAdvice_ForCactus()
        {
            // Arrange
            var plant = new Plant
            {
                Name = "Cactus",
                Location = "Комната",
                CareType = "Кактус",
                Status = "Здоровое",
                WateringFrequencyDays = 14,
                FertilizerFrequencyWeeks = 4
            };

            // Act
            var recommendation = _careService.GetCareRecommendation(plant);

            // Assert
            Assert.Contains("Минимальный полив", recommendation);
        }

        [Fact]
        public void GetCareRecommendation_ReturnsSpraying_WhenNeedsSpraying()
        {
            // Arrange
            var plant = new Plant
            {
                Name = "Test",
                Location = "Комната",
                CareType = "Цветущий",
                Status = "Здоровое",
                WateringFrequencyDays = 5,
                FertilizerFrequencyWeeks = 4,
                NeedsSpraying = true
            };

            // Act
            var recommendation = _careService.GetCareRecommendation(plant);

            // Assert
            Assert.Contains("опрыскивание", recommendation.ToLower());
        }

        [Fact]
        public void GetSmartRecommendations_ReturnsNonEmptyList()
        {
            // Arrange
            var plant = new Plant
            {
                Name = "Test",
                Location = "Комната",
                CareType = "Кактус",
                Status = "Здоровое",
                WateringFrequencyDays = 7,
                FertilizerFrequencyWeeks = 4
            };

            // Act
            var recommendations = _careService.GetSmartRecommendations(plant);

            // Assert
            Assert.NotEmpty(recommendations);
        }
    }
}