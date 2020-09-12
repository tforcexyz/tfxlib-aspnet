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
using Microsoft.AspNetCore.Authorization;

namespace Xyz.TForce.AspNet.Security.Authorization
{

  internal class AdvancedPolicyProvider : INamedPolicyProvider
  {

    public bool IsSupported(string policyName)
    {
      return policyName.StartsWith(PolicyTypes.Advanced);
    }

    public AuthorizationPolicy GetPolicy(string policyName)
    {
      string[] policyParts = policyName.Split(new[] { AuthorizationConstants.TypeDelimiter }, System.StringSplitOptions.RemoveEmptyEntries);
      AuthorizationPolicyBuilder policyBuilder = new AuthorizationPolicyBuilder();
      for (int i = 1; i < policyParts.Length; i++)
      {
        string policyPart = policyParts[i];
        int equalMarkIndex = policyPart.IndexOf('=');
        if (equalMarkIndex < 0)
        {
          continue;
        }
        string policySubType = policyPart.Substring(0, equalMarkIndex);
        string policyValue = policyPart.Substring(equalMarkIndex + 1);
        if (policySubType == PolicySubTypes.Permission)
        {
          _ = policyBuilder.AddRequirements(new PermissionRequirement(policyValue));
        }
        else if (policySubType == PolicySubTypes.Role)
        {
          _ = policyBuilder.AddRequirements(new RoleRequirement(policyValue));
        }
        else if (policySubType == PolicySubTypes.User)
        {
          _ = policyBuilder.AddRequirements(new UserRequirement(policyValue));
        }
      }
      return policyBuilder.Build();
    }
  }
}
#endif
