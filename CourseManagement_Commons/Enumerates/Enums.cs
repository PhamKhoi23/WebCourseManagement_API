using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebCourseManagement2_Commons.Enumerates
{
    public class Enums
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum GioiTinh
        {
            Nam,
            Nu
        }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum LoaiComment
        {
            BaiViet,
            BaiHoc
        }
        public enum TestCaseEnum
        {
            SUCCESSFULLY = 1,
            WRONG = 2,
            EXCEPTION = 3,
            WAIT = 4
        }
        public enum XacMinhViPham
        {
            DaViPham = 1,
            KhongViPham = 2
        }
    }
}
