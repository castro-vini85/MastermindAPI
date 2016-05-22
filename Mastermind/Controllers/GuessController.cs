using System.Web.Http;

namespace Mastermind.Controllers
{
    public class GuessController : ApiController
    {
        private Services.GuessService guessService;

        public GuessController()
        {
            guessService = new Services.GuessService();
        }

        public Models.GuessResponse Post([FromBody]Models.GuessParam value)
        {
            var response = guessService.GuessSecret(value);

            return response;
        }
    }
}
