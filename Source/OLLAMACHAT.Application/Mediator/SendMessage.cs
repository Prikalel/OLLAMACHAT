using Hangfire.States;

namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator;

/// <summary>
/// Отправка промта llm.
/// </summary>
public sealed class SendMessage
{
    /// <summary>
    /// Команда на выполнение llm генерации.
    /// </summary>
    /// <param name="Message">Строка от пользователя.</param>
    /// <returns>
    /// Возвращает id задачи HF.
    /// </returns>
    public sealed record Command(string Message) : IRequest<string>;

    /// <inheritdoc />
    public sealed class Handler(
        IRepository<User> userRepository,
        IBackgroundJobClientV2 backgroundJobClient,
        ILogger<Handler> logger) : IRequestHandler<Command, string>
    {
        /// <inheritdoc />
        public async ValueTask<string> Handle(Command request, CancellationToken cancellationToken)
        {
            string? jobId = null;
            try
            {
                User user = await userRepository.GetOrCreateUser("alex");
                UserChat activeChat = user.GetOrCreateActiveChat(null, out bool _);

                if (request.Message.Trim().ToLower().Equals("/undo"))
                {
                    logger.LogInformation("Will delete last message");
                    activeChat.UndoLastMessage();
                    await userRepository.UpdateAsync(user);
                    jobId = backgroundJobClient.Create(() => ReturnLastMessageDeletedText(),
                        new ScheduledState(TimeSpan.FromHours(3)));
                }
                else
                {
                    logger.LogInformation("Will generate llm response from model {Model}", activeChat.Model);
                    // создали таску на генерацию ответа. запуск через сколько угодно главное дать нам время обновить состояние чата перед постановкой в очередь.
                    jobId = backgroundJobClient.Create<ILlmBackgroundService>(llmService => llmService.GenerateTextResponse(
                            request.Message,
                            activeChat.Model,
                            activeChat.Id,
                            activeChat.Messages),
                        new ScheduledState(TimeSpan.FromHours(3)));

                    ChatState state = activeChat.UserEnteredPrompt(request.Message, jobId);
                    await userRepository.UpdateAsync(user); // сохранили состояние чата с идентификатором задачи
                }


                // поставили задачу в очередь
                backgroundJobClient.ChangeState(jobId, new EnqueuedState(), ScheduledState.StateName);

                logger.LogInformation("Returning job id {Id}, chat {Id}",
                    jobId,
                    activeChat.Id);
            }
            catch (Exception ex)
            {
                // Если что-то пошло не так - удалили задачу на генерацию ответа
                if (jobId != null)
                {
                    backgroundJobClient.ChangeState(jobId, new DeletedState());
                }

                throw;
            }

            return jobId!;
        }

        public async Task<string> ReturnLastMessageDeletedText() {
            return await Task.FromResult("Last message deleted from chat");
        }
    }
}
