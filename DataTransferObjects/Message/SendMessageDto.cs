namespace sigma_backend.DataTransferObjects.Message
{
    public class SendMessageDto
        {
            public string? ReceiverUsername { get; set; }
            public string? Content { get; set; }
        }
}
