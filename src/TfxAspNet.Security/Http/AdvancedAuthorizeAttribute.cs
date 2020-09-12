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

#if NET45
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http.Controllers;

namespace Xyz.TForce.AspNet.Security.Http
{

  public class AdvancedAuthorizeAttribute : AuthorizeExAttribute
  {

    private string _permissionsString = string.Empty;
    private string[] _permissionsArray = EmptyArray;
    private string _rolesString = string.Empty;
    private string[] _rolesArray = EmptyArray;
    private string _usersString = string.Empty;
    private string[] _usersArray = EmptyArray;

    /// <summary>Gets or sets the permissions needed to authorize.</summary>
    /// <returns>The permission string.</returns>
    public string Permissions
    {
      get { return _permissionsString ?? string.Empty; }
      set
      {
        _permissionsString = value;
        _permissionsArray = SplitString(value);
      }
    }

    /// <summary>Gets or sets the authorized roles.</summary>
    /// <returns>The roles string.</returns>
    public string Roles
    {
      get { return _rolesString ?? string.Empty; }
      set
      {
        _rolesString = value;
        _rolesArray = SplitString(value);
      }
    }

    /// <summary>Gets or sets the authorized users.</summary>
    /// <returns>The users string.</returns>
    public string Users
    {
      get { return _usersString ?? string.Empty; }
      set
      {
        _usersString = value;
        _usersArray = SplitString(value);
      }
    }

    protected override bool IsAuthorized(HttpActionContext actionContext)
    {
      if (actionContext == null)
      {
        throw new ArgumentNullException(nameof(actionContext));
      }
      IPrincipal principal = actionContext.ControllerContext.RequestContext.Principal;
      if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
      {
        return false;
      }
      List<Claim> claims = ((ClaimsPrincipal)principal).Claims
        .ToList();
      return CheckPermission(_permissionsArray, claims)
        && CheckRole(_rolesArray, claims)
        && CheckUser(_usersArray, claims);
    }
  }
}
#endif
