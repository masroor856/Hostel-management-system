// Interfaces/IContactMessageService.cs

using HostelManagementSystem.DTOs;
using HostelManagementSystem.Models;

namespace HostelManagementSystem.Interfaces
{
    public interface IContactMessageService
    {
        Task SendAsync(ContactMessageDto dto);

        Task<IEnumerable<ContactMessage>>
            GetMessagesAsync(string? search);

        Task<ContactMessage?> GetByIdAsync(int id);

        Task<bool> MarkUnreadAsync(int id);

        Task<bool> MarkReadAsync(int id);

        Task<bool> DeleteAsync(int id);
    }
}