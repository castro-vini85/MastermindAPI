namespace Mastermind.Models
{
    public class NewGameResponse : BaseResponse
    {
        public string PlayerName { get; set; }
        public string GameId { get; set; }
        public int CodeLength { get; set; }
        public char[] AvailableColors { get; set; }
    }
}