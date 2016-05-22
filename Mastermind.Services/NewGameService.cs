using System;
using System.Text;

namespace Mastermind.Services
{
    public class NewGameService
    {
        public Models.NewGameResponse StartNewGame(Models.NewGameParam game)
        {
            var validation = ValidateInput(game);

            if (validation.ResponseId == 0)
            {
                var codeLenght = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("codeLength"));
                var availableColors = System.Configuration.ConfigurationManager.AppSettings.Get("availableColors");

                var sBuilder = new StringBuilder();
                sBuilder.AppendFormat("{0}{1}{2}{3}", game.PlayerName, codeLenght, availableColors, DateTime.Now.Ticks);

                var gameHash = Helpers.Md5Helper.GetMD5(sBuilder.ToString());

                var secret = GenerateSecret(codeLenght, availableColors.ToCharArray());

                var newGameDAO = new DAO.NewGameDAO();
                newGameDAO.CreateNewGame(game, codeLenght, availableColors, gameHash, secret);

                return new Models.NewGameResponse
                {
                    PlayerName = game.PlayerName,
                    CodeLength = codeLenght,
                    AvailableColors = availableColors.ToCharArray(),
                    GameId = gameHash,
                    ResponseId = validation.ResponseId,
                    ResponseMessage = validation.ResponseMessage
                };
            }
            else
            {
                return new Models.NewGameResponse
                {
                    ResponseId = validation.ResponseId,
                    ResponseMessage = validation.ResponseMessage
                };
            };
        }

        private string GenerateSecret(int codeLength, char[] availableColors)
        {
            var secret = new StringBuilder();
            Random rnd = new Random();

            for (var i = 1; i<= codeLength; i++)
            {
                secret.Append(availableColors[rnd.Next(0, availableColors.Length)]);
            }

            return secret.ToString();
        }

        private Models.BaseResponse ValidateInput(Models.NewGameParam game)
        {
            if (string.IsNullOrEmpty(game.PlayerName))
            {
                return new Models.BaseResponse { ResponseId = 1, ResponseMessage = "Invalid Player Name" };
            }

            return new Models.BaseResponse { ResponseId = 0, ResponseMessage = "OK" };
        }
    }
}
