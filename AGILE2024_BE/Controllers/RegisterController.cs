using AGILE2024_BE.Data.Requests;
using AGILE2024_BE.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private IConfiguration _config;
        private MySqlDataSource _database;
        public RegisterController(IConfiguration config, MySqlDataSource database)
        {
            _config = config;
            _database = database;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_config["JWT_Secret"]);
        }

        [Authorize(Roles = "Spravca")]
        [HttpPost]
        public IActionResult Post([FromBody]RegisterRequestCustom req)
        {

            using var connection = _database.OpenConnection();

            string query = "select id_user from user_tab where email = @parEmail";
            using MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("parEmail", req.Email);

            var user_id = command.ExecuteScalar();

            if (user_id != null)
            {
                return Conflict("User with this email already exists!");
            }

            (string hashedPassword, byte[] salt) = PasswordHasher.HashPassword(req.Password);

            string query2 = "insert into user_tab (email, password, salt, name, surname, id_role) values(@parEmail, @parPassword, @parSalt, @parName, @parSurname, @parRole)";
            using MySqlCommand command2 = new MySqlCommand(query2, connection);
            command2.Parameters.AddWithValue("parEmail", req.Email);
            command2.Parameters.AddWithValue("parPassword", hashedPassword);
            command2.Parameters.AddWithValue("parSalt", salt);
            command2.Parameters.AddWithValue("parName", req.Name);
            command2.Parameters.AddWithValue("parSurname", req.Surname);
            command2.Parameters.AddWithValue("parRole", req.Id_Role);

            int rowsAffected = command2.ExecuteNonQuery();

            connection.Close();

            return rowsAffected > 0 ? Ok("User registered successfully") : StatusCode(500, "An error occurred while registering the user");
        }
    }
}
