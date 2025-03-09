using FinanceTrackerAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Currency
{
    public int Id { get; set; }

    [Column("currency_code")]
    public string CurrencyCode { get; set; } // USD, EUR, KZT и т. д.

    [Column("currency_name")]
    public string CurrencyName { get; set; } // Полное название валюты

    [Column("exchange_rate")]
    public decimal ExchangeRate { get; set; } // Курс относительно 1 USD

    public ICollection<UserParameter> UserParameters { get; set; } = new List<UserParameter>();
}
