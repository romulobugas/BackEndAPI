using BackEndAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackEndAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SetupController : ControllerBase
    {
        private readonly IDatabaseInitializer _databaseInitializer;

        public SetupController(IDatabaseInitializer databaseInitializer)
        {
            _databaseInitializer = databaseInitializer;
        }

        [HttpPost]
        [Route("Initialize")]
        public IActionResult Initialize()
        {
            _databaseInitializer.Initialize();
            return Ok("Initialization completed.");
        }
    }
}
