using AGILE2024_BE.Data.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IConfiguration _config;
        private MySqlDataSource _database;
        public UsersController(IConfiguration config, MySqlDataSource database)
        {
            _config = config;
            _database = database;
        }

        [Authorize(Roles = "Spravca")]
        [HttpGet]
        public IEnumerable<RegisterUsers> Get()
        {
            using var connection = _database.OpenConnection();

            string query = "select * from user_tab";
            using var command = new MySqlCommand(query, connection);

            command.Prepare();
            using var reader = command.ExecuteReader();

            List<RegisterUsers> list = new();
            while (reader.Read())
            {
                RegisterUsers user = new()
                {
                    Email = reader.GetString("email"),
                    Name = reader.GetString("name"),
                    Surname = reader.GetString("surname"),
                    Title_Before = reader.IsDBNull("title_before") ? null : reader.GetString("title_before"),
                    Title_After = reader.IsDBNull("title_after") ? null : reader.GetString("title_after"),
                    Id_User = reader.GetInt32("id_user")
                };
                list.Add(user);
            }
            connection.Close();
            return list;
        }
    }
}
