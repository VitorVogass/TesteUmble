using Desafio.Umbler.Domain.Dtos;

namespace Desafio.Umbler.Application.Services.Interface;

public interface IDomainLookupService
{
    Task<DomainDto> GetDomainAsync(string domainName);
}
