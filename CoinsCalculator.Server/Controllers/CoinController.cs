namespace CoinsCalculator.Server.Controllers
{
    using CoinsCalculator.Server.Model;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [ApiController]
    [Route("api/[controller]")]
    public class CoinController : ControllerBase
    {
        private readonly List<double> AllowedDenominations = new List<double>
    {
        0.01, 0.05, 0.1, 0.2, 0.5, 1, 2, 5, 10, 50, 100, 1000
    };

        [HttpPost("calculate-coins")]
        public IActionResult CalculateCoins([FromBody] CoinRequest request)
        {
            // Validate target amount
            if (request.TargetAmount < 0 || request.TargetAmount > 10000)
            {
                return BadRequest("Target amount must be between 0 and 10,000.00");
            }

            // Validate coin denominations
            if (request.CoinDenominations == null || request.CoinDenominations.Count == 0)
            {
                return BadRequest("Coin denominations are required.");
            }

            if (!request.CoinDenominations.All(coin => AllowedDenominations.Contains(coin)))
            {
                return BadRequest($"All coin denominations must be one of the following: {string.Join(", ", AllowedDenominations)}");
            }

            // Calculate the minimum number of coins
            var result = GetMinimumCoins(request.TargetAmount, request.CoinDenominations);

            if (result == null)
            {
                return BadRequest("The target amount cannot be made up with the given coin denominations.");
            }

            // Return coins in ascending order
            return Ok(result.OrderBy(x => x).ToList());
        }

        private List<double> GetMinimumCoins(double targetAmount, List<double> coinDenominations)
        {
            // Sort coin denominations in descending order to calculate minimum coins
            coinDenominations.Sort();
            coinDenominations.Reverse();

            var result = new List<double>();
            double remainingAmount = targetAmount;

            foreach (var coin in coinDenominations)
            {
                while (remainingAmount >= coin)
                {
                    remainingAmount = Math.Round(remainingAmount - coin, 2); // Handle floating-point precision
                    result.Add(coin);
                }
            }

            // If there is still some remaining amount that cannot be covered, return null
            if (remainingAmount > 0)
            {
                return null;
            }
            return result;
        }
    }

}
