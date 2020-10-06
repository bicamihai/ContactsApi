using System;
using ContactsApi.Data;
using ContactsApi.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContactsApi.Tests
{
    public class DatabasePreparation
    {
        public static ApplicationDbContext PrepareApplicationDbContext(string loggedInUserId, string anotherUserId)
        {
            var applicationDbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                                       .UseInMemoryDatabase(databaseName: "ContactDatabase")
                                       .Options;
            var applicationDbContext = new ApplicationDbContext(applicationDbOptions);
            applicationDbContext.Users.Add(new IdentityUser()
            {
                Id = loggedInUserId
            });
            applicationDbContext.Users.Add(new IdentityUser()
            {
                Id = anotherUserId,
            });
            applicationDbContext.SaveChanges();
            return applicationDbContext;
        }

        public static ContactContext PrepareContactContext(string loggedInUserId, string anotherUserId, int contactOfLoggedInUserId, int contactOfAnotherUserId)
        {
            var contactOptions = new DbContextOptionsBuilder<ContactContext>()
                                 .UseInMemoryDatabase(databaseName: "ContactDatabase")
                                 .Options;

            var contactContext = new ContactContext(contactOptions);
            contactContext.Contacts.Add(new Contact
            {
                Id = contactOfLoggedInUserId,
                FirstName = "Mihai",
                LastName = "Bica",
                Address = "B-dul N.Titulescu",
                Email = "mihai@yahoo.com",
                MobilePhoneNumber = "+40743123456",
                UserId = loggedInUserId
            });
            contactContext.Contacts.Add(new Contact
            {
                Id = contactOfAnotherUserId,
                FirstName = "Madalina",
                LastName = "Bica",
                Address = "Paris",
                Email = "mihai@yahoo.com",
                MobilePhoneNumber = "+40743654321",
                UserId = anotherUserId
            });
            contactContext.SaveChanges();


            return contactContext;
        }
    }
}
