using Microsoft.Extensions.Options;
using Route256.Week1.Homework.PriceCalculator.Api.Bll.Options;
using Route256.Week1.Homework.PriceCalculator.Api.Bll.Services.Interfaces;
using Route256.Week1.Homework.PriceCalculator.Api.Dal.Repositories.Interfaces;
using System.Transactions;

namespace Route256.Week1.Homework.PriceCalculator.Api.HostedServices;

public sealed class GoodsSyncHostedService: BackgroundService
{
    private readonly IGoodsRepository _repository;
    private readonly IServiceProvider _serviceProvider;
    private int _goodsUpdateTime;
    private readonly IDisposable _disposGoodsSyncHostedOptions;

    public GoodsSyncHostedService(
        IGoodsRepository repository,
        IServiceProvider serviceProvider,
        IOptionsMonitor<GoodsSyncHostedOptions> options)
    {
        _repository = repository;
        _serviceProvider = serviceProvider;
        _goodsUpdateTime = options.CurrentValue.GoodsUpdateTime;
        
        _disposGoodsSyncHostedOptions = options.OnChange(x =>
        {
            _goodsUpdateTime = x.GoodsUpdateTime;
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var goodsService = scope.ServiceProvider.GetRequiredService<IGoodsService>();
                var goods = goodsService.GetGoods().ToList();
                foreach (var good in goods)
                    _repository.AddOrUpdate(good);
            }
            
            await Task.Delay(TimeSpan.FromSeconds(_goodsUpdateTime), stoppingToken);
        }
    }
    
    public override void Dispose()
    {
        _disposGoodsSyncHostedOptions?.Dispose();
        base.Dispose();
    }
}