using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCourseManagement2_Business.Interfaces;
using WebCourseManagement2_Models.RequestModels.BaiVietRequests;
using WebCourseManagement2_Models.RequestModels.BannerRequest;
using WebCourseManagement2_Models.RequestModels.BaoCaoRequests;
using WebCourseManagement2_Models.RequestModels.LoaiKhoaHocRequests;
using WebCourseManagement2_Models.ResponseModels.DataBaiViet;
using WebCourseManagement2_Models.ResponseModels.DataBanner;
using WebCourseManagement2_Models.ResponseModels.DataBaoCao;
using WebCourseManagement2_Models.ResponseModels.DataLoaiKhoaHoc;
using WebCourseManagement2_Models.Responses;

namespace WebCourseManagement2_API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILoaiKhoaHocService _loaiKhoaHocService;
        private readonly IBaiVietService _baivietService;
        private readonly IBannerService _bannerService;
        private readonly IBaoCaoService _baoCaoService;
        public AdminController(IUserService userService, ILoaiKhoaHocService loaiKhoaHocService, IBaiVietService baiVietService, IBannerService bannerService, IBaoCaoService baoCaoService)
        {
            _userService = userService;
            _loaiKhoaHocService = loaiKhoaHocService;
            _baivietService = baiVietService;
            _bannerService = bannerService;
            _baoCaoService = baoCaoService;
        }
        [HttpPut("KhoaTaiKhoanNguoiDung/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> KhoaTaiKhoanNguoiDung([FromRoute] int id)
        {
            return Ok(await _userService.KhoaTaiKhoanNguoiDung(id));
        }
        [HttpPut("MoKhoaTaiKhanNguoiDung/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> MoKhoaTaiKhanNguoiDung([FromRoute] int id)
        {
            return Ok(await _userService.MoKhoaTaiKhanNguoiDung(id));
        }
        [HttpPost("ThemLoaiKhoaHoc")]
        [Authorize(Roles = "Admin, Mod")]
        public async Task<IActionResult> ThemLoaiKhoaHoc([FromBody] Request_ThemLoaiKhoaHoc request)
        {
            return Ok(await _loaiKhoaHocService.ThemLoaiKhoaHoc(request));
        }
        [HttpPut("CapNhatThongTinLoaiKhoaHoc")]
        [Authorize(Roles = "Admin, Mod")]
        public async Task<IActionResult> CapNhatThongTinLoaiKhoaHoc([FromBody] Request_SuaLoaiKhoaHoc request)
        {
            return Ok(await _loaiKhoaHocService.CapNhatThongTinLoaiKhoaHoc(request));
        }
        [HttpDelete("XoaLoaiKhoaHoc/{loaiKhoaHocId}")]
        [Authorize(Roles = "Admin, Mod")]
        public async Task<IActionResult> XoaLoaiKhoaHoc([FromRoute] int loaiKhoaHocId)
        {
            return Ok(await _loaiKhoaHocService.XoaLoaiKhoaHoc(loaiKhoaHocId));
        }
        [HttpPut("DuyetBaiViet/{baiVietId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DuyetBaiViet([FromRoute] int baiVietId)
        {
            return Ok(await _baivietService.DuyetBaiViet(baiVietId));
        }
        [HttpPost("TaoBanner")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TaoBanner([FromForm] Request_TaoBanner request)
        {
            return Ok(await _bannerService.TaoBanner(request));
        }
        [HttpPost("ThemLoaiBaiViet")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ThemLoaiBaiViet([FromBody] Request_ThemLoaiBaiViet request)
        {
            return Ok(await _baivietService.ThemLoaiBaiViet(request));
        }
        [HttpPut("SuaLoaiBaiViet")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SuaLoaiBaiViet([FromBody] Request_SuaLoaiBaiViet request)
        {
            return Ok(await _baivietService.SuaLoaiBaiViet(request));
        }
        [HttpDelete("XoaLoaiBaiViet/{loaiBaiVietId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> XoaLoaiBaiViet([FromRoute] int loaiBaiVietId)
        {
            return Ok(await _baivietService.XoaLoaiBaiViet(loaiBaiVietId));
        }
        [HttpPut("DuyetBaoCao")]
        [Authorize(Roles = "Admin")]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> DuyetBaoCao([FromForm] Request_DuyetBaoCao request)
        {
            return Ok(await _baoCaoService.DuyetBaoCao(request));
        }
        [HttpDelete("XoaBanner/{bannerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> XoaBanner([FromRoute] int bannerId)
        {
            return Ok(await _bannerService.XoaBanner(bannerId));
        }
        [HttpGet("GetAllBaiVietChuaDuocDuyet")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBaiVietChuaDuocDuyet()
        {
            return Ok(await _baivietService.GetAllBaiVietChuaDuocDuyet());
        }
        [HttpPut("TuChoiBaiViet/{baiVietId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TuChoiBaiViet([FromRoute] int baiVietId)
        {
            return Ok(await _baivietService.TuChoiBaiViet(baiVietId));
        }
    }
}
