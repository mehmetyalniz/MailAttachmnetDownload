using System;
using MailAttechmentDowlond;

namespace MailAttachmentDowlond.Models
{
    public class Mail
    {
        public bool Success { get; set; }
        public string Id { get; set; }
        public List<MailAttachment> MailAttachmentList { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}