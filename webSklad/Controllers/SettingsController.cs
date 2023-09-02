using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using webSklad.Data;
using webSklad.Models.ViewModels;
using webSklad.Models;
using Microsoft.EntityFrameworkCore;
using webSklad.Interfaces;
using webSklad.Repository;
using MvcPaging;

namespace webSklad.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ISettingsRepository _settingsRepository;

        public SettingsController(IUserRepository userRepository, ISettingsRepository settingsRepository)
        {


            _userRepository = userRepository;
            _settingsRepository = settingsRepository;
        }
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }


                var workers = _settingsRepository.GetWorkersForCurrentUser(currentUser.Id);
                var shops = _settingsRepository.GetShopsForCurrentUser(currentUser.Id);

                var model = new Tuple<List<User>, List<ShopPostInfo>>(workers, shops);
                return View(model);
            }
            else
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }

                var createdByUser = await _userRepository.GetUserByIdAsync(currentUser.CreatedByUserId);
                if (createdByUser == null)
                {
                    return NotFound();
                }

                var workers = _settingsRepository.GetWorkersForCurrentUser(currentUser.Id);
                var shops = _settingsRepository.GetShopsForCurrentUser(currentUser.Id);

                var model = new Tuple<List<User>, List<ShopPostInfo>>(workers, shops);
                return View(model);

            }

            return NotFound();
        }



        public IActionResult CreateWorker()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorker(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await _userRepository.GetCurrentyUser();
            if (currentUser == null)
            {
                return NotFound();
            }

            var user = new User
            {
                UserName = model.Email,
                NameUser = model.NameUser,
                SurnameUser = model.SurnameUser,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                CreatedByUserId = currentUser.Id
            };

            var result = await _userRepository.CreateUserAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userRepository.AddUserToRoleAsync(currentUser, "Worker");
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user != null)
            {
                var result = await _userRepository.DeleteUserAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to delete the user.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User not found.");
            }

            var workers = await _userRepository.GetWorkersAsync();

            return View("Index", workers);
        }


        // Not okay
        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userRepository.GetUserRolesAsync(user);

                var editableRoleNames = new List<string>
                {
                    "Admin",
                    "Worker",
                    "ShopOwner"
                };

                var editableRoles = await _userRepository.GetEditableRolesAsync(editableRoleNames);

                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserEmail = user.Email,
                    UserId = user.Id,
                    UserRoles = userRoles,
                    AllRoles = editableRoles
                };
                return View(model);
            }
            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userRepository.GetUserRolesAsync(user);
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);

                await _userRepository.AddUserToRolesAsync(user, addedRoles);
                await _userRepository.RemoveUserFromRolesAsync(user, removedRoles);

                return RedirectToAction("Index");

            }
            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            EditUserViewModel model = new EditUserViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                NameUser = user.NameUser,
                SurnameUser = user.SurnameUser,
                PhoneNumber = user.PhoneNumber
            };
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userRepository.GetUserByIdAsync(model.UserId);

            if (user != null)
            {

                user.Email = model.Email;
                user.NameUser = model.NameUser;
                user.SurnameUser = model.SurnameUser;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userRepository.UpdateUserAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            return NotFound();
        }



        [HttpGet]
        public async Task<IActionResult> EditMyInfo()
        {
            var currentUser = await _userRepository.GetCurrentyUser();
            if (currentUser == null)
            {
                return NotFound();
            }


            EditUserViewModel model = new EditUserViewModel
            {
                UserId = currentUser.Id,
                Email = currentUser.Email,
                NameUser = currentUser.NameUser,
                SurnameUser = currentUser.SurnameUser,
                PhoneNumber = currentUser.PhoneNumber
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditMyInfo(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userRepository.GetCurrentyUser();
            if (user != null)
            {

                user.Email = model.Email;
                user.NameUser = model.NameUser;
                user.SurnameUser = model.SurnameUser;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userRepository.UpdateUserAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            return NotFound();
        }


       


    }
}