using System.Text;
using HostelManagementSystem.Data;
using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HostelManagementSystem.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICustomEmailSender _emailSender;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        IAccountRepository repository,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IHttpContextAccessor httpContextAccessor,
        ICustomEmailSender emailSender,
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<AccountService> logger)
    {
        _repository = repository;
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
        _emailSender = emailSender;
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    // ==========================
    // LOGIN
    // ==========================

    public async Task<SignInResult> LoginAsync(LoginDto dto)
    {
        return await _signInManager.PasswordSignInAsync(
            dto.Email,
            dto.Password,
            dto.RememberMe,
            false);
    }

    public async Task<IdentityUser?> GetUserByEmailAsync(string email)
    {
        return await _repository.GetUserByEmailAsync(email);
    }

    public async Task<bool> IsAdminAsync(IdentityUser user)
    {
        return await _userManager.IsInRoleAsync(user, "Admin");
    }

    public async Task<bool> HasStudentApplicationAsync(string email)
    {
        var student = await _repository.GetStudentByEmailAsync(email);

        return student != null &&
               student.HostelApplications.Any();
    }

    // ==========================
    // REGISTER (Transactional)
    // ==========================


public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
{
    Console.WriteLine("TEST: RegisterAsync method was reached.");
    using var transaction =
        await _context.Database.BeginTransactionAsync();

    IdentityUser? user = null;

    try
    {
        Console.WriteLine("REGISTER: Starting registration.");

        user = new IdentityUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var result =
            await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            Console.WriteLine("REGISTER: User creation failed.");

            await transaction.RollbackAsync();
            return result;
        }

        var student = new Student
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Gender = dto.Gender,
            Department = dto.Department
        };

        await _repository.AddStudentAsync(student);
        await _repository.SaveChangesAsync();

        Console.WriteLine(
            "REGISTER: Student saved. About to send confirmation email.");

        await SendConfirmationEmailAsync(dto.Email);

        Console.WriteLine(
            "REGISTER: Confirmation email request succeeded.");

        await transaction.CommitAsync();

        Console.WriteLine("REGISTER: Registration completed.");

        return IdentityResult.Success;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(
            $"REGISTER ERROR: {ex}");

        try
        {
            await transaction.RollbackAsync();
        }
        catch (Exception rollbackEx)
        {
            Console.Error.WriteLine(
                $"ROLLBACK ERROR: {rollbackEx.Message}");
        }

        if (user != null)
        {
            try
            {
                var existingUser =
                    await _userManager.FindByEmailAsync(dto.Email);

                if (existingUser != null)
                {
                    var deleteResult =
                        await _userManager.DeleteAsync(existingUser);

                    if (!deleteResult.Succeeded)
                    {
                        Console.Error.WriteLine(
                            "USER CLEANUP ERROR: " +
                            string.Join(
                                "; ",
                                deleteResult.Errors.Select(e => e.Description)));
                    }
                }
            }
            catch (Exception cleanupEx)
            {
                Console.Error.WriteLine(
                    $"USER CLEANUP ERROR: {cleanupEx.Message}");
            }
        }

        try
        {
            var existingStudent =
                await _repository.GetStudentByEmailAsync(dto.Email);

            if (existingStudent != null)
            {
                _context.Students.Remove(existingStudent);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception cleanupEx)
        {
            Console.Error.WriteLine(
                $"STUDENT CLEANUP ERROR: {cleanupEx.Message}");
        }

        return IdentityResult.Failed(
            new IdentityError
            {
                Description =
                    "Verification email could not be sent. Please try again."
            });
    }
}



    // ==========================
    // EMAIL CONFIRMATION
    // ==========================

    public async Task<bool> ConfirmEmailAsync(
        string userId,
        string code)
    {
        var user =
            await _repository.GetUserByIdAsync(userId);

        if (user == null)
            return false;

        var decodedCode =
            Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(code));

        var result =
            await _userManager.ConfirmEmailAsync(
                user,
                decodedCode);

        return result.Succeeded;
    }

    public async Task<string?> GenerateConfirmationTokenAsync(
        string email)
    {
        var user =
            await _repository.GetUserByEmailAsync(email);

        if (user == null)
            return null;

        if (await _userManager.IsEmailConfirmedAsync(user))
            return null;

        var token =
            await _userManager.GenerateEmailConfirmationTokenAsync(user);

        return WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(token));
    }

    public async Task SendConfirmationEmailAsync(string email)
    {
        var user =
            await _repository.GetUserByEmailAsync(email);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (await _userManager.IsEmailConfirmedAsync(user))
            return;

        var token =
            await _userManager.GenerateEmailConfirmationTokenAsync(user);

        token = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(token));

      var baseUrl = _configuration["APP_BASE_URL"];

if (string.IsNullOrWhiteSpace(baseUrl))
{
    throw new InvalidOperationException(
        "APP_BASE_URL is not configured.");
}

var callbackUrl =
    $"{baseUrl.TrimEnd('/')}/Account/ConfirmEmail" +
    $"?userId={Uri.EscapeDataString(user.Id)}" +
    $"&code={Uri.EscapeDataString(token)}";

        var body = $"""
            <h2>Confirm your Hostel Management account</h2>

            <p>Thank you for registering.</p>

            <p>Please click the button below to verify your email.</p>

            <p>
                <a href="{callbackUrl}">
                    Confirm Email
                </a>
            </p>
            """;

        await _emailSender.SendEmailAsync(
            email,
            "Confirm your Hostel Management account",
            body);
    }

    // ==========================
    // PASSWORD RESET
    // ==========================

    public async Task<string?> GeneratePasswordResetTokenAsync(
        string email)
    {
        var user =
            await _repository.GetUserByEmailAsync(email);

        if (user == null)
            return null;

        if (!await _userManager.IsEmailConfirmedAsync(user))
            return null;

        var token =
            await _userManager.GeneratePasswordResetTokenAsync(user);

        return WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(token));
    }

    public async Task SendPasswordResetEmailAsync(string email)
    {
        var token =
            await GeneratePasswordResetTokenAsync(email);

        if (token == null)
            return;

        var callbackUrl =
            $"http://localhost:5236/Account/ResetPassword" +
            $"?email={Uri.EscapeDataString(email)}" +
            $"&token={Uri.EscapeDataString(token)}";

        var body = $"""
            <h2>Reset your Hostel Management password</h2>

            <p>Click the button below to reset your password.</p>

            <p>
                <a href="{callbackUrl}">
                    Reset Password
                </a>
            </p>
            """;

        await _emailSender.SendEmailAsync(
            email,
            "Reset your Hostel Management password",
            body);
    }

    public async Task<bool> ResetPasswordAsync(
        ResetPasswordDto dto)
    {
        var user =
            await _repository.GetUserByEmailAsync(dto.Email);

        if (user == null)
            return false;

        var decodedToken =
            Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(dto.Token));

        var result =
            await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                dto.Password);

        return result.Succeeded;
    }

    // ==========================
    // SIGN OUT
    // ==========================

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}