using inforpatissien_api.Models;
using inforpatissien_api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
namespace inforpatissien_api.Controllers
{
    public class RecipeController : ApiController
    {
        /// <summary>
        /// Récupération de toutes les recettes
        /// </summary>
        [HttpGet]
        [Route("recipes")]
        [ResponseType(typeof(IPResponseRecipeData))]
        public HttpResponseMessage GetRecipes([FromUri] IPParamPaginationData param)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, RecipeService.GetRecipes(param));
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
        /// Récupération d'une recette par son code
        /// </summary>
        [HttpGet]
        [Route("recipes/{recipeCode}")]
        [ResponseType(typeof(IPRecipeData))]
        public HttpResponseMessage GetRecipe(string recipeCode)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, RecipeService.GetRecipe(recipeCode));
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