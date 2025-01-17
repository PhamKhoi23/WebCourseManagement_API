
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Commons.DefaultConstants;

namespace WebCourseManagement2_Commons.HandleExceptions
{
    public class InvalidException : Exception
    {
        public string Name { get; private set; }

        public InvalidException(string name)
        {
            Name = name;
        }
        public override string Message => string.Format(Constants.ExceptionMessage.INVALID, Name);
    }
}
