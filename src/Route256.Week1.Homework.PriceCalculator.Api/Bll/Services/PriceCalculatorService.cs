using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Route256.Week1.Homework.PriceCalculator.Api.Bll.Models.PriceCalculator;
using Route256.Week1.Homework.PriceCalculator.Api.Bll.Options;
using Route256.Week1.Homework.PriceCalculator.Api.Bll.Services.Interfaces;
using Route256.Week1.Homework.PriceCalculator.Api.Dal.Entities;
using Route256.Week1.Homework.PriceCalculator.Api.Dal.Repositories.Interfaces;

namespace Route256.Week1.Homework.PriceCalculator.Api.Bll.Services;

public class PriceCalculatorService : IPriceCalculatorService
{
    private decimal _volumeToPriceRatio;
    private decimal _weightToPriceRatio;
    private int _defaultDistance;
    private readonly IStorageRepository _storageRepository;
    
    public PriceCalculatorService(
        IOptionsSnapshot<PriceCalculatorOptions> options,
        IStorageRepository storageRepository)
    {
        _volumeToPriceRatio = options.Value.VolumeToPriceRatio;
        _weightToPriceRatio = options.Value.WeightToPriceRatio;
        _defaultDistance = options.Value.DefaultDistance;
        _storageRepository = storageRepository;
    }
    
    public decimal CalculatePrice(IReadOnlyList<GoodModel> goods, ParamsModel param)
    {
        if (!goods.Any())
        {
            throw new ArgumentOutOfRangeException(nameof(goods));
        }

        InitParams(param, out var distance);

        var volumePrice = CalculatePriceByVolume(goods, out var volume);
        var weightPrice = CalculatePriceByWeight(goods, out var weight);

        var resultPrice = Math.Max(volumePrice, weightPrice) * ((decimal)distance / 1000); 
        
        _storageRepository.Save(new StorageEntity(
            DateTime.UtcNow,
            volume,
            weight,
            resultPrice,
            distance));
        
        return resultPrice;
    }

    private decimal CalculatePriceByVolume(
        IReadOnlyList<GoodModel> goods,
        out decimal volume)
    {
        volume = goods
            .Select(x => x.Height * x.Width * x.Length / 1000)
            .Sum();

        return volume * _volumeToPriceRatio;
    }
    
    private decimal CalculatePriceByWeight(
        IReadOnlyList<GoodModel> goods,
        out decimal weight)
    {
        weight = goods
            .Select(x => x.Weight / 1000)
            .Sum();

        return weight * _weightToPriceRatio;
    }

    public CalculationLogModel[] QueryLog(int take)
    {
        if (take == 0)
        {
            return Array.Empty<CalculationLogModel>();
        }
        
        var log = _storageRepository.Query()
            .OrderByDescending(x => x.At)
            .Take(take)
            .ToArray();

        return log
            .Select(x => new CalculationLogModel(
                x.Volume, 
                x.Weight,
                x.Price))
            .ToArray();
    }

    private void InitParams(
        ParamsModel param,
        out int distance)
    {
        distance = _defaultDistance;

        if (param != null)
        {
            distance = param.Distance;
        }
    }
}