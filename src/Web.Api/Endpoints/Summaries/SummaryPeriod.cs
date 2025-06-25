using System.ComponentModel.DataAnnotations;
using Application.Summaries.Daily;
using Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Summaries;

public class SummaryPeriod : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/summaries",
            async ([FromQuery][Required] SummaryType type, ISender sender, CancellationToken cancellationToken) =>
        {
            Result<SummaryPeriodQueryResponse> result = await sender.Send(new SummaryPeriodQuery { SummaryType = type }, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .Produces<SummaryPeriodQueryResponse>()
        .WithTags(Tags.SummaryPeriod);
    }
}
