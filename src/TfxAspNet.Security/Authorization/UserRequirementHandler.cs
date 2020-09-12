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
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Xyz.TForce.AspNet.Security.Authorization
{

  public class UserRequirementHandler : AuthorizationHandler<UserRequirement>
  {

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRequirement requirement)
    {
      if (requirement.Users.Length == 0)
      {
        context.Succeed(requirement);
        return Task.FromResult(0);
      }
      IList<string> userRoles = context.User.Claims
        .Where(x => { return x.Type == ClaimTypes.NameIdentifier; })
        .Select(x => { return x.Value; })
        .ToList();
      bool matchAny = requirement.Users.Any(x => { return userRoles.Contains(x); });
      if (matchAny)
      {
        context.Succeed(requirement);
      }
      return Task.FromResult(0);
    }
  }
}
#endif
