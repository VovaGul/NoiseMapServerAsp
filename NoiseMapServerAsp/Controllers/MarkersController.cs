using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NoiseMapServerAsp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarkersController : ControllerBase
    {
        private readonly ApplicationContext _applicationContext;
        protected readonly IWebHostEnvironment _hostingEnvironment;
        private readonly AudioRepository _audioRepository;

        public MarkersController(ApplicationContext applicationContext, IWebHostEnvironment hostingEnvironment, AudioRepository audioRepository)
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

        [HttpPost("audio/add")]
        public async void PostAudio()
        {
            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                var file = files[0];
                String path = Directory.GetParent(Directory.GetCurrentDirectory()) + "/Audio/" + file.FileName;
                System.Console.WriteLine(path);
                try
                {
                    var fileStream = new FileStream(path, FileMode.Create);
                    using (fileStream)
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                }
            }
        }

        [HttpPost("add")]
        public void PostMarker(Marker marker)
        {
            _applicationContext.Markers.Add(marker);
            _applicationContext.SaveChanges();
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
