using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Models.RequestModels.ThucHanhRequests;
using WebCourseManagement2_Models.ResponseModels.DataThucHanh;
using WebCourseManagement2_Models.Responses;

namespace WebCourseManagement2_Business.Interfaces
{
    public interface IThucHanhService
    {
        Task<ResponseObject<DataResponseThucHanh>> ThemYeuCauThucHanh(Request_ThemYeuCauThucHanh request);
    }
}
