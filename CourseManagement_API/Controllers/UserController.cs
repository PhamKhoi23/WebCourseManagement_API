using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCourseManagement_Models.RequestModels.KhoaHocRequests;
using WebCourseManagement2_Business.Interfaces;
using WebCourseManagement2_Models.RequestModels.BaiHocRequests;
using WebCourseManagement2_Models.RequestModels.BaiVietRequests;
using WebCourseManagement2_Models.RequestModels.BaoCaoRequests;
using WebCourseManagement2_Models.RequestModels.BinhLuanBaiHocRequest;
using WebCourseManagement2_Models.RequestModels.ChuongHocRequests;
using WebCourseManagement2_Models.RequestModels.InputRequests;
using WebCourseManagement2_Models.RequestModels.KhoaHocNguoiDungRequests;
using WebCourseManagement2_Models.RequestModels.KhoaHocRequests;
using WebCourseManagement2_Models.RequestModels.NguoiDungRequests;
using WebCourseManagement2_Models.RequestModels.NguoiDungThichBinhLuanBaiHocRequests;
using WebCourseManagement2_Models.RequestModels.NguoiDungThichBinhLuanBaiVietRequests;
using WebCourseManagement2_Models.RequestModels.TestCaseRequests;
using WebCourseManagement2_Models.RequestModels.ThichBaiVietRequests;
using WebCourseManagement2_Models.ResponseModels.DataBaiHoc;
using WebCourseManagement2_Models.ResponseModels.DataBaiViet;
using WebCourseManagement2_Models.ResponseModels.DataBanner;
using WebCourseManagement2_Models.ResponseModels.DataBaoCao;
using WebCourseManagement2_Models.ResponseModels.DataBinhLuanBaiHoc;
using WebCourseManagement2_Models.ResponseModels.DataChuongHoc;
using WebCourseManagement2_Models.ResponseModels.DataHoaDon;
using WebCourseManagement2_Models.ResponseModels.DataKhoaHoc;
using WebCourseManagement2_Models.ResponseModels.DataKhoaHocCuaNguoiDung;
using WebCourseManagement2_Models.ResponseModels.DataLoaiKhoaHoc;
using WebCourseManagement2_Models.ResponseModels.DataNguoiDung;
using WebCourseManagement2_Models.ResponseModels.DataNguoiDungHoanThanhBaiHoc;
using WebCourseManagement2_Models.ResponseModels.DataTestCase;
using WebCourseManagement2_Models.Responses;
using WebCourseManagement2_Repositories.HandlePagination;
using Request_TraLoiBinhLuanBaiViet = WebCourseManagement2_Models.RequestModels.BaiVietRequests.Request_TraLoiBinhLuanBaiViet;

namespace WebCourseManagement2_API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILoaiKhoaHocService _loaiKhoaHocService;
        private readonly IKhoaHocService _khoaHocService;
        private readonly IChuongHocService _chuongHocService;
        private readonly IBaiHocService _baiHocService;
        private readonly IVNPayService _vnpayService;
        private readonly IBinhLuanBaiHocService _binhLuanBaiHocService;
        private readonly IThichBinhLuanBaiHocService _thichBinhLuanBaiHocService;
        private readonly IBaiVietService _baiVietService;
        private readonly IThichBinhLuanBaiVietService _thichBinhLuanBaiVietService;
        private readonly INguoiDungThichBaiVietService _thichBaiVietService;
        private readonly IKhoaHocNguoiDungService _khoaHocNguoiDungService;
        private readonly ITestCaseService _testCaseService;
        private readonly IBaoCaoService _baoCaoService;
        private readonly IBannerService _bannerService;

        public UserController(IUserService userService, ILoaiKhoaHocService loaiKhoaHocService, IKhoaHocService khoaHocService, IChuongHocService chuongHocService, IBaiHocService baiHocService, IVNPayService vnpayService, IBinhLuanBaiHocService binhLuanBaiHocService, IThichBinhLuanBaiHocService thichBinhLuanBaiHocService, IBaiVietService baiVietService, IThichBinhLuanBaiVietService thichBinhLuanBaiVietService, INguoiDungThichBaiVietService thichBaiVietService, IKhoaHocNguoiDungService khoaHocNguoiDungService, ITestCaseService testCaseService,IBaoCaoService baoCaoService, IBannerService bannerService)
        {
            _userService = userService;
            _loaiKhoaHocService = loaiKhoaHocService;
            _khoaHocService = khoaHocService;
            _chuongHocService = chuongHocService;
            _baiHocService = baiHocService;
            _vnpayService = vnpayService;
            _binhLuanBaiHocService = binhLuanBaiHocService;
            _thichBinhLuanBaiHocService = thichBinhLuanBaiHocService;
            _baiVietService = baiVietService;
            _thichBinhLuanBaiVietService = thichBinhLuanBaiVietService;
            _thichBaiVietService = thichBaiVietService;
            _khoaHocNguoiDungService = khoaHocNguoiDungService;
            _testCaseService = testCaseService;
            _baoCaoService = baoCaoService;
            _bannerService = bannerService;
        }
        [HttpGet("GetAllsNguoiDung")]
        public async Task<IActionResult> GetAllsNguoiDung()
        {
            return Ok(await _userService.GetAlls());
        }

        [HttpGet("GetNguoiDungById/{id}")]
        public async Task<IActionResult> GetNguoiDungById([FromRoute] int id)
        {
            return Ok(await _userService.GetNguoiDungById(id));
        }
        [HttpPut("CapNhatThongTinNguoiDung")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> CapNhatThongTinNguoiDung([FromForm]Request_CapNhatThongTinNguoiDung request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _userService.CapNhatThongTinNguoiDung(id, request));
        }
        [HttpDelete("NguoiDungXoaTaiKhoan")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> NguoiDungXoaTaiKhoan()
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _userService.NguoiDungXoaTaiKhoan(id));
        }
        [HttpGet("GetAllsLoaiKhoaHoc")]
        public async Task<IActionResult> GetAllsLoaiKhoaHoc(int pageSize = 10, int pageNumber = 1)
        {
            return Ok(await _loaiKhoaHocService.GetAlls(pageSize, pageNumber));
        }
        [HttpGet("GetAllsKhoaHoc")]
        public async Task<IActionResult> GetAllsKhoaHoc(int pageSize = 10, int pageNumber = 1)
        {
            return Ok(await _khoaHocService.GetAlls(pageSize, pageNumber));
        }
        [HttpGet("GetKhoaHocById/{khoaHocId}")]
        public async Task<IActionResult> GetKhoaHocById([FromRoute] int khoaHocId)
        {
            return Ok(await _khoaHocService.GetKhoaHocById(khoaHocId));
        }
        [HttpPost("ThemKhoaHoc")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> ThemKhoaHoc([FromForm] Request_ThemKhoaHoc request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _khoaHocService.ThemKhoaHoc(id, request));
        }
        [HttpPut("SuaThongTinKhoaHoc")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> SuaThongTinKhoaHoc([FromForm] Request_SuaThongTinKhoaHoc request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _khoaHocService.SuaThongTinKhoaHoc(id, request));
        }
        [HttpDelete("XoaKhoaHoc/{khoaHocId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> XoaKhoaHoc([FromRoute] int khoaHocId)
        {
            return Ok(await _khoaHocService.XoaKhoaHoc(khoaHocId));
        }
        [HttpDelete("XoaChuongHoc/{chuongHocId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> XoaChuongHoc([FromRoute] int chuongHocId)
        {
            return Ok(await _chuongHocService.XoaChuongHoc(chuongHocId));
        }
        [HttpPost("ThemChuongHoc")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> ThemChuongHoc([FromForm]Request_ThemChuongHoc request)
        {
            return Ok(await _chuongHocService.ThemChuongHoc(request));
        }
        [HttpPut("SuaThongTinChuongHoc")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> SuaThongTinChuongHoc([FromBody] Request_SuaThongTinChuongHoc request)
        {
            return Ok(await _chuongHocService.SuaThongTinChuongHoc(request)); 
        }
        [HttpGet("GetChuongHocById/{chuongHocId}")]
        public async Task<IActionResult> GetChuongHocById([FromRoute] int chuongHocId)
        {
            return Ok(await _chuongHocService.GetChuongHocById(chuongHocId));
        }
        [HttpGet("GetAlls")]
        public async Task<IActionResult> GetAllsChuongHoc(int pageSize = 10, int pageNumber = 1)
        {
            return Ok(await _chuongHocService.GetAlls(pageSize, pageNumber));
        }
        [HttpDelete("XoaBaiHoc/{baiHocId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> XoaBaiHoc([FromRoute] int baiHocId)
        {
            return Ok(await _baiHocService.XoaBaiHoc(baiHocId));
        }
        [HttpPut("SuaThongTinBaiHoc")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SuaThongTinBaiHoc([FromBody] Request_CapNhatThongTinBaiHoc request)
        {
            return Ok(await _baiHocService.SuaThongTinBaiHoc(request));
        }
        [HttpPost("ThemBaiHoc")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ThemBaiHoc([FromBody] Request_ThemBaiHoc request)
        {
            return Ok(await _baiHocService.ThemBaiHoc(request));
        }
        [HttpGet("GetBaiHocById/{baiHocId}")]
        public async Task<IActionResult> GetBaiHocById([FromRoute] int baiHocId)
        {
            return Ok(await _baiHocService.GetBaiHocById(baiHocId));
        }
        [HttpGet("GetAllsBaiHoc")]
        public async Task<IActionResult> GetAllsBaiHoc([FromQuery] InputBaiHoc input, int pageSize = 10, int pageNumber = 1)
        {
            return Ok(await _baiHocService.GetAlls(input, pageSize, pageNumber));
        }
        [HttpPost("DangKyKhoaHoc")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DangKyKhoaHoc([FromBody] Request_DangKyKhoaHoc request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _khoaHocService.DangKyKhoaHoc(id, request));
        }
        [HttpPost("TaoDuongDanThanhToan/{hoaDonId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> TaoDuongDanThanhToan([FromRoute] int hoaDonId)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _vnpayService.TaoDuongDanThanhToan(hoaDonId, HttpContext, id));
        }
        [HttpGet("Return")]
        public async Task<IActionResult> Return()
        {
            var vnpayData = HttpContext.Request.Query;

            return Ok(await _vnpayService.VNPayReturn(vnpayData));
        }
        [HttpDelete("XoaBinhLuanBaiHoc/{binhLuanId}")]
        [Authorize(AuthenticationSchemes =  JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> XoaBinhLuanBaiHoc([FromRoute] int binhLuanId)
        {
            return Ok(await _binhLuanBaiHocService.XoaBinhLuan(binhLuanId));
        }
        [HttpPost("TraLoiBinhLuanBaiHoc")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> TraLoiBinhLuan([FromBody] Request_TraLoiBinhLuan request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _binhLuanBaiHocService.TraLoiBinhLuan(id, request));
        }
        [HttpPost("TaoBinhLuanBaiHoc")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> TaoBinhLuanBaiHoc([FromBody] Request_TaoBinhLuanBaiHoc request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _binhLuanBaiHocService.TaoBinhLuan(id, request));
        }
        [HttpPut("SuaBinhLuanBaiHoc")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SuaBinhLuanBaiHoc([FromBody] Request_SuaBinhLuanBaiHoc request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _binhLuanBaiHocService.SuaBinhLuan(id, request));
        }
        [HttpPost("ThichBinhLuanBaiHoc")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ThichBinhLuanBaiHoc([FromBody] Request_ThichBinhLuanBaiHoc request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _thichBinhLuanBaiHocService.ThichBinhLuanBaiHoc(id, request));
        }
        [HttpGet("GetAllsBaiViet")]
        public async Task<IActionResult> GetAllsBaiViet([FromQuery] InputBaiViet? input)
        {
            return Ok(await _baiVietService.GetAlls(input));
        }
        [HttpGet("GetBaiVietById/{baiVietId}")]
        public async Task<IActionResult> GetBaiVietById([FromRoute] int baiVietId)
        {
            return Ok(await _baiVietService.GetBaiVietById(baiVietId));
        }
        [HttpPut("SuaBaiViet")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SuaBaiViet([FromForm] Request_SuaBaiViet request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _baiVietService.SuaBaiViet(id, request));
        }
        [HttpPost("TaoBaiViet")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> TaoBaiViet([FromForm] Request_TaoBaiViet request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _baiVietService.TaoBaiViet(id, request));
        }
        [HttpDelete("XoaBaiViet/{baiVietId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> XoaBaiViet([FromRoute] int baiVietId)
        {
            return Ok(await _baiVietService.XoaBaiViet(baiVietId));
        }
        [HttpGet("getkhoahoc")]
        public async Task<IActionResult> GetAllsKhoahoc([FromQuery] InputKhoaHoc? input)
        {
            return Ok(await _khoaHocService.GetAllsKhoahoc(input));
        }
        [HttpPut("SuaBinhLuanBaiViet")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SuaBinhLuanBaiViet([FromBody] Request_SuaBinhLuanBaiViet request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _baiVietService.SuaBinhLuanBaiViet(id, request));
        }
        [HttpPost("TaoBinhLuanBaiViet")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> TaoBinhLuanBaiViet([FromBody] Request_TaoBinhLuanBaiViet request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _baiVietService.TaoBinhLuanBaiViet(id, request));
        }
        [HttpPost("TraLoiBinhLuanBaiViet")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> TraLoiBinhLuanBaiViet([FromBody] Request_TraLoiBinhLuanBaiViet request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _baiVietService.TraLoiBinhLuanBaiViet(id, request));
        }
        [HttpDelete("XoaBinhLuanBaiViet/{binhLuanId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> XoaBinhLuanBaiViet([FromRoute] int binhLuanId)
        {
            return Ok(await _baiVietService.XoaBinhLuan(binhLuanId));
        }
        [HttpPost("ThichBinhLuanBaiViet")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ThichBinhLuanBaiViet([FromBody] Request_NguoiDungThichBinhLuanBaiViet request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _thichBinhLuanBaiVietService.ThichBinhLuanBaiViet(id, request));
        }
        [HttpPost("LikeBaiViet")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> LikeBaiViet([FromBody] Request_ThichBaiViet request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _thichBaiVietService.LikeBaiViet(id, request));
        }

        [HttpGet("GetAllLKH")]
        public async Task<IActionResult> GetAllLoaiKhoahocs()
        {
            return Ok(await _loaiKhoaHocService.GetAllLoaiKhoahocs());
        }
        [HttpGet("GetKhoaHocByNguoiDung/{nguoiDungId}")]
        public async Task<IActionResult> GetKhoaHocByNguoiDung([FromRoute] int nguoiDungId)
        {
            return Ok(await _khoaHocService.GetKhoaHocByNguoiDung(nguoiDungId));
        }
        [HttpPut("DanhGiaKhoaHoc")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DanhGiaKhoaHoc([FromBody] Request_DanhGiaKhoaHoc request)
        {
            return Ok(await _khoaHocNguoiDungService.DanhGiaKhoaHoc(request));
        }
        [HttpPost("TaoTestCase")]
        public async Task<IActionResult> TaoTestCase([FromBody] Request_TaoTestCase request)
        {
            return Ok(await _testCaseService.TaoTestCase(request));
        }
        [HttpGet("GetAllsLoaiBaiViet")]
        public async Task<IActionResult> GetAllsLoaiBaiViet(string? tenLoaiBaiViet)
        {
            return Ok(await _baiVietService.GetAllsLoaiBaiViet(tenLoaiBaiViet));
        }
        [HttpGet("GetLoaiBaiVietById/{loaiBaiVietId}")]
        public async Task<IActionResult> GetLoaiBaiVietById([FromRoute] int loaiBaiVietId)
        {
            return Ok(await _baiVietService.GetLoaiBaiVietById(loaiBaiVietId));
        }
        [HttpPost("GuiBaoCao")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> GuiBaoCao([FromForm] Request_GuiBaoCao request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _baoCaoService.GuiBaoCao(id, request));
        }
        [HttpGet("GetAllKhoaHocTheoNguoiTao/{nguoiTaoId}")]
        public async Task<IActionResult> GetAllKhoaHocTheoNguoiTao([FromRoute] int nguoiTaoId)
        {
            return Ok(await _khoaHocService.GetAllKhoaHocTheoNguoiTao(nguoiTaoId));
        }
        [HttpGet("GetAllKhoaHocTheoNguoiDung/{nguoiDungId}")]
        public async Task<IActionResult> GetAllKhoaHocTheoNguoiDung([FromRoute] int nguoiDungId)
        {
            return Ok(await _khoaHocService.GetAllKhoaHocTheoNguoiDung(nguoiDungId));
        }
        [HttpGet("GetAllsBanner")]
        public async Task<IActionResult> GetAllsBanner()
        {
            return Ok(await _bannerService.GetAlls());
        }
        [HttpPut("CapNhatTrangThaiBaiHocCuaNguoiDung/{nguoiDungHoanThanhBaiHocId}")]
        public async Task<IActionResult> CapNhatTrangThaiBaiHocCuaNguoiDung([FromRoute] int nguoiDungHoanThanhBaiHocId)
        {
            return Ok(await _baiHocService.CapNhatTrangThaiBaiHocCuaNguoiDung(nguoiDungHoanThanhBaiHocId));
        }
        [HttpGet("GetNguoiDungThichBaiVietById/{thichBaiVietId}")]
        public async Task<IActionResult> GetNguoiDungThichBaiVietById([FromRoute] int thichBaiVietId)
        {
            return Ok(await _thichBaiVietService.GetNguoiDungThichBaiVietById(thichBaiVietId));
        }
        [HttpGet("GetBaiVietByIdNguoiDung/{nguoiDungId}")]
        public async Task<IActionResult> GetBaiVietByIdNguoiDung([FromRoute] int nguoiDungId)
        {
            return Ok(await _baiVietService.GetBaiVietByIdNguoiDung(nguoiDungId));
        }
    }
}
