using System;
using System.Data.SQLite;
using System.Text;

namespace Mastermind.DAO
{
    public class NewGameDAO
    {
        private string connectionString;

        public NewGameDAO()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["baseConnection"].ToString();
        }

        public void CreateNewGame(Models.NewGameParam game, int codeLength, string availableColors, string gameHash, string secret)
        {
            using (var sqLiteConnection = new SQLiteConnection(connectionString))
            {
                sqLiteConnection.Open();

                StringBuilder sql = new StringBuilder();

                sql.Append("insert into mm_games(player_name,creation_date,code_length,available_colors,game_hash, secret) ");
                sql.AppendFormat("values('{0}', datetime('now','localtime'), {1}, '{2}', '{3}', '{4}');", game.PlayerName, codeLength, availableColors, gameHash, secret);

                using (var command = new SQLiteCommand(sql.ToString(), sqLiteConnection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
