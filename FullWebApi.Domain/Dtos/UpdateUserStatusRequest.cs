using FullWebApi.Domain.Enums;

namespace FullWebApi.Domain.Dtos;

public class UpdateUserStatusRequest
{
    public UserStatus Status { get; set; }
    public string? Reason { get; set; }
}
