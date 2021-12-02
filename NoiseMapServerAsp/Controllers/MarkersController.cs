using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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
        [EnableCors(Startup.MyAllowSpecificOrigins)]
        public List<Marker> GetAll()
        {
            return _applicationContext.Markers.ToList();
        }

        [HttpGet("{id}")]
        public Marker GetById(int id)
        {
            return _applicationContext.Markers.Where(marker => marker.Id == id).Single();
        }

        [HttpGet("audio/{id}")]
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


        [HttpPost("add")]
        public Marker PostMarker(Marker marker)
        {
            var createdMarker = _applicationContext.Markers.Add(marker).Entity;
            _applicationContext.SaveChanges();
            return createdMarker;
        }

        [HttpPut("edit/{id}")]
        public void UpdateMarker(Marker marker)
        {
            _applicationContext.Markers.Update(marker);
            _applicationContext.SaveChanges();
        }

        [HttpDelete("delete/{id}")]
        public void DeleteMarker(int id)
        {
            Marker marker = _applicationContext.Markers.Where(marker => marker.Id == id).Single();
            if (marker != null)
            {
                _applicationContext.Markers.Remove(marker);
                _applicationContext.SaveChanges();
            }
            
        }
    }
}
