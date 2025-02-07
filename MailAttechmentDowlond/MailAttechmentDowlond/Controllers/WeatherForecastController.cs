using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq; // JSON işleme için
using System.Text;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http;
using MailAttachmentDowlond;
using MailAttachmentDowlond.Models;
using MailAttachmentDowlond.Controllers;
using MailAttechmentDowlond;

namespace MailAttachmentDowlond.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        LoginResult loginResult = OutlookProviderAsync();
        MailAttachmentDownloaderResponse response = OutlookProviderMailAttachmentDownload(loginResult);

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }


    private static LoginResult OutlookProviderAsync()
    {
        string tenantId = "50d99842-d12a-4f5e-9eb5-9bea49a3ca53";
        string url = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
        string postData = "client_id=033fa99c-8646-4173-8229-dc16da014dec&scope=https%3A%2F%2Fgraph.microsoft.com%2F.default&client_secret=ApB8Q~aSLvWD5-WerFfxUr~oOMMTQhTvZbvJtdxb&grant_type=client_credentials";

        var loginResponse = HttpClientPost(url, postData);
        var loginResult = JObject.Parse(loginResponse);

        LoginResult result = new LoginResult
        {
            Token = loginResult["access_token"]?.ToString(),
            TokenType = loginResult["token_type"]?.ToString(),
            ExpiresIn = (int?)loginResult["expires_in"],
            RefreshToken = loginResult["refresh_token"]?.ToString()
        };

        result.Token = "eyJ0eXAiOiJKV1QiLCJub25jZSI6ImppcF9UUzREODA1YlZXcGVrS0lJaGpVRUxNekVmcGVoVzEzSWQxZXBfOUkiLCJhbGciOiJSUzI1NiIsIng1dCI6IktRMnRBY3JFN2xCYVZWR0JtYzVGb2JnZEpvNCIsImtpZCI6IktRMnRBY3JFN2xCYVZWR0JtYzVGb2JnZEpvNCJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9mMTRmYzdhNS0zMmJkLTQ2YWUtOTgzYi0zNzAxZDVkNDE1YmUvIiwiaWF0IjoxNzI0ODM3NzgwLCJuYmYiOjE3MjQ4Mzc3ODAsImV4cCI6MTcyNDkyNDQ4MSwiYWNjdCI6MCwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhYQUFBQUN1MER2OWtxOVZIazdicEhmVGRIK3FydVhlZGZOTnNJbVVweFF3K0pSSUFYdzZWaHo5MVdZQUFhYUtBMHdGS3RpOW1CMXI2S0JPbjFzK1pJYlhJR1JhV3pxY1VGMEtKcTUyTkNjTmxqVE5jPSIsImFtciI6WyJwd2QiLCJtZmEiXSwiYXBwX2Rpc3BsYXluYW1lIjoiR3JhcGggRXhwbG9yZXIiLCJhcHBpZCI6ImRlOGJjOGI1LWQ5ZjktNDhiMS1hOGFkLWI3NDhkYTcyNTA2NCIsImFwcGlkYWNyIjoiMCIsImZhbWlseV9uYW1lIjoiR8O8bHRlbiIsImdpdmVuX25hbWUiOiLDlm1lciIsImlkdHlwIjoidXNlciIsImlwYWRkciI6IjJhMDI6ZmYwOjNkMDQ6ZmI1OmU5MTU6M2NhZTpiYjU4OjM4MGQiLCJuYW1lIjoiT21lciBHdWx0ZW4iLCJvaWQiOiJhMmIyZjBkNy1mYmEzLTQxYTYtYmYxYS0wMDU2OWZiZmJiN2EiLCJwbGF0ZiI6IjMiLCJwdWlkIjoiMTAwMzIwMDFDMDRBNTMwQSIsInJoIjoiMC5BVEVBcGNkUDhiMHlya2FZT3pjQjFkUVZ2Z01BQUFBQUFBQUF3QUFBQUFBQUFBQXhBUGMuIiwic2NwIjoiTWFpbC5SZWFkIE1haWwuUmVhZEJhc2ljIE1haWwuUmVhZFdyaXRlIG9wZW5pZCBwcm9maWxlIFVzZXIuUmVhZCBlbWFpbCIsInNpZ25pbl9zdGF0ZSI6WyJrbXNpIl0sInN1YiI6InNQSXRtZjFVS29veHdNQ25wQnFmRGQxeE96WUluV0VCODlvenF2YmVHbXciLCJ0ZW5hbnRfcmVnaW9uX3Njb3BlIjoiRVUiLCJ0aWQiOiJmMTRmYzdhNS0zMmJkLTQ2YWUtOTgzYi0zNzAxZDVkNDE1YmUiLCJ1bmlxdWVfbmFtZSI6Im9tZXIuZ3VsdGVuQHNvbHZlcmEuY29tLnRyIiwidXBuIjoib21lci5ndWx0ZW5Ac29sdmVyYS5jb20udHIiLCJ1dGkiOiJWNkY4UG5ZeUNFT0ZQQjhCSGIxaUFBIiwidmVyIjoiMS4wIiwid2lkcyI6WyJiNzlmYmY0ZC0zZWY5LTQ2ODktODE0My03NmIxOTRlODU1MDkiXSwieG1zX2NjIjpbIkNQMSJdLCJ4bXNfaWRyZWwiOiIxMiAxIiwieG1zX3NzbSI6IjEiLCJ4bXNfc3QiOnsic3ViIjoiQTc2V3MybDlIWFQ4R0N1RTR4bENSSDVSYkxKbUNVeDRLdzljeVZ2ZklScyJ9LCJ4bXNfdGNkdCI6MTU0NTIxMTkwNX0.ibWbJoJLRfmhAAD-jtIfaBCJ_EQRET4jaZtAt2CXUBvGwo8LVGqbuVbcfslqlvLqFdYZQH8feHxGjBaPjwC_6G6WEsmLJGWVzcC9gq50aq-oQVkH2dmOE9_hkVEuonn83EETcy7oQ2uuP2QZeGAH2wNB4yP22mFloc-3PYbXhNLymbLnuHz0WHJTZ-JvAiHR8ToY6z88T5JB_EDHQHrNLj39sfKjFeHjzLVfiEdbZ6GNGlNdiI-X3WZFeTzyId0CGV5jL8S6lx5rqhsEvA3P_QBvKqBO5f8QAHJxu09ONRGPBsJcvTu2z1LwIuEaUMMNSzkPxrchxbe9Yxo1u1um7w";
        return result;

    }

    private static MailAttachmentDownloaderResponse OutlookProviderMailAttachmentDownload(LoginResult loginResult)
    {
        string graphUrl = "https://graph.microsoft.com/v1.0/me/mailFolders/inbox/messages?$filter=isRead eq false";
        string downloadFolder = Path.Combine("/Users/mehmetyalniz/Desktop", "mailattachments");
        Directory.CreateDirectory(downloadFolder);

        MailAttachmentDownloaderResponse response = new MailAttachmentDownloaderResponse();
        List<Mail> mailList = new List<Mail>();

        // Sayfalama için döngü
        while (!string.IsNullOrEmpty(graphUrl))
        {
            var messagesResponse = HttpClientGet(graphUrl, loginResult.Token);
            var messages = JObject.Parse(messagesResponse);

            foreach (var message in messages["value"])
            {
                string messageId = message["id"]?.ToString();
                string attachmentsUrl = $"https://graph.microsoft.com/v1.0/me/mailFolders/inbox/messages/{messageId}/attachments";
                var attachmentsResponse = HttpClientGet(attachmentsUrl, loginResult.Token);
                var attachments = JObject.Parse(attachmentsResponse);

                // Ekler varsa işlemi başlat
                if (attachments["value"].HasValues)
                {
                    Mail mail = new Mail();
                    mail.Id = messageId;
                    List<MailAttachment> attachmentList = new List<MailAttachment>();

                    foreach (var attachment in attachments["value"])
                    {
                        MailAttachment mailAttachment = new MailAttachment();
                        string attachmentId = attachment["id"]?.ToString();
                        string attachmentName = attachment["name"]?.ToString();
                        string safeFileName = Path.GetInvalidFileNameChars().Aggregate(attachmentName, (current, c) => current.Replace(c.ToString(), string.Empty));
                        string filePath = Path.Combine(downloadFolder, safeFileName);

                        string attachmentUrl = $"https://graph.microsoft.com/v1.0/me/mailFolders/inbox/messages/{messageId}/attachments/{attachmentId}/$value";
                        byte[] attachmentResponse = HttpClientGetBinary(attachmentUrl, loginResult.Token);

                        System.IO.File.WriteAllBytes(filePath, attachmentResponse);

                        mailAttachment.Id = attachmentId;
                        mailAttachment.Path = filePath;
                        mailAttachment.Success = true;
                        attachmentList.Add(mailAttachment);
                        Console.WriteLine($"Ek indirildi: {safeFileName}");
                    }

                    // Mail'i okundu olarak işaretle
                    MarkEmailAsRead(messageId, loginResult.Token);

                    mail.MailAttachmentList = attachmentList;
                    mail.Success = true;
                    mailList.Add(mail);
                }
            }

            // NextLink'i kontrol et ve varsa bir sonraki sayfaya git
            graphUrl = messages["@odata.nextLink"]?.ToString();
        }

        response.Success = true;
        response.MailList = mailList;
        return response;
    }

    private static void MarkEmailAsRead(string messageId, string token)
    {
        string graphUrl = $"https://graph.microsoft.com/v1.0/me/messages/{messageId}";
        var data = new StringContent("{\"isRead\": true}", Encoding.UTF8, "application/json");

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PatchAsync(graphUrl, data).Result;

            if (!response.IsSuccessStatusCode)
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine($"PATCH Error: {responseContent}");
                response.EnsureSuccessStatusCode();
            }
            else
            {
                Console.WriteLine($"Mail {messageId} okundu olarak işaretlendi.");
            }
        }
    }



    private static byte[] HttpClientGetBinary(string url, string token)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine($"GET Error: {responseContent}");
                response.EnsureSuccessStatusCode();
            }

            // İkili (binary) veriyi al
            return response.Content.ReadAsByteArrayAsync().Result;
        }
    }

    private static string HttpClientPost(string url, string content)
    {
        var data = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
        using (var client = new HttpClient())
        {
            var response = client.PostAsync(url, data).Result;
            var responseContent = response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"POST Error: {responseContent}");
                response.EnsureSuccessStatusCode();
            }
            return responseContent.Result;
        }
    }

    private static string HttpClientGet(string url, string token)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.GetAsync(url).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            if (!response.IsSuccessStatusCode)
            {
                // Burada hata yanıtını daha detaylı görmek için responseContent'i inceleyebilirsiniz
                Console.WriteLine($"GET Error: {responseContent}");
                response.EnsureSuccessStatusCode();
            }
            return responseContent;
        }
    }

}