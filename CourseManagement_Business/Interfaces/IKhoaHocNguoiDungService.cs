using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Models.RequestModels.KhoaHocNguoiDungRequests;
using WebCourseManagement2_Models.ResponseModels.DataKhoaHocCuaNguoiDung;
using WebCourseManagement2_Models.Responses;

namespace WebCourseManagement2_Business.Interfaces
{
    public interface IKhoaHocNguoiDungService
    {
        Task<ResponseObject<DataResponseKhoaHocCuaNguoiDung>> DanhGiaKhoaHoc(Request_DanhGiaKhoaHoc request);
    }
}
