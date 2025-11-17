using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase<AccountController>
    {
        private readonly SessionManager _sessionManager;
        private readonly SignInManager _signInManager;
        private readonly TokenValidationParametersFactory _tokenValidationParametersFactory;
        private readonly TokenProviderOptionsFactory _tokenProviderOptionsFactory;
        private readonly IConfiguration _appConfiguration;
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="localizer">The localizer.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="tokenValidationParametersFactory">The token validation parameters factory.</param>
        /// <param name="tokenProviderOptionsFactory">The token provider options factory.</param>
        /// <param name="jwtTokenService">The JWT token service.</param>
        public AccountController(
                            SignInManager signInManager,
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            IMapper mapper,
                            IUserService userService,
                            TokenValidationParametersFactory tokenValidationParametersFactory,
                            TokenProviderOptionsFactory tokenProviderOptionsFactory,
                            IJwtTokenService jwtTokenService) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._sessionManager = new SessionManager(this._session);
            this._signInManager = signInManager;
            this._tokenProviderOptionsFactory = tokenProviderOptionsFactory;
            this._tokenValidationParametersFactory = tokenValidationParametersFactory;
            this._appConfiguration = configuration;
            this._userService = userService;
            this._jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// Login Method - Authenticates user with username and password
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AuthenticateUserRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new ApiResult<object>(Status.Error, errors, "Invalid request data"));
                }

                // Authenticate user
                var authResult = await _userService.AuthenticateUserAsync(model.Username, model.Password);

                if (authResult != LoginResult.Success)
                {
                    return Unauthorized(new ApiResult<object>(Status.Error, null, "Invalid username or password"));
                }

                // Get user details
                var user = await _userService.GetUserByUsernameAsync(model.Username);

                if (user == null || !user.IsActive)
                {
                    var message = user == null ? "User not found" : "Account is deactivated. Please contact administrator.";
                    return Unauthorized(new ApiResult<object>(Status.Error, null, message));
                }

                // Set session data (keeping for backward compatibility)
                this._session.SetString("HasSession", "Exist");
                this._session.SetString("UserName", user.Username);
                this._session.SetString("UserId", user.UserId);
                this._session.SetString("Role", user.Role);

                // Generate JWT token
                var jwtToken = _jwtTokenService.GenerateToken(user);

                // Create response model
                var response = new LoginResponseModel
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Role = user.Role,
                    DepartmentId = user.DepartmentId,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    IsActive = user.IsActive,
                    Token = jwtToken
                };

                return Ok(new ApiResult<LoginResponseModel>(Status.Success, response, "Login successful"));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "Error during login for user: {Username}", model.Username);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred during login. Please try again."));
            }
        }

        /// <summary>
        /// Get User Information by Username
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetUser(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return BadRequest(new ApiResult<object>(Status.Error, null, "Username is required"));
                }

                var user = await _userService.GetUserByUsernameAsync(username);

                if (user == null)
                {
                    return NotFound(new ApiResult<object>(Status.Error, null, "User not found"));
                }

                var response = new LoginResponseModel
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Role = user.Role,
                    DepartmentId = user.DepartmentId,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    IsActive = user.IsActive,
                    Token = null
                };

                return Ok(new ApiResult<LoginResponseModel>(Status.Success, response, "User found"));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "Error getting user: {Username}", username);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while retrieving user information"));
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateCredentials([FromBody] AuthenticateUserRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new ApiResult<object>(Status.Error, errors, "Invalid request data"));
                }

                var isValid = await _userService.ValidateUserCredentialsAsync(model.Username, model.Password);

                if (!isValid)
                {
                    return Ok(new ApiResult<bool>(Status.Success, false, "Invalid credentials"));
                }

                var user = await _userService.GetUserByUsernameAsync(model.Username);
                var response = new LoginResponseModel
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Role = user.Role,
                    DepartmentId = user.DepartmentId,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    IsActive = user.IsActive,
                    Token = null
                };

                return Ok(new ApiResult<LoginResponseModel>(Status.Success, response, "Credentials valid"));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "Error validating credentials for user: {Username}", model.Username);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred during validation"));
            }
        }

        /// <summary>
        /// Sign Out current account
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignOutUser()
        {
            try
            {
                await this._signInManager.SignOutAsync();

                // Clear session data
                this._session.Clear();

                return Ok(new ApiResult<object>(Status.Success, null, "Signed out successfully"));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "Error during sign out");
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred during sign out"));
            }
        }

        /// <summary>
        /// Test JWT Protected Endpoint
        /// </summary>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var department = User.FindFirst("Department")?.Value;

                var userInfo = new
                {
                    UserId = userId,
                    Username = username,
                    Role = role,
                    Email = email,
                    Department = department,
                    IsAuthenticated = User.Identity.IsAuthenticated,
                    AuthenticationType = User.Identity.AuthenticationType
                };

                return Ok(new ApiResult<object>(Status.Success, userInfo, "Profile retrieved successfully"));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while retrieving profile"));
            }
        }
    }
}