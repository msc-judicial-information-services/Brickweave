﻿using System.Threading.Tasks;

namespace Brickweave.Cqrs.Cli.Tests.Models
{
    public class CreateFooHandler : ICommandHandler<CreateFoo, string>
    {
        public Task<string> HandleAsync(CreateFoo command)
        {
            return Task.FromResult("success!");
        }
    }
}