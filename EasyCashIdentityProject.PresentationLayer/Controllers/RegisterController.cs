﻿using EasyCashIdentityProject.DtoLayer.Dtos.AppUserDtos;
using EasyCashIdentityProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Net;
using System.Net.Mail;

namespace EasyCashIdentityProject.PresentationLayer.Controllers
{
	public class RegisterController : Controller
	{
		private readonly UserManager<AppUser> _userManager;

		public RegisterController(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Index(AppUserRegisterDto appUserRegisterDto)
		{
			if (ModelState.IsValid)
			{
				Random random = new Random();
				int code = random.Next(100000, 1000000);
				AppUser appUser = new AppUser()
				{
					UserName = appUserRegisterDto.Username,
					Name = appUserRegisterDto.Name,
					Surname = appUserRegisterDto.Surname,
					Email = appUserRegisterDto.Email,
					City = "aaaa",
					District = "bbbb",
					ImageUrl = "cccc",
					ConfirmCode = code
				};
				var result = await _userManager.CreateAsync(appUser, appUserRegisterDto.Password);
				if (result.Succeeded)
				{
					//	MimeMessage mimeMessage = new MimeMessage();
					//	MailboxAddress mailboxAddressFrom = new MailboxAddress("Easy Cash Admin", "muhammedhasankucuk39@gmail.com");
					//	MailboxAddress mailboxAddressTo = new MailboxAddress("User", appUser.Email);

					//	mimeMessage.From.Add(mailboxAddressFrom);
					//	mimeMessage.To.Add(mailboxAddressTo);

					//	BodyBuilder bodyBuilder = new BodyBuilder();
					//	bodyBuilder.TextBody = "Kayıt İşleminizi Tamamlamak için Doğrulama Kodunu Giriniz : " + code;
					//	mimeMessage.Body = bodyBuilder.ToMessageBody();
					//	mimeMessage.Subject = "Easy Cash Onay Kodu";
					//  SmtpClient client = new SmtpClient();
					//  client.Connect("smtp.gmail.com", 587, false);
					//  client.Authenticate("muhammedhasankucuk39@gmail.com", "");
					//  client.Send(mimeMessage);
					//  client.Disconnect(true);

					SmtpClient client = new SmtpClient("smtp.gmail.com");
					client.Port = 465;
					client.EnableSsl = false;
					client.Credentials = new NetworkCredential("muhammedhasankucuk39@gmail.com", "fmpvdsfwpqbvfkqe");

					MailMessage mail = new MailMessage("muhammedhasankucuk39@gmail.com", appUser.Email);
					mail.Subject = "Easy Cash Onay Kodu";
					mail.Body = "Kayıt İşleminizi Tamamlamak için Doğrulama Kodunu Giriniz : " + code;
					client.Send(mail);



					return RedirectToAction("index", "ConfirmMail");
				}
				else
				{
					foreach (var item in result.Errors)
					{
						ModelState.AddModelError("", item.Description);
					}
				}
			}
			return View();
		}
	}
}
