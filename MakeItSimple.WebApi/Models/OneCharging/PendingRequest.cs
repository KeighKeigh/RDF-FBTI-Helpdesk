namespace MakeItSimple.WebApi.Models.OneCharging
{
    public class PendingRequest : BaseIdEntity
    {
        public string IdPrefix { get; set; } = string.Empty;
        public string IdNo { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
    }
}
