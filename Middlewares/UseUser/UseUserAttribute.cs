using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HotChocolate.Types.Descriptors;

namespace graphql_playground.Middlewares.UseUser
{
    public class UseUserAttribute : ObjectFieldDescriptorAttribute
    {
        public UseUserAttribute([CallerLineNumber] int order = 0)
        {
            Order = order;
        }

        protected override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
        {
            descriptor.Use<UserMiddleware>();
        }
    }
}