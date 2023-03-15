using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QChat.Application.Interfaces;
using QChat.Common.ExtentionMethods;
using QChat.Domain.Entities;

namespace QChat.Application.Services.Chats.Commands;

public class NewMessageCommand : IRequest
{
    public NewMessageCommand(string? userId, string? chatId, string? messageBody)
    {
        UserId = userId;
        ChatId = chatId;
        MessageBody = messageBody;
        PostDate = DateTime.Now;
    }
    public NewMessageCommand() { }
    public string? UserId { get; set; }
    public string? ChatId { get; set; }
    public string? MessageBody { get; set; }
    public DateTime PostDate { get; init; }
    public class Validator : AbstractValidator<NewMessageCommand>
    {
        public Validator()
        {
            RuleFor(e => e.UserId)
                .NotEmpty().WithMessage("کاربر معتبر نیست");

            RuleFor(e => e.ChatId)
                .NotEmpty().WithMessage("چت معتبر نیست")
                .Must(c=>long.TryParse(c,out _)).WithMessage("چت معتبر نیست");

            RuleFor(e => e.MessageBody)
                .NotEmpty().WithMessage("پیام را وارد کنید");
        }
    }
    public class Handler : IRequestHandler<NewMessageCommand>
    {
        private readonly IChatDbContext _context;
        private readonly IMapper _mapper;

        public Handler(IChatDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task Handle(NewMessageCommand request, CancellationToken cancellationToken)
        {
            var chat = await _context.UserChats
                .AsNoTracking()
                .SingleOrDefaultAsync(e => e.UserId == request.UserId.ToGuid() && e.ChatId ==long.Parse(request.ChatId));

            if (chat == null)
                return;

            var message = _mapper.Map<Message>(request);
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
