// Services/ContactMessageService.cs

using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;

namespace HostelManagementSystem.Services
{
    public class ContactMessageService
        : IContactMessageService
    {
        private readonly IContactMessageRepository
            _repository;

        public ContactMessageService(
            IContactMessageRepository repository)
        {
            _repository = repository;
        }

        public async Task SendAsync(
            ContactMessageDto dto)
        {
            var message = new ContactMessage
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Message = dto.Message,
                DateSent = DateTime.Now,
                IsRead = false
            };

            await _repository.AddAsync(message);
            await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ContactMessage>>
            GetMessagesAsync(string? search)
        {
            return await _repository
                .GetAllAsync(search);
        }

        public async Task<ContactMessage?>
            GetByIdAsync(int id)
        {
            return await _repository
                .GetByIdAsync(id);
        }

        public async Task<bool> MarkUnreadAsync(int id)
        {
            var message =
                await _repository.GetByIdAsync(id);

            if (message == null)
                return false;

            message.IsRead = false;

            await _repository.SaveChangesAsync();

            return true;
        }
        public async Task<bool> MarkReadAsync(int id)
        {
            var message =
            await _repository.GetByIdAsync(id);

            if (message == null)
            return false;

            message.IsRead = true;

            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var message =
                await _repository.GetByIdAsync(id);

            if (message == null)
                return false;

            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}