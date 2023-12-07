using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oro_Smart.Data;
using Oro_Smart.Data.ViewModels;
using Oro_Smart.Models;

namespace Oro_Smart.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

       
        public IActionResult Login() => View(new LoginVM());

        [HttpPost]
        public async Task<IActionResult> Login([Bind(nameof(LoginVM.EmailAddress), nameof(LoginVM.Password))] LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);

            var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck)
                {
                  
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                TempData["Error"] = "Wrong credentials. Please, try again";
                return View(loginVM);
            }

            TempData["Error"] = "Wrong credentials. Please, try again";
            return View(loginVM);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Settings()
        {
            // Retrieve the current user
            var user = _userManager.GetUserAsync(User).Result;

            // Map user data to the view model
            var viewModel = new SettingsViewModel
            {
                FullName = user.FullName,
                Email = user.Email
            };

            return View(viewModel);
        }

        /*[HttpPost]
        public async Task<IActionResult> Settings(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the current user
                var user = await _userManager.GetUserAsync(User);

                // Update user data
                user.FullName = model.FullName;
                user.Email = model.Email;

                // Update password if provided
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    var result = await _userManager.ChangePasswordAsync(user, null, model.NewPassword);

                    if (!result.Succeeded)
                    {
                        // Handle password change failure (e.g., invalid password)
                        ModelState.AddModelError(string.Empty, "Password change failed.");
                        return View(model);
                    }
                }

                // Update user in the database
                await _userManager.UpdateAsync(user);

                // Redirect to a success page or refresh the settings page
                return RedirectToAction("Settings", new { success = true });
            }

            // If here, there are validation errors, return the view with errors
            return View(model);
        }*/

        [HttpPost]
        public async Task<IActionResult> Settings(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);

                    // Update user data
                    user.FullName = model.FullName;
                    user.Email = model.Email;

                    // Update password if provided
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        // Remove the existing password (if any)
                        var removePasswordResult = await _userManager.RemovePasswordAsync(user);

                        if (!removePasswordResult.Succeeded)
                        {
                            foreach (var error in removePasswordResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(model);
                        }

                        // Add the new password
                        var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);

                        if (!addPasswordResult.Succeeded)
                        {
                            foreach (var error in addPasswordResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(model);
                        }
                    }

                    // Update user in the database
                    var updateResult = await _userManager.UpdateAsync(user);

                    if (!updateResult.Succeeded)
                    {
                        foreach (var error in updateResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }

                    TempData["Success"] = "Settings updated successfully.";
                    return RedirectToAction("Settings");
                }
                catch (Exception ex)
                {
                    // Log the exception and handle it appropriately
                  
                    ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                    return View(model);
                }
            }

            // If here, there are validation errors, return the view with errors
            return View(model);
        }





    }

}
