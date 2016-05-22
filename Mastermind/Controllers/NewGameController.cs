using System.Web.Http;

namespace Mastermind.Controllers
{
    public class NewGameController : ApiController
    {
        private Services.NewGameService newGameService;

        public NewGameController()
        {
            newGameService = new Services.NewGameService();
        }

        public Models.NewGameResponse Post([FromBody]Models.NewGameParam value)
        {
            var response = newGameService.StartNewGame(value);

            return response;
        }
    }
}
