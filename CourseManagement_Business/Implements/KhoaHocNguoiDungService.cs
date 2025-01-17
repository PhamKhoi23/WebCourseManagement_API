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
using WebCourseManagement2_Models.RequestModels.KhoaHocNguoiDungRequests;
using WebCourseManagement2_Models.ResponseModels.DataKhoaHocCuaNguoiDung;
using WebCourseManagement2_Models.Responses;

namespace WebCourseManagement2_Business.Implements
{
    public class KhoaHocNguoiDungService : IKhoaHocNguoiDungService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseKhoaHocCuaNguoiDung> _responseObject;
        private readonly KhoaHocNguoiDungConverter _converter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public KhoaHocNguoiDungService(AppDbContext context, ResponseObject<DataResponseKhoaHocCuaNguoiDung> responseObject, KhoaHocNguoiDungConverter converter, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _responseObject = responseObject;
            _converter = converter;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseObject<DataResponseKhoaHocCuaNguoiDung>> DanhGiaKhoaHoc(Request_DanhGiaKhoaHoc request)
        {
            try
            {
                var currentUser = _httpContextAccessor.HttpContext.User;
                var userId = currentUser.FindFirst("Id").Value;
                var khoaHoc = await _context.khoaHocCuaNguoiDungs.Include(x => x.KhoaHoc).AsNoTracking().SingleOrDefaultAsync(x => x.KhoaHocId == request.KhoaHocId);
                if (khoaHoc == null)
                {
                    return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Khóa học của người dùng không tồn tại", null);
                }
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return _responseObject.ResponseError(StatusCodes.Status401Unauthorized, "Người dùng chưa được xác thực", null);
                }
                if (khoaHoc.NguoiDungId != int.Parse(userId))
                {
                    return _responseObject.ResponseError(StatusCodes.Status403Forbidden, "Người dùng không có quyền thực hiện chức năng này", null);
                }
                khoaHoc.DanhGiaId = request.DanhGiaId;
                khoaHoc.KhoaHocId = request.KhoaHocId;
                _context.SaveChanges();
                var item = _context.khoaHocCuaNguoiDungs.Include(x => x.DanhGia).Where(x => x.KhoaHocId == khoaHoc.Id).ToList();
                double tongSoSao = 0;
                int tongSoVote = 0;
                item.ForEach(x =>
                {
                    tongSoSao += x.DanhGia.SoSao;
                    tongSoVote = _context.danhGias.Count(y => y.Id == x.DanhGia.Id);
                });
                khoaHoc.KhoaHoc.SoSaoTrungBinh = tongSoSao * 1.0 / tongSoVote;
                _context.SaveChanges();
                return _responseObject.ResponseSuccess("Đánh giá khóa học thành công", _converter.EntityToDTO(khoaHoc));
            }
            catch (Exception ex)
            {
                return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }
    }
}
