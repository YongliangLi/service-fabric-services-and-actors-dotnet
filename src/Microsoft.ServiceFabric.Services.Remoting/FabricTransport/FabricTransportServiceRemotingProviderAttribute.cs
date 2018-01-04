﻿// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.FabricTransport
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
#if !DotNetCoreClr
    using Microsoft.ServiceFabric.Services.Remoting.V1;
    using Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Runtime;
#endif


    /// <summary>
    ///     Sets Fabric TCP transport as the default service remoting transport provider in the assembly.
    /// </summary>
    public class FabricTransportServiceRemotingProviderAttribute : ServiceRemotingProviderAttribute
    {
        /// <summary>
        ///     Gets or sets the maximum size of the remoting message in bytes.
        ///     If value for this property is not specified or it is less than or equals to zero,
        ///     a default value of 4,194,304 bytes (4 MB) is used.
        /// </summary>
        /// <value>
        ///     The maximum size of the remoting message in bytes. If this value is not specified
        ///     or it is less than or equals to zero, a default value of 4,194,304 bytes (4 MB) is used.
        /// </value>
        public long MaxMessageSize { get; set; }


        /// <summary>
        ///     Gets or Sets the operation timeout in seconds. If the operation is not completed in the specified
        ///     time, it will be timed out. By default, exception handler of
        ///     <see cref="FabricTransportServiceRemotingClientFactory" />
        ///     retries the timed out exception. It is recommended to not change the operation timeout from it's default value.
        /// </summary>
        /// <value>
        ///     The operation timeout in seconds. If not specified or less than zero, default operation timeout
        ///     of maximum value is used.
        /// </value>
        public long OperationTimeoutInSeconds { get; set; }


        /// <summary>
        ///     Gets or Sets the keep alive timeout in seconds. This settings is useful in the scenario when the client
        ///     and service are connected via load balancer that closes the connection if it is idle for some time.
        ///     If keep alive timeout is configured, the connection will be kept alive by sending ping messages at
        ///     that interval.
        /// </summary>
        /// <value>
        ///     The keep alive timeout in seconds.
        /// </value>
        public long KeepAliveTimeoutInSeconds { get; set; }


        /// <summary>
        ///     Gets or Sets the connect timeout in milliseconds. This settings specifies the maximum time allowed for the
        ///     connection
        ///     to be established.
        /// </summary>
        /// <value>
        ///     The connect timeout in Milliseconds.
        /// </value>
        /// <remarks>Default Value for ConnectTimeout Timeout is 5 seconds.</remarks>
        public long ConnectTimeoutInMilliseconds { get; set; }

        /// <summary>
        ///     Creates a V2 service remoting listener for remoting the service interface.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="serviceImplementation">
        ///     The service implementation object.
        /// </param>
        /// <returns>
        ///     A <see cref="FabricTransportServiceRemotingListener" />
        ///     as <see cref="IServiceRemotingListener" />
        ///     for the specified service implementation.
        /// </returns>
        public override IServiceRemotingListener CreateServiceRemotingListenerV2(ServiceContext serviceContext, IService serviceImplementation)
        {
            FabricTransportRemotingListenerSettings settings = FabricTransportRemotingListenerSettings.GetDefault();
            settings.MaxMessageSize = this.GetAndValidateMaxMessageSize(settings.MaxMessageSize);
            settings.OperationTimeout = this.GetAndValidateOperationTimeout(settings.OperationTimeout);
            settings.KeepAliveTimeout = this.GetKeepAliveTimeout(settings.KeepAliveTimeout);
            return new V2.FabricTransport.Runtime.FabricTransportServiceRemotingListener(
                serviceContext,
                serviceImplementation,
                settings);
        }

        /// <summary>
        ///     Creates a  V2 service remoting client factory for connecting to the service over remoted service interfaces.
        /// </summary>
        /// <param name="callbackMessageHandler">
        ///     The client implementation where the callbacks should be dispatched.
        /// </param>
        /// <returns>
        ///     A <see cref="FabricTransportServiceRemotingClientFactory" />
        ///     as <see cref="V2.Client.IServiceRemotingClientFactory" />
        ///     that can be used with <see cref="Remoting.Client.ServiceProxyFactory" /> to
        ///     generate service proxy to talk to a stateless or stateful service over remoted actor interface.
        /// </returns>
        public override IServiceRemotingClientFactory CreateServiceRemotingClientFactoryV2(
            IServiceRemotingCallbackMessageHandler callbackMessageHandler)
        {
            FabricTransportRemotingSettings settings = FabricTransportRemotingSettings.GetDefault();
            settings.MaxMessageSize = this.GetAndValidateMaxMessageSize(settings.MaxMessageSize);
            settings.OperationTimeout = this.GetAndValidateOperationTimeout(settings.OperationTimeout);
            settings.KeepAliveTimeout = this.GetKeepAliveTimeout(settings.KeepAliveTimeout);
            settings.ConnectTimeout = this.GetConnectTimeout(settings.ConnectTimeout);
            return new V2.FabricTransport.Client.FabricTransportServiceRemotingClientFactory(
                settings,
                callbackMessageHandler);
        }

        private long GetAndValidateMaxMessageSize(long maxMessageSize)
        {
            return this.MaxMessageSize > 0 ? this.MaxMessageSize : maxMessageSize;
        }

        private TimeSpan GetAndValidateOperationTimeout(TimeSpan operationTimeout)
        {
            return this.OperationTimeoutInSeconds > 0
                ? TimeSpan.FromSeconds(this.OperationTimeoutInSeconds)
                : operationTimeout;
        }

        private TimeSpan GetKeepAliveTimeout(TimeSpan keepAliveTimeout)
        {
            return this.KeepAliveTimeoutInSeconds > 0
                ? TimeSpan.FromSeconds(this.KeepAliveTimeoutInSeconds)
                : keepAliveTimeout;
        }

        private TimeSpan GetConnectTimeout(TimeSpan connectTimeout)
        {
            return this.ConnectTimeoutInMilliseconds > 0
                ? TimeSpan.FromMilliseconds(this.ConnectTimeoutInMilliseconds)
                : connectTimeout;
        }


#if !DotNetCoreClr

        /// <summary>
        ///     Creates a service remoting listener for remoting the service interface.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="serviceImplementation">
        ///     The service implementation object.
        /// </param>
        /// <returns>
        ///     A <see cref="FabricTransportServiceRemotingListener" />
        ///     as <see cref="IServiceRemotingListener" />
        ///     for the specified service implementation.
        /// </returns>
        public override IServiceRemotingListener CreateServiceRemotingListener(
            ServiceContext serviceContext,
            IService serviceImplementation)
        {
            FabricTransportRemotingListenerSettings settings = FabricTransportRemotingListenerSettings.GetDefault();
            settings.MaxMessageSize = this.GetAndValidateMaxMessageSize(settings.MaxMessageSize);
            settings.OperationTimeout = this.GetAndValidateOperationTimeout(settings.OperationTimeout);
            settings.KeepAliveTimeout = this.GetKeepAliveTimeout(settings.KeepAliveTimeout);
            return new FabricTransportServiceRemotingListener(serviceContext, serviceImplementation, settings);
        }

        /// <summary>
        ///     Creates a  V1 service remoting client factory for connecting to the service over remoted service interfaces.
        /// </summary>
        /// <param name="callbackClient">
        ///     The client implementation where the callbacks should be dispatched.
        /// </param>
        /// <returns>
        ///     A <see cref="FabricTransportServiceRemotingClientFactory" />
        ///     as <see cref="V1.Client.IServiceRemotingClientFactory" />
        ///     that can be used with <see cref="Remoting.Client.ServiceProxyFactory" /> to
        ///     generate service proxy to talk to a stateless or stateful service over remoted actor interface.
        /// </returns>
        public override V1.Client.IServiceRemotingClientFactory CreateServiceRemotingClientFactory(
            IServiceRemotingCallbackClient callbackClient)
        {
            FabricTransportRemotingSettings settings = FabricTransportRemotingSettings.GetDefault();
            settings.MaxMessageSize = this.GetAndValidateMaxMessageSize(settings.MaxMessageSize);
            settings.OperationTimeout = this.GetAndValidateOperationTimeout(settings.OperationTimeout);
            settings.KeepAliveTimeout = this.GetKeepAliveTimeout(settings.KeepAliveTimeout);
            settings.ConnectTimeout = this.GetConnectTimeout(settings.ConnectTimeout);
            return new FabricTransportServiceRemotingClientFactory(settings, callbackClient);
        }

#endif
    }
}