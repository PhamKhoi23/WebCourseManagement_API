using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Models.RequestModels.ChuongHocRequests;
using WebCourseManagement2_Models.ResponseModels.DataChuongHoc;
using WebCourseManagement2_Models.Responses;
using WebCourseManagement2_Repositories.HandlePagination;

namespace WebCourseManagement2_Business.Interfaces
{
    public interface IChuongHocService
    {
        Task<ResponseObject<DataResponseChuongHoc>> ThemChuongHoc(Request_ThemChuongHoc request);
        Task<ResponseObject<DataResponseChuongHoc>> SuaThongTinChuongHoc(Request_SuaThongTinChuongHoc request);
        Task<string> XoaChuongHoc(int chuongHocId);
        Task<PageResult<DataResponseChuongHoc>> GetAlls(int pageSize, int pageNumber);
        Task<ResponseObject<DataResponseChuongHoc>> GetChuongHocById(int chuongHocId);
    }
}
