using System.Text.Json.Serialization;

namespace CoreBanking.Infrastructure.Entity;
public class Account
{
    public Guid Id { get; set; }
    public string Number { get; set; } = default!;
    public decimal Balance { get; set; }
    public Guid CustomerId { get; set; }
    [JsonIgnore]
    public Customer Customer { get; set; } = default!;
    [JsonIgnore]
    public ICollection<Transaction> Transactions { get; set; } = [];
}
