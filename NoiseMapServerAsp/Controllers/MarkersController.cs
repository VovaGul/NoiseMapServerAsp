using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NoiseMapServerAsp.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace NoiseMapServerAsp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarkersController : ControllerBase
    {
        private readonly ApplicationContext _applicationContext;
        protected readonly IWebHostEnvironment _hostingEnvironment;
        private readonly AudioRepository _audioRepository;
        private readonly IHubContext<ClientConnectionHub> _hubContext;

        public MarkersController(ApplicationContext applicationContext, AudioRepository audioRepository, IHubContext<ClientConnectionHub> hubContext)
        {
            _applicationContext = applicationContext;
            _audioRepository = audioRepository;
            _hubContext = hubContext;

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
        public async Task PostAudio()
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
        [Authorize]
        public async Task<Marker> PostMarker(Marker marker)
        {
            var createdMarker = _applicationContext.Markers.Add(marker).Entity;
            _applicationContext.SaveChanges();
            await _hubContext.Clients.All.SendAsync("AddMarker", marker.Id);
            return createdMarker;
        }

        [HttpPut("edit")]
        public async Task UpdateMarker(Marker marker)
        {
            Console.WriteLine(marker.Volume.ToString());
            _applicationContext.Markers.Update(marker);
            _applicationContext.SaveChanges();
            await _hubContext.Clients.All.SendAsync("UpdateMarker", marker.Id);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task DeleteMarker(int id)
        {
            Marker marker = _applicationContext.Markers.Where(marker => marker.Id == id).Single();
            if (marker != null)
            {
                _applicationContext.Markers.Remove(marker);
                _applicationContext.SaveChanges();
                await _hubContext.Clients.All.SendAsync("DeleteMarker", marker.Id);
            }
            
        }
    }
}
