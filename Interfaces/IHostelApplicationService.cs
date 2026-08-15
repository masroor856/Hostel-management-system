// Interfaces/IHostelApplicationService.cs

using HostelManagementSystem.DTOs;

namespace HostelManagementSystem.Interfaces
{
    public interface IHostelApplicationService
    {
        Task<IEnumerable<HostelApplicationDto>> SearchAsync(
            string? search,
            string? status);

        Task<HostelApplicationDto?> GetByIdAsync(int id);

        Task<bool> CreateAsync(HostelApplicationDto dto);

        Task<bool> UpdateAsync(HostelApplicationDto dto);

        Task<bool> DeleteAsync(int id);

        Task<bool> UpdateStatusAsync(
            int id,
            string status);

        Task<StudentDto?> GetStudentByEmail(
            string email);

        Task<bool> HasExistingApplicationAsync(
            int studentId);

      Task<IEnumerable<HostelRoomDto>> GetAvailableRoomsAsync(string email);

        Task<IEnumerable<HostelRoomDto>>
            GetRoomsForStudentAsync(int studentId);
    }
}