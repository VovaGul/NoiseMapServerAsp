using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoiseMapServerAsp.Models;
using System.Threading.Tasks;

namespace NoiseMapServerAsp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
            var result = await _signInManager.PasswordSignInAsync(objLoginModel.UserName,
                                  objLoginModel.Password, true, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return LocalRedirect(objLoginModel.ReturnUrl);
            }
            else
            {
                ViewBag.Message = "Invalid Credential";
                return View(objLoginModel);
            }

        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect("/");
        }
    }
}
