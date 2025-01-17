using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Models.RequestModels.BaiVietRequests;
using WebCourseManagement2_Models.RequestModels.InputRequests;
using WebCourseManagement2_Models.ResponseModels.DataBaiViet;
using WebCourseManagement2_Models.Responses;
using WebCourseManagement2_Repositories.HandlePagination;

namespace WebCourseManagement2_Business.Interfaces
{
    public interface IBaiVietService
    {
        Task<ResponseObject<DataResponseBaiViet>> TaoBaiViet(int nguoiDungId, Request_TaoBaiViet request);
        Task<ResponseObject<DataResponseBaiViet>> SuaBaiViet(int nguoiDungId, Request_SuaBaiViet request);
        Task<string> XoaBaiViet(int baiVietId);
        Task<ResponseObject<DataResponseBaiViet>> GetBaiVietById(int baiVietId);
        Task<IQueryable<DataResponseBaiViet>> GetAlls(InputBaiViet input);
        Task<ResponseObject<DataResponseBinhLuanBaiViet>> TaoBinhLuanBaiViet(int nguoiDungId, Request_TaoBinhLuanBaiViet request);
        Task<ResponseObject<DataResponseBinhLuanBaiViet>> SuaBinhLuanBaiViet(int nguoiDungId, Request_SuaBinhLuanBaiViet request);
        Task<string> XoaBinhLuan(int binhLuanId);
        Task<ResponseObject<DataResponseBinhLuanBaiViet>> TraLoiBinhLuanBaiViet(int nguoiDungId, Request_TraLoiBinhLuanBaiViet request);
        Task<string> DuyetBaiViet(int baiVietId);
        Task<ResponseObject<DataResponseLoaiBaiViet>> ThemLoaiBaiViet(Request_ThemLoaiBaiViet request);
        Task<ResponseObject<DataResponseLoaiBaiViet>> SuaLoaiBaiViet(Request_SuaLoaiBaiViet request);
        Task<string> XoaLoaiBaiViet(int loaiBaiVietId);
        Task<IQueryable<DataResponseLoaiBaiViet>> GetAllsLoaiBaiViet(string? tenLoaiBaiViet);
        Task<ResponseObject<DataResponseLoaiBaiViet>> GetLoaiBaiVietById(int loaiBaiVietId);
        Task<IQueryable<DataResponseBaiViet>> GetAllBaiVietChuaDuocDuyet();
        Task<IQueryable<DataResponseBaiViet>> GetBaiVietByIdNguoiDung(int nguoiDungId);
        Task<ResponseObject<DataResponseBaiViet>> TuChoiBaiViet(int baiVietId);
    }
}
