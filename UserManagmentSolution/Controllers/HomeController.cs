using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UserManagmentSolution.Models;

namespace UserManagmentSolution.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard() => View();

        [Authorize(Roles = "User,Admin")]
        public IActionResult UserDashboard() => View();
    }
}
