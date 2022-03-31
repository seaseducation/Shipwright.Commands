// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

namespace Shipwright.Commands;

/// <summary>
/// Defines a command that returns a result when executed.
/// </summary>
/// <typeparam name="TResult">Type returned by executing the command.</typeparam>
public abstract record Command<TResult>;

/// <summary>
/// Defines a command that returns no result when executed.
/// This is shorthand for a <see cref="Command{TResult}"/> that returns <see cref="ValueTuple"/>.
/// </summary>
public abstract record Command : Command<ValueTuple>;
