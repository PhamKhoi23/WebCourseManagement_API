using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Business.Interfaces;
using WebCourseManagement2_Models.DataContexts;
using WebCourseManagement2_Models.Entities;
using WebCourseManagement2_Models.RequestModels.NguoiDungThichBinhLuanBaiVietRequests;

namespace WebCourseManagement2_Business.Implements
{
    public class ThichBinhLuanBaiVietService : IThichBinhLuanBaiVietService
    {
        private readonly AppDbContext _context;
        public ThichBinhLuanBaiVietService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<string> ThichBinhLuanBaiViet(int nguoiDungId, Request_NguoiDungThichBinhLuanBaiViet request)
        {
            try
            {
                var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.Id == nguoiDungId);
                var binhLuan = await _context.binhLuanBaiViets.SingleOrDefaultAsync(x => x.Id == request.BinhLuanBaiVietId && x.TrangThaiBinhLuanId == 1);
                if (binhLuan == null)
                {
                    return "Bình luận không tồn tại";
                }
                var nguoiLike = await _context.nguoiDungThichBinhLuanBaiHocs.SingleOrDefaultAsync(x => x.NguoiDungThichBinhLuanId == nguoiDungId && x.BinhLuanBaiHocId == binhLuan.Id);
                if (binhLuan != null)
                {
                    _context.nguoiDungThichBinhLuanBaiHocs.Remove(nguoiLike);
                    _context.SaveChanges();
                }
                else
                {
                    NguoiDungThichBinhLuanBaiViet item = new NguoiDungThichBinhLuanBaiViet
                    {
                        BinhLuanBaiVietId = request.BinhLuanBaiVietId,
                        DaThich = true,
                        NguoiDungThichBinhLuanId = nguoiDungId,
                        ThoiGianThich = DateTime.Now
                    };
                    _context.nguoiDungThichBinhLuanBaiViets.Add(item);
                    _context.SaveChanges();
                }

                binhLuan.SoLuotThich = _context.nguoiDungThichBinhLuanBaiViets.Count(x => x.BinhLuanBaiVietId == request.BinhLuanBaiVietId);
                _context.SaveChanges();
                return "Bạn đã thích bình luận bài viết";
            }catch(Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}
