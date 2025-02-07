using System;
namespace MailAttachmentDowlond.Models
{
    public class MailAttachment
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public bool Success { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}