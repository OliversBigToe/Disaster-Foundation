using APPR6312_Assignment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MSUnitTesting
{
    [TestClass]
    public class MSUnitTest1
    {
        private IConfigurationRoot _config;

        private DbContextOptions<AppDbContext> AppContext;

        public MSUnitTest1()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _config = builder.Build();
            AppContext = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(_config.GetConnectionString("Online"))
                .Options;
        }

        //Test method for my Disasters table
        [TestMethod]
        public void Disaster()
        {
            using (var context = new AppDbContext(AppContext))
            {
                context.Database.EnsureCreated();
                var disaster = new Disasters()
                {
                    disasterName = "Unit Testing",
                    disasterStartDate = new DateTime(2022, 09, 11),
                    disasterEndDate = new DateTime(2022, 09, 11),
                    disasterLocation = "South Africa",
                    aidType = "Help",
                    disasterDescription = "I seriously need help",
                    allocatedMoney = 31314,
                    allocatedGoods = 1,
                    goodsCategory = "Help"
                };

                context.Disaster.AddRange(disaster);
                context.SaveChanges();
            }
        }

        //Test method for my Users table
        [TestMethod]
        public void Users()
        {
            using (var context = new AppDbContext(AppContext))
            {
                context.Database.EnsureCreated();
                var users = new User()
                {
                    userEmail = "stevenk@gmail.com",
                    userPassword = "fd75c7f5cd42026d2e4a6e6b49e8eb88",
                    userName = "Steven",
                    userSurname = "King",
                    userRole = "Admin"
                };

                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }
    }
}