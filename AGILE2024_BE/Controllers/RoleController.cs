using AGILE2024_BE.Data.Database;
using Microsoft.AspNetCore.Authorization;
using AGILE2024_BE.Data.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private IConfiguration _config;
        private MySqlDataSource _database;
        public RoleController(IConfiguration config, MySqlDataSource database)
        {
            _config = config;
            _database = database;
        }

        [Authorize(Roles = "Spravca")]
        [HttpGet]
        public IEnumerable<Role> Get()
        {
            using var connection = _database.OpenConnection();

            string query = "select * from role_tab";
            using var command = new MySqlCommand(query, connection);

            command.Prepare();
            using var reader = command.ExecuteReader();
            
            List<Role> list = new List<Role>();
            while (reader.Read())
            {
                Role role = new Role()
                {
                    Id_Role = reader.GetInt32("id_role"),
                    Label = reader.GetString("label"),
                    Description = reader.IsDBNull("description") ? null : reader.GetString("description")
                };
                list.Add(role);
            }
            connection.Close();
            return list;
        }
    }
}
