using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelManagementSystem.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    // =========================
    // LOGIN
    // =========================

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = new LoginDto
        {
            Email = model.Email,
            Password = model.Password,
            RememberMe = model.RememberMe
        };

        var result = await _accountService.LoginAsync(dto);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        var user = await _accountService.GetUserByEmailAsync(model.Email);

        if (user != null && await _accountService.IsAdminAsync(user))
            return RedirectToAction("Index", "AdminDashboard");

        return RedirectToAction("Index", "StudentDashboard");
    }

    // =========================
    // REGISTER
    // =========================

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        if (!ModelState.IsValid)
            return View(ToViewModel(model));

        try
        {
            var result = await _accountService.RegisterAsync(model);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(ToViewModel(model));
            }

            return View(
                "RegisterConfirmation",
                new ResendConfirmationViewModel
                {
                    Email = model.Email
                });
        }
        catch
        {
            ModelState.AddModelError(
                "",
                "We couldn't create your account. Please try again.");

            return View(ToViewModel(model));
        }
    }

    // =========================
    // EMAIL CONFIRMATION
    // =========================

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(code))
            return BadRequest();

        var success = await _accountService.ConfirmEmailAsync(userId, code);

        if (!success)
            return View("Error");

        return View("ConfirmEmail");
    }

  [HttpPost]
[AllowAnonymous]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ResendConfirmationEmail(
    ResendConfirmationViewModel model)
{
    if (!ModelState.IsValid)
        return View("RegisterConfirmation", model);

    try
    {
        await _accountService.SendConfirmationEmailAsync(model.Email);

        TempData["Success"] =
            "A new verification email has been sent.";

        return View("RegisterConfirmation", model);
    }
    catch
    {
        ModelState.AddModelError(
            "",
            "Unable to send verification email. Please check your internet connection and try again.");

        return View("RegisterConfirmation", model);
    }
}

    // =========================
    // FORGOT PASSWORD
    // =========================

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await _accountService.SendPasswordResetEmailAsync(model.Email);

        return RedirectToAction(nameof(ForgotPasswordConfirmation));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

   [HttpPost]
[AllowAnonymous]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ResendPasswordResetLink(ForgotPasswordDto model)
{
    if (string.IsNullOrWhiteSpace(model.Email))
    {
        TempData["Error"] = "Email address is missing.";
        return RedirectToAction(nameof(ForgotPassword));
    }

    await _accountService.SendPasswordResetEmailAsync(model.Email);

    TempData["Success"] = "A new password reset link has been sent.";

    return View("ForgotPasswordConfirmation", model);
}
    // =========================
    // RESET PASSWORD
    // =========================

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string email, string token)
    {
        return View(new ResetPasswordDto
        {
            Email = email,
            Token = token
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var success = await _accountService.ResetPasswordAsync(model);

        if (!success)
        {
            ModelState.AddModelError(
                "",
                "Unable to reset password.");

            return View(model);
        }

        TempData["Success"] =
            "Password reset successfully.";

        return RedirectToAction(nameof(Login));
    }

    // =========================
    // LOGOUT
    // =========================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _accountService.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }

    // =========================
    // HELPERS
    // =========================

    private static RegisterViewModel ToViewModel(RegisterDto dto)
    {
        return new RegisterViewModel
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Gender = dto.Gender,
            Department = dto.Department,
            Password = dto.Password,
            ConfirmPassword = dto.ConfirmPassword,
            ReturnUrl = dto.ReturnUrl
        };
    }
}