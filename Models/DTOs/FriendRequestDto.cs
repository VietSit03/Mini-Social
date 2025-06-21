namespace MiniSocialAPI.Models.DTOs
{
    public class FriendRequestDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public Guid ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
