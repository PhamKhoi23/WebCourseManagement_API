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
using WebCourseManagement2_Models.RequestModels.LoaiKhoaHocRequests;
using WebCourseManagement2_Models.ResponseModels.DataLoaiKhoaHoc;
using WebCourseManagement2_Models.Responses;
using WebCourseManagement2_Repositories.HandlePagination;

namespace WebCourseManagement2_Business.Implements
{
    public class LoaiKhoaHocService : ILoaiKhoaHocService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseLoaiKhoaHoc> _responseObject;
        private readonly LoaiKhoaHocConverter _converter;
        public LoaiKhoaHocService(AppDbContext context, LoaiKhoaHocConverter converter, ResponseObject<DataResponseLoaiKhoaHoc> responseObject)
        {
            _context = context;
            _responseObject = responseObject;
            _converter = converter;
        }
        public async Task<ResponseObject<DataResponseLoaiKhoaHoc>> CapNhatThongTinLoaiKhoaHoc(Request_SuaLoaiKhoaHoc request)
        {
            try
            {
                var loaiKhoaHoc = await _context.loaiKhoaHocs.SingleOrDefaultAsync(x => x.Id == request.LoaiKhoaHocId);
                if (loaiKhoaHoc == null)
                {
                    return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy loại khóa học", null);
                }
                if (string.IsNullOrWhiteSpace(request.TenLoaiKhoaHoc))
                {
                    return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng điền đầy đủ thông tin", null);
                }
                loaiKhoaHoc.TenLoaiKhoaHoc = request.TenLoaiKhoaHoc;
                _context.SaveChanges();
                return _responseObject.ResponseSuccess("Cập nhật thông tin loại khóa học thành công", _converter.EntityToDTO(loaiKhoaHoc));
            }
            catch (Exception ex)
            {
                return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }

        public async Task<IQueryable<DataResponseLoaiKhoaHoc>> GetAllLoaiKhoahocs()
        {
            return _context.loaiKhoaHocs.Select(x => _converter.EntityToDTO(x)).AsQueryable();
        }

        public async Task<PageResult<DataResponseLoaiKhoaHoc>> GetAlls(int pageSize, int pageNumber)
        {
            var query = _context.loaiKhoaHocs.AsQueryable();
            var result = Pagination.GetPagedData(query.Select(x => _converter.EntityToDTO(x)), pageSize, pageNumber);
            return result;
        }

        public async Task<ResponseObject<DataResponseLoaiKhoaHoc>> ThemLoaiKhoaHoc(Request_ThemLoaiKhoaHoc request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.TenLoaiKhoaHoc))
                {
                    return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng điền đầy đủ thông tin", null);
                }
                LoaiKhoaHoc loaiKhoaHoc = new LoaiKhoaHoc
                {
                    TenLoaiKhoaHoc = request.TenLoaiKhoaHoc
                };
                _context.loaiKhoaHocs.Add(loaiKhoaHoc);
                _context.SaveChanges();
                return _responseObject.ResponseSuccess("Thêm loại khóa học thành công", _converter.EntityToDTO(loaiKhoaHoc));
            }
            catch (Exception ex)
            {
                return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }

        public async Task<string> XoaLoaiKhoaHoc(int loaiKhoaHocId)
        {
            try
            {
                var loaiKhoaHoc = await _context.loaiKhoaHocs.SingleOrDefaultAsync(x => x.Id == loaiKhoaHocId);
                if (loaiKhoaHoc == null)
                {
                    return "Loại khóa học không tồn tại";
                }
                _context.loaiKhoaHocs.Remove(loaiKhoaHoc);
                _context.SaveChanges();
                return "Xóa loại khóa học thành công";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;

            }

        }
    }
}
