namespace VelikiyPrikalel.OLLAMACHAT.Web.Services;

using Data.StatelessEnums;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class UserChatBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserChatBackgroundService> logger;

    public UserChatBackgroundService(IServiceProvider serviceProvider, ILogger<UserChatBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<OllamaChatContext>();

                var userChats = dbContext.UserChats
                    .Where(uc => (uc.State == ChatState.WaitingImageGeneration || uc.State == ChatState.WaitingMessageGeneration)
                        && uc.EnqueuedCompletionJobId != null)
                    .ToList();

                foreach (var userChat in userChats)
                {
                    var jobId = userChat.EnqueuedCompletionJobId!;
                    var llmResponse = ChatResponseObtainer.ObtainChatResponseByJobId(jobId);
                    if (llmResponse.Completed)
                    {
                        throw new InvalidOperationException($"Состояние {userChat.Id} completed но при этом он ждёт чего-то");
                    }

                    if (llmResponse.Failed)
                    {
                        userChat.GenerationFailed();
                        dbContext.UserChats.Update(userChat);
                        logger.LogWarning("Считаем что чат {chat} не смог завершить операцию", userChat.Id);
                    }
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Вызываем каждую минуту так как может быть проблема с тем что пользователь сам не запросит результат
        }
    }
}