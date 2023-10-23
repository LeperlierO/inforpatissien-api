using inforpatissien_api.Helpers;
using inforpatissien_api.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace inforpatissien_api.Services
{
    public class AuthenticationService
    {
        public static string GenerateJwtToken(int id, string username, bool gamer, string avatarUrl, int difficulty, string videoUrl, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, id.ToString()),
                new Claim("gamer", gamer.ToString()),
                new Claim("avatarUrl", avatarUrl),
                new Claim("videoUrl", videoUrl),
                new Claim("difficulty", difficulty.ToString())
            };

            roles.ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"])));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(Convert.ToString(ConfigurationManager.AppSettings["config:JwtExpireDays"])));

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static IFPUserData GetUserByLogin(IFPLoginData _login)
        {
            IFPUserData user = null;
            SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECT"].ToString());

            string sqlRequest = "SELECT * " +
                                "FROM IFPUSER " +
                                "WHERE USRMAIL = @email AND USRPASSWORD = @password";

            SqlCommand cmd = new SqlCommand(String.Empty, connect);

            cmd.Parameters.AddWithValue("@email", _login.email);
            cmd.Parameters.AddWithValue("@password", _login.password);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;

            try
            {
                connect.Open();
                SqlDataReader areader = cmd.ExecuteReader();
                if (areader.Read())
                {
                    user = SqlDataReaderToUser(areader);
                }
                areader.Close();
                connect.Close();
            }
            catch (SqlException ex)
            {
                connect.Close();
            }

            return user;
        }

        public static IFPUserData GetUser(int _id)
        {
            IFPUserData user = null;
            SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECT"].ToString());

            string sqlRequest = "SELECT * " +
                                "FROM IFPUSER " +
                                "WHERE USRID = @id";

            SqlCommand cmd = new SqlCommand(String.Empty, connect);

            cmd.Parameters.AddWithValue("@id", _id);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;

            try
            {
                connect.Open();
                SqlDataReader areader = cmd.ExecuteReader();
                if (areader.Read())
                {
                    user = SqlDataReaderToUser(areader);
                }
                areader.Close();
                connect.Close();
            }
            catch (SqlException ex)
            {
                connect.Close();
            }

            return user;
        }

        public static List<IFPUserData> GetGamers()
        {
            List<IFPUserData> users = new List<IFPUserData>();
            SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECT"].ToString());

            string sqlRequest = "SELECT * " +
                                "FROM IFPUSER " +
                                "WHERE USRGAMER = 1";

            SqlCommand cmd = new SqlCommand(String.Empty, connect);

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;

            try
            {
                connect.Open();
                SqlDataReader areader = cmd.ExecuteReader();
                while (areader.Read())
                {
                    users.Add(SqlDataReaderToUser(areader));
                }
                areader.Close();
                connect.Close();
            }
            catch (SqlException ex)
            {
                connect.Close();
            }

            return users;
        }

        public static List<IFPUserData> GetPodium()
        {
            List<IFPUserData> users = new List<IFPUserData>();
            SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECT"].ToString());

            string sqlRequest = "SELECT * " +
                                "FROM IFPUSER " +
                                "WHERE USRGAMER = 1 " +
                                "AND USRWIN IS NOT NULL " +
                                "ORDER BY USRWIN ";

            SqlCommand cmd = new SqlCommand(String.Empty, connect);

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;

            try
            {
                connect.Open();
                SqlDataReader areader = cmd.ExecuteReader();
                while (areader.Read())
                {
                    users.Add(SqlDataReaderToUser(areader));
                }
                areader.Close();
                connect.Close();
            }
            catch (SqlException ex)
            {
                connect.Close();
            }

            return users;
        }

        public static void SetUserPodium()
        {
            IFPUserData session = Common.GetSession();

            SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECT"].ToString());

            string sqlRequest = "UPDATE IFPUSER SET USRWIN=@now WHERE USRID=@user";

            SqlCommand cmd = new SqlCommand(String.Empty, connect);

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;

            cmd.Parameters.AddWithValue("@now", DateTime.Now.ToUniversalTime());
            cmd.Parameters.AddWithValue("@user", session.id);

            try
            {
                connect.Open();
                cmd.ExecuteNonQuery();
                connect.Close();
            }
            catch (SqlException ex)
            {
                connect.Close();
            }
        }


        public static IFPUserData SqlDataReaderToUser(SqlDataReader _reader)
        {
            IFPUserData user = new IFPUserData();
            user.id = Convert.ToInt32(_reader["USRID"]);
            user.name = Convert.ToString(_reader["USRNAME"]);
            user.mail = Convert.ToString(_reader["USRMAIL"]);
            user.owner = Convert.ToBoolean(_reader["USROWNER"]);
            user.gamer = Convert.ToBoolean(_reader["USRGAMER"]);
            user.avatarUrl = Convert.ToString(_reader["USRAVATARURL"]);
            user.difficulty = Convert.ToInt32(_reader["DFCID"]);
            user.videoUrl = Convert.ToString(_reader["USRVIDEOURL"]);
            return user;
        }
    }
}