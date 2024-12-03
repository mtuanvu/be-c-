using five_birds_be.Data;
using five_birds_be.Dto;
using five_birds_be.DTO.Request;
using five_birds_be.DTO.Response;
using five_birds_be.Jwt;
using five_birds_be.Models;
using five_birds_be.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace five_birds_be.Services
{
    public class UserService
    {
        private DataContext _dataContext;
        private JwtService _jservice;
        private IMemoryCache _cache;
        private IHttpContextAccessor _httpContextAccessor;
        private EmailService _emailService;
        public UserService(DataContext dataContext, JwtService jservice, IHttpContextAccessor httpContextAccessor, EmailService emailService, IMemoryCache cache)
        {
            _dataContext = dataContext;
            _jservice = jservice;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _cache = cache;

        }

        public async Task<List<User>> GetUsersPaged(int pageNumber)
        {
            if (pageNumber < 1) pageNumber = 1;

            return await _dataContext.Users
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();
        }

        public async Task<ApiResponse<string>> Register(UserDTO userDTO)
        {
            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == userDTO.Email);
            if (existingUser != null)
            {
                return ApiResponse<string>.Failure(400, "Email already in use.");
            }

            var user = new User
            {
                UserName = userDTO.UserName,
                Email = userDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password),
                Create_at = DateTime.UtcNow
            };

            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();

            var subject = "Account registration successful";
            var body = $@"
                    <html>
                        <body>
                            <p>Hello <strong>{userDTO.UserName}</strong>,</p>

                            <p>Congratulations on successfully registering an account with our system.</p>
                            
                            <p><strong>Your account details:</strong></p>
                            <ul>
                                <li>Email: {userDTO.Email}</li>
                            </ul>
                            
                            <p>Thank you for using our services.</p>
                            
                            <p>Best regards,</p>
                            <p><em>The Support Team</em></p>
                        </body>
                    </html>
                ";

            await _emailService.SendEmailAsync(userDTO.Email, subject, body);

            return ApiResponse<string>.Success(200, null, "Account created successfully and confirmation email sent.");
        }

        public async Task<ApiResponse<UserResponseDTO>> UpdateUser(UserDTO userDTO)
        {
            var userId = _jservice.GetUserIdFromHttpContext();
            var user = await _dataContext.Users.FindAsync(userId);

            if (user == null) return ApiResponse<UserResponseDTO>.Failure(404, " UserId NotFound");

            if (!BCrypt.Net.BCrypt.Verify(userDTO.Password, user.Password))
                return ApiResponse<UserResponseDTO>.Failure(400, "Incorrect password");

            if (!string.IsNullOrEmpty(userDTO.NewPassword))
            {
                if (userDTO.NewPassword.Length < 6)
                    return ApiResponse<UserResponseDTO>.Failure(400, "Password must be at least 6 characters long");

                user.Password = BCrypt.Net.BCrypt.HashPassword(userDTO.NewPassword);
            }

            user.UserName = userDTO.UserName;
            user.Email = userDTO.Email;
            user.Create_at = user.Create_at;
            user.Update_at = DateTime.Now;

            var userResponseDTO = new UserResponseDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                Create_at = user.Create_at,
                Update_at = user.Update_at,
            };

            await _dataContext.SaveChangesAsync();
            return ApiResponse<UserResponseDTO>.Success(200, userResponseDTO, "Updata user success");
        }

        public async Task<ApiResponse<string>> Login(UserLoginDTO userDTO)
        {
            var user = await _dataContext.Users
            .FirstOrDefaultAsync(u => u.UserName == userDTO.UserName);

            if (user == null) return ApiResponse<string>.Failure(400, "Incorrect username");

            var passwordMatch = BCrypt.Net.BCrypt.Verify(userDTO.Password, user.Password);

            if (!passwordMatch) return ApiResponse<string>.Failure(400, "Incorrect password");

            var token = _jservice.GenerateJwtToken(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("token", token, cookieOptions);

            return ApiResponse<string>.Success(200, token, "User logged in successfully.");
        }

        public async Task<ApiResponse<int>> GetUserById()
        {
            var userId = _jservice.GetUserIdFromHttpContext();
            if (userId == null) return ApiResponse<int>.Failure(400, "userid not found");

            return ApiResponse<int>.Success(200, userId.Value, "get User id success");
        }
        public async Task<ApiResponse<string>> ForgotPassword(ForgotPasswordRequest request)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null) return ApiResponse<string>.Failure(404, "Email does not exist.");

            var otp = new Random().Next(100000, 999999).ToString();

            var hashedOtp = BCrypt.Net.BCrypt.HashPassword(otp);
            _cache.Set(user.Email, hashedOtp, TimeSpan.FromMinutes(5));

            var subject = "Reset Password OTP";
            var body = $"Your OTP code to reset your password is: {otp}. This code will expire in 5 minutes.";
            await _emailService.SendEmailAsync(user.Email, subject, body);

            return ApiResponse<string>.Success(200, null, "An OTP has been sent to your email.");
        }

        public async Task<ApiResponse<string>> VerifyOtpAndResetPassword(VerifyOtpRequest request)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return ApiResponse<string>.Failure(404, "Email does not exist.");

            if (!_cache.TryGetValue(request.Email, out string cachedHashedOtp))
            {
                return ApiResponse<string>.Failure(400, "OTP has expired.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Otp, cachedHashedOtp))
            {
                return ApiResponse<string>.Failure(400, "Invalid OTP.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.Password = hashedPassword;

            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();

            _cache.Remove(request.Email);

            return ApiResponse<string>.Success(200, null, "Password has been successfully reset.");
        }


        private string GenerateRandomPassword()
        {
            var random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            return new string(Enumerable.Repeat(chars, 10)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());
        }

        public User ConvertToEntity(UserResponseDTO userResponseDTO)
        {
            return new User
            {
                UserName = userResponseDTO.UserName,
                Email = userResponseDTO.Email,
            };
        }


    }
}