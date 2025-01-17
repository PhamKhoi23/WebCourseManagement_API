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
using WebCourseManagement2_Models.RequestModels.ThichBaiVietRequests;
using WebCourseManagement2_Models.ResponseModels.DataBaiViet;

namespace WebCourseManagement2_Business.Implements
{
    public class NguoiDungThichBaiVietService : INguoiDungThichBaiVietService
    {
        private readonly AppDbContext _context;
        private readonly NguoiDungThichBaiVietConverter _converter;
        public NguoiDungThichBaiVietService(AppDbContext context, NguoiDungThichBaiVietConverter converter)
        {
            _context = context;
            _converter = converter;
        }

        public async Task<DataResponseNguoiDungThichBaiViet> GetNguoiDungThichBaiVietById(int thichBaiVietId)
        {
            return _converter.EntityToDTO(await _context.nguoiDungThichBaiViets.SingleOrDefaultAsync(x => x.Id == thichBaiVietId));
        }

        public async Task<string> LikeBaiViet(int nguoiDungId, Request_ThichBaiViet request)
        {
            try
            {
                var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.Id == nguoiDungId && x.IsActive == true && x.DaKhoa == false && x.TrangThaiNguoiDungId == 2);
                var baiViet = await _context.baiViets.SingleOrDefaultAsync(x => x.Id == request.BaiVietId && x.TrangThaiBaiVietId == 2);
                if (baiViet == null)
                {
                    return "Bài viết không tồn tại";
                }
                var nguoiLike = await _context.nguoiDungThichBaiViets.SingleOrDefaultAsync(x => x.NguoiDungId == nguoiDungId && x.BaiVietId == baiViet.Id);
                if (nguoiLike != null)
                {
                    _context.nguoiDungThichBaiViets.Remove(nguoiLike);
                    _context.SaveChanges();
                }
                else
                {
                    NguoiDungThichBaiViet item = new NguoiDungThichBaiViet
                    {
                        BaiVietId = request.BaiVietId,
                        DaThich = true,
                        NguoiDungId = nguoiDungId,
                        ThoiGianThich = DateTime.Now
                    };
                    _context.nguoiDungThichBaiViets.Add(item);
                    _context.SaveChanges();
                }

                baiViet.SoLuotThich = _context.nguoiDungThichBaiViets.Count(x => x.BaiVietId == request.BaiVietId);
                _context.SaveChanges();
                return "Bạn đã thích bình luận bài viết";
            }catch(Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}
