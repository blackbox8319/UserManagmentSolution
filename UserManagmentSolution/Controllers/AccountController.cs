using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagmentSolution.Models;

namespace UserManagmentSolution.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // ------------------------------
        // 🔹 Normal Registration
        // ------------------------------
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("", "⚠️ Email is already registered.");
                return View(model);
            }
            if (await _userManager.FindByNameAsync(model.Username) != null)
            {
                ModelState.AddModelError("", "⚠️ Username already taken.");
                return View(model);
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToRoleDashboard(model.Role);
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }
        // ---------------- NORMAL LOGIN ----------------
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                var roles = await _userManager.GetRolesAsync(user);
                return RedirectToRoleDashboard(roles.FirstOrDefault());
            }

            ModelState.AddModelError("", "⚠️ Invalid username or password.");
            return View(model);
        }

        // ---------------- GOOGLE SSO ----------------
        [HttpPost]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return RedirectToAction("Login");

            ApplicationUser user;

            // Try to sign in existing external login
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);

            if (result.Succeeded)
            {
                user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            }
            else
            {
                // Get email from Google claims
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    // Get full name from Google safely
                    var fullName = info.Principal.Identity.Name ?? "";

                    // Split and assign first and last names using LINQ
                    var nameParts = fullName
                        .Trim()
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .ToArray();

                    var firstName = nameParts.FirstOrDefault() ?? "";
                    var lastName = nameParts.Length > 1 ? nameParts.Last() : "";

                    // Create new user
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                        return RedirectToAction("Login");

                    // Assign default role "User"
                    if (!await _roleManager.RoleExistsAsync("User"))
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    await _userManager.AddToRoleAsync(user, "User");
                }

                // Link Google login if not already linked
                var logins = await _userManager.GetLoginsAsync(user);
                if (!logins.Any(x => x.LoginProvider == info.LoginProvider && x.ProviderKey == info.ProviderKey))
                    await _userManager.AddLoginAsync(user, info);
            }

            // Sign in user
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Redirect based on role
            var roles = await _userManager.GetRolesAsync(user);
            return RedirectToRoleDashboard(roles.FirstOrDefault());
        }

        // ---------------- LOGOUT ----------------
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // ---------------- Role-based redirect ----------------
        private IActionResult RedirectToRoleDashboard(string? role)
        {
            if (role == "Admin") return RedirectToAction("Index", "Admin");
            return RedirectToAction("Index", "User");
        }
    }
}
