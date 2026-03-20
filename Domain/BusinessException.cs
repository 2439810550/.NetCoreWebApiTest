using day1.Domain.Enum;

namespace day1.Domain
{

    /// <summary>
    /// 业务异常类，用于表示在业务逻辑层发生的异常情况。它继承自系统的Exception类，并添加了一个Code属性，用于存储具体的业务错误代码。
    /// </summary>
    public class BusinessException:Exception
    {
        public int Code { get; set; }
        public BusinessException(BusinessErrorCode businessErrorCode,string message) : base(message)
        {
            Code=(int)businessErrorCode;
        }
    }
}
