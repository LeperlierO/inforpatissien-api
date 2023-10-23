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
    public class TokenAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if(System.Web.HttpContext.Current.Items["User"] == null)
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.Unauthorized };
                actionContext.Response.Content = new ObjectContent(typeof(object), new { Message = "Les informations d'authentification sont manquantes ou incorrectes" }, new JsonMediaTypeFormatter());
            }

            base.OnAuthorization(actionContext);
        }
    }
}