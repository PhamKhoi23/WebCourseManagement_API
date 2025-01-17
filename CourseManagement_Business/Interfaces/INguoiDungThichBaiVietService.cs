using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Models.RequestModels.ThichBaiVietRequests;
using WebCourseManagement2_Models.ResponseModels.DataBaiViet;
using WebCourseManagement2_Models.Responses;

namespace WebCourseManagement2_Business.Interfaces
{
    public interface INguoiDungThichBaiVietService
    {
        Task<string> LikeBaiViet(int nguoiDungId, Request_ThichBaiViet request);
        Task<DataResponseNguoiDungThichBaiViet> GetNguoiDungThichBaiVietById(int thichBaiVietId);
    }
}
