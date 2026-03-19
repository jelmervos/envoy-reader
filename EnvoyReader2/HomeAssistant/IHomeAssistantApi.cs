using Refit;

internal interface IHomeAssistantApi
{
    [Headers("Authorization: Bearer")]
    [Get("/api/states/{entityId}")]
    Task<State> GetState(string entityId);
}
