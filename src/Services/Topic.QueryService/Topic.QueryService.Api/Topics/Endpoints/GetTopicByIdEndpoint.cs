using Core.MediatR;
using Topic.QueryService.Api.ResponseDtos;
using Topic.QueryService.Api.Topics.Queries.GetTopicById;
using Topic.QueryService.Domain.Entities;

namespace Topic.QueryService.Api.Topics.Endpoints;

public class GetTopicByIdEndpoint : BaseQueryEndpoint
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/topics/{id}", async (
                Guid id,
                ILogger<GetTopicByIdEndpoint> logger,
                IQueryDispatcher<TopicEntity> queryDispatcher) =>
            {
                return await ExecuteQueryAsync(
                    logger,
                    async () => await queryDispatcher.SendAsync(
                        new GetTopicByIdQuery { Id = id }
                    ),
                    "Неверный id топика"
                );
            })
            .Produces<QueryTopicsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}