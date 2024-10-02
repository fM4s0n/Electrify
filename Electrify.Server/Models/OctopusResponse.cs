using System.Text.Json.Serialization;

namespace Electrify.Server.Models;

public class OctopusResponse
{ 
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("next")]
    public object? Next { get; set; }
    
    [JsonPropertyName("previous")]
    public object? Previous { get; set; }

    [JsonPropertyName("results")] 
    public Results[] Results { get; set; } = [];
}

public class Results
{
    [JsonPropertyName("value_exc_vat")]
    public double ValueExcludingVat { get; set; }
    
    [JsonPropertyName("value_inc_vat")]
    public double ValueIncVat { get; set; }
    
    [JsonPropertyName("valid_from")]
    public string? ValidFrom { get; set; }

    [JsonPropertyName("valid_to")] 
    public string? ValidTo { get; set; }
    
    [JsonPropertyName("payment_method")]
    public object? PaymentMethod { get; set; }
}