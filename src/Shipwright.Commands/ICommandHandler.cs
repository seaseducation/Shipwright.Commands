// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

namespace Shipwright.Commands;

/// <summary>
/// Defines a handler that can execute a command.
/// </summary>
/// <typeparam name="TCommand">Type of the commands the handler can execute.</typeparam>
/// <typeparam name="TResult">Type returned by the command's execution.</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface ICommandHandler<TCommand,TResult> where TCommand : Command<TResult>
{
    /// <summary>
    /// Executes the given command.
    /// </summary>
    /// <param name="command">Command to execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the command's execution as an awaitable task.</returns>
    public Task<TResult> Execute( TCommand command, CancellationToken cancellationToken );
}
