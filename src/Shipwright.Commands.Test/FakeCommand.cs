// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

using FluentValidation;

namespace Shipwright.Commands;

public record FakeCommand : Command<Guid>
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public class Handler : ICommandHandler<FakeCommand, Guid>
    {
        public Guid Result { get; init; } = Guid.NewGuid();

        public Task<Guid> Execute( FakeCommand command, CancellationToken cancellationToken ) => Task.FromResult( command.Id );
    }

    public class Validator : AbstractValidator<FakeCommand>
    {
        public Validator()
        {
            RuleFor( _ => _.Id ).NotEqual( Guid.Empty );
        }
    }
}
