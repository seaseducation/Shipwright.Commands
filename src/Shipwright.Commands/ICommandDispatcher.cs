// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

namespace Shipwright.Commands;

/// <summary>
/// Defines a dispatcher for locating and executing handlers for commands.
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Locates and executes the handler for the given command.
    /// </summary>
    /// <typeparam name="TResult">Type returned by the command's execution.</typeparam>
    /// <param name="command">Command whose handler to locate and execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the command's execution.</returns>
    public Task<TResult> Execute<TResult>( Command<TResult> command, CancellationToken cancellationToken );
}
