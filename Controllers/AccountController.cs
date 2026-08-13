using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HostelManagementSystem.Models;
using HostelManagementSystem.ViewModels;

namespace HostelManagementSystem.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(
        IAccountService accountService)
    {
        _accountService = accountService;
    }

  
[HttpGet]
public IActionResult Login(string? returnUrl = null)
{
    return View(new LoginViewModel
    {
        ReturnUrl = returnUrl
    });
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(
    LoginViewModel model,
    string? returnUrl = null)
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

    if (result.Succeeded)
    {
        if (!string.IsNullOrEmpty(returnUrl) &&
            Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        if (result.Succeeded)
{
    var user = await _accountService.GetUserByEmailAsync(model.Email);

    if (user != null && await _accountService.IsAdminAsync(user))
    {
        return RedirectToAction("Index", "AdminDashboard");
    }

    return RedirectToAction("Index", "StudentDashboard");
}
    }

    ModelState.AddModelError(
        "",
        "Invalid email or password.");

    return View(model);
}

    [HttpGet]
    [AllowAnonymous]
    [HttpGet]
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
    {
        return View(new RegisterViewModel
        {
            FullName = model.FullName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Gender = model.Gender,
            Department = model.Department,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword,
            ReturnUrl = model.ReturnUrl
        });
    }

    try
    {
        var result =
            await _accountService.RegisterAsync(model);

       if (!result.Succeeded)
{
    foreach (var error in result.Errors)
    {
        ModelState.AddModelError(
            "",
            error.Description);
    }

    return View(new RegisterViewModel
    {
        FullName = model.FullName,
        Email = model.Email,
        PhoneNumber = model.PhoneNumber,
        Gender = model.Gender,
        Department = model.Department,
        Password = model.Password,
        ConfirmPassword = model.ConfirmPassword,
        ReturnUrl = model.ReturnUrl
    });
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

        return View(new RegisterViewModel
        {
            FullName = model.FullName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Gender = model.Gender,
            Department = model.Department,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword,
            ReturnUrl = model.ReturnUrl
        });
    }
}
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(
        string userId,
        string code)
    {
        if (string.IsNullOrEmpty(userId) ||
            string.IsNullOrEmpty(code))
        {
            return BadRequest();
        }

        var success =
            await _accountService.ConfirmEmailAsync(
                userId,
                code);

        if (!success)
            return View("Error");

        return View("ConfirmEmail");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var token =
            await _accountService
                .GeneratePasswordResetTokenAsync(
                    model.Email);

        if (token == null)
        {
            return RedirectToAction(
                nameof(ForgotPasswordConfirmation));
        }

        return RedirectToAction(
            nameof(ForgotPasswordConfirmation));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(
        string email,
        string token)
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
    public async Task<IActionResult> ResetPassword(
        ResetPasswordDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var success =
            await _accountService
                .ResetPasswordAsync(model);

        if (!success)
        {
            ModelState.AddModelError(
                "",
                "Unable to reset password.");

            return View(model);
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _accountService.SignOutAsync();

        return RedirectToAction(
            "Index",
            "Home");
    }

    private IActionResult RedirectToLocal(
        string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) &&
            Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(
            "Index",
            "Home")!;
    }
}