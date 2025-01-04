namespace CoinsCalculatorTest
{
    using CoinsCalculator.Server.Controllers;
    using CoinsCalculator.Server.Model;
    using NUnit.Framework;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    [TestFixture]
    public class CoinControllerTests
    {
        private CoinController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new CoinController();
        }

        [Test]
        public void CalculateCoins_ValidInput_ReturnsCorrectCoins()
        {
            // Arrange
            var request = new CoinRequest
            {
                TargetAmount = 7.03,
                CoinDenominations = new List<double> { 0.01, 0.5, 1, 5, 10 }
            };

            // Act
            var result = _controller.CalculateCoins(request) as OkObjectResult;
            var resultCoins = result?.Value as List<double>;

            // Assert
            Assert.IsNotNull(resultCoins);
            Assert.That(resultCoins, Is.EqualTo(new List<double> { 0.01, 0.01, 0.01, 1, 1, 5 }));
        }

        [Test]
        public void CalculateCoins_InvalidTargetAmount_ReturnsBadRequest()
        {
            // Arrange
            var request = new CoinRequest
            {
                TargetAmount = -1, // Invalid target amount
                CoinDenominations = new List<double> { 0.01, 0.5, 1, 5, 10 }
            };

            // Act
            var result = _controller.CalculateCoins(request) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 400);
            Assert.AreEqual(result.Value, "Target amount must be between 0 and 10,000.00");
        }

        [Test]
        public void CalculateCoins_InvalidCoinDenominations_ReturnsBadRequest()
        {
            // Arrange
            var request = new CoinRequest
            {
                TargetAmount = 7.03,
                CoinDenominations = new List<double> { 11 } // Invalid coin denomination (not in AllowedDenominations)
            };

            // Act
            var result = _controller.CalculateCoins(request) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 400);
            Assert.AreEqual(result.Value, "All coin denominations must be one of the following: 0.01, 0.05, 0.1, 0.2, 0.5, 1, 2, 5, 10, 50, 100, 1000");
        }

        [Test]
        public void CalculateCoins_EmptyCoinDenominations_ReturnsBadRequest()
        {
            // Arrange
            var request = new CoinRequest
            {
                TargetAmount = 7.03,
                CoinDenominations = new List<double>() // Empty coin denominations
            };

            // Act
            var result = _controller.CalculateCoins(request) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 400);
            Assert.AreEqual(result.Value, "Coin denominations are required.");
        }

        [Test]
        public void CalculateCoins_CannotMakeTargetAmount_ReturnsBadRequest()
        {
            // Arrange
            var request = new CoinRequest
            {
                TargetAmount = 7.03,
                CoinDenominations = new List<double> { 0.1, 0.5 } // Not enough denominations to make the target
            };

            // Act
            var result = _controller.CalculateCoins(request) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 400);
            Assert.AreEqual(result.Value, "The target amount cannot be made up with the given coin denominations.");
        }
    }
}
