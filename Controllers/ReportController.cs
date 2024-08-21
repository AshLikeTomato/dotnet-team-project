using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DotnetProject2025.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetProject2025.Controllers
{
    [Route("Report")]
    public class ReportController : Controller
    {
        private readonly SmtpSettings _smtpSettings;

        public ReportController(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        [HttpGet]
        public IActionResult Report()
        {
            return View("~/Views/Home/Report.cshtml");
        }

        [HttpPost]
        public IActionResult SubmitReport(ViolationReport report)
        {
            if (ModelState.IsValid)
            {
                // Gửi email báo cáo
                SendReportEmail(report);

                // Thông báo thành công hoặc chuyển hướng
                ViewBag.Message = "Báo cáo của bạn đã được gửi thành công!";
                return View("~/Views/Home/Report.cshtml");
            }

            return View("~/Views/Home/Report.cshtml", report);
        }

        private void SendReportEmail(ViolationReport report)
        {
            try
            {
                var fromAddress = new MailAddress(report.Email, report.UserName);
                var toAddress = new MailAddress("yuuwhisky@gmail.com");
                const string subject = "Báo cáo vi phạm mới";
                string body = $"Tên người dùng: {report.UserName}\nĐịa chỉ email: {report.Email}\nNội dung báo cáo: {report.ReportDetails}";

                var smtp = new SmtpClient
                {
                    Host = _smtpSettings.Host,
                    Port = _smtpSettings.Port,
                    EnableSsl = _smtpSettings.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (SmtpException ex)
            {
                Debug.WriteLine($"Lỗi SMTP: {ex.Message}");
                // Thêm thông báo lỗi cho người dùng hoặc ghi log
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi: {ex.Message}");
                // Thêm thông báo lỗi cho người dùng hoặc ghi log
            }
        }
    }


}