namespace VelikiyPrikalel.OLLAMACHAT.Web.ChatControl;

public static class ChatResponseObtainer
{
    public static ChatResponse ObtainChatResponseByJobId(string jobId)
    {
        IMonitoringApi monitoringApi = JobStorage.Current.GetMonitoringApi();
        JobDetailsDto jobDetails = monitoringApi.JobDetails(jobId);
        string? currentState = jobDetails?.History.FirstOrDefault()?.StateName;

        if (currentState == null)
        {
            return new(true, true, "Error in task state");
        }

        if (currentState == EnqueuedState.StateName
            || currentState == ProcessingState.StateName || currentState == AwaitingState.StateName)
        {
            return new(false, false, null);
        }
        else if (jobDetails.History.Any(x => x.StateName == FailedState.StateName)
                 && jobDetails.History.All(x => x.StateName != SucceededState.StateName))
        {
            // todo: мб тут надо подождать вместо возврата игроку ответа?
            return new(true, true, "Failed to complete");
        }

        string jobValue = jobDetails.History
            .First(x => x.StateName == SucceededState.StateName)
            .Data["Result"];
        string deserializeString = JsonConvert.DeserializeObject<string>(jobValue)!;
        deserializeString = Regex.Replace(deserializeString, @"<think>.*?</think>", string.Empty, RegexOptions.Singleline);

        return new(true, false, deserializeString);
    }

    public record ChatResponse(bool Completed, bool Failed, string? Response);
}
