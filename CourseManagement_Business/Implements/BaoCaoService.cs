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
using WebCourseManagement2_Models.RequestModels.BaoCaoRequests;
using WebCourseManagement2_Models.ResponseModels.DataBaoCao;
using WebCourseManagement2_Models.Responses;

namespace WebCourseManagement2_Business.Implements
{
    public class BaoCaoService : IBaoCaoService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseBaoCao> _responseObject;
        private readonly BaoCaoConverter _converter;
        public BaoCaoService(AppDbContext context, BaoCaoConverter converter, ResponseObject<DataResponseBaoCao> responseObject)
        {
            _context = context;
            _responseObject = responseObject;
            _converter = converter;
        }

        public async Task<ResponseObject<DataResponseBaoCao>> DuyetBaoCao(Request_DuyetBaoCao request)
        {
            try
            {
                var baoCao = await _context.baoCaos.SingleOrDefaultAsync(x => x.Id == request.BaoCaoId);
                if (baoCao == null)
                {
                    return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy dữ liệu", null);
                }
                switch (request.XacMinhViPham.ToString())
                {
                    case "DaViPham":
                        baoCao.DaViPham = true; 
                        break;
                    case "KhongViPham":
                        baoCao.DaViPham = false;
                        break;

                }
                baoCao.DaXacMinh = true;
                baoCao.ThoiGianXacMinh = DateTime.Now;
                _context.SaveChanges();
                var nguoiBiBaoCao = _context.nguoiDungs.SingleOrDefault(x => x.Id == baoCao.NguoiViPhamId);
                nguoiBiBaoCao.SoLanViPham = _context.baoCaos.Count(x => x.NguoiViPhamId == baoCao.NguoiViPhamId && x.DaViPham == true);
                if(nguoiBiBaoCao.SoLanViPham >= 5)
                {
                    nguoiBiBaoCao.DaKhoa = true;
                    nguoiBiBaoCao.ThoiGianKhoa = DateTime.Now;
                }
                _context.SaveChanges();
                return _responseObject.ResponseSuccess("Duyet bao cao thanh cong", _converter.EntityToDTO(baoCao));
            }
            catch(Exception ex)
            {
                return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }

        public async Task<ResponseObject<DataResponseBaoCao>> GuiBaoCao(int nguoiDungId, Request_GuiBaoCao request)
        {
            try
            {
                var nguoiDung = await _context.nguoiDungs.SingleOrDefaultAsync(x => x.Id == nguoiDungId);
                
                BaoCao baoCao = new BaoCao();
                baoCao.ThoiGianGuiBaoCao = DateTime.Now;
                baoCao.DaViPham = false;
                baoCao.DaXacMinh = false;
                baoCao.LoaiVanDeId = request.LoaiVanDeId;
                baoCao.NguoiGuiBaoCaoId = nguoiDungId;
                baoCao.GhiChuBaoCao = request.GhiChuBaoCao;
                if (request.BinhLuanId.HasValue && !request.BaiVietId.HasValue)
                {
                    baoCao.LoaiBaoCaoId = 1;
                    baoCao.BinhLuanId = request.BinhLuanId;
                    baoCao.LoaiBinhLuan = request.LoaiBinhLuan;
                    if (request.LoaiBinhLuan.ToString().Equals("BaiViet"))
                    {
                        var binhLuanViPham = _context.binhLuanBaiViets.SingleOrDefault(x => x.Id == request.BinhLuanId);
                        baoCao.NguoiViPhamId = binhLuanViPham.NguoiBinhLuanId;
                        var listBaoCao = _context.baoCaos.Where(x => x.NguoiViPhamId == binhLuanViPham.NguoiBinhLuanId).ToList();
                        foreach (var item in listBaoCao)
                        {
                            if(listBaoCao.Count >= 1 && listBaoCao.Count <= 3 && request.BinhLuanId == item.BinhLuanId && item.ThoiGianGuiBaoCao >= DateTime.Now && item.ThoiGianGuiBaoCao <= DateTime.Now.AddHours(24) && item.NguoiGuiBaoCaoId == nguoiDungId)
                            {
                                throw new ArgumentException("Bạn không thể gửi quá 3 báo cáo cho 1 bình luận trong 1 ngày");
                            }
                        }
                    }
                    else
                    {
                        var binhLuanViPham = _context.binhLuanBaiHocs.SingleOrDefault(x => x.Id == request.BinhLuanId);
                        baoCao.NguoiViPhamId = binhLuanViPham.NguoiBinhLuanId;
                        var listBaoCao = _context.baoCaos.Where(x => x.NguoiViPhamId == binhLuanViPham.NguoiBinhLuanId).ToList();
                        foreach (var item in listBaoCao)
                        {
                            if (listBaoCao.Count >= 1 && listBaoCao.Count <= 3 && request.BinhLuanId == item.BinhLuanId && item.ThoiGianGuiBaoCao >= DateTime.Now && item.ThoiGianGuiBaoCao <= DateTime.Now.AddHours(24) && item.NguoiGuiBaoCaoId == nguoiDungId)
                            {
                                throw new ArgumentException("Bạn không thể gửi quá 3 báo cáo cho 1 bình luận trong 1 ngày");
                            }
                        }
                    }
                    
                }
                else if(!request.BinhLuanId.HasValue && request.BaiVietId.HasValue)
                {
                    baoCao.LoaiBaoCaoId = 2;
                    baoCao.BaiVietId = request.BaiVietId;
                    var baiVietViPham = _context.baiViets.SingleOrDefault(x => x.Id == request.BaiVietId);
                    baoCao.NguoiViPhamId = baiVietViPham.NguoiTaoBaiVietId;
                    var listBaoCao = _context.baoCaos.Where(x => x.NguoiViPhamId == baiVietViPham.NguoiTaoBaiVietId).ToList();
                    foreach (var item in listBaoCao)
                    {
                        if (listBaoCao.Count >= 1 && listBaoCao.Count <= 3 && request.BaiVietId == item.BaiVietId && item.ThoiGianGuiBaoCao >= DateTime.Now && item.ThoiGianGuiBaoCao <= DateTime.Now.AddHours(24) && item.NguoiGuiBaoCaoId == nguoiDungId)
                        {
                            throw new ArgumentException("Bạn không thể gửi quá 3 báo cáo cho 1 bài viết trong 1 ngày");
                        }
                    }
                }
                _context.baoCaos.Add(baoCao);
                _context.SaveChanges();
                return _responseObject.ResponseSuccess("Gửi báo cáo thành công! Quản trị viên sẽ xem xét báo cáo của bạn", _converter.EntityToDTO(baoCao));
            }catch (Exception ex)
            {
                return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
            
        }
    }
}
