using Route256.Week1.Homework.PriceCalculator.Api.Bll.Models.PriceCalculator;

namespace Route256.Week1.Homework.PriceCalculator.Api.Bll.Services.Interfaces;

public interface IPriceCalculatorService
{
    decimal CalculatePrice(IReadOnlyList<GoodModel> goods, ParamsModel param = null);

    CalculationLogModel[] QueryLog(int take);
}