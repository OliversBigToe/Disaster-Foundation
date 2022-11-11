using APPR6312_Assignment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace APPRUnitTest
{
    public class UnitTest1
    {
        private AppDbContext _context;

        public UnitTest1()
        {

            var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task Users()
        {
            var users = await _context.Users.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<User>>(users);
        }

        [Fact]
        public async Task Disasters()
        {
            var disasters = await _context.Disaster.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<Disasters>>(disasters);
        }

        [Fact]
        public async Task Goods()
        {
            var goods = await _context.Goods.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<Good>>(goods);
        }

        [Fact]
        public async Task Money()
        {
            var money = await _context.Money.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<Cash>>(money);
        }

        [Fact]
        public async Task Transactions()
        {
            var trans = await _context.Transactions.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<Transaction>>(trans);
        }

        [Fact]
        public async Task Inventory()
        {
            var inv = await _context.Inventory.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<Inventories>>(inv);
        }
    }
}