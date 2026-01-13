namespace Sklep_internetowy.Models
{
    public class Users
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
