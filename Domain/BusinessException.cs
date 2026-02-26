using day1.Domain.Enum;

namespace day1.Domain
{
    public class BusinessException:Exception
    {
        public int Code { get; set; }
        public BusinessException(BusinessErrorCode businessErrorCode,string message) : base(message)
        {
            Code=(int)businessErrorCode;
        }
    }
}
