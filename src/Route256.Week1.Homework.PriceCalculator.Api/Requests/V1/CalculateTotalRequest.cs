namespace Route256.Week1.Homework.PriceCalculator.Api.Requests.V1;

/// <summary>
/// ID товара, чью полную стоимость товара нужно рассчитать
/// </summary>
public record CalculateTotalRequest(
    int Id,
    int Distance);