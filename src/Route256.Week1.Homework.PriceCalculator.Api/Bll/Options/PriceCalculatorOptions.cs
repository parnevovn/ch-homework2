namespace Route256.Week1.Homework.PriceCalculator.Api.Bll.Options;

public sealed class PriceCalculatorOptions
{
    public decimal VolumeToPriceRatio { get; set; }
    public decimal WeightToPriceRatio { get; set; }
    public int DefaultDistance { get; set; }
}