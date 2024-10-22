using AGILE2024_BE.Data;
using AGILE2024_BE.Data.Models;
using AGILE2024_BE.Data.Requests;
using AGILE2024_BE.Data.Responses;
using AGILE2024_BE.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private MySqlDataSource _database;
        public LoginController(IConfiguration config, MySqlDataSource database)
        {
            _config = config;
            _database = database;
        }

        [HttpPost]
        public IActionResult Post([FromBody] LoginRequestCustom req)
        {
            using var connection = _database.OpenConnection();

            string query = "select email, password, id_role, salt, name, surname, title_before, title_after from user_tab where email = @parEmail";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("parEmail", req.Email);

            command.Prepare();
            using var reader = command.ExecuteReader();
            bool exists = reader.Read();


            if (!exists)
            {
                return NotFound("User with this email does not exist!");
            }

            UserCustom user = new UserCustom()
            {
                Email = reader.GetString(0),
                Password = reader.GetString(1),
                Id_Role = reader.GetInt32(2),
                Salt = (byte[])reader["Salt"],
                Name = reader.GetString(4),
                Surname = reader.GetString(5),
                Title_Before = reader.IsDBNull(6) ? null : reader.GetString(6),
                Title_After = reader.IsDBNull(7) ? null : reader.GetString(7)
            };

            connection.Close();

            bool match = PasswordHasher.VerifyPassword(user.Password, user.Salt, req.Password);

            if (!match)
            {
                return Unauthorized("Passwords do not match!");
            }

            string token = string.Empty;

            try
            {
                token = JWTHelper.CreateJWT(user, _config);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            LoginResponseCustom res = new LoginResponseCustom()
            {
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Token = token,
                Role = ((Roles)user.Id_Role).ToString(),
                Title_After = user.Title_After,
                Title_Before = user.Title_Before
            };

            return Ok(res);
        }
    }
}
