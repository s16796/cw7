using cw7.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace cw7.Services
{
    public class JWTauthService : IJWTauthService
    {

        private IConfiguration _configuration { get; set; }
        public JWTauthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JWTResponse Login(LoginRequest request)
        {//First login, verify password, assign refresh token
            using (var client = new SqlConnection("Data Source = db-mssql.pjwstk.edu.pl; Initial Catalog = s16796; Integrated Security = True"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    client.Open();
                    var transaction = client.BeginTransaction();
                    command.Transaction = transaction;
                    command.CommandText = "Select Password, Salt FROM Student where IndexNumber=@login";
                    command.Parameters.AddWithValue("login", request.Login);
                    var dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        var salt = (byte[])dr["salt"];
                        var password = dr["Password"].ToString();
                        dr.Close();
                        if (PasswordHasherService.Verify(request.Password, password, salt))
                        {
                            var claims = new[]
                            {
                            new Claim("IndexNumber", request.Login),
                            new Claim(ClaimTypes.Role,"employee")
                            };

                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
                            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                            var token = new JwtSecurityToken
                                (
                                issuer: "Gakko",
                                audience: "Students",
                                claims: claims,
                                expires: DateTime.Now.AddMinutes(10),
                                signingCredentials: creds
                                );

                            command.CommandText = "UPDATE Student SET Refresh_Token = @refresh WHERE IndexNumber = @login";
                            var refresh = Guid.NewGuid().ToString();
                            command.Parameters.AddWithValue("refresh", refresh);
                            command.ExecuteNonQuery();
                            transaction.Commit();
                            return new JWTResponse
                            {
                                Token = new JwtSecurityTokenHandler().WriteToken(token),
                                RefreshToken = refresh
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public JWTResponse RefreshToken(RefreshRequest refresh)
        {//token refreshing
            using (var client = new SqlConnection("Data Source = db-mssql.pjwstk.edu.pl; Initial Catalog = s16796; Integrated Security = True"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    client.Open();
                    var transaction = client.BeginTransaction();
                    command.Transaction = transaction;
                    command.CommandText = "Select IndexNumber FROM Student where Refresh_Token=@refresh";
                    command.Parameters.AddWithValue("refresh", refresh.RefreshToken);
                    var dr = command.ExecuteReader();
                    command.Parameters.Clear();
                    if (dr.Read())
                    {
                        var index = dr["IndexNumber"].ToString();
                        dr.Close();
                        var claims = new[]
                        {
                         new Claim("IndexNumber", index),
                         new Claim(ClaimTypes.Role,"employee")
                        };
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken
                        (
                         issuer: "Gakko",
                         audience: "Students",
                         claims: claims,
                         expires: DateTime.Now.AddMinutes(10),
                         signingCredentials: creds
                        );
                        command.CommandText = "UPDATE Student SET Refresh_Token = @refresh WHERE IndexNumber = @login";
                        var refreshtok = Guid.NewGuid().ToString();
                        command.Parameters.AddWithValue("login", index);
                        command.Parameters.AddWithValue("refresh", refreshtok);
                        command.ExecuteNonQuery();
                        transaction.Commit();
                        return new JWTResponse
                        {
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            RefreshToken = refreshtok
                        };
                    }   
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}
