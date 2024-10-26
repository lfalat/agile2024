using AGILE2024_BE.Data.Requests;
using AGILE2024_BE.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System.Text.RegularExpressions;

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
        public IActionResult Post([FromBody] RegisterRequestCustom req)
        {
            IActionResult returnAction = CheckForm(req);
            if (returnAction != null)
                return returnAction;
            /**using var connection = _database.OpenConnection();

            string query = "select id_user from user_tab where email = @parEmail";
            using MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("parEmail", req.Email);

            var user_id = command.ExecuteScalar();

            if (user_id != null)
            {
                return Conflict("User with this email already exists!");
            }**/

            (string hashedPassword, byte[] salt) = PasswordHasher.HashPassword(req.Password);

            using var connection = _database.OpenConnection();
            string query2 = "insert into user_tab (email, password, salt, name, surname, id_role, title_before, title_after, id_superior) values(@parEmail, @parPassword, @parSalt, @parName, @parSurname, @parRole, @parTitle_Before, @parTitle_After, @parId_Superior)";
            using MySqlCommand command2 = new MySqlCommand(query2, connection);
            command2.Parameters.AddWithValue("parEmail", req.Email);
            command2.Parameters.AddWithValue("parPassword", hashedPassword);
            command2.Parameters.AddWithValue("parSalt", salt);
            command2.Parameters.AddWithValue("parName", req.Name);
            command2.Parameters.AddWithValue("parSurname", req.Surname);
            command2.Parameters.AddWithValue("parRole", req.Id_Role);
            command2.Parameters.AddWithValue("parTitle_Before", req.Title_Before);
            command2.Parameters.AddWithValue("parTitle_After", req.Title_After);
            command2.Parameters.AddWithValue("parId_Superior", req.Id_Superior);
            int rowsAffected = command2.ExecuteNonQuery();

            connection.Close();

            return rowsAffected > 0 ? Ok("User registered successfully") : StatusCode(500, "An error occurred while registering the user");
        }

        private IActionResult? CheckEmail(string email)
        {
            // Kontrola na vyplnenie emailu
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email musí byť vyplnený");
            }
            // Kontrola na správny formát emailu
            if (!Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
            {
                return BadRequest("Email musí byť v správnom formáte");
            }
            // Kontrola na unikátnosť emailu
            using var connection = _database.OpenConnection();
            string query = "select id_user from user_tab where email = @parEmail";
            using MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("parEmail", email);
            var user_id = command.ExecuteScalar();
            connection.Close();
            if (user_id != null)
            {
                return Conflict("User with this email already exists!");
            }
            return null;
        }
        private IActionResult? CheckPassword(string password, string refPassword)
        {
            int passwordLength = 13;
            // Kontrola na vyplnenie hesla
            if (string.IsNullOrEmpty(password))
            {
                return BadRequest("Heslo musí byť vyplnené");
            }
            // Kontrola na potvrdenie hesla
            if (string.IsNullOrEmpty(refPassword))
            {
                return BadRequest("Potvrdenie hesla musí byť vyplnené");
            }
            // Kontrola na zhodu hesiel
            if (password != refPassword)
            {
                return BadRequest("Heslá sa nezhodujú");
            }
            // Kontrola na dĺžku hesla
            if (password.Length < passwordLength)
            {
                return BadRequest("Heslo musí mať minimálne dĺžku " + passwordLength + " znakov");
            }

            // Kontrola na veľké písmeno
            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                return BadRequest("Heslo musí obsahovať aspoň jedno veľké písmeno (A-Z).");
            }

            // Kontrola na malé písmeno
            if (!Regex.IsMatch(password, "[a-z]"))
            {
                return BadRequest("Heslo musí obsahovať aspoň jedno malé písmeno (a-z).");
            }

            // Kontrola na číslicu
            if (!Regex.IsMatch(password, "[0-9]"))
            {
                return BadRequest("Heslo musí obsahovať aspoň jednu číslicu (0-9).");
            }

            // Kontrola na špeciálny znak
            if (!Regex.IsMatch(password, "[^A-Za-z0-9]"))
            {
                return BadRequest("Heslo musí obsahovať aspoň jeden špeciálny znak (napr. !@#$%^&*).");
            }
            return null;
        }

        private IActionResult? CheckForm([FromBody] RegisterRequestCustom req)
        {
            if (req == null)
            {
                return BadRequest("Neplatný formát požiadavky");
            }
            if (string.IsNullOrEmpty(req.Surname))
            {
                return BadRequest("Priezvisko musí byť vyplnené");
            }
            if (string.IsNullOrEmpty(req.Name))
            {
                return BadRequest("Meno musí byť vyplnené");
            }
            IActionResult returnAction = CheckEmail(req.Email);
            if (returnAction != null)
                return returnAction;
            returnAction = CheckPassword(req.Password, req.ConfirmPassword);
            if (returnAction != null)
                return returnAction;
            return null;
        }
    }
}
