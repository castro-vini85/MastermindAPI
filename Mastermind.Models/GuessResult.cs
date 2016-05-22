namespace Mastermind.Models
{
    public class GuessResult
    {
        public string Guess { get; set; }
        public int Exact { get; set; }
        public int Near { get; set; }
    }
}
