
using System.Text;
using HostelManagementSystem.Data;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using HostelManagementSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ICustomEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context,
            ICustomEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
        }

        // LOGIN

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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user == null)
                        return RedirectToLocal(model.ReturnUrl);

                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                        return RedirectToAction("Index", "AdminDashboard");

                    var student = await _context.Students
                        .Include(s => s.HostelApplications)
                        .FirstOrDefaultAsync(s => s.Email == model.Email);

                    if (student == null || !student.HostelApplications.Any())
                        return RedirectToAction("Apply", "HostelApplication");

                    return RedirectToAction("Index", "StudentDashboard");
                }

                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(
                        "",
                        "Please verify your email before logging in.");

                    return View(model);
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(
                        "",
                        "Your account is temporarily locked. Please try again later.");

                    return View(model);
                }

                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for {Email}", model.Email);

                ModelState.AddModelError(
                    "",
                    "We couldn't log you in right now. Please try again.");

                return View(model);
            }
        }

        // REGISTER

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            return View(new RegisterViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            IdentityUser? user = null;

            try
            {
                await using var transaction =
                    await _context.Database.BeginTransactionAsync();

                user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(
                    user,
                    model.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View(model);
                }

                var student = new Student
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Department = model.Department
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                var code =
                    await _userManager.GenerateEmailConfirmationTokenAsync(user);

                code = WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Action(
                    nameof(ConfirmEmail),
                    "Account",
                    new
                    {
                        userId = user.Id,
                        code
                    },
                    protocol: Request.Scheme);

                if (string.IsNullOrWhiteSpace(callbackUrl))
                    throw new InvalidOperationException(
                        "Could not generate the email confirmation URL.");

                await _emailSender.SendEmailAsync(
                    model.Email,
                    "Confirm your Hostel Management account",
                    $@"
<h2>Welcome to Hostel Management System</h2>
<p>Thank you for registering.</p>
<p>Please click the link below to verify your email.</p>
<p>
<a href='{callbackUrl}'
style='background:#0d6efd;padding:12px 25px;color:white;
text-decoration:none;border-radius:6px;font-weight:bold;'>
Verify Email
</a>
</p>
<p>If you didn't create this account, simply ignore this email.</p>");

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Registration and confirmation email completed for {Email}",
                    model.Email);

                return View(
                    "RegisterConfirmation",
                    new ResendConfirmationViewModel
                    {
                        Email = model.Email
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Registration or confirmation email failed for {Email}",
                    model.Email);

                // If registration failed after creating the Identity user,
                // remove the partially created records.
                if (user != null)
                {
                    try
                    {
                        var existing =
                            await _userManager.FindByIdAsync(user.Id);

                        if (existing != null)
                        {
                            var student = await _context.Students
                                .FirstOrDefaultAsync(
                                    s => s.Email == model.Email);

                            if (student != null)
                            {
                                _context.Students.Remove(student);
                                await _context.SaveChangesAsync();
                            }

                            var deleteResult =
                                await _userManager.DeleteAsync(existing);

                            if (!deleteResult.Succeeded)
                            {
                                _logger.LogError(
                                    "Could not clean up partially registered user {Email}: {Errors}",
                                    model.Email,
                                    string.Join(
                                        "; ",
                                        deleteResult.Errors.Select(
                                            e => e.Description)));
                            }
                        }
                    }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogError(
                            cleanupEx,
                            "Cleanup failed after registration error for {Email}",
                            model.Email);
                    }
                }

                ModelState.AddModelError(
                    "",
                    "We couldn't complete your registration or send the verification email. Please try again.");

                return View(model);
            }
        }

        // EMAIL CONFIRMATION

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(
            string? userId,
            string? code)
        {
            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(code))
            {
                ViewBag.StatusMessage =
                    "The verification link is invalid or incomplete.";

                return View();
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    ViewBag.StatusMessage =
                        "This account could not be found.";

                    return View();
                }

                var decodedCode = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(code));

                var result = await _userManager.ConfirmEmailAsync(
                    user,
                    decodedCode);

                ViewBag.StatusMessage = result.Succeeded
                    ? "Your email has been verified successfully. You can now log in."
                    : "Email verification failed. The link may have expired. Please request a new one.";

                if (!result.Succeeded)
                {
                    _logger.LogWarning(
                        "Email confirmation failed for {Email}: {Errors}",
                        user.Email,
                        string.Join(
                            "; ",
                            result.Errors.Select(e => e.Description)));
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Email confirmation failed for user ID {UserId}",
                    userId);

                ViewBag.StatusMessage =
                    "We couldn't verify your email. Please request a new verification link.";

                return View();
            }
        }

        // FORGOT PASSWORD

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            // Avoid revealing whether an account exists.
            if (user == null ||
                !await _userManager.IsEmailConfirmedAsync(user))
            {
                return View(
                    "ForgotPasswordConfirmation",
                    new ForgotPasswordViewModel
                    {
                        Email = model.Email
                    });
            }

            try
            {
                var token =
                    await _userManager.GeneratePasswordResetTokenAsync(user);

                token = WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(token));

                var callbackUrl = Url.Action(
                    nameof(ResetPassword),
                    "Account",
                    new
                    {
                        token,
                        email = user.Email
                    },
                    protocol: Request.Scheme);

                if (string.IsNullOrWhiteSpace(callbackUrl))
                    throw new InvalidOperationException(
                        "Could not generate the password reset URL.");

                await _emailSender.SendEmailAsync(
                    user.Email!,
                    "Reset your Hostel Management password",
                    $@"
<h2>Password Reset</h2>
<p>You requested to reset your password.</p>
<p><a href='{callbackUrl}'>Reset Password</a></p>
<p>If you didn't request this, simply ignore this email.</p>");

                return View(
                    "ForgotPasswordConfirmation",
                    new ForgotPasswordViewModel
                    {
                        Email = model.Email
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Password reset email failed for {Email}",
                    model.Email);

                ModelState.AddModelError(
                    "",
                    "We couldn't send the password reset email right now. Please try again later.");

                return View(model);
            }
        }

        // RESET PASSWORD

        [HttpGet]
        public IActionResult ResetPassword(
            string? token,
            string? email)
        {
            if (string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(email))
            {
                return RedirectToAction(nameof(Login));
            }

            return View(new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user =
                    await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                    return RedirectToAction(
                        nameof(ResetPasswordConfirmation));

                var decodedToken = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(model.Token));

                var result = await _userManager.ResetPasswordAsync(
                    user,
                    decodedToken,
                    model.Password);

                if (result.Succeeded)
                {
                    TempData["Success"] =
                        "Your password has been reset successfully.";

                    return RedirectToAction(
                        nameof(ResetPasswordConfirmation));
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Password reset failed for {Email}",
                    model.Email);

                ModelState.AddModelError(
                    "",
                    "We couldn't reset your password. Please request a new reset link.");

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // RESEND PASSWORD RESET LINK

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendPasswordResetLink(
            ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View("ForgotPassword", model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null ||
                !await _userManager.IsEmailConfirmedAsync(user))
            {
                return View(
                    "ForgotPasswordConfirmation",
                    new ForgotPasswordViewModel
                    {
                        Email = model.Email
                    });
            }

            try
            {
                var token =
                    await _userManager.GeneratePasswordResetTokenAsync(user);

                token = WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(token));

                var callbackUrl = Url.Action(
                    nameof(ResetPassword),
                    "Account",
                    new
                    {
                        token,
                        email = user.Email
                    },
                    protocol: Request.Scheme);

                if (string.IsNullOrWhiteSpace(callbackUrl))
                    throw new InvalidOperationException(
                        "Could not generate the password reset URL.");

                await _emailSender.SendEmailAsync(
                    user.Email!,
                    "Reset your Hostel Management password",
                    $@"
<h2>Password Reset</h2>
<p>You requested to reset your password.</p>
<p><a href='{callbackUrl}'>Reset Password</a></p>
<p>If you didn't request this, simply ignore this email.</p>");

                TempData["Success"] =
                    "A new password reset link has been sent.";

                return View(
                    "ForgotPasswordConfirmation",
                    new ForgotPasswordViewModel
                    {
                        Email = model.Email
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Resending password reset link failed for {Email}",
                    model.Email);

                ModelState.AddModelError(
                    "",
                    "We couldn't send the password reset email right now. Please try again later.");

                return View("ForgotPassword", model);
            }
        }

        // RESEND EMAIL CONFIRMATION

        [HttpGet]
        public IActionResult ResendConfirmationEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmationEmail(
            ResendConfirmationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user =
                await _userManager.FindByEmailAsync(model.Email);

            // Avoid revealing whether an account exists.
            if (user == null)
            {
                return View(
                    "ResendConfirmationEmailConfirmation",
                    new ResendConfirmationViewModel
                    {
                        Email = model.Email
                    });
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                TempData["Success"] =
                    "Your email has already been verified.";

                return RedirectToAction(nameof(Login));
            }

            try
            {
                var code =
                    await _userManager.GenerateEmailConfirmationTokenAsync(user);

                code = WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Action(
                    nameof(ConfirmEmail),
                    "Account",
                    new
                    {
                        userId = user.Id,
                        code
                    },
                    protocol: Request.Scheme);

                if (string.IsNullOrWhiteSpace(callbackUrl))
                    throw new InvalidOperationException(
                        "Could not generate the email confirmation URL.");

                await _emailSender.SendEmailAsync(
                    user.Email!,
                    "Confirm your Hostel Management account",
                    $@"
<h2>Email Verification</h2>
<p>Please click the link below to verify your email.</p>
<p><a href='{callbackUrl}'>Verify Email</a></p>
<p>If you did not create this account, you can safely ignore this email.</p>");

                TempData["Success"] =
                    "A new verification email has been sent successfully.";

                return View(
                    "ResendConfirmationEmailConfirmation",
                    new ResendConfirmationViewModel
                    {
                        Email = model.Email
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Resending confirmation email failed for {Email}",
                    model.Email);

                ModelState.AddModelError(
                    "",
                    "We couldn't send the verification email right now. Please try again later.");

                return View(model);
            }
        }

        // LOGOUT

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // LOCAL REDIRECT

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
