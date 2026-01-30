using Desafio.Umbler.Infrastructure.Data;
using Desafio.Umbler.Infrastructure.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using DomainInfra = Desafio.Umbler.Infrastructure.Data.Domain;

namespace Desafio.Umbler.Test.Infrastructure;

[TestClass]
public class DomainRepositoryTests
{
    [TestMethod]
    public async Task Domain_Not_In_Database()
    {
        //arrange 
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: $"Find_searches_url_{Guid.NewGuid()}")
            .Options;

        // Use a clean instance of the context to run the test
        using (var db = new DatabaseContext(options))
        {
            var domainRepository = new DomainRepository(db);

            //act
            var response = await domainRepository.GetByNameAsync("test.com");

            //assert
            Assert.IsNull(response);
        }
    }


    [TestMethod]
    public async Task Domain_In_Database()
    {
        //arrange 
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: $"Find_searches_url_{Guid.NewGuid()}")
            .Options;

        var domain = new DomainInfra
        {
            Id = 1,
            Ip = "192.168.0.1",
            Name = "test.com",
            UpdatedAt = DateTime.Now,
            HostedAt = "umbler.corp",
            Ttl = 60,
            WhoIs = "Ns.umbler.com"
        };

        // Insert seed data into the database using one instance of the context
        using (var db = new DatabaseContext(options))
        {
            db.Domains.Add(domain);
            db.SaveChanges();
        }

        // Use a clean instance of the context to run the test
        using (var db = new DatabaseContext(options))
        {
            var domainRepository = new DomainRepository(db);

            //act
            var response = await domainRepository.GetByNameAsync("test.com");

            //assert
            Assert.AreEqual(response.Ip, domain.Ip);
            Assert.AreEqual(response.Name, domain.Name);
        }
    }
}
