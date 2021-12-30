using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoiseMapServerAsp.Controllers
{
    public class HomeController : Controller
    {
        //[Authorize]
        public IActionResult Index()
        {
            var a = HttpContext.User.Claims;
            return View();
        }
    }
}
