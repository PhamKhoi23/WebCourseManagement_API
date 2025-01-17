using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Business.Interfaces;
using WebCourseManagement2_Models.Converters;
using WebCourseManagement2_Models.DataContexts;
using WebCourseManagement2_Models.Entities;
using WebCourseManagement2_Models.RequestModels.ThucHanhRequests;
using WebCourseManagement2_Models.ResponseModels.DataThucHanh;
using WebCourseManagement2_Models.Responses;

namespace WebCourseManagement2_Business.Implements
{
    public class ThucHanhService : IThucHanhService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseThucHanh> _responseObject;
        private readonly ThucHanhConverter _converter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ThucHanhService(AppDbContext context, ResponseObject<DataResponseThucHanh> responseObject, ThucHanhConverter converter, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _responseObject = responseObject;
            _converter = converter;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseObject<DataResponseThucHanh>> ThemYeuCauThucHanh(Request_ThemYeuCauThucHanh request)
        {
            try
            {
                var baiHoc = await _context.baiHocs.SingleOrDefaultAsync(x => x.Id == request.BaiHocId);
                var chuongHoc = await _context.chuongHocs.SingleOrDefaultAsync(x => x.Id == baiHoc.ChuongHocId);
                var khoaHoc = await _context.khoaHocs.SingleOrDefaultAsync(x => x.Id == chuongHoc.KhoaHocId);
                var currentUser = _httpContextAccessor.HttpContext.User;
                string userId = currentUser.FindFirst("Id").Value;
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return _responseObject.ResponseError(StatusCodes.Status401Unauthorized, "Người dùng này chưa được xác thực", null);
                }
                if (int.Parse(userId) != khoaHoc.NguoiTaoId)
                {
                    return _responseObject.ResponseError(StatusCodes.Status403Forbidden, "Bạn không có quyền thực hiện chức năng này", null);
                }
                ThucHanh thucHanh = new ThucHanh
                {
                    BaiHocId = request.BaiHocId,
                    CallTestCode = request.CallTestCode,
                    ChiTietDeBai = request.ChiTietDeBai,
                    CodeDauVao = request.CodeDauVao,
                    CodeKhoiDau = request.CodeKhoiDau,
                    DeBai = request.DeBai,
                    GiaiThich = request.GiaiThich,
                    GoiY = request.GoiY,
                    HoTroDaNgonNgu = true,
                    MongDoiDauRa = request.MongDoiDauRa,
                    NgonNguMacDinhId = request.NgonNguMacDinhId
                };
                _context.thucHanhs.Add(thucHanh);
                _context.SaveChanges();
                return _responseObject.ResponseSuccess("Thêm yêu cầu thực hành thành công", _converter.EntityToDTO(thucHanh));
            }catch(Exception ex)
            {
                return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }
    }
}
