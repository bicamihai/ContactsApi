﻿using System.Collections.Generic;

namespace ContactsApi.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string UserId { get; set; }
        public virtual ICollection<ContactSkill> ContactSkills { get; set; }
    }
}
