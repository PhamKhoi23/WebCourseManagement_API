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
using WebCourseManagement2_Models.RequestModels.BinhLuanBaiHocRequest;
using WebCourseManagement2_Models.ResponseModels.DataBinhLuanBaiHoc;
using WebCourseManagement2_Models.Responses;

namespace WebCourseManagement2_Business.Implements
{
    public class BinhLuanBaiHocService : IBinhLuanBaiHocService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseBinhLuanBaiHoc> _responseObject;
        private readonly BinhLuanBaiHocConverter _converter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BinhLuanBaiHocService(AppDbContext context, ResponseObject<DataResponseBinhLuanBaiHoc> responseObject, BinhLuanBaiHocConverter converter, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _responseObject = responseObject;
            _converter = converter;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseObject<DataResponseBinhLuanBaiHoc>> TaoBinhLuan(int nguoiDungId, Request_TaoBinhLuanBaiHoc request)
        {
            try
            {
                var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.Id == nguoiDungId);
                var baiHoc = await _context.baiHocs.SingleOrDefaultAsync(x => x.Id == request.BaiHocId);
                if (baiHoc == null)
                {
                    return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy bài viết", null);
                }
                if (string.IsNullOrWhiteSpace(request.NoiDungBinhLuan))
                {
                    return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng nhập bình luận", null);
                }
                BinhLuanBaiHoc binhLuan = new BinhLuanBaiHoc
                {
                    IsActive = true,
                    BaiHocId = request.BaiHocId,
                    NguoiBinhLuanId = nguoiDungId,
                    NoiDungBinhLuan = request.NoiDungBinhLuan,
                    SoLuotThich = 0,
                    SoLuotTraLoiBinhLuan = 0,
                    ThoiGianTao = DateTime.Now,
                    TrangThaiBinhLuanId = 1
                };
                _context.binhLuanBaiHocs.Add(binhLuan);
                _context.SaveChanges();
                return _responseObject.ResponseSuccess("Tạo bình luận thành công", _converter.EntityToDTO(binhLuan));
            }catch(Exception ex)
            {
                return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }
        public async Task<ResponseObject<DataResponseBinhLuanBaiHoc>> SuaBinhLuan(int nguoiDungId, Request_SuaBinhLuanBaiHoc request)
        {
            try
            {
                var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.Id == nguoiDungId);
                var binhLuan = await _context.binhLuanBaiHocs.SingleOrDefaultAsync(x => x.Id == request.BinhLuanId);
                if (binhLuan == null)
                {
                    return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Bình luận không tồn tại", null);
                }
                if (string.IsNullOrWhiteSpace(request.NoiDungBinhLuan))
                {
                    return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng nhập bình luận", null);
                }
                if (nguoiDungId != binhLuan.NguoiBinhLuanId)
                {
                    return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Bạn không có quyền sửa bình luận này", null);
                }
                binhLuan.NguoiBinhLuanId = nguoiDungId;
                binhLuan.NoiDungBinhLuan = request.NoiDungBinhLuan;
                binhLuan.ThoiGianCapNhat = DateTime.Now;
                _context.SaveChanges();
                return _responseObject.ResponseSuccess("Sửa bình luận thành công", _converter.EntityToDTO(binhLuan));
            }catch(Exception ex)
            {
                return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }
        public async Task<ResponseObject<DataResponseBinhLuanBaiHoc>> TraLoiBinhLuan(int nguoiDungId, Request_TraLoiBinhLuan request)
        {
            try
            {
                var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.Id == nguoiDungId);
                var binhLuanGoc = await _context.binhLuanBaiHocs.SingleOrDefaultAsync(x => x.Id == request.BinhLuanGocId);
                if (binhLuanGoc == null)
                {
                    return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Bình luận không tồn tại", null);
                }
                if (string.IsNullOrWhiteSpace(request.NoiDungBinhLuan))
                {
                    return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng nhập bình luận", null);
                }
                BinhLuanBaiHoc binhLuan = new BinhLuanBaiHoc
                {
                    IsActive = true,
                    BaiHocId = request.BaiHocId,
                    NguoiBinhLuanId = nguoiDungId,
                    NoiDungBinhLuan = request.NoiDungBinhLuan,
                    SoLuotThich = 0,
                    SoLuotTraLoiBinhLuan = 0,
                    ThoiGianTao = DateTime.Now,
                    TrangThaiBinhLuanId = 1,
                    BinhLuanGocId = request.BinhLuanGocId
                };
                _context.binhLuanBaiHocs.Add(binhLuan);
                _context.SaveChanges();
                return _responseObject.ResponseSuccess("Trả lời bình luận thành công", _converter.EntityToDTO(binhLuan));
            }catch(Exception ex)
            {
                return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }

        public async Task<string> XoaBinhLuan(int binhLuanId)
        {
            try
            {
                var binhLuan = await _context.binhLuanBaiHocs.SingleOrDefaultAsync(x => x.Id == binhLuanId);
                var currentUser = _httpContextAccessor.HttpContext.User;
                var userId = currentUser.FindFirst("Id").Value;
                if (binhLuan == null)
                {
                    return "Bình luận không tồn tại";
                }
                if (binhLuan.NguoiBinhLuanId != int.Parse(userId))
                {
                    return "Bạn không có quyền xóa bình luận này";
                }
                binhLuan.TrangThaiBinhLuanId = 2;
                binhLuan.IsActive = false;
                binhLuan.ThoiGianXoa = DateTime.Now;
                _context.SaveChanges();
                return "Xóa bình luận thành công";
            }catch(Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}
