using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QChat.Application.Interfaces;
using QChat.Common.ExtentionMethods;
using QChat.Domain.Entities;

namespace QChat.Application.Services.Users.Queries;

public class GetChatsQuery : IRequest<Result<List<ChatBreifDto>>>
{
    public GetChatsQuery(string? userId)
    {
        UserId = userId;
    }

    public string? UserId { get; set; }
    public class Handler : IRequestHandler<GetChatsQuery, Result<List<ChatBreifDto>>>
    {
        private readonly IChatDbContext _context;

        public Handler(IChatDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<ChatBreifDto>>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
        {
            var chats = _context.UserChats
                .AsNoTracking()
                .Include(e => e.Chat)
                .Where(e => e.UserId == request.UserId.ToGuid())
                .Select(e => (ChatBreifDto)e.Chat);

            return chats.ToList();
        }
    }
}
public class ChatBreifDto
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? ImageSrc { get; set; }
    public static explicit operator ChatBreifDto(Chat chat)
    {

        return new ChatBreifDto
        {
            Id = chat.Id,
            Title = chat.Title,
            ImageSrc = chat.ImageSrc,
        };
    }
}
