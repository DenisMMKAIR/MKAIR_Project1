namespace WebAPI.Controllers.Requests;

public record GetListRequest(int Skip = 0, int Take = 0);