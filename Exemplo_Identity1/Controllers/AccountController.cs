using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Exemplo_Identity1.Models;

public class AccountController : Controller
{
    //gereniador de usuários
    private readonly UserManager<IdentityUser> _userManager;
    //gerenciador de login
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Definir o campo EmailConfirmed como True
                //Esta ação é recomendada apenas para testes
                //O correto é enviar uma solicitação de confirmação por e-mail.
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        else
        {
            // Log model state errors
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Ou use outro mecanismo de logging
                }
            }
        }
        //return RedirectToAction("Index", "Home");
        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(RegisterViewModel model, string returnUrl = "/Home/Index")
    {
        // Remover a validação de ConfirmPassword no contexto do login
        ModelState.Remove(nameof(RegisterViewModel.ConfirmPassword));

        if (ModelState.IsValid)
        {
            // Verifique se o usuário existe
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                Console.WriteLine("Usuário não encontrado.");
                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
                return View(model);
            }

            Console.WriteLine($"Usuário encontrado: {user.UserName}");

            //busca o usuário no banco de dados
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                Console.WriteLine("Login bem-sucedido.");
                //se o login for bem sucedido
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (result.IsLockedOut)
            {
                Console.WriteLine("Usuário bloqueado.");
                ModelState.AddModelError(string.Empty, "Conta bloqueada. Tente novamente mais tarde.");
            }
            else if (result.RequiresTwoFactor)
            {
                Console.WriteLine("Autenticação de dois fatores necessária.");
                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
            }
            else
            {
                Console.WriteLine("Tentativa de login inválida.");
                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
            }
        }

        
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
       
}

