using Desafio.Umbler.Domain.Dtos;
using Desafio.Umbler.Infrastructure.Adapters.Interface;
using Whois.NET;

namespace Desafio.Umbler.Infrastructure.Adapters.Implementation;

public class WhoisClientAdapter : IWhoisClient
{
    public async Task<WhoisResultDto> QueryAsync(string query)
    {
        return await WhoisClient.QueryAsync(query)
            .ContinueWith(t =>
            {
                var resp = t.Result;
                return new WhoisResultDto
                {
                    Raw = resp?.Raw ?? string.Empty,
                    OrganizationName = resp?.OrganizationName
                };
            });
    }
}