using Desafio.Umbler.Application.Services.Implementation;
using Desafio.Umbler.Domain.Dtos;
using Desafio.Umbler.Infrastructure.Adapters.Interface;
using Desafio.Umbler.Infrastructure.Data;
using Desafio.Umbler.Infrastructure.Repositories.Implementation;
using DnsClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using DomainInfra = Desafio.Umbler.Infrastructure.Data.Domain;

namespace Desafio.Umbler.Test.Application;

[TestClass]
public class DomainLookupServiceTests
{
    //testes Domain_Moking_LookupClient e Domain_Moking_WhoisClient unificados em um único teste
    //para evitar 2 testes identicos devido a implementação atual.
    [TestMethod]
    public async Task Domain_Moking_LookupClient_And_WhoisClient()
    {
        //arrange 
        var dnsMock = new Mock<IDnsClient>();
        var whoisMock = new Mock<IWhoisClient>();
        var domainName = "test.com";

        var dnsresult = new DnsResultDto { Ip = "192.168.0.1", Ttl = 60 };

        dnsMock.Setup(d => d.QueryAsync(domainName, QueryType.ANY))
            .ReturnsAsync(dnsresult);

        whoisMock.Setup(w => w.QueryAsync(dnsresult.Ip))
            .ReturnsAsync(new WhoisResultDto { Raw = "raw", OrganizationName = "umbler.corp" });

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: $"Find_searches_url_{Guid.NewGuid()}")
            .Options;

        // Use a clean instance of the context to run the test
        using (var db = new DatabaseContext(options))
        {
            var repository = new DomainRepository(db);
            var service = new DomainLookupService(repository, dnsMock.Object, whoisMock.Object);

            //act
            var result = await service.GetDomainAsync(domainName);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(domainName, result.Name);
            Assert.AreEqual("192.168.0.1", result.Ip);
            Assert.AreEqual("umbler.corp", result.HostedAt);

            dnsMock.Verify(d => d.QueryAsync(domainName, QueryType.ANY), Times.Once);
            whoisMock.Verify(w => w.QueryAsync(dnsresult.Ip), Times.Once);
        }
    }


    [TestMethod]
    public async Task Domain_With_Valid_Ttl_Does_Not_Call_Dns_Or_Whois()
    {
        // arrange
        var dnsMock = new Mock<IDnsClient>();
        var whoisMock = new Mock<IWhoisClient>();

        var domainName = "test.com";

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: $"Find_searches_url_{Guid.NewGuid()}")
            .Options;

        using (var db = new DatabaseContext(options))
        {
            // domínio já existente no banco com TTL válido
            db.Domains.Add(new DomainInfra
            {
                Id = 1,
                Ip = "192.168.0.1",
                Name = "test.com",
                UpdatedAt = DateTime.Now,
                HostedAt = "umbler.corp",
                Ttl = 300,
                WhoIs = "Ns.umbler.com"
            });

            await db.SaveChangesAsync();

            var repository = new DomainRepository(db);
            var service = new DomainLookupService(
                repository,
                dnsMock.Object,
                whoisMock.Object
            );

            // act
            var result = await service.GetDomainAsync(domainName);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(domainName, result.Name);
            Assert.AreEqual("192.168.0.1", result.Ip);
            Assert.AreEqual("umbler.corp", result.HostedAt);

            dnsMock.Verify(
                d => d.QueryAsync(It.IsAny<string>(), It.IsAny<QueryType>()),
                Times.Never
            );

            whoisMock.Verify(
                w => w.QueryAsync(It.IsAny<string>()),
                Times.Never
            );
        }
    }

    [TestMethod]
    public async Task Domain_With_Expired_Ttl_Requeries_And_Updates()
    {
        // arrange
        var dnsMock = new Mock<IDnsClient>();
        var whoisMock = new Mock<IWhoisClient>();

        var domainName = "test.com";

        var dnsResult = new DnsResultDto
        {
            Ip = "10.0.0.1",
            Ttl = 120
        };

        dnsMock
            .Setup(d => d.QueryAsync(domainName, QueryType.ANY))
            .ReturnsAsync(dnsResult);

        whoisMock
            .Setup(w => w.QueryAsync(dnsResult.Ip))
            .ReturnsAsync(new WhoisResultDto
            {
                Raw = "raw-new",
                OrganizationName = "umbler.updated"
            });

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: $"Find_searches_url_{Guid.NewGuid()}")
            .Options;

        using (var db = new DatabaseContext(options))
        {
            // domínio existente, porém com TTL expirado
            db.Domains.Add(new DomainInfra
            {
                Id = 1,
                Ip = "192.168.0.1",
                Name = "test.com",
                UpdatedAt = DateTime.Now,
                HostedAt = "umbler.corp",
                Ttl = 0,
                WhoIs = "Ns.umbler.com"
            });

            await db.SaveChangesAsync();

            var repository = new DomainRepository(db);
            var service = new DomainLookupService(
                repository,
                dnsMock.Object,
                whoisMock.Object
            );

            // act
            var result = await service.GetDomainAsync(domainName);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual("10.0.0.1", result.Ip);
            Assert.AreEqual("umbler.updated", result.HostedAt);

            dnsMock.Verify(
                d => d.QueryAsync(domainName, QueryType.ANY),
                Times.Once
            );

            whoisMock.Verify(
                w => w.QueryAsync(dnsResult.Ip),
                Times.Once
            );

            var entityInDb = db.Domains.First(d => d.Name == domainName);
            Assert.AreEqual("10.0.0.1", entityInDb.Ip);
            Assert.AreEqual("umbler.updated", entityInDb.HostedAt);
        }
    }
}
