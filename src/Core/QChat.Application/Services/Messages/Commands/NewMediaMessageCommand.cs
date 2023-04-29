using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QChat.Application.Interfaces;
using QChat.Application.Services.Common.Commands;
using QChat.Common.ExtentionMethods;
using QChat.Domain.Entities;

namespace QChat.Application.Services.Messages.Commands;

public class NewMediaMessageCommand : IRequest<Maybe<List<string>>>
{
    public string? UserId { get; set; }
    public IFormFile? Media { get; set; }
    public string? ChatId { get; set; }
    public class Validator : AbstractValidator<NewMediaMessageCommand>
    {
        public Validator()
        {
            RuleFor(e => e.UserId)
                .NotEmpty().WithMessage("کاربر معتبر نیست");

            RuleFor(e => e.ChatId)
                .NotEmpty().WithMessage("چت معتبر نیست")
                .Must(c => long.TryParse(c, out _)).WithMessage("چت معتبر نیست");
        }
    }
    public class Handler : IRequestHandler<NewMediaMessageCommand, Maybe<List<string>>>
    {
        private readonly IChatDbContext _context;
        private readonly IMediator _mediator;

        public Handler(IChatDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Maybe<List<string>>> Handle(NewMediaMessageCommand request, CancellationToken cancellationToken)
        {
            var chat = await _context.UserChats
                .AsNoTracking()
                .SingleOrDefaultAsync(e => e.UserId == request.UserId.ToGuid() && e.ChatId == long.Parse(request.ChatId));

            if (chat == null)
                return Maybe<List<string>>.None;

            var resultUpload = await _mediator.Send(new UploadFileCommand(request.Media));

            if (resultUpload.IsFailure)
                return Maybe<List<string>>.None;

            var message = new Message
            {
                ChatId = long.Parse(request.ChatId),
                Content = request.Media.FileName,
                FileSrc = resultUpload.Value,
                PostDate = DateTime.Now,
                UserId = request.UserId.ToGuid()
            };
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync(cancellationToken);
            return await _context
                .UserChats
                .Where(e => e.ChatId == long.Parse(request.ChatId))
                .Select(e => e.UserId.ToString().ToLower())
                .ToListAsync();
        }
    }
}
