// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

namespace Shipwright.Commands.Implementations;

/// <summary>
/// Decorates a command handler to add pre-execute cancellation support.
/// </summary>
/// <typeparam name="TCommand">Type of the command whose handler to decorate.</typeparam>
/// <typeparam name="TResult">Type returned by the command's execution.</typeparam>
public class CommandCancellationDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult> where TCommand : Command<TResult>
{
    readonly ICommandHandler<TCommand, TResult> _inner;

    public CommandCancellationDecorator( ICommandHandler<TCommand, TResult> inner )
    {
        _inner = inner ?? throw new ArgumentNullException( nameof(inner) );
    }

    public async Task<TResult> Execute( TCommand command, CancellationToken cancellationToken )
    {
        if ( command == null ) throw new ArgumentNullException( nameof(command) );

        cancellationToken.ThrowIfCancellationRequested();
        return await _inner.Execute( command, cancellationToken );
    }
}
