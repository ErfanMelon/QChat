using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QChat.Application.Interfaces;
using QChat.Common.ExtentionMethods;

namespace QChat.Application.Services.Messages.Commands;

public class DeleteMessageCommand : IRequest<Result>
{
    public DeleteMessageCommand(string? userId, string? messageId, string? chatId)
    {
        UserId = userId;
        MessageId = messageId;
        ChatId = chatId;
    }

    public string? UserId { get; set; }
    public string? MessageId { get; set; }
    public string? ChatId { get; set; }
    public class Validator : AbstractValidator<DeleteMessageCommand>
    {
        private readonly IChatDbContext _context;
        public Validator(IChatDbContext context)
        {
            _context = context;

            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(e => e.UserId)
                .NotEmpty().WithMessage("کاربر معتبر نیست");

            RuleFor(e => e.ChatId)
                .NotEmpty().WithMessage("چت یافت نشد");

            RuleFor(e => e.MessageId)
                .NotEmpty().WithMessage("پیام را انتخاب کنید")
                .MustAsync(ValidMessage).WithMessage("پیام در دسترس نیست");

            RuleFor(e => e)
                .MustAsync(UserInChat).WithMessage("چت معتبر نیست")
                .MustAsync(CanDeleteMessage).WithMessage("شما نمیتوانید این پیام را پاک کنید");
            ;
        }

        private Task<bool> UserInChat(DeleteMessageCommand e, CancellationToken arg2)
        {
            return _context.UserChats.AnyAsync(s => s.UserId == e.UserId.ToGuid() && s.ChatId == long.Parse(e.ChatId));
        }

        private Task<bool> ValidMessage(string arg1, CancellationToken arg2)
        {
            return _context.Messages.AnyAsync(e => e.Id == arg1.ToGuid());
        }

        private Task<bool> CanDeleteMessage(DeleteMessageCommand command, CancellationToken arg2)
        {
            return _context.Messages.AnyAsync(e => e.UserId == command.UserId.ToGuid() && e.Id == command.MessageId.ToGuid());
        }
    }
    public class Handler : IRequestHandler<DeleteMessageCommand, Result>
    {
        private readonly IChatDbContext _context;

        public Handler(IChatDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _context.Messages.SingleAsync(e => e.Id == request.MessageId.ToGuid());
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success("پیام حذف شد");
        }
    }
}
