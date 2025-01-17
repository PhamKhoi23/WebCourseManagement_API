using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Models.RequestModels.LoaiKhoaHocRequests;
using WebCourseManagement2_Models.ResponseModels.DataLoaiKhoaHoc;
using WebCourseManagement2_Models.Responses;
using WebCourseManagement2_Repositories.HandlePagination;

namespace WebCourseManagement2_Business.Interfaces
{
    public interface ILoaiKhoaHocService
    {
        Task<ResponseObject<DataResponseLoaiKhoaHoc>> ThemLoaiKhoaHoc(Request_ThemLoaiKhoaHoc request);
        Task<ResponseObject<DataResponseLoaiKhoaHoc>> CapNhatThongTinLoaiKhoaHoc(Request_SuaLoaiKhoaHoc request);
        Task<string> XoaLoaiKhoaHoc(int loaiKhoaHocId);
        Task<PageResult<DataResponseLoaiKhoaHoc>> GetAlls(int pageSize, int pageNumber);
        Task<IQueryable<DataResponseLoaiKhoaHoc>> GetAllLoaiKhoahocs();
    }
}
