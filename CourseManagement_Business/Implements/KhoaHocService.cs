using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement_Models.RequestModels.KhoaHocRequests;
using WebCourseManagement2_Business.Interfaces;
using WebCourseManagement2_Models.ConfigModels.MomoPayment;
using WebCourseManagement2_Models.Converters;
using WebCourseManagement2_Models.DataContexts;
using WebCourseManagement2_Models.Entities;
using WebCourseManagement2_Models.RequestModels.InputRequests;
using WebCourseManagement2_Models.RequestModels.KhoaHocRequests;
using WebCourseManagement2_Models.ResponseModels.DataHoaDon;
using WebCourseManagement2_Models.ResponseModels.DataKhoaHoc;
using WebCourseManagement2_Models.Responses;
using WebCourseManagement2_Repositories.HandleImage;
using WebCourseManagement2_Repositories.HandlePagination;

namespace WebCourseManagement2_Business.Implements
{
    public class KhoaHocService : IKhoaHocService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseKhoaHoc> _responseObject;
        private readonly KhoaHocConverter _converter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ResponseObject<DataResponseHoaDon> _responseObjectHoaDon;
        private readonly HoaDonConverter _hoaDonConverter;
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;
        public KhoaHocService(AppDbContext context, KhoaHocConverter converter, ResponseObject<DataResponseKhoaHoc> responseObject, IHttpContextAccessor httpContextAccessor, HoaDonConverter hoaDonConverter, ResponseObject<DataResponseHoaDon> responseObjectHoaDon, AuthService authService, IConfiguration configuration)
        {
            _context = context;
            _responseObject = responseObject;
            _converter = converter;
            _httpContextAccessor = httpContextAccessor;
            _hoaDonConverter = hoaDonConverter;
            _responseObjectHoaDon = responseObjectHoaDon;
            _authService = authService;
            _configuration = configuration;
        }
        public async Task<PageResult<DataResponseKhoaHoc>> GetAlls(int pageSize, int pageNumber)
        {
            var query = _context.khoaHocs.Where(x => x.IsActive == true).Select(x => _converter.EntityToDTO(x));
            var result = Pagination.GetPagedData(query, pageSize, pageNumber);
            return result;
        }

        public async Task<ResponseObject<DataResponseKhoaHoc>> GetKhoaHocById(int khoaHocId)
        {
            var khoaHoc = await _context.khoaHocs.SingleOrDefaultAsync(x => x.Id  == khoaHocId && x.IsActive == true);
            if (khoaHoc == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy khóa học", null);
            }
            return _responseObject.ResponseSuccess("Lấy dữ liệu thành công", _converter.EntityToDTO(khoaHoc));
        }

        public async Task<ResponseObject<DataResponseKhoaHoc>> ThemKhoaHoc(int nguoiTaoId, Request_ThemKhoaHoc request)
        {
            try
            {
                var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.Id == nguoiTaoId);
                var loaiKhoaHoc = await _context.loaiKhoaHocs.SingleOrDefaultAsync(x => x.Id == request.LoaiKhoaHocId);
                if (loaiKhoaHoc == null)
                {
                    return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy loại khóa học", null);
                }
                KhoaHoc khoaHoc = new KhoaHoc
                {
                    AnhKhoaHoc = await HandleUploadImage.Upfile(request.AnhKhoaHoc),
                    IsActive = true,
                    DaXoa = false,
                    GiaKhoaHoc = request.GiaKhoaHoc,
                    LoaiKhoaHocId = request.LoaiKhoaHocId,
                    PhanTramGiamGia = request.PhanTramGiamGia == 0 ? 0 : request.PhanTramGiamGia,
                    GiaKhoaHocThucTe = request.PhanTramGiamGia == null  ? request.GiaKhoaHoc : (request.GiaKhoaHoc - request.GiaKhoaHoc * request.PhanTramGiamGia),
                    MoTaKhoaHoc = request.MoTaKhoaHoc,
                    NgayTao = DateTime.Now,
                    NguoiTaoId = nguoiTaoId,
                    SoHocVienDaHoanThanh = 0,
                    TieuDeKhoaHoc = request.TieuDeKhoaHoc,
                    TongThoiGianKhoaHoc = 0,
                    SoHocVienHocKhoaHoc = 0,
                    TrailerKhoaHoc = request.TrailerKhoaHoc,
                    SoSaoTrungBinh = 0
                };
                _context.khoaHocs.Add(khoaHoc);
                _context.SaveChanges();

                List<ThongBao> listThongBao = new List<ThongBao>();
                List<KhoaHocCuaNguoiDung> listKhoaHoc = _context.khoaHocCuaNguoiDungs.Include(x => x.KhoaHoc).Include(x => x.NguoiDung).Where(x => x.KhoaHoc.NguoiTaoId == khoaHoc.NguoiTaoId).ToList();
                for(int i = 0; i < listKhoaHoc.Count; i++)
                {
                    ThongBao thongBao = new ThongBao
                    {
                        DaXemThongBao = false,
                        HeThongThongBao =true,
                        LinkThongBao = "",
                        NguoiDungGuiThongBaoId = khoaHoc.NguoiTaoId,
                        NguoiDungId = listKhoaHoc[i].NguoiDungId,
                        NoiDungThongBao = $"Một giảng viên bạn từng học đã tạo khóa học mới! Hãy tìm hiểu xem",
                        ThoiGianThongBao = DateTime.Now,
                        AnhThongBao = khoaHoc.AnhKhoaHoc
                    };
                    listThongBao.Add(thongBao);
                }
                _context.thongBaos.AddRange(listThongBao);
                _context.SaveChanges();
                return _responseObject.ResponseSuccess("Thêm khóa học thành công", _converter.EntityToDTO(khoaHoc));
            }catch(Exception ex)
            {
                return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }
        public async Task<ResponseObject<DataResponseKhoaHoc>> SuaThongTinKhoaHoc(int nguoiSuaId, Request_SuaThongTinKhoaHoc request)
        {
            try
            {
                var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.Id == nguoiSuaId);
                var khoaHoc = await _context.khoaHocs.SingleOrDefaultAsync(x => x.Id == request.KhoaHocId);
                var loaiKhoaHoc = await _context.loaiKhoaHocs.SingleOrDefaultAsync(x => x.Id == request.LoaiKhoaHocId);
                if (khoaHoc == null)
                {
                    return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy khóa học", null);
                }
                if (loaiKhoaHoc == null)
                {
                    return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy loại khóa học", null);
                }
                if (nguoiSuaId != khoaHoc.NguoiTaoId)
                {
                    return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Bạn không có quyền thực hiện chức năng này", null);
                }
                khoaHoc.MoTaKhoaHoc = request.MoTaKhoaHoc;
                khoaHoc.AnhKhoaHoc = await HandleUploadImage.Upfile(request.AnhKhoaHoc);
                khoaHoc.NgayCapNhat = DateTime.Now;
                khoaHoc.GiaKhoaHocThucTe = request.PhanTramGiamGia == 0 ? request.GiaKhoaHoc : request.GiaKhoaHoc - request.GiaKhoaHoc * request.PhanTramGiamGia;
                khoaHoc.PhanTramGiamGia = request.PhanTramGiamGia == 0 ? 0 : request.PhanTramGiamGia;
                khoaHoc.GiaKhoaHoc = request.GiaKhoaHoc;
                khoaHoc.LoaiKhoaHocId = request.LoaiKhoaHocId;
                khoaHoc.TieuDeKhoaHoc = request.TieuDeKhoaHoc;
                _context.SaveChanges();
                return _responseObject.ResponseSuccess("Cập nhật thông tin khóa học thành công", _converter.EntityToDTO(khoaHoc));
            }catch(Exception ex)
            {
                return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }


        public async Task<string> XoaKhoaHoc(int khoaHocId)
        {
            try
            {
                var khoaHoc = await _context.khoaHocs.SingleOrDefaultAsync(x => x.Id == khoaHocId);
                var currentUser = _httpContextAccessor.HttpContext.User;
                var userId = currentUser.FindFirst("Id").Value;
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return "Người dùng chưa được xác thực";
                }
                if (khoaHoc == null)
                {
                    return "Khóa học không tồn tại";
                }
                if (int.Parse(userId) != khoaHoc.NguoiTaoId)
                {
                    return "Bạn không có quyền thực hiện chức năng này";
                }
                khoaHoc.IsActive = false;
                _context.SaveChanges();
                return "Xóa khóa học thành công";
            }catch(Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        public async Task<IQueryable<DataResponseKhoaHoc>> GetAllsKhoahoc(InputKhoaHoc input)
        {
            var result = _context.khoaHocs.Where(x => x.IsActive == true).AsQueryable();
            if (input.LoaiKhoaHocId.HasValue)
            {
                result = result.Where(x => x.LoaiKhoaHocId == input.LoaiKhoaHocId);
            }
            if (!string.IsNullOrWhiteSpace(input.TieuDeKhoaHoc))
            {
                result = result.Where(x => x.TieuDeKhoaHoc.ToLower().Contains(input.TieuDeKhoaHoc.ToLower()));
            }
            if(input.TuNgay.HasValue && input.DenNgay.HasValue)
            {
                result = result.Where(x => x.NgayTao >= input.TuNgay && x.NgayTao <= input.DenNgay);
            }
            if(input.GiaTu.HasValue && input.GiaDen.HasValue)
            {
                result = result.Where(x => x.GiaKhoaHocThucTe >= input.GiaTu && x.GiaKhoaHocThucTe <= input.GiaDen);
            }

            return result.OrderByDescending(x => x.SoSaoTrungBinh).Select(x => _converter.EntityToDTO(x));

        }
        public async Task<ResponseObject<DataResponseHoaDon>> DangKyKhoaHoc(int nguoiDungId, Request_DangKyKhoaHoc request)
        {
            try
            {
                var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.Id == nguoiDungId);
                var khoaHoc = await _context.khoaHocs.SingleOrDefaultAsync(x => x.Id == request.KhoaHocId);
                if (khoaHoc == null)
                {
                    return _responseObjectHoaDon.ResponseError(StatusCodes.Status404NotFound, "Khóa học không tồn tại", null);
                }
                HoaDonDangKy hoaDon = new HoaDonDangKy
                {
                    KhoaHocId = request.KhoaHocId,
                    MaGiaoDich = "MyBugs_" + DateTime.Now.Ticks.ToString() + new Random().Next(10000, 99999).ToString(),
                    NguoiDungId = nguoiDungId,
                    ThoiGianTao = DateTime.Now,
                    TongTien = khoaHoc.GiaKhoaHocThucTe,
                    TrangThaiHoaDonId = 1
                };
                _context.hoaDonDangKies.Add(hoaDon);
                _context.SaveChanges();
                _authService.SendEmail(new WebCourseManagement2_Repositories.EmailTo
                {
                    To = nguoiDung.Email,
                    Subject = "Thông báo đăng ký học",
                    Content = "Bạn đã đăng ký học thành công! Vui lòng thanh toán"
                });
                return _responseObjectHoaDon.ResponseSuccess("Đăng ký khóa học thành công! Vui lòng thanh toán", _hoaDonConverter.EntityToDTO(hoaDon));
            }catch(Exception ex)
            {
                return _responseObjectHoaDon.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }
        private string PaymentMomo(int hoaDonId)
        {
            var hoaDon = _context.hoaDonDangKies.SingleOrDefault(x => x.Id == hoaDonId);
            var tongTien = double.Parse((hoaDon.TongTien).ToString());

            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOV2NN20220607";
            string accessKey = "zanxjP13gFbJCX2t";
            string serectkey = "slOO415FigGnrQzZQkk02qWcdCUFkbH2";
            string orderInfo = "test";
            string returnUrl = $"https://localhost:7046/api/user/DangKyKhoaHoc";
            string notifyurl = "https://momo.vn/notify";
            string amount = tongTien + "";
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
            notifyurl + "&extraData=" +
            extraData;

            MomoSecurity crypto = new MomoSecurity();
            string signature = crypto.signSHA256(rawHash, serectkey);

            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet"},
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return jmessage.GetValue("payUrl").ToString();
        }

        public async Task<IQueryable<DataResponseKhoaHoc>> GetKhoaHocByNguoiDung(int nguoiDungId)
        {
            var result = _context.khoaHocs.Where(x => x.NguoiTaoId == nguoiDungId).Select(x => _converter.EntityToDTO(x));
            return result;
        }

        public async Task<IQueryable<DataResponseKhoaHoc>> GetAllKhoaHocTheoNguoiTao(int nguoiTaoId)
        {
            var query = _context.khoaHocs.Where(x => x.NguoiTaoId == nguoiTaoId && x.IsActive == true).Select(x => _converter.EntityToDTO(x)).AsQueryable();
            return query;
        }

        public async Task<IQueryable<DataResponseKhoaHoc>> GetAllKhoaHocTheoNguoiDung(int nguoiDungId)
        {
            var query = await _context.khoaHocCuaNguoiDungs.Include(x => x.KhoaHoc).AsNoTracking().Where(x => x.NguoiDungId == nguoiDungId).ToListAsync();
            List<DataResponseKhoaHoc> listKhoaHoc = new List<DataResponseKhoaHoc>();
            query.ForEach(x =>
            {
                listKhoaHoc.Add(_converter.EntityToDTO(_context.khoaHocs.SingleOrDefault(y => y.Id == x.KhoaHocId && y.IsActive == true)));
            });
            return listKhoaHoc.AsQueryable();
        }
    }
}
