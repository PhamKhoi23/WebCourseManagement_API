using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Models.RequestModels.TestCaseRequests;
using WebCourseManagement2_Models.ResponseModels.DataTestCase;
using WebCourseManagement2_Models.Responses;

namespace WebCourseManagement2_Business.Interfaces
{
    public interface ITestCaseService
    {
        Task<ResponseObject<DataResponseTestCaseResult>> TaoTestCase(Request_TaoTestCase request);
        //Task<ResponseObject<TestCaseResult>> RunTestCase(Request_TaoTestCase testCase);
    }
}
