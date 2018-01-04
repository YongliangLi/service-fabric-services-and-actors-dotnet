﻿// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.Builder
{
    using Microsoft.ServiceFabric.Services.Remoting.Builder;

    internal class ActorCodeBuilderNames
        : CodeBuilderNames
    {
        public ActorCodeBuilderNames()
            : base("actor")
        {
        }

        public override string GetDataContractNamespace()
        {
            return Constants.Namespace;
        }
    }
}