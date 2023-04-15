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
            var chatIds =await _context.UserChats
                .AsNoTracking()
                .Where(e => e.UserId == request.UserId.ToGuid())
                .Select(e=>e.ChatId)
                .ToListAsync();

            var chats =new List<ChatBreifDto>();
            foreach (long id in chatIds)
            {
                var chat =await _context.Chats
                    .AsNoTracking()
                    .Include(e=>e.Messages)
                    .Include(e=>e.UserChats)
                    .ThenInclude(e=>e.User)
                    .SingleAsync(e=>e.Id==id);

                string title="";
                if (chat is PrivateChat)
                    title = chat.UserChats.DistinctBy(e => e.UserId).Single(e => e.UserId != request.UserId.ToGuid()).User.UserName;
                else
                    title = chat.Title;
                chats.Add(new ChatBreifDto
                {
                    Id=chat.Id,
                    Title=title,
                    ImageSrc=chat.ImageSrc,
                    LastMessageDate=chat.Messages.LastOrDefault()?.PostDate.ToShamsi()
                });
            }
            return chats;
        }
    }
}
public class ChatBreifDto
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? ImageSrc { get; set; }
    public string? LastMessageDate { get; set; }
}
