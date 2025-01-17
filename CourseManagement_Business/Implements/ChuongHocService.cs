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
using WebCourseManagement2_Models.RequestModels.ChuongHocRequests;
using WebCourseManagement2_Models.ResponseModels.DataChuongHoc;
using WebCourseManagement2_Models.Responses;
using WebCourseManagement2_Repositories.HandleImage;
using WebCourseManagement2_Repositories.HandlePagination;

namespace WebCourseManagement2_Business.Implements
{
    public class ChuongHocService : IChuongHocService
    {
        private readonly AppDbContext _context;
        private readonly ChuongHocConverter _converter;
        private readonly ResponseObject<DataResponseChuongHoc> _responseObject;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ChuongHocService(AppDbContext context, ChuongHocConverter converter, ResponseObject<DataResponseChuongHoc> responseObject, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _converter = converter;
            _responseObject = responseObject;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<PageResult<DataResponseChuongHoc>> GetAlls(int pageSize, int pageNumber)
        {
            var query = _context.chuongHocs.Where(x => x.IsActive == true).Select(x => _converter.EntityToDTO(x));
            var result = Pagination.GetPagedData(query, pageSize, pageNumber);
            return result;
        }

        public async Task<ResponseObject<DataResponseChuongHoc>> GetChuongHocById(int chuongHocId)
        {
            var chuongHoc = await _context.chuongHocs.SingleOrDefaultAsync(x => x.Id == chuongHocId && x.IsActive == true);
            return _responseObject.ResponseSuccess("Lấy dữ liệu thành công", _converter.EntityToDTO(chuongHoc));
        }

        public async Task<ResponseObject<DataResponseChuongHoc>> SuaThongTinChuongHoc(Request_SuaThongTinChuongHoc request)
        {
            try
            {
                var currentUser = _httpContextAccessor.HttpContext.User;
                var chuongHoc = await _context.chuongHocs.SingleOrDefaultAsync(x => x.Id == request.ChuongHocId);
                var khoaHoc = chuongHoc.KhoaHoc;
                var userId = currentUser.FindFirst("Id").Value;
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return _responseObject.ResponseError(StatusCodes.Status401Unauthorized, "Người dùng chưa được xác thực", null);
                }
                if (khoaHoc.NguoiTaoId != int.Parse(userId))
                {
                    return _responseObject.ResponseError(StatusCodes.Status403Forbidden, "Người dùng không có quyền thực hiện chức năng này", null);
                }
                chuongHoc.TenChuong = request.TenChuongHoc;
                chuongHoc.ThoiGianCapNhat = DateTime.Now;
                chuongHoc.AnhChuongHoc = await HandleUploadImage.Upfile(request.AnhChuongHoc);
                _context.SaveChanges();
                return _responseObject.ResponseSuccess("Cập nhật thông tin chương học thành công", _converter.EntityToDTO(chuongHoc));
            }catch(Exception ex)
            {
                return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }

        public async Task<ResponseObject<DataResponseChuongHoc>> ThemChuongHoc(Request_ThemChuongHoc request)
        {
            try
            {
                var currentUser = _httpContextAccessor.HttpContext.User;
                var khoaHoc = await _context.khoaHocs.SingleOrDefaultAsync(x => x.Id == request.KhoaHocId);
                var userId = currentUser.FindFirst("Id").Value;
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return _responseObject.ResponseError(StatusCodes.Status401Unauthorized, "Người dùng chưa được xác thực", null);
                }
                if (khoaHoc.NguoiTaoId != int.Parse(userId))
                {
                    return _responseObject.ResponseError(StatusCodes.Status403Forbidden, "Người dùng không có quyền thực hiện chức năng này", null);
                }
                ChuongHoc chuongHoc = new ChuongHoc
                {
                    KhoaHocId = request.KhoaHocId,
                    SoBaiHocTrongChuong = 0,
                    TenChuong = request.TenChuong,
                    ThoiGianTao = DateTime.Now,
                    TongThoiGianHocTrongChuong = 0,
                    IsActive = true,
                    AnhChuongHoc = await HandleUploadImage.Upfile(request.AnhChuongHoc)
                };
                _context.chuongHocs.Add(chuongHoc);
                _context.SaveChanges();
                return _responseObject.ResponseSuccess("Thêm chương học thành công", _converter.EntityToDTO(chuongHoc));
            }catch(Exception ex)
            {
                return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }

        public async Task<string> XoaChuongHoc(int chuongHocId)
        {
            try
            {
                var currentUser = _httpContextAccessor.HttpContext.User;
                var chuongHoc = await _context.chuongHocs.SingleOrDefaultAsync(x => x.Id == chuongHocId);
                var khoaHoc = chuongHoc.KhoaHoc;
                var userId = currentUser.FindFirst("Id").Value;
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return "Người dùng chưa được xác thực";
                }
                if (khoaHoc.NguoiTaoId != int.Parse(userId))
                {
                    return "Người dùng không có quyền thực hiện chức năng này";
                }
                chuongHoc.IsActive = false;
                _context.SaveChanges();
                return "Xóa chương học thành công";
            }catch(Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}
