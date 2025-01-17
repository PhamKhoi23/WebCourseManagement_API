using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCourseManagement2_Business.Interfaces
{
    public interface IVNPayService
    {
        Task<string> TaoDuongDanThanhToan(int hoaDonId, HttpContext httpContext, int id);
        Task<string> VNPayReturn(IQueryCollection vnpayData);
    }
}
