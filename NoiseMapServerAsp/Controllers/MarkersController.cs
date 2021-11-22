using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoiseMapServerAsp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarkersController : ControllerBase
    {
        private readonly ApplicationContext _applicationContext;
        private readonly AudioRepository _audioRepository;

        public MarkersController(ApplicationContext applicationContext, AudioRepository audioRepository)
        {
            _applicationContext = applicationContext;
            _audioRepository = audioRepository;

        }

        //GET api/markers/all
        [HttpGet("all")]
        public List<Marker> GetAll()
        {
            return _applicationContext.Markers.ToList();
        }

        [HttpGet("{id}")]
        public Marker GetById(int id)
        {
            return _applicationContext.Markers.Where(marker => marker.Id == id).Single();
        }

        [HttpPost("audio/{id}")]
        public async Task GetAudio(int id)
        {
            var marker = _applicationContext.Markers.Where(marker => marker.Id == id).Single();

            var stream = _audioRepository.GetAudio(marker);

            Response.StatusCode = 200;
            Response.ContentType = "audio/mpeg";
            using (Response.Body)
            {
                await stream.CopyToAsync(Response.Body);
            }
            stream.Dispose();
        }
    }
}
