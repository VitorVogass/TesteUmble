using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DomainEntity = Desafio.Umbler.Domain.Entities.Domain;


namespace Desafio.Umbler.Test.Domain;

[TestClass]
public class DomainEntityTests
{
    [TestMethod]
    public void IsTtlExpired_When_Not_Expired_Returns_False()
    {
        // arrange
        var domain = new DomainEntity(
            name: "test.com",
            ip: "192.168.0.1",
            ttl: 60,
            updatedAt: DateTime.Now.AddMinutes(-10)
        );

        // act
        var expired = domain.IsTtlExpired();

        // assert
        Assert.IsFalse(expired);
    }

    [TestMethod]
    public void IsTtlExpired_When_Expired_Returns_True()
    {
        // arrange
        var domain = new DomainEntity(
            name: "test.com",
            ip: "192.168.0.1",
            ttl: 10,
            updatedAt: DateTime.Now.AddMinutes(-30)
        );

        // act
        var expired = domain.IsTtlExpired();

        // assert
        Assert.IsTrue(expired);
    }
}
