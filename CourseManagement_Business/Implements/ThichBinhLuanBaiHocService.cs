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
using WebCourseManagement2_Models.RequestModels.NguoiDungThichBinhLuanBaiHocRequests;
using WebCourseManagement2_Models.ResponseModels.DataNguoiDungThichBinhLuanBaiHoc;
using WebCourseManagement2_Models.Responses;

namespace WebCourseManagement2_Business.Implements
{
    public class ThichBinhLuanBaiHocService : IThichBinhLuanBaiHocService
    {
        private readonly AppDbContext _context;
        public ThichBinhLuanBaiHocService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> ThichBinhLuanBaiHoc(int nguoiDungId, Request_ThichBinhLuanBaiHoc request)
        {
            try
            {
                var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.Id == nguoiDungId);
                var binhLuan = await _context.binhLuanBaiHocs.SingleOrDefaultAsync(x => x.Id == request.BinhLuanBaiHocId && x.TrangThaiBinhLuanId == 1);
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
                    NguoiDungThichBinhLuanBaiHoc item = new NguoiDungThichBinhLuanBaiHoc
                    {
                        BinhLuanBaiHocId = request.BinhLuanBaiHocId,
                        DaThich = true,
                        NguoiDungThichBinhLuanId = nguoiDungId,
                        ThoiGianThich = DateTime.Now
                    };
                    _context.nguoiDungThichBinhLuanBaiHocs.Add(item);
                    _context.SaveChanges();
                }

                binhLuan.SoLuotThich = _context.nguoiDungThichBinhLuanBaiHocs.Count(x => x.BinhLuanBaiHocId == request.BinhLuanBaiHocId);
                _context.SaveChanges();
                return "Bạn đã thích bình luận bài học";
            }catch(Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}
