using System;
namespace MailAttachmentDowlond.Controllers
{
    public class LoginResult
    {
        public dynamic Token { get; set; }

        public dynamic TokenType { get; set; }

        public dynamic ExpiresIn { get; set; }

        public dynamic RefreshToken { get; set; }

    }
}