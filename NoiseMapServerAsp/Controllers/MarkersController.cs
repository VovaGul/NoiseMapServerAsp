using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace NoiseMapServerAsp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarkersController : ControllerBase
    {
        private readonly ApplicationContext _applicationContext;

        public MarkersController(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }


        //GET api/markers/all
        [HttpGet("all")]
        public List<Marker> GetAll()
        {
            return _applicationContext.Markers.ToList();
        }
    }
}
