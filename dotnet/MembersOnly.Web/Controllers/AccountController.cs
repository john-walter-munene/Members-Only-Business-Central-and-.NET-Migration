using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MembersOnly.Web.Models;
using MembersOnly.Web.Services;

namespace MembersOnly.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly IBusinessCentralClient _businessCentral;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> users,
        SignInManager<ApplicationUser> signIn,
        IBusinessCentralClient businessCentral,
        ILogger<AccountController> logger)
    {
        _users = users;
        _signIn = signIn;
        _businessCentral = businessCentral;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        // Return the form with errors, like your Express validation step.
        if (!ModelState.IsValid) return View(model);

        // Create the login account first. Identity hashes the password.
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim()
        };

        var result = await _users.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            // Show Identity's errors, such as weak passwords or duplicate emails.
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        try
        {
            // Create the matching Business Central member, which starts as Pending.
            var memberNo = await _businessCentral.RegisterMemberAsync(user.Id, user.Email!, user.FirstName, user.LastName);

            // Save the link back to the Business Central record.
            user.ClubMemberNo = memberNo;
            await _users.UpdateAsync(user);
        }
        catch (Exception ex)
        {
            // Compensate: remove the Identity user so the two systems stay consistent.
            _logger.LogError(ex, "Business Central registration failed for {Email}", user.Email);
            await _users.DeleteAsync(user);

            ModelState.AddModelError(string.Empty, "We could not complete your registration. Please try again.");
            return View(model);
        }

        // Send the user to sign in. Registration never grants membership.
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // Check the password against Identity's stored hash.
        var result = await _signIn.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Incorrect email or password.");
            return View(model);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}