﻿using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MimeKit;
using WebApp01.Repository;
using WebApp01.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace WebApp01.Controllers
{
    [Authorize(Roles = "Customer")]
    public class MailController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MailController (DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Contact(MailModel mail, string Email)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            
            else
            {  
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com");
                    client.Authenticate("tungng14@gmail.com", "dyuxzvyzweayjiga");
                    var mailContent = TempData["mailContent"];
                    ViewBag.Mail = mailContent;
                    var bodyBuilder = new BodyBuilder
                    {
                       
                        HtmlBody = "<p> San pham cua ban se duoc gui toi dia chi " + mailContent+" trong vong 7 ngay ke tiep. Xin chan thanh cam on!</p>",
                        TextBody = "Thank you so much"
                    };
                    var message = new MimeMessage
                    {
                        Body = bodyBuilder.ToMessageBody()
                    };
                    message.From.Add(new MailboxAddress("Shop", "tungng14@gmail.com"));
                    message.To.Add(new MailboxAddress("Customer", Email));
                    message.Subject = "Ban da chon hinh thuc ship COD";
                    client.Send(message);

                    client.Disconnect(true);
                }

                _dataContext.Add(mail);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Đã gửi mail thành công về địa chỉ e-mail của bạn";
                return RedirectToAction("Index");
            }
        }
    }
}
