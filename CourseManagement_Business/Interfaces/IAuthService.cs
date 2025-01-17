using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Models.RequestModels.AuthRequests;
using WebCourseManagement2_Models.ResponseModels.DataNguoiDung;
using WebCourseManagement2_Models.Responses;
using WebCourseManagement2_Repositories;

namespace WebCourseManagement2_Business.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseObject<DataResponseNguoiDung>> DangKy(Request_DangKy request);
        Task<ResponseObject<DataResponseToken>> DangNhap(Request_DangNhap request);
        Task<ResponseObject<DataResponseNguoiDung>> XacNhanDangKyTaiKhoan(Request_XacNhanDangKyTaiKhoan request);
        Task<string> XacNhanQuenMatKhau(Request_XacNhanQuenMatKhau request);
        Task<string> TaoMatKhauMoi(Request_TaoMatKhauMoi request);
        Task<string> DoiMatKhau(int nguoiDungId, Request_DoiMatKhau request);
        ResponseObject<DataResponseToken> RenewAccessToken(Request_RenewToken request);
        string SendEmail(EmailTo emailTo);
    }
}
