using System.Collections.Generic;

namespace Mastermind.Models
{
    public class GuessResponse : BaseResponse
    {
        public string GameId { get; set; }
        public IEnumerable<GuessResult> Guesses { get; set; }
        public bool GameSolved { get; set; }
        public string Guess { get; set; }
        public int Exact { get; set; }
        public int Near { get; set; }
    }
}
