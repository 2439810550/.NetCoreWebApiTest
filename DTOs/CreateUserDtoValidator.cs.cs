using FluentValidation;
namespace day1.DTOs
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDTO>
    {
        public CreateUserDtoValidator() 
        {
         RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("用户名不能为空")
            .MinimumLength(3).WithMessage("用户名长度不能小于3位");
        RuleFor(x => x.PassWord)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(6).WithMessage("密码长度不能小于6位")
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d._-]+$").WithMessage("密码必须包含字母和数字且不能有._-之外的符号");
        }
    }
}
