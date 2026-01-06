using Core.MediatR;
using Topic.QueryService.Api.ResponseDtos;
using Topic.QueryService.Api.Topics.Queries.GetTopicsWithComments;
using Topic.QueryService.Domain.Entities;

namespace Topic.QueryService.Api.Topics.Endpoints;

public class GetTopicsWithCommentsEndpoint : BaseQueryEndpoint
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/topics/withComments", async (
                ILogger<GetTopicsWithCommentsEndpoint> logger,
                IQueryDispatcher<TopicEntity> queryDispatcher) =>
            {
                return await ExecuteQueryAsync(
                    logger,
                    async () => await queryDispatcher.SendAsync(
                        new GetTopicsWithCommentsQuery()
                    ),
                    "Ошибка запроса"
                );
            })
            .Produces<QueryTopicsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}