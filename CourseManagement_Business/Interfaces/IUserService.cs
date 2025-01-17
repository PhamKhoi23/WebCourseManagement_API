using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Models.RequestModels.NguoiDungRequests;
using WebCourseManagement2_Models.ResponseModels.DataNguoiDung;
using WebCourseManagement2_Models.Responses;
using WebCourseManagement2_Repositories.HandlePagination;

namespace WebCourseManagement2_Business.Interfaces
{
    public interface IUserService
    {
        Task<IQueryable<DataResponseNguoiDung>> GetAlls();
        Task<ResponseObject<DataResponseNguoiDung>> GetNguoiDungById(int id);
        Task<ResponseObject<DataResponseNguoiDung>> CapNhatThongTinNguoiDung(int nguoiDungId, Request_CapNhatThongTinNguoiDung request);
        Task<string> NguoiDungXoaTaiKhoan(int id);
        Task<string> KhoaTaiKhoanNguoiDung(int id);
        Task<string> MoKhoaTaiKhanNguoiDung(int id);
    }
}
