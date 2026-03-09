using System;
using PlantCareAssistant.Core.Models;
using PlantCareAssistant.Core.Services;
using Xunit;

namespace PlantCareAssistant.Tests.BusinessLogic
{
    public class CareCalculationTests
    {
        private readonly CareService _careService;

        public CareCalculationTests()
        {
            _careService = new CareService();
        }

        [Fact]
        public void NextWateringDate_CalculatedCorrectly_FromLastWatering()
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
                LastWateringDate = new DateTime(2025, 1, 1)
            };

            // Act
            var nextDate = plant.NextWateringDate;

            // Assert
            Assert.Equal(new DateTime(2025, 1, 8), nextDate);
        }

        [Fact]
        public void NextFertilizerDate_CalculatedCorrectly_FromLastFertilizing()
        {
            // Arrange
            var plant = new Plant
            {
                Name = "Test",
                Location = "Комната",
                CareType = "Кактус",
                Status = "Здоровое",
                WateringFrequencyDays = 7,
                FertilizerFrequencyWeeks = 2,
                LastFertilizerDate = new DateTime(2025, 1, 1)
            };

            // Act
            var nextDate = plant.NextFertilizerDate;

            // Assert
            Assert.Equal(new DateTime(2025, 1, 15), nextDate);
        }

        [Fact]
        public void RequiresWatering_ReturnsTrue_WhenDatePassed()
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
            var requires = plant.RequiresWatering;

            // Assert
            Assert.True(requires);
        }

        [Fact]
        public void RequiresWatering_ReturnsFalse_WhenDateNotPassed()
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
                LastWateringDate = DateTime.Now.AddDays(-2)
            };

            // Act
            var requires = plant.RequiresWatering;

            // Assert
            Assert.False(requires);
        }

        [Fact]
        public void CareActionNeeded_ReturnsBoth_WhenBothOverdue()
        {
            // Arrange
            var plant = new Plant
            {
                Name = "Test",
                Location = "Комната",
                CareType = "Кактус",
                Status = "Здоровое",
                WateringFrequencyDays = 7,
                FertilizerFrequencyWeeks = 1,
                LastWateringDate = DateTime.Now.AddDays(-10),
                LastFertilizerDate = DateTime.Now.AddDays(-10)
            };

            // Act
            var action = plant.CareActionNeeded;

            // Assert
            Assert.Contains("Полив", action);
            Assert.Contains("удобрение", action.ToLower());
        }

        [Fact]
        public void CareActionNeeded_ReturnsWateringOnly_WhenOnlyWateringOverdue()
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
                LastWateringDate = DateTime.Now.AddDays(-10),
                LastFertilizerDate = DateTime.Now.AddDays(-2)
            };

            // Act
            var action = plant.CareActionNeeded;

            // Assert
            Assert.Equal("Требуется полив", action);
        }

        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(5, 5, 10)]
        [InlineData(10, 7, 17)]
        public void AddDays_CalculatesCorrectly(int startDay, int addDays, int expectedDay)
        {
            // Arrange
            var baseDate = new DateTime(2025, 1, startDay);

            // Act
            var result = baseDate.AddDays(addDays);

            // Assert
            Assert.Equal(expectedDay, result.Day);
        }

        [Theory]
        [InlineData(1, 1, 8)]
        [InlineData(1, 2, 15)]
        [InlineData(15, 1, 22)]
        public void AddWeeks_CalculatesCorrectly(int startDay, int weeks, int expectedDay)
        {
            // Arrange
            var baseDate = new DateTime(2025, 1, startDay);

            // Act
            var result = baseDate.AddWeeks(weeks);

            // Assert
            Assert.Equal(expectedDay, result.Day);
        }
    }
}