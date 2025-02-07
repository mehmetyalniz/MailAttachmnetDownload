using System;

namespace MailAttachmentDowlond.Models
{
    public class MailAttachmentDownloaderResponse
    {
        public bool Success { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public List<Mail> MailList { get; set; }
    }
}