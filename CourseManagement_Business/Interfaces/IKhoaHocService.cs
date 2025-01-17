using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement_Models.RequestModels.KhoaHocRequests;
using WebCourseManagement2_Models.RequestModels.InputRequests;
using WebCourseManagement2_Models.RequestModels.KhoaHocRequests;
using WebCourseManagement2_Models.ResponseModels.DataHoaDon;
using WebCourseManagement2_Models.ResponseModels.DataKhoaHoc;
using WebCourseManagement2_Models.Responses;
using WebCourseManagement2_Repositories.HandlePagination;

namespace WebCourseManagement2_Business.Interfaces
{
    public interface IKhoaHocService
    {
        Task<ResponseObject<DataResponseKhoaHoc>> ThemKhoaHoc(int nguoiTaoId, Request_ThemKhoaHoc request);
        Task<ResponseObject<DataResponseKhoaHoc>> SuaThongTinKhoaHoc(int nguoiSuaId, Request_SuaThongTinKhoaHoc request);
        Task<string> XoaKhoaHoc(int khoaHocId);
        Task<ResponseObject<DataResponseKhoaHoc>> GetKhoaHocById(int khoaHocId);
        Task<PageResult<DataResponseKhoaHoc>> GetAlls(int pageSize, int pageNumber);
        Task<ResponseObject<DataResponseHoaDon>> DangKyKhoaHoc(int nguoiDungId, Request_DangKyKhoaHoc request);
        Task<IQueryable<DataResponseKhoaHoc>> GetAllsKhoahoc(InputKhoaHoc input);
        Task<IQueryable<DataResponseKhoaHoc>> GetKhoaHocByNguoiDung(int nguoiDungId);
        Task<IQueryable<DataResponseKhoaHoc>> GetAllKhoaHocTheoNguoiTao(int nguoiTaoId);
        Task<IQueryable<DataResponseKhoaHoc>> GetAllKhoaHocTheoNguoiDung(int nguoiDungId);
    }
}
