using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QChat.Application.Interfaces;
using QChat.Common.Security;
using QChat.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace QChat.Application.Services.Users.Commands;

public class CreateUserCommand : IRequest<Result>
{
    [Display(Name = "نام کاربری")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(100, ErrorMessage = "{0} نباید بیشتر از 100 کاراکتر باشد")]
    [MinLength(3, ErrorMessage = "{0} نباید کمتر از 3 کاراکتر باشد")]
    public string? UserName { get; set; }

    [Display(Name = "گذرواژه")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(100, ErrorMessage = "{0} نباید بیشتر از 100 کاراکتر باشد")]
    [MinLength(8, ErrorMessage = "{0} نباید کمتر از 8 کاراکتر باشد")]
    [DataType(DataType.Password)]
    public string? PassWord { get; set; }

    [Display(Name = "تکرار گذرواژه")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(100, ErrorMessage = "{0} نباید بیشتر از 100 کاراکتر باشد")]
    [DataType(DataType.Password)]
    [Compare(nameof(PassWord), ErrorMessage = "گذرواژه و تکرار آن برابر نیست")]
    public string? RePassWord { get; set; }

    public class Validator : AbstractValidator<CreateUserCommand>
    {
        private readonly IChatDbContext _context;

        public Validator(IChatDbContext context)
        {
            _context = context;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(e => e.UserName)
                .NotEmpty()
                .MustAsync(UniqueUserName).WithMessage("نام کاربری استفاده شده");
        }

        private async Task<bool> UniqueUserName(string arg1, CancellationToken arg2)
        {
            return !(await _context.Users.AnyAsync(c=>c.UserName==arg1));
        }
    }
    public class Handler : IRequestHandler<CreateUserCommand, Result>
    {
        private readonly IChatDbContext _context;

        public Handler(IChatDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = (User)request;

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
    public static explicit operator User(CreateUserCommand command)
    {
        return new User
        {
            Password= PasswordHasher.EncodePasswordMd5(command.PassWord),
            UserName=command.UserName
        };
    }
}
