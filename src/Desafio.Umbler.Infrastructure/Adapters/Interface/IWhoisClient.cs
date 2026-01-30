using Desafio.Umbler.Domain.Dtos;

namespace Desafio.Umbler.Infrastructure.Adapters.Interface;

public interface IWhoisClient
{
    Task<WhoisResultDto> QueryAsync(string query);
}
