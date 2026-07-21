using System.Text.Encodings.Web;
using Learnova.Domain.Entites;
using Learnova.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Learnova.Infrastructure.Email
{
    public sealed class SmtpEmailSender(IOptions<EmailSettings> options)
        : IEmailSender<ApplicationUser>
    {
        private readonly EmailSettings _settings = options.Value;

        public Task SendConfirmationLinkAsync(
            ApplicationUser user,
            string email,
            string confirmationLink)
        {
            return SendEmailAsync(
                email,
                "Confirm your Learnova email",
                $"""
                <p>Welcome to Learnova.</p>
                <p>Please <a href="{confirmationLink}">confirm your email address</a>.</p>
                """,
                $"Welcome to Learnova. Confirm your email address: {confirmationLink}");
        }

        public Task SendPasswordResetLinkAsync(
            ApplicationUser user,
            string email,
            string resetLink)
        {
            return SendEmailAsync(
                email,
                "Reset your Learnova password",
                $"""
                <p>A password reset was requested for your Learnova account.</p>
                <p><a href="{resetLink}">Reset your password</a>.</p>
                <p>If you did not request this, you can ignore this email.</p>
                """,
                $"Reset your Learnova password: {resetLink}");
        }

        public Task SendPasswordResetCodeAsync(
            ApplicationUser user,
            string email,
            string resetCode)
        {
            var encodedCode = HtmlEncoder.Default.Encode(resetCode);

            return SendEmailAsync(
                email,
                "Your Learnova password reset code",
                $"""
                <p>A password reset was requested for your Learnova account.</p>
                <p>Your reset code is:</p>
                <p><strong>{encodedCode}</strong></p>
                <p>If you did not request this, you can ignore this email.</p>
                """,
                $"Your Learnova password reset code is: {resetCode}");
        }

        private async Task SendEmailAsync(
            string recipientEmail,
            string subject,
            string htmlBody,
            string textBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(MailboxAddress.Parse(recipientEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = textBody
            }.ToMessageBody();

            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls);

            var smtpPassword = string.Concat(
                _settings.Password.Where(character => !char.IsWhiteSpace(character)));

            await smtpClient.AuthenticateAsync(
                _settings.Username,
                smtpPassword);

            await smtpClient.SendAsync(message);
            await smtpClient.DisconnectAsync(true);
        }
    }
}
