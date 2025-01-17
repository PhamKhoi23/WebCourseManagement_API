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
using WebCourseManagement2_Models.RequestModels.BannerRequest;
using WebCourseManagement2_Models.ResponseModels.DataBanner;
using WebCourseManagement2_Models.Responses;
using WebCourseManagement2_Repositories.HandleImage;

namespace WebCourseManagement2_Business.Implements
{
    public class BannerService : IBannerService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseBanner> _responseObject;
        private readonly BannerConverter _converter;
        private readonly IHttpContextAccessor _contextAccessor;
        public BannerService(AppDbContext context, ResponseObject<DataResponseBanner> responseObject, BannerConverter converter, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _responseObject = responseObject;
            _converter = converter;
            _contextAccessor = contextAccessor;
        }

        public async Task<IQueryable<DataResponseBanner>> GetAlls()
        {
            return _context.banners.Select(x => _converter.EntityToDTO(x));
        }

        public async Task<ResponseObject<DataResponseBanner>> TaoBanner(Request_TaoBanner request)
        {
            if(request.AnhBanner == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng chọn ảnh banner", null);
            }
            Banner banner = new Banner
            {
                AnhBanner = await HandleUploadImage.Upfile(request.AnhBanner)
            };
            _context.banners.Add(banner);
            _context.SaveChanges();
            return _responseObject.ResponseSuccess("Tạo banner thành công", _converter.EntityToDTO(banner));
        }

        public async Task<string> XoaBanner(int bannerId)
        {
            var banner = await _context.banners.SingleOrDefaultAsync(x => x.Id == bannerId);
            _context.banners.Remove(banner); _context.SaveChanges();
            return "Xóa banner thành công";
        }
    }
}
