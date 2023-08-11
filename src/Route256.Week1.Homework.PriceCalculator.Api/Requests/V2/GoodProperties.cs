using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Route256.Week1.Homework.PriceCalculator.Api.Requests.V2;

/// <summary>
/// Харектеристики товара
/// </summary>
public record GoodProperties(
    int Height,
    int Length,
    int Width,
    int Weight);