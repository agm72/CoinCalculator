namespace CoinsCalculator.Server.Model
{
    public class CoinRequest
    {
        public double TargetAmount { get; set; }
        public List<double> CoinDenominations { get; set; }
    }
}
