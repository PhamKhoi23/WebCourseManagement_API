using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Business.Interfaces;
using WebCourseManagement2_Models.Converters;
using WebCourseManagement2_Models.DataContexts;
using WebCourseManagement2_Models.RequestModels.AuthRequests;
using WebCourseManagement2_Models.ResponseModels.DataNguoiDung;
using WebCourseManagement2_Models.Responses;
using WebCourseManagement2_Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebCourseManagement2_Models.Entities;
using BcryptNet = BCrypt.Net.BCrypt;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using WebCourseManagement2_Commons.DefaultConstants;

namespace WebCourseManagement2_Business.Implements
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseNguoiDung> _responseObject;
        private readonly ResponseObject<DataResponseToken> _responseTokenObject;
        private readonly NguoiDungConverter _converter;
        private readonly IConfiguration _configuration;
        public AuthService(AppDbContext context, ResponseObject<DataResponseNguoiDung> responseObject, ResponseObject<DataResponseToken> responseTokenObject, NguoiDungConverter converter, IConfiguration configuration)
        {
            _context = context;
            _responseObject = responseObject;
            _responseTokenObject = responseTokenObject;
            _converter = converter;
            _configuration = configuration;
        }
        #region Chức năng GenerateRefreshToken
        public string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }
        #endregion
        #region Chức năng gửi email
        public string SendEmail(EmailTo emailTo)
        {
            if (!Utilities.IsValidEmail(emailTo.To))
            {
                return "Định dạng email không hợp lệ";
            }
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("minhquantb00@gmail.com", "jvztzxbtyugsiaea"),
                EnableSsl = true
            };
            try
            {
                var message = new MailMessage();
                message.From = new MailAddress("minhquantb00@gmail.com");
                message.To.Add(emailTo.To);
                message.Subject = emailTo.Subject;
                message.Body = emailTo.Content;
                message.IsBodyHtml = true;
                smtpClient.Send(message);

                return "Xác nhận gửi email thành công, lấy mã để xác thực";
            }
            catch (Exception ex)
            {
                return "Lỗi khi gửi email: " + ex.Message;
            }
        }
        #endregion
        #region Chức năng GenerateCodeActive
        private int GenerateCodeActive()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }
        #endregion
        #region Chức năng đăng ký
        public async Task<ResponseObject<DataResponseNguoiDung>> DangKy(Request_DangKy request)
        {
            var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.TaiKhoan.Equals(request.TaiKhoan));
            if(string.IsNullOrWhiteSpace(request.TaiKhoan) || string.IsNullOrWhiteSpace(request.MatKhau) || string.IsNullOrWhiteSpace(request.HoVaTen) || string.IsNullOrWhiteSpace(request.SoDienThoai) || string.IsNullOrWhiteSpace(request.Email))
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng điền đầy đủ thông tin", null);
            }
            if (!Utilities.IsValidEmail(request.Email))
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Định dạng email không hợp lệ", null);
            }
            if (!Utilities.IsValidPhoneNumber(request.SoDienThoai))
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Định dạng số điện thoại không hợp lệ", null);
            }
            if(nguoiDung != null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Tên tài khoản đã được sử dụng", null);
            }
            if(_context.nguoiDungs.Any(x => x.Email.Equals(request.Email)))
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Email đã được sử dụng", null);
            }
            if (_context.nguoiDungs.Any(x => x.Email.Equals(request.SoDienThoai)))
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Số điện thoại đã được sử dụng", null);
            }
            NguoiDung item = new NguoiDung
            {
                AnhDaiDien = "https://scontent.xx.fbcdn.net/v/t1.15752-9/423737674_1480040725879365_4588731426501580823_n.png?stp=dst-png_s206x206&_nc_cat=101&ccb=1-7&_nc_sid=5f2048&_nc_eui2=AeGxnP82_YkVFLWwGazF7yI5KZjnfOEJczUpmOd84QlzNRfr9gdH6syoper3Yh0oOyo5y2ipmbd59Dwk6Ozyz536&_nc_ohc=eJwRiZR1zw0AX94Q9w9&_nc_ad=z-m&_nc_cid=0&_nc_ht=scontent.xx&oh=03_AdSAsCug3mmQNHjjqLBlE4vswrK6MxAOKWbXdPwaEh6CfA&oe=66109EB0",
                IsActive = true,
                DaKhoa = false,
                Email = request.Email,
                GioiTinh = request.GioiTinh,
                TaiKhoan = request.TaiKhoan,
                HoVaTen = request.HoVaTen,
                MatKhau = BcryptNet.HashPassword(request.MatKhau),
                NgaySinh = request.NgaySinh,
                SoDienThoai = request.SoDienThoai,
                QuyenHanId = 3,
                ThoiGianTao = DateTime.Now,
                TrangThaiNguoiDungId = 1,
            };
             _context.nguoiDungs.Add(item);
            _context.SaveChanges();
            XacNhanEmail xacNhanEmail = new XacNhanEmail
            {
                DaXacNhan = false,
                MaXacNhan = "MyBugs_" + GenerateCodeActive().ToString(),
                NguoiDungId = item.Id,
                ThoiGianHetHan = DateTime.Now.AddHours(8)
            };
             _context.xacNhanEmails.Add(xacNhanEmail);
            _context.SaveChanges();
            string message = SendEmail(new EmailTo
            {
                To = item.Email,
                Subject = "Nhận mã xác nhận để đăng ký tài khoản tại đây:",
                Content = $"Mã xác nhận của bạn là: {xacNhanEmail.MaXacNhan}"
            });
            return _responseObject.ResponseSuccess("Đăng ký tài khoản thành công", _converter.EntityToDTO(item));
        }
        #endregion
        #region GenerateAccessToken
        public DataResponseToken GenerateAccessToken(NguoiDung nguoiDung)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection(Constants.AppSettingKeys.AUTH_SECRET).Value!);
            var role = _context.quyenHans.SingleOrDefault(x => x.Id == nguoiDung.QuyenHanId);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim("Id", nguoiDung.Id.ToString()),
                    new Claim("Email", nguoiDung.Email),
                    new Claim("Name", nguoiDung.HoVaTen),
                    new Claim("Username", nguoiDung.TaiKhoan),
                    new Claim("Image", nguoiDung.AnhDaiDien),
                    new Claim(ClaimTypes.Role, role?.MaQuyenHan ?? "")
                }),
                Expires = DateTime.Now.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            RefreshToken rf = new RefreshToken
            {
                Token = refreshToken,
                ThoiGianHetHan = DateTime.Now.AddHours(10),
                NguoiDungId = nguoiDung.Id
            };

            _context.refreshTokens.Add(rf);
            _context.SaveChanges();

            DataResponseToken data = new DataResponseToken
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                DataResponseNguoiDung = _converter.EntityToDTO(nguoiDung)
            };
            return data;
        }
        #endregion
        #region Chức năng đăng nhập
        public async Task<ResponseObject<DataResponseToken>> DangNhap(Request_DangNhap request)
        {
            var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.TaiKhoan.Equals(request.TaiKhoan));
            if(string.IsNullOrWhiteSpace(request.TaiKhoan) || string.IsNullOrWhiteSpace(request.MatKhau))
            {
                return _responseTokenObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng điền đầy đủ thông tin", null);
            }
            if(nguoiDung == null)
            {
                return _responseTokenObject.ResponseError(StatusCodes.Status400BadRequest, "Sai tên tài khoản", null);
            }
            bool checkPass = BcryptNet.Verify(request.MatKhau, nguoiDung.MatKhau);
            if(!checkPass)
            {
                return _responseTokenObject.ResponseError(StatusCodes.Status400BadRequest, "Mật khẩu không chính xác", null);
            }
            if(nguoiDung.TrangThaiNguoiDungId == 1)
            {
                return _responseTokenObject.ResponseError(StatusCodes.Status400BadRequest, "Tài khoản người dùng chưa được kích hoạt! Vui lòng kích hoạt tài khoản để sử dụng", null);
            }
            if(nguoiDung.IsActive == false || nguoiDung.DaKhoa == true)
            {
                return _responseTokenObject.ResponseError(StatusCodes.Status400BadRequest, "Tài khoản đã bị khóa hoặc bị vô hiệu! Vui lòng thử lại", null);
            }
            return _responseTokenObject.ResponseSuccess("Đăng nhập thành công", GenerateAccessToken(nguoiDung));
        }
        #endregion
        #region Chức năng xác thực đăng ký tài khoản
        public async Task<ResponseObject<DataResponseNguoiDung>> XacNhanDangKyTaiKhoan(Request_XacNhanDangKyTaiKhoan request)
        {
            var confirmEmail = await _context.xacNhanEmails.SingleOrDefaultAsync(x => x.MaXacNhan.Equals(request.MaXacNhan));
            if (confirmEmail == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Mã xác nhận không chính xác", null);
            }
            var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.Id == confirmEmail.NguoiDungId);
            if (confirmEmail.ThoiGianHetHan < DateTime.Now)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mã xác nhận đã hết hạn", null);
            }
            nguoiDung.TrangThaiNguoiDungId = 2;
            confirmEmail.DaXacNhan = true;
            _context.SaveChanges();

            return _responseObject.ResponseSuccess("Kích hoạt tài khoản thành công", _converter.EntityToDTO(nguoiDung));
        }
        #endregion
        #region Chức năng đổi mật khẩu
        public async Task<string> DoiMatKhau(int nguoiDungId, Request_DoiMatKhau request)
        {
            var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.Id == nguoiDungId);
            if (!BcryptNet.Verify(request.MatKhauCu, nguoiDung.MatKhau))
            {
                return "Mật khẩu không chính xác";
            }
            if (BcryptNet.Verify(request.MatKhauMoi, nguoiDung.MatKhau))
            {
                return "Bạn đã đặt mật khẩu này trước đó! Vui lòng đặt mật khẩu khác";
            }
            if(!request.MatKhauMoi.Equals(request.XacNhanMatKhauMoi))
            {
                return "Mật khẩu không trùng khớp";
            }
            nguoiDung.MatKhau = BcryptNet.HashPassword(request.MatKhauMoi);
            _context.SaveChanges();
            return "Đổi mật khẩu thành công";
        }
        #endregion
        #region Chức năng RenewAccessToken
        public ResponseObject<DataResponseToken> RenewAccessToken(Request_RenewToken request)
        {
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var secretKey = _configuration.GetSection("AppSettings:SecretKey").Value;

                var tokenValidation = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey))
                };

                var tokenAuthentication = jwtTokenHandler.ValidateToken(request.AccessToken, tokenValidation, out var validatedToken);
                if (!(validatedToken is JwtSecurityToken jwtSecurityToken) || jwtSecurityToken.Header.Alg != SecurityAlgorithms.HmacSha256)
                {
                    return _responseTokenObject.ResponseError(StatusCodes.Status400BadRequest, "Token không hợp lệ", null);
                }
                var refreshToken = _context.refreshTokens.SingleOrDefault(x => x.Token.Equals(request.RefreshToken));
                if (refreshToken == null)
                {
                    return _responseTokenObject.ResponseError(StatusCodes.Status404NotFound, "RefreshToken không tồn tại trong database", null);
                }
                if (refreshToken.ThoiGianHetHan < DateTime.Now)
                {
                    return _responseTokenObject.ResponseError(StatusCodes.Status401Unauthorized, "RefreshToken đã hết hạn", null);
                }
                var user = _context.nguoiDungs.SingleOrDefault(x => x.Id == refreshToken.NguoiDungId);
                if (user == null)
                {
                    return _responseTokenObject.ResponseError(StatusCodes.Status404NotFound, "Người dùng không tồn tại", null);
                }
                var newToken = GenerateAccessToken(user);
                return _responseTokenObject.ResponseSuccess("Token đã được làm mới thành công", newToken);
            }
            catch (SecurityTokenValidationException ex)
            {
                return _responseTokenObject.ResponseError(StatusCodes.Status400BadRequest, "Lỗi xác thực token: " + ex.Message, null);
            }
            catch (Exception ex)
            {
                return _responseTokenObject.ResponseError(StatusCodes.Status500InternalServerError, "Lỗi không xác định: " + ex.Message, null);
            }
        }
        #endregion
        #region Chức năng xác nhận quên mật khẩu
        public async Task<string> XacNhanQuenMatKhau(Request_XacNhanQuenMatKhau request)
        {
            var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.Email.Equals(request.Email));
            if(nguoiDung == null)
            {
                return "Người dùng không tồn tại";
            }
            var xacNhanEmailList = _context.xacNhanEmails.Where(x => x.NguoiDungId == nguoiDung.Id).ToList();
            _context.xacNhanEmails.RemoveRange(xacNhanEmailList);
            _context.SaveChanges();
            XacNhanEmail xacNhanEmail = new XacNhanEmail
            {
                DaXacNhan = false,
                MaXacNhan = "MyBugs_" + GenerateCodeActive(),
                NguoiDungId = nguoiDung.Id,
                ThoiGianHetHan = DateTime.Now.AddMinutes(15)
            };
            _context.xacNhanEmails.Add(xacNhanEmail);
            _context.SaveChanges();
            string message = SendEmail(new EmailTo
            {
                To = nguoiDung.Email,
                Subject = "Nhận mã xác nhận để tạo mật khẩu mới tại đây: ",
                Content = $"Mã xác nhận của bạn là: {xacNhanEmail.MaXacNhan}"
            });
            return "Mã xác nhận đã gửi về email của bạn! Vui lòng kiểm tra email";
        }
        #endregion
        #region Chức năng tạo mật khẩu mới
        public async Task<string> TaoMatKhauMoi(Request_TaoMatKhauMoi request)
        {
            var xacNhanEmail = await _context.xacNhanEmails.SingleOrDefaultAsync(x => x.MaXacNhan.Equals(request.MaXacNhan));
            if(xacNhanEmail == null)
            {
                return "Mã xác nhận không chính xác";
            }
            var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.Id == xacNhanEmail.NguoiDungId);
            if (BcryptNet.Verify(request.MatKhauMoi, nguoiDung.MatKhau))
            {
                return "Mật khẩu đã được sử dụng trước đó";
            }
            if (!request.MatKhauMoi.Equals(request.XacNhanMatKhauMoi))
            {
                return "Mật khẩu không trùng khớp";
            }
            nguoiDung.MatKhau = BcryptNet.HashPassword(request.MatKhauMoi);
            xacNhanEmail.DaXacNhan = true;
            _context.SaveChanges();
            return "Tạo mật khẩu mới thành công";
        }
        #endregion
    }
}
