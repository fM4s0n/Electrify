using System.ComponentModel.DataAnnotations;

namespace Electrify.Models;

public sealed record Tariff
{
    [Key]
    public DateTime DateTime { get; set; }
    public double Price { get; set; }
}