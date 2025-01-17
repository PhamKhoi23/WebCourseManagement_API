using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCourseManagement2_Commons.DefaultConstants
{
    public class Constants
    {
        public class AppSettingKeys
        {
            public const string DEFAULT_CONTROLLER_ROUTE = "api/[controller]";
            public const string DEFAULT_CONNECTION = "DefaultConnection";
            public const string AUTH_SECRET = "AuthSecret";
        }

        public class ExceptionMessage
        {
            public const string NOT_FOUND = "{0} không được tìm thấy.";
            public const string ITEM_NOT_FOUND = "Không tìm thấy bản ghi.";
            public const string ALREADY_EXIST = "{0} đã tồn tại.";
            public const string SUCCESS = "{0} thành công.";
            public const string INVALID = "{0} không hợp lệ.";
            public const string FILE_NOT_FOUND = "File {0} không được tìm thấy";
            public const string EXPIRATION_TIME = "Hết hạn";
            public const string VERIFIED = "Tài khoản đã được kích hoạt";
            public const string REQUEST_TO_FILL_INFORMATION = "Vui lòng điền đầy đủ thông tin";
            public const string INVALID_PASSWORD = "Mật khẩu phải có ít nhất 1 chữ in hoa, 1 chữ thường, 1 chữ số, 1 ký tự đặc biệt";
            public const string INVALID_EMAIL = "Email sai định dạng";
            public const string INVALID_PHONE_NUMBER = "Số điện thoại sai định dạng";
            public const string ALREADY_ACCOUNT_NAME_EXISTED = "Tên tài khoản đã tồn tại trên hệ thống! Vui lòng thử lại";
            public const string ALREADY_EMAIL_EXISTED = "Email đã tồn tại trên hệ thống! Vui lòng thử lại";
            public const string SUBJECT_EMAIL = "Nhận mã xác nhận để xác nhận đăng ký tài khoản mới từ đây: ";
            public const string REGISTER_SUCCESS = "Đăng ký tài khoản thành công, nhận mã xác nhận gửi về email để đăng ký tài khoản";
        }
        public class DefaultValue
        {
            public const string DEFAULT_CONTROLLER_ROUTE = "api/[controller]/[action]";
            public const string DEFAULT_CONTROLLER_ROUTE_WITHOUT_ACTION = "api/[controller]";
            public const string DEFAULT_ACTION_ROUTE = "[action]";
        }
    }
}
