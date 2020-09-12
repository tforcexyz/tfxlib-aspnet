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

using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
#if NETSTANDARD2_0
#else
using System;
using System.Security.Claims;
using System.Web;
#endif

namespace Xyz.TForce.AspNet.Security.Mvc
{

  public class PermissionsAttribute : AuthorizeExAttribute
  {

    private string _permissionsString = string.Empty;
    private string[] _permissionsArray = EmptyArray;

    public PermissionsAttribute()
    {
    }

    public PermissionsAttribute(params string[] permissions)
    {
      ISet<string> permissionSet = new HashSet<string>();
      foreach (string permission in permissions)
      {
        string[] splitedPermissions = SplitString(permission);
        foreach (string splitedPermission in splitedPermissions)
        {
          _ = permissionSet.Add(splitedPermission);
        }
      }
      _permissionsArray = permissionSet.ToArray();
      _permissionsString = string.Join(AuthorizationConstants.ListDelimiter, _permissionsArray);
    }

    /// <summary>Gets or sets the permissions that users need to access the controller or action method.</summary>
    /// <returns>The permissions that users need to access the controller or action method.</returns>
    public string Permissions
    {
      get
      {
        return _permissionsString ?? string.Empty;
      }
      set
      {
        _permissionsString = value;
        _permissionsArray = SplitString(value);
      }
    }

#if NETSTANDARD2_0
    public new string Policy
    {
      get
      {
        return $"{PolicyTypes.Permission}{AuthorizationConstants.TypeDelimiter}{_permissionsString}";
      }
    }
#else
    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
      if (httpContext == null)
      {
        throw new ArgumentNullException(nameof(httpContext));
      }
      IPrincipal principal = httpContext.User;
      if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
      {
        return false;
      }
      List<Claim> claims = ((ClaimsPrincipal)principal).Claims
        .ToList();
      return CheckPermission(_permissionsArray, claims);
    }
#endif
  }
}
