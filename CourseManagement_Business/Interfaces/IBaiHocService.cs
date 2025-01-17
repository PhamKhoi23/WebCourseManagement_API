using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Models.RequestModels.BaiHocRequests;
using WebCourseManagement2_Models.RequestModels.InputRequests;
using WebCourseManagement2_Models.ResponseModels.DataBaiHoc;
using WebCourseManagement2_Models.ResponseModels.DataNguoiDungHoanThanhBaiHoc;
using WebCourseManagement2_Models.Responses;
using WebCourseManagement2_Repositories.HandlePagination;

namespace WebCourseManagement2_Business.Interfaces
{
    public interface IBaiHocService
    {
        Task<ResponseObject<DataResponseBaiHoc>> ThemBaiHoc(Request_ThemBaiHoc request);
        Task<ResponseObject<DataResponseBaiHoc>> SuaThongTinBaiHoc(Request_CapNhatThongTinBaiHoc request);
        Task<string> XoaBaiHoc(int baiHocId);
        Task<ResponseObject<DataResponseBaiHoc>> GetBaiHocById(int baiHocId);
        Task<PageResult<DataResponseBaiHoc>> GetAlls(InputBaiHoc input, int pageSize, int pageNumber);
        Task<ResponseObject<DataResponseNguoiDungHoanThanhBaiHoc>> CapNhatTrangThaiBaiHocCuaNguoiDung(int nguoiDungHoanThanhBaiHocId);
    }
}
