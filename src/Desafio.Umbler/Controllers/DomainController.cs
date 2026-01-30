using Desafio.Umbler.Application.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Desafio.Umbler.Controllers
{
    [Route("api")]
    public class DomainController : Controller
    {
        private readonly IDomainLookupService _domainLookupService;

        public DomainController(IDomainLookupService domainLookupService)
        {
            _domainLookupService = domainLookupService;
        }

        [HttpGet, Route("domain/{domainName}")]
        public async Task<IActionResult> Get(string domainName)
        {
            if (string.IsNullOrWhiteSpace(domainName) ||
                !domainName.Contains('.'))
            {
                return BadRequest("Domínio inválido. Exemplo: exemplo.com");
            }

            var domainDto = await _domainLookupService.GetDomainAsync(domainName);

            return Ok(domainDto);
        }
    }
}
