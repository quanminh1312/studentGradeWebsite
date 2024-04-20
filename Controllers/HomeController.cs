using doancoso.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace doancoso.Controllers
{
    public class HomeController : Controller
    {
		private readonly ILogger<HomeController> _logger;
		public const string AdminUsername = "admin";
		public const string AdminPassword = "1234";
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SIUDBContext _context;

		public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, SIUDBContext context)
		{
			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_context = context;
			CheckRole().Wait();
		}
		public async Task CheckRole()
		{
			if (!await _roleManager.RoleExistsAsync("Admin"))
			{
				var resultt = await _roleManager.CreateAsync(new IdentityRole("Admin"));
				if (!resultt.Succeeded)
				{
					await CheckRole();
				}
			}
			if (!await _roleManager.RoleExistsAsync("Student"))
			{
				var resultt = await _roleManager.CreateAsync(new IdentityRole("Student"));
				if (!resultt.Succeeded)
				{
					await CheckRole();
				}
			}
			if (!await _roleManager.RoleExistsAsync("Teacher"))
			{
				var resultt = await _roleManager.CreateAsync(new IdentityRole("Teacher"));
				if (!resultt.Succeeded)
				{
					await CheckRole();
				}
			}
		}
        public IActionResult Index()
        {
            return View();
        }
		public IActionResult LogOut()
		{
			_signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
		public IActionResult Login()
        {
            return View();
        }
		public IActionResult  AccessDenied()
		{
            return RedirectToAction("Login");
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(string username, string password)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByNameAsync(username);
				if (user != null && user.Pending == true)
				{
					Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, password, true, false);
					if (result.Succeeded)
					{
                        await _userManager.AddToRoleAsync(user, user.Role);
                        return RedirectToAction("Index", user.Role , new { area = user.Role });
					}
					ModelState.AddModelError("", "Invalid username or password.");
				}
			}
			return RedirectToAction("Index");
		}
		public IActionResult SignUp()
        {
            return View();
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SignUp(Account account)
		{
			if (ModelState.IsValid)
			{
				if (account.Username == AdminUsername && account.Password == AdminPassword)
				{
					var admin = await _userManager.FindByNameAsync(AdminUsername);
					if (admin == null)
					{
						admin = new User { UserName = AdminUsername, Pending= true, Role = "Admin" };
						var res = await _userManager.CreateAsync(admin, AdminPassword);
						if (!res.Succeeded)
						{
							ModelState.AddModelError("", "Error while creating user");
							return RedirectToAction("Index");
						}
					}
					account.Role = "Admin";
					account.Pending = true;
                    _context.Accounts.Add(account);
					_context.SaveChanges();
					return RedirectToAction("Index", "Home");
                }
				User user = new User
				{
					UserName = account.Username,
					Email = account.Email,
					Pending = false,
					Role = account.Role
				};
				IdentityResult result = await _userManager.CreateAsync(user, account.Password!);
				if (result.Succeeded)
				{
					_context.Accounts.Add(account);
					_context.SaveChanges();
					return RedirectToAction("Index", "Home");
				}
				else
				{
					foreach (IdentityError error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
				}
			}
			return View(account);
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
