using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FullWebApi.Application.Interfaces;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Enums;

namespace FullWebApi.Api.EndPoints;

public class GetAllUsersRequest
{
  [QueryParam] public string? SearchTerm { get; set; }
  [QueryParam] public UserStatus? Status { get; set; }
  [QueryParam] public DateTime? CreatedAfter { get; set; }
  [QueryParam] public DateTime? CreatedBefore { get; set; }
  [QueryParam] public string SortBy { get; set; } = "CreatedAt";
  [QueryParam] public string SortOrder { get; set; } = "desc";
  [QueryParam] public int Page { get; set; } = 1;
  [QueryParam] public int PageSize { get; set; } = 10;
}

public class GetAllUsers : Endpoint<GetAllUsersRequest, PaginatedResponse<UserDto>>
{
  private readonly IUserServices _userServices;
  
  public GetAllUsers(IUserServices userServices)
  {
    _userServices = userServices;
  }

  public override void Configure()
  {
    Get("/api/user/users");
    AllowAnonymous();
    Tags("Users");
    Summary(s =>
    {
      s.Summary = "Get all users with pagination and filtering";
      s.Description = "Returns a paginated list of users. Supports filtering by searchTerm, status (0=PendingVerification, 1=Active, 2=Inactive, 3=Suspended), createdAfter, createdBefore. Supports sorting by username, email, status, or createdAt. Use sortOrder=asc or desc.";
    });
  }

  public override async Task HandleAsync(GetAllUsersRequest req, CancellationToken ct)
  {
    // Validate pagination parameters
    if (req.Page < 1) req.Page = 1;
    if (req.PageSize < 1) req.PageSize = 10;
    if (req.PageSize > 100) req.PageSize = 100; // Max page size limit

    var searchRequest = new UserSearchRequest
    {
      SearchTerm = req.SearchTerm,
      Status = req.Status,
      CreatedAfter = req.CreatedAfter,
      CreatedBefore = req.CreatedBefore,
      SortBy = req.SortBy,
      SortOrder = req.SortOrder,
      Page = req.Page,
      PageSize = req.PageSize
    };

    var result = await _userServices.GetUsersAsync(searchRequest);
    
    await Send.OkAsync(result, ct);
  }
}
