using Application.Interfaces.Services;
using Application.ViewModel;
using Core.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IUserService userService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateUserAsync(model, model.Password);
                if (result)
                {
                    var user = await _userService.FindByEmailAsync(model.Email);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("AdminDashboard", "Chat");
                    }
                    else if (roles.Contains("User"))
                    {
                        return RedirectToAction("UserChat", "Chat");
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to register user.");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.FindByEmailAsync(loginViewModel.Email);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, loginViewModel.Password, false);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        var roles = await _userManager.GetRolesAsync(user);

                        if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("AdminDashboard", "Chat");
                        }
                        else if (roles.Contains("User"))
                        {
                            return RedirectToAction("UserChat", "Chat");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect password.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email not found.");
                }
            }
            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
