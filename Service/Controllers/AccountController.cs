using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Domain.Entities;
using Service.Models;
using Service.Service;

namespace Service.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Student> _userManager;
        private readonly SignInManager<Student> _signInManager;

        public AccountController(UserManager<Student> userMgr, SignInManager<Student> signInMgr)
        {
            _userManager = userMgr;
            _signInManager = signInMgr;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Student student = new Student
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MiddleName = model.MiddleName,
                    Group = model.Group,
                    UserName = model.Email,
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(student, model.Password!);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(student);
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { studentId = student.Id, code = code },
                        protocol: HttpContext.Request.Scheme);
                    EmailSender emailSender = new EmailSender();
                    await emailSender.SendEmailAsync(model.Email!, "Подтверждение регистрации",
                        $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>нажмите</a>");

                    return View("RegisterEmail");
                }
                else
                {
                    return View("Error");
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string studentId, string code)
        {
            if (studentId == null || code == null)
            {
                return View("Error");
            }
            var student = await _userManager.FindByIdAsync(studentId);
            if (student == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(student, code);
            if (result.Succeeded)
                return View("ConfirmEmail");
            else
                return View("Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                Student student = await _userManager.FindByEmailAsync(model.Email!);
                if (student != null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(student))
                    {
                        ModelState.AddModelError(string.Empty, "Вы не подтвердили свой email");
                        return View(model);
                    }
                    await _signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(student, model.Password!, false, false);
                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/");
                    }
                }
                ModelState.AddModelError(nameof(LoginViewModel.Email), "Неверный логин или пароль");
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var student = await _userManager.FindByEmailAsync(model.Email!);
                if (student == null || !await _userManager.IsEmailConfirmedAsync(student))
                {
                    // пользователь с данным email может отсутствовать в бд
                    // тем не менее мы выводим стандартное сообщение, чтобы скрыть 
                    // наличие или отсутствие пользователя в бд
                    return View("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(student);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { studentId = student.Id, code = code }, protocol: HttpContext.Request.Scheme);
                EmailSender emailSender = new EmailSender();
                await emailSender.SendEmailAsync(model.Email!, "Сброс пароля",
                    $"Для сброса пароля пройдите по ссылке: <a href='{callbackUrl}'>нажмите</a>");
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string? code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var student = await _userManager.FindByEmailAsync(model.Email!);
            if (student == null)
            {
                return View("ResetPasswordConfirmation");
            }
            var result = await _userManager.ResetPasswordAsync(student, model.Code!, model.Password!);
            if (result.Succeeded)
            {
                return View("ResetPasswordConfirmation");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
