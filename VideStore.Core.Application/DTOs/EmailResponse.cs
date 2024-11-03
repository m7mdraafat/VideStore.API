namespace VideStore.Application.DTOs
{
    public record EmailResponse
    (
        string Subject,
        string Body,
        string To
    );
}
