using Desafio.Umbler.Domain.Dtos;
using Desafio.Umbler.Infrastructure.Adapters.Interface;
using DnsClient;

namespace Desafio.Umbler.Infrastructure.Adapters.Implementation;

public class DnsClientAdapter : IDnsClient
{
    private readonly ILookupClient _lookupClient;

    public DnsClientAdapter(ILookupClient lookupClient)
    {
        _lookupClient = lookupClient;
    }

    public async Task<DnsResultDto> QueryAsync(string name, QueryType queryType)
    {
        var result = await _lookupClient.QueryAsync(name, queryType);
        var record = result.Answers.ARecords().FirstOrDefault();
        var aRecords = result.Answers.ARecords().Select(r => r.Address.ToString()).ToList();

        return new DnsResultDto
        {
            Ip = record?.Address?.ToString(),
            Ttl = record?.TimeToLive ?? 0,
        };
    }
}