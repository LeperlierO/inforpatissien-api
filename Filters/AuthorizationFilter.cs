using inforpatissien_api.Services;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace inforpatissien_api.Filters
{
    public class AuthorizationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                string auth = actionContext.Request.Headers.Authorization.ToString().Split(' ')[1];
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"])));
                tokenHandler.ValidateToken(auth, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "unique_name").Value);

                System.Web.HttpContext.Current.Items["User"] = AuthenticationService.GetUser(userId);
            }
            catch(Exception e)
            {
                System.Web.HttpContext.Current.Items["User"] = null;
            }

            base.OnAuthorization(actionContext);
        }
    }
}