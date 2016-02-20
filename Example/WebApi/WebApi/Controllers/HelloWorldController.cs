using System.Web.Http;

namespace WebApi.Controllers
{
    public class HelloWorldController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok("Hello World!");
        }
    }
}
