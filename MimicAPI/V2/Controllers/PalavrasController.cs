using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.V2.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/palavras")]
    public class PalavrasController : ControllerBase
    {
        [Route("", Name = "ObterTodas")]
        public string ObterTodas()
        {
            return "Versão 2.0";
        }
    }
}
