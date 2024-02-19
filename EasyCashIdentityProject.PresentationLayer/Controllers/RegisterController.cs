using EasyCashIdentityProject.DtoLayer.Dtos.AppUserDtos;
using EasyCashIdentityProject.EntityLayer.Concrete;
using EasyCashIdentityProject.PresentationLayer.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;


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
            var user = await _userManager.FindByEmailAsync(appUserRegisterDto.Email);
			ViewBag.User = user;
			if (ModelState.IsValid && user == null)
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
					MimeMessage mimeMessage = new MimeMessage();
					MailboxAddress mailboxAddressFrom = new MailboxAddress("Easy Cash Admin", "muhammedhasankucuk39@gmail.com");
					MailboxAddress mailboxAddressTo = new MailboxAddress("User", appUser.Email);

					mimeMessage.From.Add(mailboxAddressFrom);
					mimeMessage.To.Add(mailboxAddressTo);

					BodyBuilder bodyBuilder = new BodyBuilder();
					bodyBuilder.TextBody = "Kayıt İşleminizi Tamamlamak için Doğrulama Kodunu Giriniz : " + code;
					mimeMessage.Body = bodyBuilder.ToMessageBody();
					mimeMessage.Subject = "Easy Cash Onay Kodu";
					SmtpClient client = new SmtpClient();
					client.Connect("smtp.gmail.com", 587, false);
					client.Authenticate("muhammedhasankucuk39@gmail.com", "ysjskobvgvbcbruh");
					client.Send(mimeMessage);
					client.Disconnect(true);

					TempData["Mail"] = appUserRegisterDto.Email;

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
