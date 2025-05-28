namespace TrackNSave.Server.Models.DTOs
{
    public class GetUsersRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
