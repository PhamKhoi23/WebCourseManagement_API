using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Business.Interfaces;
using WebCourseManagement2_Commons.Enumerates;
using WebCourseManagement2_Models.Converters;
using WebCourseManagement2_Models.DataContexts;
using WebCourseManagement2_Models.RequestModels.TestCaseRequests;
using WebCourseManagement2_Models.ResponseModels.DataTestCase;
using WebCourseManagement2_Models.Responses;

namespace WebCourseManagement2_Business.Implements
{
    public class TestCaseService : ITestCaseService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseTestCaseResult> _responseObject;
        public TestCaseService(AppDbContext context, ResponseObject<DataResponseTestCaseResult> responseObject)
        {
            _context = context;
            _responseObject = responseObject;
        }

        public async Task<ResponseObject<DataResponseTestCaseResult>> TaoTestCase(Request_TaoTestCase request)
        {
            List<DataResponseTestCase> listTest = new List<DataResponseTestCase>();
            DataResponseTestCaseResult dataResponseTestCases = new DataResponseTestCaseResult();
            if (string.IsNullOrWhiteSpace(request.CodeInput))
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng nhập code đầu vào", null);
            }
            if(request.ThucHanhId == null || request.ThucHanhId == 0)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng chọn bài thực hành", null);
            }
            try
            {
                var testList = await _context.testCases.Where(x => x.ThucHanhId == request.ThucHanhId).ToListAsync();
                var thucHanh = await _context.thucHanhs.SingleOrDefaultAsync(x => x.Id == request.ThucHanhId);
                if (thucHanh.CodeDauVao == null)
                {
                    foreach (var test in testList)
                    {
                        DataResponseTestCase data = new DataResponseTestCase();
                        data.LockTestCase = test.KhoaTestCase;
                        if (request.CodeInput.Contains("Directory") || request.CodeInput.Contains("File") || request.CodeInput.Contains("FileInfo"))
                        {
                            data.Result = WebCourseManagement2_Commons.Enumerates.Enums.TestCaseEnum.EXCEPTION;
                            data.Output = "Lỗi!!!";
                            data.RunTimeTotal = "Không tính được!";
                            listTest.Add(data);
                            continue;
                        }
                        var runCode = await ToolCompilerService.RunCodeCSharp(request.CodeInput);
                        data.Input = "Không có code đầu vào";
                        data.ExpectOutput = test.MongDoiDauRa;
                        if (!runCode.Success)
                        {
                            data.Result = WebCourseManagement2_Commons.Enumerates.Enums.TestCaseEnum.EXCEPTION;
                            data.Output = runCode.Exception;
                            data.RunTimeTotal = "Không tính được!";
                            listTest.Add(data);
                            continue;
                        }
                        if (runCode.Success)
                        {
                            if (!runCode.Result.Equals(test.MongDoiDauRa))
                            {
                                data.Result = Enums.TestCaseEnum.WRONG;
                                data.Output = runCode.Result;
                                data.RunTimeTotal = runCode.Time;
                                listTest.Add(data);
                                continue;
                            }
                            data.Result = Enums.TestCaseEnum.SUCCESSFULLY;
                            data.Output = runCode.Result;
                            data.RunTimeTotal = runCode.Time;
                            listTest.Add(data);
                            continue;
                        }
                    }
                }
                else
                {
                    string callTestCode = thucHanh.CallTestCode;
                    foreach (var item in testList)
                    {
                        DataResponseTestCase data = new DataResponseTestCase();
                        data.LockTestCase = item.KhoaTestCase;
                        callTestCode = callTestCode.Replace("variable", item.CodeDauVao);
                        if (request.CodeInput.Contains("Directory") || request.CodeInput.Contains("File") || request.CodeInput.Contains("FileInfo"))
                        {
                            data.Result = Enums.TestCaseEnum.EXCEPTION;
                            data.Output = "Security Protect!";
                            data.RunTimeTotal = "Lỗi! không tính được";
                            listTest.Add(data);
                            continue;
                        }
                        var runCodeResult = await ToolCompilerService.RunCodeCSharp(request.CodeInput + callTestCode);
                        callTestCode = thucHanh.CallTestCode;
                        data.Input = item.CodeDauVao;
                        data.ExpectOutput = item.MongDoiDauRa;
                        if (!runCodeResult.Success)
                        {
                            data.Result = Enums.TestCaseEnum.EXCEPTION;
                            data.Output = runCodeResult.Exception;
                            data.RunTimeTotal = "Lỗi! không tính được";
                            listTest.Add(data);
                            continue;
                        }
                        if (runCodeResult.Success)
                        {
                            if (!runCodeResult.Result.Equals(item.MongDoiDauRa))
                            {
                                data.Result = Enums.TestCaseEnum.EXCEPTION;
                                data.Output = runCodeResult.Result;
                                data.RunTimeTotal = runCodeResult.Time;
                                listTest.Add(data);
                                continue;
                            }
                            data.Result = Enums.TestCaseEnum.SUCCESSFULLY;
                            data.Output = runCodeResult.Result;
                            data.RunTimeTotal = runCodeResult.Time;
                            listTest.Add(data);
                            continue;
                        }

                    }
                }

                return _responseObject.ResponseSuccess("Tạo test case thành công", dataResponseTestCases);
            }
            catch(Exception ex)
            {
                return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }
    }
}
