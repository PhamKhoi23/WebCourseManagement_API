using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Models.RequestModels.BannerRequest;
using WebCourseManagement2_Models.ResponseModels.DataBanner;
using WebCourseManagement2_Models.Responses;

namespace WebCourseManagement2_Business.Interfaces
{
    public interface IBannerService
    {
        Task<ResponseObject<DataResponseBanner>> TaoBanner(Request_TaoBanner request);
        Task<IQueryable<DataResponseBanner>> GetAlls();
        Task<string> XoaBanner(int bannerId);
    }
}
