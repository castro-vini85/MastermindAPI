using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using Mastermind.Models;

namespace Mastermind.DAO
{
    public class GuessDAO
    {
        private string connectionString;

        public GuessDAO()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["baseConnection"].ToString();
        }

        public string getSecret(string gameId)
        {
            string secret = null;

            using (var sqLiteConnection = new SQLiteConnection(connectionString))
            {
                sqLiteConnection.Open();

                StringBuilder sql = new StringBuilder();

                sql.AppendFormat("select secret from mm_games where game_hash='{0}';", gameId);

                using (var command = new SQLiteCommand(sql.ToString(), sqLiteConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            secret = reader["secret"].ToString();
                        }
                    }
                }
            }

            return secret;
        }

        public IEnumerable<GuessResult> SaveAndReturnGuesses(GuessParam guess, GuessResult guessResults, bool solved)
        {
            var response = new List<GuessResult>();

            using (var sqLiteConnection = new SQLiteConnection(connectionString))
            {
                sqLiteConnection.Open();

                StringBuilder sql = new StringBuilder();

                sql.Append("insert into mm_guesses(game_hash,guess,exact,near,guess_date) ");
                sql.AppendFormat("values ('{0}', '{1}', {2}, {3}, datetime('now','localtime'));", guess.GameId, guess.Guess, guessResults.Exact, guessResults.Near);

                using (var command = new SQLiteCommand(sql.ToString(), sqLiteConnection))
                {
                    command.ExecuteNonQuery();
                }

                if (solved)
                {
                    sql.Clear();

                    sql.AppendFormat("update mm_games set completion_date = datetime('now','localtime') where game_hash='{0}';", guess.GameId);

                    using (var command = new SQLiteCommand(sql.ToString(), sqLiteConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                sql.Clear();

                sql.AppendFormat("select guess, exact, near from mm_guesses where game_hash='{0}' order by guess_date;", guess.GameId);

                using (var command = new SQLiteCommand(sql.ToString(), sqLiteConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            response.Add(new GuessResult
                            {
                                Guess = reader["guess"].ToString(),
                                Exact = Convert.ToInt32(reader["exact"]),
                                Near = Convert.ToInt32(reader["near"])
                            });
                        }
                    }
                }
            }

            return response;
        }

        public DateTime? ValidateGameID(string gameId)
        {
            DateTime? creation = null;

            using (var sqLiteConnection = new SQLiteConnection(connectionString))
            {
                sqLiteConnection.Open();

                StringBuilder sql = new StringBuilder();

                sql.AppendFormat("select completion_date from mm_games where game_hash='{0}';", gameId);

                using (var command = new SQLiteCommand(sql.ToString(), sqLiteConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (string.IsNullOrEmpty(reader["completion_date"].ToString()))
                            {
                                creation = DateTime.MinValue;
                            }
                            else
                            {
                                creation = Convert.ToDateTime(reader["completion_date"].ToString());
                            }
                        }
                    }
                }
            }

            return creation;

        }
    }
}
