﻿// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.Wcf
{
    using System;
    using Microsoft.ServiceFabric.Actors.Client;
    using Microsoft.ServiceFabric.Actors.Remoting.V1.Wcf.Client;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Wcf.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Communication.Wcf;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V1;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using IServiceRemotingClientFactory = Microsoft.ServiceFabric.Services.Remoting.V1.Client.IServiceRemotingClientFactory;

    /// <summary>
    ///     Sets WCF as the default remoting provider for actors.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class WcfActorRemotingProviderAttribute : ActorRemotingProviderAttribute
    {
        private const long DefaultMaxMessageSize = 4 * 1024 * 1024;
        private static readonly TimeSpan DefaultOpenCloseTimeout = TimeSpan.FromMilliseconds(5000);

        /// <summary>
        ///     Gets or Sets the maximum size of the remoting message in bytes.
        ///     If value for this property is not specified or it is less than or equals to zero,
        ///     a default value of 4,194,304 bytes (4 MB) is used.
        /// </summary>
        /// <value>
        ///     The maximum size of the remoting message in bytes. If this value is not specified
        ///     or it is less than or equals to zero, a default value of 4,194,304 bytes (4 MB) is used.
        /// </value>
        public long MaxMessageSize { get; set; }

        /// <summary>
        ///     Gets or Sets time to wait in milliseconds for opening the connection.
        /// </summary>
        /// <value>
        ///     Time to wait in milliseconds for opening the connection. If this value is not specified
        ///     or it is less than zero, default value of 5000 milliseconds is used.
        /// </value>
        public long OpenTimeoutInMilliSeconds { get; set; }

        /// <summary>
        ///     Gets or Sets time to wait in milliseconds for messages to drain on the connections before aborting the connection.
        /// </summary>
        /// <value>
        ///     Time to wait in milliseconds for messages to drain on the connections before aborting the connection.
        /// </value>
        public long CloseTimeoutInMilliSeconds { get; set; }

        /// <summary>
        ///     Creates a service remoting client factory to connect to the remoted actor interfaces.
        /// </summary>
        /// <param name="callbackClient">
        ///     Client implementation where the callbacks should be dispatched.
        /// </param>
        /// <returns>
        ///     A <see cref="Microsoft.ServiceFabric.Actors.Remoting.V1.Wcf.Client.WcfActorRemotingClientFactory" />
        ///     as <see cref="Microsoft.ServiceFabric.Services.Remoting.V1.Client.IServiceRemotingClientFactory" />
        ///     that can be used with <see cref="ActorProxyFactory" /> to
        ///     generate actor proxy to talk to the actor over remoted actor interface.
        /// </returns>
        public override IServiceRemotingClientFactory CreateServiceRemotingClientFactory(
            IServiceRemotingCallbackClient callbackClient)
        {
            return new WcfActorRemotingClientFactory(
                WcfUtility.CreateTcpClientBinding(
                    this.GetMaxMessageSize(),
                    this.GetOpenTimeout(),
                    this.GetCloseTimeout()),
                callbackClient);
        }


        /// <summary>
        ///     Creates a V2 service remoting listener for remoting the actor interfaces.
        /// </summary>
        /// <param name="actorService">
        ///     The implementation of the actor service that hosts the actors whose interfaces
        ///     needs to be remoted.
        /// </param>
        /// <returns>
        ///     An <see cref="IServiceRemotingListener" />
        ///     for the specified actor service.
        /// </returns>
        public override IServiceRemotingListener CreateServiceRemotingListenerV2(ActorService actorService)
        {
            return new WcfActorServiceRemotingListener(
                actorService,
                WcfUtility.CreateTcpListenerBinding(
                    this.GetMaxMessageSize(),
                    this.GetOpenTimeout(),
                    this.GetCloseTimeout()));
        }

        /// <summary>
        ///     Creates a V2 service remoting client factory to connect to the remoted actor interfaces.
        /// </summary>
        /// <param name="callbackMessageHandler">
        ///     Client implementation where the callbacks should be dispatched.
        /// </param>
        /// <returns>
        ///     A <see cref="Microsoft.ServiceFabric.Actors.Remoting.V2.Wcf.Client.WcfActorRemotingClientFactory" />
        ///     as <see cref="Microsoft.ServiceFabric.Services.Remoting.V2.Client.IServiceRemotingClientFactory" />
        ///     that can be used with <see cref="ActorProxyFactory" /> to
        ///     generate actor proxy to talk to the actor over remoted actor interface.
        /// </returns>
        public override Services.Remoting.V2.Client.IServiceRemotingClientFactory CreateServiceRemotingClientFactoryV2(
            IServiceRemotingCallbackMessageHandler callbackMessageHandler)
        {
            return new V2.Wcf.Client.WcfActorRemotingClientFactory(
                WcfUtility.CreateTcpClientBinding(
                    this.GetMaxMessageSize(),
                    this.GetOpenTimeout(),
                    this.GetCloseTimeout()),
                callbackMessageHandler);
        }

        /// <summary>
        ///     Creates a service remoting listener for remoting the actor interfaces.
        /// </summary>
        /// <param name="actorService">
        ///     The implementation of the actor service that hosts the actors whose interfaces
        ///     needs to be remoted.
        /// </param>
        /// <returns>
        ///     An <see cref="IServiceRemotingListener" />
        ///     for the specified actor service.
        /// </returns>
        public override IServiceRemotingListener CreateServiceRemotingListener(
            ActorService actorService)
        {
            return new V1.Wcf.Runtime.WcfActorServiceRemotingListener(
                actorService,
                WcfUtility.CreateTcpListenerBinding(
                    this.GetMaxMessageSize(),
                    this.GetOpenTimeout(),
                    this.GetCloseTimeout()));
        }

        private long GetMaxMessageSize()
        {
            return this.MaxMessageSize > 0 ? this.MaxMessageSize : DefaultMaxMessageSize;
        }

        private TimeSpan GetCloseTimeout()
        {
            return this.CloseTimeoutInMilliSeconds > 0 ? TimeSpan.FromMilliseconds(this.CloseTimeoutInMilliSeconds) : DefaultOpenCloseTimeout;
        }

        private TimeSpan GetOpenTimeout()
        {
            return this.OpenTimeoutInMilliSeconds > 0 ? TimeSpan.FromMilliseconds(this.OpenTimeoutInMilliSeconds) : DefaultOpenCloseTimeout;
        }
    }
}