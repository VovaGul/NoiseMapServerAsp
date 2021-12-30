using DAL.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoiseMapServerAsp.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NoiseMapServerAsp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(/*UserManager<User> userManager, SignInManager<User> signInManager*/)
        {

        }

        public IActionResult Login(string ReturnUrl = "/")
        {
            LoginModel objLoginModel = new LoginModel();
            objLoginModel.ReturnUrl = ReturnUrl;
            return View(objLoginModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel objLoginModel)
        {
            //A claim is a statement about a subject by an issuer and
            //represent attributes of the subject that are useful in the context of authentication and authorization operations.
            //var claims = new List<Claim> {
            //    new Claim(ClaimTypes.NameIdentifier,Convert.ToString(user.Id)),
            //    new Claim(ClaimTypes.Name, user.UserName),
            //};
            var claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier,Convert.ToString(1)),
                    new Claim(ClaimTypes.Name,objLoginModel.UserName),
                    new Claim("FavoriteDrink","Tea")
                    };
            //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity
            var principal = new ClaimsPrincipal(identity);
            //SignInAsync is a Extension method for Sign in a principal for the specified scheme.
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                principal, new AuthenticationProperties() { IsPersistent = objLoginModel.RememberLogin });

            return LocalRedirect(objLoginModel.ReturnUrl);
        }

        public async Task<IActionResult> LogOut()
        {
            //SignOutAsync is Extension method for SignOut
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //Redirect to home page
            return LocalRedirect("/");
        }
    }
}
