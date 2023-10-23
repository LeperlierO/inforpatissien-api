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
using System.Web.Http.Cors;
using System.Web.Http.Description;
using System.Diagnostics;
using inforpatissien_api.Filters;
using inforpatissien_api.Helpers;
using System.Configuration;

namespace inforpatissien_api.Controllers
{
    public class RecipeController : ApiController
    {
        /// <summary>
        /// Récupération d'une recette à par son identifiant
        /// </summary>
        [HttpGet]
        [Route("recipes/{id:int=-1}")]
        [ResponseType(typeof(IFPRecipeData))]
        public HttpResponseMessage GetRecipe(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, RecipeService.GetRecipe(id));
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
        /// Récupération de toutes les mini recettes
        /// </summary>
        [HttpGet]
        [Token]
        [Route("recipes-mini")]
        [ResponseType(typeof(List<IFPMiniRecipeData>))]
        public HttpResponseMessage GetMiniRecipes()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, RecipeService.GetMiniRecipes());
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