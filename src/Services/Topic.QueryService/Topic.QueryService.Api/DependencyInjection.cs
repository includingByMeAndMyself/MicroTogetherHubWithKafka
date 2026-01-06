using Carter;
using Confluent.Kafka;
using Core.MediatR;
using Microsoft.EntityFrameworkCore;
using Topic.QueryService.Api.Topics.Queries;
using Topic.QueryService.Api.Topics.Queries.GetTopicById;
using Topic.QueryService.Api.Topics.Queries.GetTopics;
using Topic.QueryService.Api.Topics.Queries.GetTopicsByAuthorName;
using Topic.QueryService.Api.Topics.Queries.GetTopicsWithComments;
using Topic.QueryService.Api.Topics.Queries.GetTopicsWithLikes;
using Topic.QueryService.Domain.Dao;
using Topic.QueryService.Domain.Entities;
using Topic.QueryService.Infrastructure.Consumers;
using Topic.QueryService.Infrastructure.Dao;
using Topic.QueryService.Infrastructure.Data;
using Topic.QueryService.Infrastructure.Handlers;
using Topic.QueryService.Infrastructure.MediatR;

namespace Topic.QueryService.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddQueryServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextWithFactory(configuration);
        
        services.AddScoped<ITopicStorage, TopicStorage>();
        services.AddScoped<ICommentStorage, CommentStorage>();
        services.AddScoped<IQueryEventHandler, QueryEventHandler>();
        
        var consumerConfig = new ConsumerConfig();
        configuration.GetSection(nameof(ConsumerConfig)).Bind(consumerConfig);
        services.AddSingleton<ConsumerConfig>(consumerConfig);

        services.AddScoped<IKafkaEventSubscriber, KafkaEventSubscriber>();
        services.AddHostedService<KafkaEventConsumerBackgroundService>();
        
        services.RegisterQueriesHandler();
        
        services.AddCarter();

        return services;
    }
    
    private static IServiceCollection RegisterQueriesHandler(
        this IServiceCollection services)
    {

        services.AddScoped<ITopicQueryHandler, TopicQueryHandler>();

        services.AddScoped<IQueryDispatcher<TopicEntity>>(provider =>
        {
            var dispatcher = new QueryDispatcher();

            var commandTopicHandler = provider
                .GetRequiredService<ITopicQueryHandler>();

            dispatcher.RegisterHandler<GetTopicsQuery>(command =>
            {
                return commandTopicHandler.HandleAsync(command);
            });
            dispatcher.RegisterHandler<GetTopicByIdQuery>(command =>
            {
                return commandTopicHandler.HandleAsync(command);
            });
            dispatcher.RegisterHandler<GetTopicsByAuthorNameQuery>(command =>
            {
                return commandTopicHandler.HandleAsync(command);
            });
            dispatcher.RegisterHandler<GetTopicsWithCommentsQuery>(command =>
            {
                return commandTopicHandler.HandleAsync(command);
            });
            dispatcher.RegisterHandler<GetTopicsWithLikesQuery>(command =>
            {
                return commandTopicHandler.HandleAsync(command);
            });
            return dispatcher;
        });

        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        DatabaseInitializer.Initialize(app);
        app.MapCarter();
        return app;
    }

    public static IServiceCollection AddDbContextWithFactory(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PgConnection")!;

        Action<DbContextOptionsBuilder> config = options => options
            .UseLazyLoadingProxies()
            .UseNpgsql(connectionString);

        services.AddSingleton<DbContextFactory>(new DbContextFactory(config));
        services.AddDbContext<ApplicationContext>(config);

        return services;
    }
}