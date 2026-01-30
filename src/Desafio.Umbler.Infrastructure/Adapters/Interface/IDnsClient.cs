using Desafio.Umbler.Domain.Dtos;
using DnsClient;

namespace Desafio.Umbler.Infrastructure.Adapters.Interface;

public interface IDnsClient
{
    Task<DnsResultDto> QueryAsync(string name, QueryType queryType);
}
