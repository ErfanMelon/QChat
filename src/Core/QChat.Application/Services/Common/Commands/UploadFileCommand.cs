using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace QChat.Application.Services.Common.Commands;

public class UploadFileCommand : IRequest<Result<string>>
{
    private const string SavePath = @"Uploads\Images\";

    public UploadFileCommand(IFormFile file)
    {
        File = file;
    }

    public IFormFile File { get; set; }
    public class Validator : AbstractValidator<UploadFileCommand>
    {
        public Validator()
        {
            RuleFor(e => e.File).NotEmpty();
        }
    }
    public class Handler : IRequestHandler<UploadFileCommand, Result<string>>
    {
        private readonly string serverPath;

        public Handler(IHostingEnvironment environment)
        {
            serverPath = Path.Combine(environment.WebRootPath, SavePath);
        }

        public async Task<Result<string>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(serverPath))
                Directory.CreateDirectory(serverPath);

            FileInfo fi = new FileInfo(request.File.FileName);
            string fileName = Guid.NewGuid() + fi.Extension;

            string filepath = Path.Combine(serverPath, fileName);
            using (var sr = new FileStream(filepath, FileMode.Create))
            {
                await request.File.CopyToAsync(sr);
            }

            return Path.Combine(SavePath, fileName);
        }
    }
}
