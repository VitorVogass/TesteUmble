using Desafio.Umbler.Application.Services.Interface;
using Desafio.Umbler.Controllers;
using Desafio.Umbler.Domain.Dtos;
using Desafio.Umbler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Desafio.Umbler.Test.Controller;

[TestClass]
public class ControllersTest
{
    [TestMethod]
    public void Home_Index_returns_View()
    {
        //arrange 
        var controller = new HomeController();

        //act
        var response = controller.Index();
        var result = response as ViewResult;

        //assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Home_Error_returns_View_With_Model()
    {
        //arrange 
        var controller = new HomeController();
        controller.ControllerContext = new ControllerContext();
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        //act
        var response = controller.Error();
        var result = response as ViewResult;
        var model = result.Model as ErrorViewModel;

        //assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(model);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("exemplo")]
    [DataRow("dominioinvalido")]
    public async Task DomainController_With_Invalid_Domain_Returns_BadRequest(string domainName)
    {
        // arrange
        var serviceMock = new Mock<IDomainLookupService>();
        var controller = new DomainController(serviceMock.Object);

        // act
        var response = await controller.Get(domainName);

        // assert
        Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task DomainController_With_Valid_Domain_Returns_Ok()
    {
        // arrange
        var domainName = "test.com";
        var serviceMock = new Mock<IDomainLookupService>();

        serviceMock
            .Setup(s => s.GetDomainAsync(domainName))
            .ReturnsAsync(new DomainDto { Ip = "192.168.0.1", Name = "test.com", HostedAt = "umbler.corp", Whois = "Ns.umbler.com" });

        var controller = new DomainController(serviceMock.Object);

        // act
        var response = await controller.Get(domainName);

        // assert
        Assert.IsInstanceOfType(response, typeof(OkObjectResult));
    }
}