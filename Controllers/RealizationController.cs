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
    [EnableCors("*", "*", "*")]
    public class RealizationController : ApiController
    {
        /// <summary>
        /// Récupération de toutes les réalisations
        /// </summary>
        [HttpPost]
        [Route("realizations")]
        [ResponseType(typeof(IFPResponseRealizationData))]
        public HttpResponseMessage GetRealizations([FromUri] IFPParamPaginationData param, [FromBody] IFPRealizationFilterData filter)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, RealizationService.GetRealizations(param, filter));
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
        /// Récupération de toutes les mini réalisations
        /// </summary>
        [HttpGet]
        [Token]
        [Route("realizations-mini")]
        [ResponseType(typeof(List<IFPMiniRealizationData>))]
        public HttpResponseMessage GetMiniRealizations()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, RealizationService.GetMiniRealizations());
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
        /// Récupération d'une réalisation par son identifiant
        /// </summary>
        [HttpGet]
        [Route("realizations/{id:int=-1}")]
        [ResponseType(typeof(IFPRealizationData))]
        public HttpResponseMessage GetRealization(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, RealizationService.GetRealization(id));
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
        /// Création d'une réalisation
        /// </summary>
        [HttpPut]
        [Token]
        [Route("realizations")]
        [ResponseType(typeof(IFPRealizationData))]
        public HttpResponseMessage CreateRealization(IFPBodyRealizationData body)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, RealizationService.CreateRealization(body));
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
        /// Modification d'une réalisation
        /// </summary>
        [HttpPut]
        [Token]
        [Route("realizations/{id:int=-1}")]
        [ResponseType(typeof(void))]
        public HttpResponseMessage SetRealization(int id, [FromBody] IFPBodyRealizationData body)
        {
            try
            {
                RealizationService.SetRealization(id, body);
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

        /// <summary>
        /// Modification des infos source/notes d'une réalisation
        /// </summary>
        [HttpPatch]
        [Token]
        [Route("realizations/{id:int=-1}")]
        [ResponseType(typeof(void))]
        public HttpResponseMessage SetRealizationAdditionals(int id, [FromBody] IFPBodyRealizationAdditionalsData body)
        {
            try
            {
                RealizationService.SetRealizationAdditionals(id, body);
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

        /// <summary>
        /// Upload de la photo d'une réalisation
        /// </summary>
        [HttpPost]
        [Token]
        [Route("realizations/upload-photo")]
        [ResponseType(typeof(IFPResponseUploadPhotoData))]
        public HttpResponseMessage UploadPhoto()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, RealizationService.UploadPhoto());
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
        /// Ajout ou modification de la photo d'une réalisation
        /// </summary>
        [HttpPost]
        [Token]
        [Route("realizations/photos")]
        [ResponseType(typeof(IFPRealizationData))]
        public HttpResponseMessage UpsertPhotos([FromUri] int realizationId, [FromUri] bool main, [FromUri] int order)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, RealizationService.UpsertPhoto(realizationId, main, order));
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
        /// Suppression d'une réalisation
        /// </summary>
        [HttpDelete]
        [Token]
        [Route("realizations/{id:int=-1}")]
        [ResponseType(typeof(void))]
        public HttpResponseMessage DeleteRealization(int id)
        {
            try
            {
                RealizationService.DeleteRealization(id);
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