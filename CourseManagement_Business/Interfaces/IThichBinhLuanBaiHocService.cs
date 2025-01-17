using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Models.RequestModels.NguoiDungThichBinhLuanBaiHocRequests;
using WebCourseManagement2_Models.ResponseModels.DataNguoiDungThichBinhLuanBaiHoc;
using WebCourseManagement2_Models.Responses;

namespace WebCourseManagement2_Business.Interfaces
{
    public interface IThichBinhLuanBaiHocService
    {
        Task<string> ThichBinhLuanBaiHoc(int nguoiDungId, Request_ThichBinhLuanBaiHoc request);
    }
}
