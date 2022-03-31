// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

using FluentValidation;
using Lamar;
using Lamar.Scanning.Conventions;
using Moq;
using Shipwright.Commands.Decorators;
using Shipwright.Commands.Dispatch;
using Xunit;

namespace Shipwright.Commands.Extensions;

public class LamarExtensionsTests
{
    public class AddShipwrightCommandDispatch
    {
        ServiceRegistry registry = new();
        ServiceRegistry method() => registry.AddShipwrightCommandDispatch();

        [Fact]
        public void requires_registry()
        {
            registry = null!;
            Assert.Throws<ArgumentNullException>( nameof(registry), method );
        }

        [Fact]
        public void registers_dispatcher_and_returns_registry()
        {
            var returned = method();
            using var container = new Container( registry );
            var actual = container.GetInstance<ICommandDispatcher>();
            Assert.IsType<CommandDispatcher>( actual );
            Assert.Same( registry, returned );
        }
    }

    public class AddShipwrightCommandCancellation
    {
        ServiceRegistry registry = new();
        ServiceRegistry method() => registry.AddShipwrightCommandCancellation();

        [Fact]
        public void requires_registry()
        {
            registry = null!;
            Assert.Throws<ArgumentNullException>( nameof(registry), method );
        }

        [Fact]
        public async Task decorates_handler_and_returns_registry()
        {
            registry.For<ICommandHandler<FakeCommand, Guid>>().Add<FakeCommand.Handler>();
            var returned = method();

            await using var container = new Container( registry );
            var handler = container.GetInstance<ICommandHandler<FakeCommand, Guid>>();
            Assert.IsType<CommandCancellationDecorator<FakeCommand, Guid>>( handler );
            Assert.Same( registry, returned );

            // ensure handler is decorated
            var command = new FakeCommand();
            var actual = await handler.Execute( command, default );
            Assert.Equal( command.Id, actual );
        }
    }

    public class AddShipwrightCommandValidation
    {
        ServiceRegistry registry = new();
        ServiceRegistry method() => registry.AddShipwrightCommandValidation();

        [Fact]
        public void requires_registry()
        {
            registry = null!;
            Assert.Throws<ArgumentNullException>( nameof(registry), method );
        }

        [Fact]
        public async Task decorates_handler_and_returns_registry()
        {
            registry.For<ICommandHandler<FakeCommand, Guid>>().Add<FakeCommand.Handler>();
            registry.For<IValidator<FakeCommand>>().Add<FakeCommand.Validator>();
            var returned = method();

            await using var container = new Container( registry );
            var handler = container.GetInstance<ICommandHandler<FakeCommand, Guid>>();
            Assert.IsType<CommandValidationDecorator<FakeCommand, Guid>>( handler );
            Assert.Same( registry, returned );

            // ensure handler is decorated
            var command = new FakeCommand();
            var actual = await handler.Execute( command, default );
            Assert.Equal( command.Id, actual );
        }
    }

    public class AddCommandHandlers
    {
        IAssemblyScanner scanner = new Mock<IAssemblyScanner>( MockBehavior.Strict ).Object;
        IAssemblyScanner method() => scanner.AddCommandHandlers();

        [Fact]
        public void requires_scanner()
        {
            scanner = null!;
            Assert.Throws<ArgumentNullException>( nameof(scanner), method );
        }

        [Fact]
        public void registers_handlers_in_assembly()
        {
            var registry = new ServiceRegistry();
            registry.Scan( assemblyScanner =>
            {
                (scanner = assemblyScanner).AssemblyContainingType<LamarExtensionsTests>();
                method();
            } );

            using var container = new Container( registry );
            var actual = container.GetInstance<ICommandHandler<FakeCommand, Guid>>();
            Assert.IsType<FakeCommand.Handler>( actual );
        }
    }
}
