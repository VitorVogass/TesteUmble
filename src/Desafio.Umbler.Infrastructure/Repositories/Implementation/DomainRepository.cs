using Desafio.Umbler.Infrastructure.Data;
using Desafio.Umbler.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Umbler.Infrastructure.Repositories.Implementation;

public class DomainRepository : IDomainRepository
{
    private readonly DatabaseContext _db;

    public DomainRepository(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<Domain.Entities.Domain?> GetByNameAsync(string name)
    {
        var domain = await _db.Domains
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Name == name);

        if (domain == null)
            return null;

        return new Domain.Entities.Domain(
            name: domain.Name,
            ip: domain.Ip,
            whoIs: domain.WhoIs,
            domain.UpdatedAt,
            domain.Ttl,
            domain.HostedAt
        );
    }

    public async Task UpsertAsync(Domain.Entities.Domain domainEntity)
    {
        var domain = await _db.Domains
            .FirstOrDefaultAsync(d => d.Name == domainEntity.Name);

        if (domain is null)
        {
            _db.Domains.Add(new Data.Domain
            {
                Name =  domainEntity.Name,
                Ip = domainEntity.Ip,
                UpdatedAt = domainEntity.UpdatedAt,
                WhoIs = domainEntity.WhoIs,
                Ttl = domainEntity.Ttl,
                HostedAt = domainEntity.HostedAt
            });
        }
        else
        {
            domain.Ip = domainEntity.Ip;
            domain.UpdatedAt = domainEntity.UpdatedAt;
            domain.WhoIs = domainEntity.WhoIs;
            domain.Ttl = domainEntity.Ttl;
            domain.HostedAt = domainEntity.HostedAt;
        }

        await _db.SaveChangesAsync();
    }
}
