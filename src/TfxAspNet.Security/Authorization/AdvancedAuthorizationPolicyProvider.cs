// Copyright 2017 T-Force Xyz
// Please refer to LICENSE & CONTRIB files in the project root for license information.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if NETSTANDARD2_0
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Xyz.TForce.AspNet.Security.Authorization
{

  public class AdvancedAuthorizationPolicyProvider : IAuthorizationPolicyProvider
  {

    public DefaultAuthorizationPolicyProvider DefaultProvider { get; private set; }

    public INamedPolicyProvider[] NamedProviders { get; private set; }

    public AdvancedAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
      DefaultProvider = new DefaultAuthorizationPolicyProvider(options);
      NamedProviders = new INamedPolicyProvider[]
      {
        new AdvancedPolicyProvider(),
        new PermissionPolicyProvider(),
        new RolePolicyProvider(),
        new UserPolicyProvider()
      };
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
      return DefaultProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
      foreach (INamedPolicyProvider provider in NamedProviders)
      {
        if (provider.IsSupported(policyName))
        {
          AuthorizationPolicy policy = provider.GetPolicy(policyName);
          return Task.FromResult(policy);
        }
      }
      return DefaultProvider.GetPolicyAsync(policyName);
    }
  }
}
#endif
