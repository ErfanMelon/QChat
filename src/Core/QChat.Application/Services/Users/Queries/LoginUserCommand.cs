using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QChat.Application.Interfaces;
using QChat.Common.Security;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace QChat.Application.Services.Users.Queries;

public class LoginUserCommand:IRequest<Result<ClaimsPrincipal>>
{
    [Display(Name = "نام کاربری")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(100, ErrorMessage = "{0} نباید بیشتر از 100 کاراکتر باشد")]
    [MinLength(3, ErrorMessage = "{0} نباید کمتر از 3 کاراکتر باشد")]
    public string? Username { get; set; }
    [Display(Name = "گذرواژه")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(100, ErrorMessage = "{0} نباید بیشتر از 100 کاراکتر باشد")]
    [MinLength(8, ErrorMessage = "{0} نباید کمتر از 8 کاراکتر باشد")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public class Validator:AbstractValidator<LoginUserCommand>
    {
        private readonly IChatDbContext _context;
        public Validator(IChatDbContext context)
        {
            _context = context;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(e => e.Username)
                .NotEmpty()
                .MustAsync(RegisteredAccount).WithMessage("کاربری با این شناسه ثبت نام نکرده");
        }

        private Task<bool> RegisteredAccount(string arg1, CancellationToken arg2)
        {
            return _context.Users.AnyAsync(c => c.UserName == arg1);
        }
    }
    public class Handler : IRequestHandler<LoginUserCommand, Result<ClaimsPrincipal>>
    {
        private readonly IChatDbContext _context;
        public Handler(IChatDbContext context)
        {
            _context = context;
        }
        public async Task<Result<ClaimsPrincipal>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user =await _context.Users
                .SingleAsync(e => e.UserName == request.Username);

            string hashpassword = PasswordHasher.EncodePasswordMd5(request.Password);
            if (hashpassword != user.Password)
                return Result.Failure<ClaimsPrincipal>("رمز عبور اشتباه است");
            else
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.UserName)
                };
                ClaimsIdentity identity=new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal=new ClaimsPrincipal(identity);

                return Result.Success(principal);
            }
        }
    }
}
