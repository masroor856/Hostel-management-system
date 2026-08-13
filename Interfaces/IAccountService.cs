// Interfaces/IAccountService.cs

using HostelManagementSystem.DTOs;
using Microsoft.AspNetCore.Identity;

namespace HostelManagementSystem.Interfaces;

public interface IAccountService
{
    Task<SignInResult> LoginAsync(LoginDto dto);

    Task<IdentityUser?> GetUserByEmailAsync(string email);

    Task<bool> IsAdminAsync(IdentityUser user);

    Task<bool> HasStudentApplicationAsync(string email);

    Task<IdentityResult> RegisterAsync(RegisterDto dto);

    Task<bool> ConfirmEmailAsync(
        string userId,
        string code);

    Task<string?> GeneratePasswordResetTokenAsync(
        string email);

    Task<bool> ResetPasswordAsync(
        ResetPasswordDto dto);

    Task<string?> GenerateConfirmationTokenAsync(
        string email);

    Task SignOutAsync();
}