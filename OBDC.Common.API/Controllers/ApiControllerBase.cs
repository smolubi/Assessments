using Microsoft.AspNetCore.Mvc;

namespace OBDC.Common.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
    }
}
