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

    public UserChatBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<OllamaChatContext>();

                var userChats = dbContext.UserChats
                    .Where(uc => uc.State == ChatState.WaitingImageGeneration || uc.State == ChatState.WaitingMessageGeneration)
                    .ToList();

                foreach (var userChat in userChats)
                {
                    userChat.GenerationFailed();
                    dbContext.UserChats.Update(userChat);
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Вызываем каждую минуту так как может быть проблема с тем что пользователь сам не запросит результат
        }
    }
}