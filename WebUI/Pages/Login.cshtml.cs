namespace WebUI.Pages
{
    public class UserLoginModel : PageModel
    {
        private readonly IEvaluatorProjectUseCases _evaluatorProjectUseCases;
        private readonly Serilog.ILogger _logger;
        private readonly ValidateCredentialService _validateCredentialService;
        private readonly IApplicationService _applicationService;

        public UserLoginModel(IEvaluatorProjectUseCases evaluatorProjectUseCases,
            Serilog.ILogger logger, ValidateCredentialService validateCredentialService, IApplicationService applicationService)
        {
            _evaluatorProjectUseCases = evaluatorProjectUseCases;
            _logger = logger;
            _validateCredentialService = validateCredentialService;
            _applicationService = applicationService;
        }

        [BindProperty]
        public string? UserId { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        public static string? UserError { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Password))
                    return Page();

                var isValid = await _validateCredentialService.Validate(new { UserId, Password });

                if (!isValid)
                {
                    UserError = "The credentials provided are not valid!";
                    return Page();
                }

                var user = (await _evaluatorProjectUseCases.GetEmployeesAsync(new { EmployeeId = UserId })).FirstOrDefault();
                var employeePermissions = (await _evaluatorProjectUseCases.GetEmployeePermissionsAsync(user.Id)).ToList();

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new AuthenticationProperties { ExpiresUtc = DateTimeOffset.UtcNow.AddDays(-1) });

                var evaluator = (await _evaluatorProjectUseCases.IsEmployeeEvaluatorAsync(new { EmployeeId = user.Id })) ? "Evaluator" : "";
                var admin = employeePermissions.Contains(nameof(Permissions.MANAGE_EVALUATION_TYPES)) ? "Admin" : "";

                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, evaluator),
                    new Claim(ClaimTypes.Role, admin),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                    new AuthenticationProperties { ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8), IsPersistent = true });


                return LocalRedirect(Url.Content("~/"));
            }
            catch (Exception exception)
            {
                var type = exception.GetType();
                var message = exception.Message;
                var trace = exception.StackTrace;

                UserError = "Oops, we're sorry. Something has gone wrong. Please contact IT Department.";
                _logger.Error("\n Type: {type} \n Message: {message} \n Stack Trace: {trace} \n", type, message, trace);

                Log.CloseAndFlush();

                return LocalRedirect(Url.Content("~/login"));
            }
        }
    }
}
