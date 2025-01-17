using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCourseManagement2_Business.Interfaces;
using WebCourseManagement2_Models.RequestModels.AuthRequests;
using WebCourseManagement2_Models.Responses;

namespace WebCourseManagement2_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("DangKy")]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> DangKy([FromForm] Request_DangKy request)
        {
            var result = await _authService.DangKy(request);

            switch (result.Status)
            {
                case 200:
                    return Ok(result);
                case 404:
                    return NotFound(result);
                case 400:
                    return BadRequest(result);
                case 401:
                    return Unauthorized(result);
                case 403:
                    return Forbid("Bạn không có quyền thực hiện chức năng này");
                default:
                    return StatusCode(500, result);
            }
        }
        [HttpPost("DangNhap")]
        public async Task<IActionResult> DangNhap([FromBody] Request_DangNhap request)
        {
            var result = await _authService.DangNhap(request);
            switch (result.Status)
            {
                case 200:
                    return Ok(result);
                case 404:
                    return NotFound(result);
                case 400:
                    return BadRequest(result);
                case 401:
                    return Unauthorized(result);
                case 403:
                    return Forbid("Bạn không có quyền thực hiện chức năng này");
                default:
                    return StatusCode(500, result);
            }
        }

        [HttpPut("XacThucDangKyTaiKhoan")]
        public async Task<IActionResult> XacThucDangKyTaiKhoan([FromBody] Request_XacNhanDangKyTaiKhoan request)
        {
            var result = await _authService.XacNhanDangKyTaiKhoan(request);
            switch (result.Status)
            {
                case 200:
                    return Ok(result);
                case 404:
                    return NotFound(result);
                case 400:
                    return BadRequest(result);
                case 401:
                    return Unauthorized(result);
                case 403:
                    return Forbid("Bạn không có quyền thực hiện chức năng này");
                default:
                    return StatusCode(500, result);
            }
        }

        [HttpPut("DoiMatKhau")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DoiMatKhau([FromBody] Request_DoiMatKhau request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _authService.DoiMatKhau(id, request);
            return Ok(result);
        }

        [HttpPost("XacNhanQuenMatKhau")]
        public async Task<IActionResult> XacNhanQuenMatKhau([FromBody] Request_XacNhanQuenMatKhau request)
        {
            var result = await _authService.XacNhanQuenMatKhau(request);
            return Ok(result);
        }
        [HttpPut("TaoMatKhauMoi")]
        public async Task<IActionResult> TaoMatKhauMoi([FromBody] Request_TaoMatKhauMoi request)
        {
            return Ok(await _authService.TaoMatKhauMoi(request));
        }
    }
}
