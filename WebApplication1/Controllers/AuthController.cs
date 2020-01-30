using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Npgsql;
namespace WebApplication1.Controllers
{
    public class AuthController : Controller
    {
        private authdbContext dB;

        public AuthController(authdbContext context)
        {
            dB = context;
        }
        [HttpPost("/token")]
        public IActionResult GetToken(string login, string password)
        {
            Users user = dB.Users.FirstOrDefault(x => x.Id == login && x.Password == password);
            if (user == null)
                return BadRequest(new { errorText = "Неверный логин или пароль" });
            //string connStr = "Host=127.0.0.1; DataBase=testdb; Username=postgres; Password=pgadmin";
            //NpgsqlConnection connection = new NpgsqlConnection(connStr);
            // connection.Open();
            //NpgsqlCommand command = new NpgsqlCommand(
            //    $"SELECT * FROM users WHERE login='{login}' AND password='{password}'", connection);
            //if(command.ExecuteNonQuery() != 1)
            //    return BadRequest(new { errorText = "Неверный логин или пароль" });
            //секретный ключ
            string secretKey = "secretKeydg46fdb54e6b6dfb6df";
            //симметричный секретный ключ
            var symmetricSecretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: "smesk.in",
                    audience: "reader",
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)),
                    signingCredentials: new SigningCredentials(symmetricSecretKey, SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt
            };

            return Json(response);
        }
    }
}