// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

using Lamar;
using Lamar.IoC;
using Moq;
using Xunit;

namespace Shipwright.Commands.Dispatch;

public class CommandDispatcherTests
{
    readonly Mock<IServiceContext> mockServiceContext = new( MockBehavior.Strict );
    IServiceContext serviceContext;
    ICommandDispatcher instance() => new CommandDispatcher( serviceContext );

    protected CommandDispatcherTests()
    {
        serviceContext = mockServiceContext.Object;
    }

    public class Constructor : CommandDispatcherTests
    {
        [Fact]
        public void requires_serviceContext()
        {
            serviceContext = null!;
            Assert.Throws<ArgumentNullException>( nameof(serviceContext), instance );
        }
    }

    public class Execute : CommandDispatcherTests
    {
        FakeCommand command = new();
        CancellationToken cancellationToken;
        Task<Guid> method() => instance().Execute( command, cancellationToken );

        [Fact]
        public async Task requires_command()
        {
            command = null!;
            await Assert.ThrowsAsync<ArgumentNullException>( nameof(command), method );
        }

        [Theory]
        [InlineData( true )]
        [InlineData( false )]
        public async Task requires_handler_to_be_registered( bool canceled )
        {
            cancellationToken = new( canceled );
            mockServiceContext.Setup( _ => _.GetInstance( typeof(ICommandHandler<FakeCommand, Guid>) ) ).Returns( null );

            await Assert.ThrowsAsync<LamarMissingRegistrationException>( method );
        }

        [Theory]
        [InlineData( true )]
        [InlineData( false )]
        public async Task returns_result_from_executed_handler_when_found( bool canceled )
        {
            cancellationToken = new( canceled );
            var mockHandler = new Mock<ICommandHandler<FakeCommand, Guid>>( MockBehavior.Strict );
            var handler = mockHandler.Object;

            var expected = Guid.NewGuid();
            mockServiceContext.Setup( _ => _.GetInstance( typeof(ICommandHandler<FakeCommand, Guid>) ) ).Returns( handler );
            mockHandler.Setup( _ => _.Execute( command, cancellationToken ) ).ReturnsAsync( expected );

            var actual = await method();
            Assert.Equal( expected, actual );
        }
    }
}
