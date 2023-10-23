using inforpatissien_api.Models;
using inforpatissien_api.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Diagnostics;
using System.Threading.Tasks;

namespace inforpatissien_api.Controllers
{
    public class AuthenticationController : ApiController
    {
        /// <summary>
        /// Permet de se connecter, récupérer le token
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage Login([FromBody] IFPLoginData _login)
        {
            try
            {
                IFPUserData user = AuthenticationService.GetUserByLogin(_login);
                if (user != null)
                {
                    var roles = new string[] { "standard" };
                    var jwtSecurityToken = AuthenticationService.GenerateJwtToken(user.id, user.name, user.gamer, user.avatarUrl, user.difficulty, user.videoUrl, roles.ToList());
                    IFPTokenData token = new IFPTokenData { token = jwtSecurityToken, userName = user.name, gamer = user.gamer, avatarUrl = user.avatarUrl, difficulty = user.difficulty, videoUrl = user.videoUrl };
                    return Request.CreateResponse(HttpStatusCode.OK, token);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Les identifiants de connexion renseignés ne sont pas corrects.");
                }
            }
            catch (HttpResponseException e)
            {
                return Request.CreateResponse(e.Response.StatusCode, new { Message = e.Response.ReasonPhrase });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

        }

        /// <summary>
        /// Permet de récupérer la liste des utilisateurs joueurs
        /// </summary>
        [HttpGet]
        [Route("gamers")]
        [ResponseType(typeof(List<IFPUserData>))]
        public HttpResponseMessage GetGamers()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, AuthenticationService.GetGamers());
            }
            catch (HttpResponseException e)
            {
                return Request.CreateResponse(e.Response.StatusCode, new { Message = e.Response.ReasonPhrase });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

        }

        /// <summary>
        /// Permet de récupérer la liste des utilisateurs joueurs qui ont terminés leur partie
        /// </summary>
        [HttpGet]
        [Route("podium")]
        [ResponseType(typeof(List<IFPUserData>))]
        public HttpResponseMessage GetPodium()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, AuthenticationService.GetPodium());
            }
            catch (HttpResponseException e)
            {
                return Request.CreateResponse(e.Response.StatusCode, new { Message = e.Response.ReasonPhrase });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Met à jour le podium
        /// </summary>
        [HttpPatch]
        [Route("podium")]
        [ResponseType(typeof(void))]
        public HttpResponseMessage SetUserPodium()
        {
            try
            {
                AuthenticationService.SetUserPodium();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (HttpResponseException e)
            {
                return Request.CreateResponse(e.Response.StatusCode, new { Message = e.Response.ReasonPhrase });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}