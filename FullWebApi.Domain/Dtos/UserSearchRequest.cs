using FullWebApi.Domain.Enums;

namespace FullWebApi.Domain.Dtos;

public class UserSearchRequest
{
    public string? SearchTerm { get; set; }
    public UserStatus? Status { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
