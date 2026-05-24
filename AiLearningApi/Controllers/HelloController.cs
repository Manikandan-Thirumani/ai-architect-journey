using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AiLearningApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "AI Architect Journey Started";
        }
    }
}
