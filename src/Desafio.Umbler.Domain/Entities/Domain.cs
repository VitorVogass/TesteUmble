using System.Net;
using System.Text.RegularExpressions;

namespace Desafio.Umbler.Domain.Entities;

public class Domain
{
    public string Name { get; }
    public string Ip { get; }
    public DateTime UpdatedAt { get; }
    public string WhoIs { get; }
    public int Ttl { get; }
    public string HostedAt { get; }

    public Domain(
        string name,
        string ip,
        string whoIs = null,
        DateTime? updatedAt = null,
        int ttl = 0,
        string hostedAt = null)
    {
        if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome do domínio é obrigatório.", nameof(name));

        var domainRegex = new Regex(@"^[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)+$");
        if (!domainRegex.IsMatch(name))
            throw new ArgumentException("Formato de domínio inválido.", nameof(name));

        if (ttl < 0)
            throw new ArgumentOutOfRangeException(nameof(ttl), "TTL deve ser maior ou igual a zero.");

        if (!string.IsNullOrWhiteSpace(ip) && !IPAddress.TryParse(ip, out _))
            throw new ArgumentException("Endereço IP inválido.", nameof(ip));

        Name = name;
        Ip = ip ?? string.Empty;
        UpdatedAt = updatedAt ?? DateTime.Now;
        WhoIs = whoIs ?? string.Empty;
        Ttl = ttl;
        HostedAt = hostedAt ?? string.Empty;
    }

    public bool IsTtlExpired()
    {
        return DateTime.Now.Subtract(UpdatedAt).TotalMinutes > Ttl;
    }
}
