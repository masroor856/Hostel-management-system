// Services/AccountService.cs

using System.Text;
using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace HostelManagementSystem.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICustomEmailSender _emailSender;

    public AccountService(
        IAccountRepository repository,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ICustomEmailSender emailSender)
    {
        _repository = repository;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    public async Task<SignInResult> LoginAsync(LoginDto dto)
    {
        return await _signInManager.PasswordSignInAsync(
            dto.Email,
            dto.Password,
            dto.RememberMe,
            false);
    }

    public async Task<IdentityUser?> GetUserByEmailAsync(
        string email)
    {
        return await _repository.GetUserByEmailAsync(email);
    }

    public async Task<bool> IsAdminAsync(
        IdentityUser user)
    {
        return await _userManager.IsInRoleAsync(
            user,
            "Admin");
    }

    public async Task<bool> HasStudentApplicationAsync(
        string email)
    {
        var student =
            await _repository.GetStudentByEmailAsync(email);

        return student != null &&
               student.HostelApplications.Any();
    }

    public async Task<IdentityResult> RegisterAsync(
        RegisterDto dto)
    {
        var user = new IdentityUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var result =
            await _userManager.CreateAsync(
                user,
                dto.Password);

        if (!result.Succeeded)
            return result;

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

        var code =
            await _userManager
                .GenerateEmailConfirmationTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(code));

        var callbackUrl =
            $"/Account/ConfirmEmail" +
            $"?userId={user.Id}" +
            $"&code={code}";

        var emailBody = $"""
            <h2>Confirm your Hostel Management account</h2>

            <p>
                Thank you for registering.
                Please confirm your email address.
            </p>

            <p>
                <a href="{callbackUrl}">
                    Confirm Email
                </a>
            </p>
            """;

        await _emailSender.SendEmailAsync(
            dto.Email,
            "Confirm your Hostel Management account",
            emailBody);

        return IdentityResult.Success;
    }

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
            await _userManager
                .GeneratePasswordResetTokenAsync(user);

        var encodedToken =
            WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(token));

        return encodedToken;
    }

    public async Task<bool> ResetPasswordAsync(
        ResetPasswordDto dto)
    {
        var user =
            await _repository.GetUserByEmailAsync(
                dto.Email);

        if (user == null)
            return false;

        var token =
            Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(
                    dto.Token));

        var result =
            await _userManager.ResetPasswordAsync(
                user,
                token,
                dto.Password);

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
            await _userManager
                .GenerateEmailConfirmationTokenAsync(user);

        return WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(token));
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}