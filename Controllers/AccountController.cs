using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TechnoShop.Models;

namespace TechnoShop.Controllers
{
  public class AccountController : Controller
  {
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;

    public AccountController(UserManager<IdentityUser> userMgr, SignInManager<IdentityUser> signInMgr)
    {
      userManager = userMgr;
      signInManager = signInMgr;
    }

    public IActionResult Login(string returnUrl = "")
    {
      Login login = new Login
      {
        ReturnUrl = returnUrl
      };
      return View(login);
    }

    [HttpPost]
    public async Task<IActionResult> Login(Login login)
    {
      if (ModelState.IsValid)
      {
        IdentityUser? user = await userManager.FindByNameAsync(login.Name ?? "");
        if (user == null)
        {
          user = await userManager.FindByEmailAsync(login.Name ?? "");
        }
        if (user != null)
        {
          await signInManager.SignOutAsync();
          if ((await signInManager.PasswordSignInAsync(user, login.Password ?? "", false, false)).Succeeded)
          {
            return Redirect(login?.ReturnUrl ?? "/");
          }
        }
        ModelState.AddModelError(nameof(login.Name), "Tên đăng nhập hoặc mật khẩu không chính xác");
      }
      return View(login);
    }

    public IActionResult Register()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(User user)
    {
      if (ModelState.IsValid)
      {
        IdentityUser identityUser = new IdentityUser
        {
          UserName = user.Name,
          Email = user.Email,
          EmailConfirmed = true
        };
        IdentityResult result = await userManager.CreateAsync(identityUser, user.Password ?? "");
        if (result.Succeeded)
        {
          await signInManager.SignInAsync(identityUser, isPersistent: false);
          return Redirect("/");
        }
        else
        {
          foreach (IdentityError error in result.Errors)
          {
            ModelState.AddModelError("", error.Description);
          }
        }
      }
      return View(user);
    }

    public async Task<IActionResult> Logout()
    {
      await signInManager.SignOutAsync();
      return RedirectToAction("Index", "Home");
    }
  }
}
