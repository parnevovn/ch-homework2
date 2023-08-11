using System.Net;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Route256.Week1.Homework.PriceCalculator.Api.Controllers;

namespace Route256.Week1.Homework.PriceCalculator.Api.Middlewaries;

internal sealed class GoodsCalculateTotalPriceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GoodsCalculateTotalPriceMiddleware> _logger;

    public GoodsCalculateTotalPriceMiddleware(
        RequestDelegate next,
        ILogger<GoodsCalculateTotalPriceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var reqPath = context.Request.Path;

        if (reqPath == "/goods/calculate-total")
        {
            var timestamp = DateTime.UtcNow;
            var urlStr = context.Request.GetDisplayUrl();
            var headersStr = getHeadersStr(context.Request.Headers);
            var reqBodyStr = await getReqBodyStr(context.Request.Body);
            var respBodyStr = "";

            using (var swapStream = new MemoryStream())
            {
                var originalRespBody = context.Response.Body;
                context.Response.Body = swapStream;

                await _next.Invoke(context);

                respBodyStr = await getRespBodyStr(swapStream, originalRespBody);
            }

            var logStr = 
                $"Request:\r\n" +
                $"Timestamp: {timestamp}\r\n" +
                $"Url: {urlStr}\r\n" +
                $"Headers: {headersStr}\r\n" +
                $"Body: {reqBodyStr}\r\n" +
                $"Response:\r\n" +
                $"Body: {respBodyStr}";

            _logger.LogInformation(logStr);
        }
        else
        {
            await _next.Invoke(context);
        }

        async Task<string> getRespBodyStr(MemoryStream swapStreamLocal, Stream originalRespBodyLocal)
        {
            swapStreamLocal.Seek(0, SeekOrigin.Begin);
            string respBodyStr = new StreamReader(swapStreamLocal).ReadToEnd();
            swapStreamLocal.Seek(0, SeekOrigin.Begin);

            await swapStreamLocal.CopyToAsync(originalRespBodyLocal);
            context.Response.Body = originalRespBodyLocal;

            return respBodyStr;
        }
    }

    private string getHeadersStr(IHeaderDictionary headers)
    {
        System.Text.StringBuilder sb = new();

        foreach (var key in headers)
        {
            sb.Append(key.Key);
            sb.Append("=");
            sb.AppendLine(key.Value);
        }

        var headersString = sb.ToString();

        return headersString;
    }

    private async Task<string> getReqBodyStr(Stream body)
    {
        string bodyString = "";

        if (body != null)
        {
            using (StreamReader sr = new StreamReader(body, Encoding.UTF8, true, 1024, true))
            {
                bodyString = await sr.ReadToEndAsync();
            }

            body.Seek(0, SeekOrigin.Begin);
        }

        return bodyString;
    }
}