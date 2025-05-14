using Microsoft.AspNetCore.Mvc;
using PerfumeBackend.Data;
using PerfumeBackend.Models;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

namespace PerfumeBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly PerfumeContext _context;
        private readonly IConfiguration _config;

        public ApplicationsController(PerfumeContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllApplications()
        {
            return Ok(await _context.Applications.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateApplication([FromBody] Application application)
        {
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            
            await SendEmailAsync(
                $"Новая заявка от {application.Name}",
                $"Телефон: {application.Phone}<br>Email: {application.Email}<br>Комментарий: {application.Comment}"
            );

            return Ok(new { Message = "Заявка сохранена и отправлена" });
        }

        private async Task SendEmailAsync(string subject, string body)
        {
            var settings = _config.GetSection("SmtpSettings");
            
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(settings["FromEmail"]));
            email.To.Add(MailboxAddress.Parse(settings["AdminEmail"]));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(settings["Host"], int.Parse(settings["Port"]), SecureSocketOptions.SslOnConnect);
            await smtp.AuthenticateAsync(settings["Username"], settings["Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}