using JWTAuth.WebApi.Models;
using JWTAuth.WebApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuth.WebApi.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly DatabaseContext context;

        public TokenController(IConfiguration config, DatabaseContext context)
        {
            _configuration = config;
            this.context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(string email, string password)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var user = await GetUser(email, password);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id", user.Id.ToString()),
                        new Claim("DisplayName", user.EmployeeName),
                        new Claim("Email", user.Email),
                        new Claim("JobTitle", user.JobTitle),
                        new Claim("Gender", user.Gender)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<Employee> GetUser(string email, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(context.Database.GetConnectionString()))
                {
                    conn.Open();
                    StringBuilder query = new StringBuilder();
                    Employee employee = null;
                    query.Append("SELECT * FROM [JwtAuthDb].[dbo].[Employees] WHERE [Email] = @email");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                    cmd.Parameters.AddWithValue("@email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (Crypto.validatePassword(reader["Password"].ToString(), password))
                            {
                                employee = (new Employee()
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                    EmployeeName = reader["EmployeeName"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    Gender = reader["Gender"].ToString(),
                                    JobTitle = reader["JobTitle"].ToString(),
                                    ModifiedDate = Convert.ToDateTime(reader["ModifiedDate"]),
                                });
                            }
                        }
                        conn.Close();
                    }

                    return employee;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
