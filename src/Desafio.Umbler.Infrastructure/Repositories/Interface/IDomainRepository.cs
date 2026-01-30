namespace Desafio.Umbler.Infrastructure.Repositories.Interface;

public interface IDomainRepository
{
    Task<Domain.Entities.Domain?> GetByNameAsync(string name);
    Task UpsertAsync(Domain.Entities.Domain domainEntity);
}
