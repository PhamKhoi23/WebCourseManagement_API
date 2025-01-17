using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Models.RequestModels.BaoCaoRequests;
using WebCourseManagement2_Models.ResponseModels.DataBaoCao;
using WebCourseManagement2_Models.Responses;

namespace WebCourseManagement2_Business.Interfaces
{
    public interface IBaoCaoService
    {
        Task<ResponseObject<DataResponseBaoCao>> GuiBaoCao(int nguoiDungId, Request_GuiBaoCao request);
        Task<ResponseObject<DataResponseBaoCao>> DuyetBaoCao(Request_DuyetBaoCao request);
    }
}
