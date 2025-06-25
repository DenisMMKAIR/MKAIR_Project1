namespace WebAPI.Controllers.Requests;

public record GetPaginatedRequest(int PageIndex = 1, int PageSize = 10);
