using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Business.Interfaces;
using WebCourseManagement2_Models.ConfigModels.VnPayPayment;
using WebCourseManagement2_Models.DataContexts;
using WebCourseManagement2_Models.Entities;
using WebCourseManagement2_Repositories;
using WebCourseManagement2_Repositories.HandleVNPayPayment;
using Utils = WebCourseManagement2_Models.ConfigModels.VnPayPayment.Utils;

namespace WebCourseManagement2_Business.Implements
{
    public class VNPayService : IVNPayService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly AuthService _authService;
        private readonly DataVnpay _vnPay;
        public VNPayService(AppDbContext context, IConfiguration configuration, AuthService authService, DataVnpay vnPay)
        {
            _context = context;
            _configuration = configuration;
            _authService = authService;
            _vnPay = vnPay;
        }

        public async Task<string> TaoDuongDanThanhToan(int hoaDonId, HttpContext httpContext, int id)
        {
            var hoaDon = await _context.hoaDonDangKies.SingleOrDefaultAsync(x => x.Id == hoaDonId);
            var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.Id == id);
            if (nguoiDung.Id == hoaDon.NguoiDungId)
            {
                if (hoaDon.TrangThaiHoaDonId == 2)
                {
                    return "Hóa đơn đã được thanh toán trước đó";
                }
                if (hoaDon.TrangThaiHoaDonId == 1 && hoaDon.TongTien != 0 && hoaDon.TongTien != null)
                {
                    VnPayLibrary pay = new VnPayLibrary();

                    pay.AddRequestData("vnp_Version", "2.1.0");
                    pay.AddRequestData("vnp_Command", "pay");
                    pay.AddRequestData("vnp_TmnCode", "77E3BAVI");
                    pay.AddRequestData("vnp_Amount", (double.Parse((hoaDon.TongTien * 100).ToString()).ToString()));
                    pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                    pay.AddRequestData("vnp_CurrCode", "VND");
                    pay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(httpContext));
                    pay.AddRequestData("vnp_Locale", "vn");
                    pay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {hoaDon.Id}");
                    pay.AddRequestData("vnp_OrderType", "other");
                    pay.AddRequestData("vnp_ReturnUrl", _configuration.GetSection("VnPay:ReturnUrl").Value);
                    pay.AddRequestData("vnp_TxnRef", hoaDon.Id.ToString());

                    string duongDanThanhToan = pay.CreateRequestUrl(_configuration.GetSection("VnPay:vnp_Url").Value, _configuration.GetSection("VnPay:vnp_HashSecret").Value);
                    return duongDanThanhToan;
                }

                return "Vui lòng kiểm tra lại hóa đơn";
            }
            return "Vui lòng kiểm tra lại thông tin người thanh toán";
        }

        public async Task<string> VNPayReturn(IQueryCollection vnpayData)
        {
            string vnp_TmnCode = _configuration.GetSection("VnPay:vnp_TmnCode").Value;
            string vnp_HashSecret = _configuration.GetSection("VnPay:vnp_HashSecret").Value;

            VNPayLibrary vnPayLibrary = new VNPayLibrary();
            foreach (var (key, value) in vnpayData)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnPayLibrary.AddResponseData(key, value);
                }
            }

            string hoaDonId = vnPayLibrary.GetResponseData("vnp_TxnRef");
            string vnp_ResponseCode = vnPayLibrary.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnPayLibrary.GetResponseData("vnp_TransactionStatus");
            string vnp_SecureHash = vnPayLibrary.GetResponseData("vnp_SecureHash");
            double vnp_Amount = Convert.ToDouble(vnPayLibrary.GetResponseData("vnp_Amount"));
            bool check = vnPayLibrary.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

            if (check)
            {
                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    var hoaDon = await _context.hoaDonDangKies.Include(x => x.KhoaHoc).AsNoTracking().FirstOrDefaultAsync(x => x.Id == Convert.ToInt32(hoaDonId));

                    if (hoaDon == null)
                    {
                        return "Không tìm thấy hóa đơn";
                    }

                    hoaDon.TrangThaiHoaDonId = 2;
                    hoaDon.ThoiGianTao = DateTime.Now;

                    var user = _context.nguoiDungs.FirstOrDefault(x => x.Id == hoaDon.NguoiDungId);
                    if (user != null)
                    {
                        string email = user.Email;
                        string mss = _authService.SendEmail(new EmailTo
                        {
                            To = email,
                            Subject = $"Thanh Toán đơn hàng: {hoaDon.Id}",
                            Content = "Bạn đã mua khóa học thành công! Từ bây giờ bạn có thể học tập tại đây"
                        });
                    }
                    KhoaHocCuaNguoiDung khoaHocCuaNguoiDung = new KhoaHocCuaNguoiDung
                    {
                        DaHoanThanh = false,
                        KhoaHocId = hoaDon.KhoaHocId,
                        PhanTramHoanThanh = 0,
                        ThoiGianDangKy = DateTime.Now,
                        NguoiDungId = hoaDon.NguoiDungId,
                    };
                    _context.khoaHocCuaNguoiDungs.Add(khoaHocCuaNguoiDung);
                    _context.SaveChanges();
                    hoaDon.KhoaHoc.SoHocVienHocKhoaHoc = _context.khoaHocCuaNguoiDungs.Count(x => x.KhoaHocId == hoaDon.KhoaHocId);
                    _context.SaveChanges();
                    var khoaHoc = _context.khoaHocs.Include(x => x.ChuongHocs).ThenInclude(x => x.BaiHocs).AsNoTracking().SingleOrDefault(x => x.Id == khoaHocCuaNguoiDung.KhoaHocId);
                    var listChuongHoc = khoaHoc.ChuongHocs.ToList();
                    List<BaiHoc> listBaiHoc = new List<BaiHoc>();
                    listChuongHoc.ForEach(x =>
                    {
                        listBaiHoc.AddRange(x.BaiHocs);
                    });
                    listBaiHoc.ForEach(x =>
                    {
                        NguoiDungHoanThanhBaiHoc item = new NguoiDungHoanThanhBaiHoc
                        {
                            BaiHocId = x.Id,
                            DaHoanThanh = false,
                            NguoiDungId = hoaDon.NguoiDungId,
                            ThoiGianMoBaiHoc = DateTime.Now
                        };
                        _context.nguoiDungHoanThanhBaiHocs.Add(item);
                    });
                    _context.SaveChanges();
                    ThongBao thongBao = new ThongBao
                    {
                        AnhThongBao = hoaDon?.KhoaHoc?.AnhKhoaHoc != "" ? hoaDon.KhoaHoc.AnhKhoaHoc : "",
                        DaXemThongBao = false,
                        HeThongThongBao = true,
                        LinkThongBao = "",
                        NguoiDungId = hoaDon.NguoiDungId,
                        NoiDungThongBao = $"Bạn đã hoàn thành đăng ký khóa học {hoaDon.KhoaHoc.TieuDeKhoaHoc}! Hãy vào học nào",
                        ThoiGianThongBao = DateTime.Now
                    };
                    _context.thongBaos.Add(thongBao);
                    _context.SaveChanges();
                    return "Giao dịch thành công đơn hàng " + hoaDon.Id;
                }
                else
                {
                    return $"Lỗi trong khi thực hiện giao dịch. Mã lỗi: {vnp_ResponseCode}";
                }
            }
            else
            {
                return "Có lỗi trong quá trình xử lý";
            }
        }
    }
}
