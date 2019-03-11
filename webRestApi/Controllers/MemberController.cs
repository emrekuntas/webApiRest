using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using webRestApi.Models;

namespace webRestApi.Controllers
{
    public class MemberController : IHttpController
    {
        public async Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {


            HttpResponseMessage result = null;
            if (controllerContext.Request.Method == HttpMethod.Get)
            {

                result = HttpGet(controllerContext);

            }
            else if (controllerContext.Request.Method == HttpMethod.Post)
            {

               //result = await HttpPost()
            }

            return result;
        }

        public HttpResponseMessage HttpGet(HttpControllerContext controllerContext)
        {

            HttpResponseMessage rest = null;

            // localhost:12345/api/Member/5
            var id = controllerContext.RouteData.Values["id"];

            if (id == null)
            {
                var context = MemberRepository.Get();
                rest = controllerContext.Request.CreateResponse(HttpStatusCode.OK, context);
            }
            else
            {
                int idAsInteger;
                if (!int.TryParse(id.ToString(), out idAsInteger))
                {

                    rest = controllerContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Id Degeri Numerik Olmalidir");
                }
                else
                {
                    var context = MemberRepository.Get(idAsInteger);
                    rest = controllerContext.Request.CreateResponse(HttpStatusCode.OK, context);
                }
            }


            return rest;
        }

        public async Task<HttpResponseMessage> HttpPost(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            HttpResponseMessage retVal = null;


            string contentString = await controllerContext.Request.Content.ReadAsStringAsync();


            Member postMember =  Newtonsoft.Json.JsonConvert.DeserializeObject<Member>(contentString);

            if (MemberRepository.IsExists(postMember.FullName))
            {
                retVal = controllerContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Ayni isimde başka biri var");

            }
            else
            {
                Member createMember = MemberRepository.Add(postMember.FullName, postMember.Age);
                retVal = controllerContext.Request.CreateResponse(HttpStatusCode.OK, createMember);
            }

            return retVal;

        }

        }
    }