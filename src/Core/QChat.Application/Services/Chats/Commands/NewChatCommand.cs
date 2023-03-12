using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QChat.Application.Interfaces;
using QChat.Application.Services.Common.Commands;
using QChat.Common.ExtentionMethods;
using QChat.Domain.Entities;

namespace QChat.Application.Services.Chats.Commands;

public class NewChatCommand : IRequest<Result<long>>
{
    public string? Title { get; set; }
    public IFormFile? Image { get; set; }
    public string? UserId { get; set; }

    public class Validator : AbstractValidator<NewChatCommand>
    {
        private readonly IChatDbContext _context;

        public Validator(IChatDbContext context)
        {
            _context = context;
            RuleFor(e => e.UserId)
                .NotEmpty()
                .MustAsync(ValidUser).WithMessage("کاربر پیدا نشد");

            RuleFor(e => e.Title)
                .NotEmpty().WithMessage("نام گروه را وارد کنید")
                .MaximumLength(100).WithMessage("نام گروه باید کمتر از 100 کاراکتر باشد")
                .MinimumLength(3).WithMessage("نام گروه باید بیشتر از 3 کاراکتر باشد");
        }

        private Task<bool> ValidUser(string arg1, CancellationToken arg2)
        {
            return _context.Users.AnyAsync(e => e.Id == arg1.ToGuid(), arg2);
        }
    }
    public class Handler : IRequestHandler<NewChatCommand, Result<long>>
    {
        private readonly IChatDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public Handler(IChatDbContext context, IMapper mapper, IMediator mediator)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<Result<long>> Handle(NewChatCommand request, CancellationToken cancellationToken)
        {
            var newChat = _mapper.Map<Chat>(request);

            if (request.Image != null)
            {
                var uploadResult = await _mediator.Send(new UploadFileCommand(request.Image));
                if (uploadResult.IsFailure)
                    throw new Exception(uploadResult.Error);
                else
                    newChat.ImageSrc = uploadResult.Value;
            }
            await _context.Chats.AddAsync(newChat);

            var user = await _context.Users
                .SingleAsync(e => e.Id == request.UserId.ToGuid());

            var userChat = new UserChat
            {
                User = user,
                UserId = user.Id,
                Chat = newChat,
                ChatId = newChat.Id
            };
            await _context.UserChats.AddAsync(userChat);

            await _context.SaveChangesAsync(cancellationToken);

            return newChat.Id;
        }
    }
}
