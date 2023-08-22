using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using webSklad.Data;
using webSklad.Models;
using webSklad.Models.ViewModels;

namespace webSklad.Controllers
{
    public class AccountController : Controller
    {
        private readonly WebSkladContext _db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(WebSkladContext db, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;

        }

        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {

                User user = new User
                {
                    UserName = model.Email,
                    NameUser = model.NameUser,
                    SurnameUser = model.SurnameUser,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    CreatedByUserId = Guid.NewGuid().ToString()
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var defaultrole = _roleManager.FindByNameAsync("ShopOwner").Result;

                    if (defaultrole != null)
                    {
                        IdentityResult roleresult = await _userManager.AddToRoleAsync(user, defaultrole.Name);
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "There is something wrong with your email and/or password");
                }
            }
            return View(model);
        }

        private async Task SignInUser(User user)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
        }


        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}
