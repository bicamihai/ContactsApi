namespace ContactsApi.Models
{
    public class ContactModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string MobilePhoneNumber { get; set; }
    }
}
