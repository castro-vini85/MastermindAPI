using System;
using System.Linq;

namespace Mastermind.Services
{
    public class GuessService
    {
        private DAO.GuessDAO guessDAO;

        public Models.GuessResponse GuessSecret(Models.GuessParam guess)
        {
            guessDAO = new DAO.GuessDAO();

            var validation = ValidateInput(guess);

            if (validation.ResponseId == 0)
            {
                var secret = guessDAO.getSecret(guess.GameId);

                if (guess.Guess.Count() != secret.Count())
                {
                    return new Models.GuessResponse
                    {
                        ResponseId = 2,
                        ResponseMessage = "Invalid Guess"
                    };
                }

                var guessResults = TryGuess(secret, guess.Guess);

                var solved = guessResults.Exact == guess.Guess.Count();

                var results = guessDAO.SaveAndReturnGuesses(guess, guessResults, solved);

                return new Models.GuessResponse
                {
                    GameId = guess.GameId,
                    Exact = guessResults.Exact,
                    Near = guessResults.Near,
                    Guess = guess.Guess,
                    Guesses = results,
                    GameSolved = solved,
                    ResponseId = validation.ResponseId,
                    ResponseMessage = validation.ResponseMessage
                };
            }
            else
            {
                return new Models.GuessResponse
                {
                    ResponseId = validation.ResponseId,
                    ResponseMessage = validation.ResponseMessage
                };
            }
        }

        private Models.GuessResult TryGuess(string secret, string guess)
        {
            int exact = 0;
            int near = 0;

            var secretArray = secret.ToCharArray().ToList();
            var guessArray = guess.ToCharArray().ToList();

            for (var i = secretArray.Count - 1; i >= 0; i--)
            {
                if (secretArray[i] == guessArray[i])
                {
                    exact++;
                    secretArray.RemoveAt(i);
                    guessArray.RemoveAt(i);
                }
            }

            for (var i = secretArray.Count - 1; i >= 0; i--)
            {
                var index = guessArray.IndexOf(secretArray[i]);

                if (index > 0)
                {
                    near++;
                    secretArray.RemoveAt(i);
                    guessArray.RemoveAt(index);
                }
            }

            return new Models.GuessResult
            {
                Exact = exact,
                Near = near,
                Guess = guess
            };
        }

        private Models.BaseResponse ValidateInput(Models.GuessParam game)
        {
            if (string.IsNullOrEmpty(game.Guess))
            {
                return new Models.BaseResponse { ResponseId = 2, ResponseMessage = "Invalid Guess" };
            }

            if (string.IsNullOrEmpty(game.GameId))
            {
                return new Models.BaseResponse { ResponseId = 3, ResponseMessage = "Invalid Game ID" };
            }
            else
            {
                DateTime? validation = guessDAO.ValidateGameID(game.GameId);

                if (!validation.HasValue)
                {
                    return new Models.BaseResponse { ResponseId = 3, ResponseMessage = "Invalid Game ID" };
                }
                else if (validation.Value != DateTime.MinValue)
                {
                    return new Models.BaseResponse { ResponseId = 4, ResponseMessage = "Game Solved" };
                }
            }

            return new Models.BaseResponse { ResponseId = 0, ResponseMessage = "OK" };
        }
    }
}
