using Desafio.Umbler.Application.Services.Interface;
using Desafio.Umbler.Domain.Dtos;
using Desafio.Umbler.Infrastructure.Adapters.Interface;
using Desafio.Umbler.Infrastructure.Repositories.Interface;
using DnsClient;

namespace Desafio.Umbler.Application.Services.Implementation;

public class DomainLookupService : IDomainLookupService
{
    private readonly IDomainRepository _domainRepository;
    private readonly IDnsClient _dnsClient;
    private readonly IWhoisClient _whoisClient;

    public DomainLookupService(
        IDomainRepository domainRepository,
        IDnsClient dnsClient,
        IWhoisClient whoisClient)
    {
        _domainRepository = domainRepository;
        _dnsClient = dnsClient;
        _whoisClient = whoisClient;
    }

    public async Task<DomainDto> GetDomainAsync(string domainName)
    {
        var domainEntity = await _domainRepository.GetByNameAsync(domainName);

        if (domainEntity == null 
            || domainEntity.IsTtlExpired())
        {
            var dnsResult = await _dnsClient.QueryAsync(domainName, QueryType.ANY);
            var whoisResult = await _whoisClient.QueryAsync(dnsResult.Ip ?? domainName);

            domainEntity = new Domain.Entities.Domain
            (
                name: domainName,
                ip: dnsResult.Ip ?? string.Empty,
                whoIs: whoisResult.Raw,
                ttl: dnsResult.Ttl,                
                hostedAt: whoisResult.OrganizationName ?? string.Empty
            );

            await _domainRepository.UpsertAsync(domainEntity);
        }

        return new DomainDto
        {
            Name = domainEntity.Name,
            Ip = domainEntity.Ip,
            Whois = domainEntity.WhoIs,
            HostedAt = domainEntity.HostedAt
        };
    }
}

