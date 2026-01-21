using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Avocado_shop.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email jest wymagany")]
            [EmailAddress(ErrorMessage = "Nieprawid³owy format email")]
            [Display(Name = "Email")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Has³o jest wymagane")]
            [StringLength(100, ErrorMessage = "Has³o musi mieæ co najmniej {2} znaków.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Has³o")]
            public string Password { get; set; } = "";

            [DataType(DataType.Password)]
            [Display(Name = "PotwierdŸ has³o")]
            [Compare("Password", ErrorMessage = "Has³a nie s¹ zgodne.")]
            public string ConfirmPassword { get; set; } = "";
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new IdentityUser 
                { 
                    UserName = Input.Email, 
                    Email = Input.Email,
                    EmailConfirmed = true // Automatycznie potwierdzamy email
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("U¿ytkownik utworzy³ nowe konto.");

                    // Przypisz rolê User
                    await _userManager.AddToRoleAsync(user, "User");

                    // Automatyczne logowanie po rejestracji
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
