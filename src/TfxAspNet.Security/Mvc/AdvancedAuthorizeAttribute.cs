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

  public class AdvancedAuthorizeAttribute : AuthorizeExAttribute
  {

    private string _permissionsString = string.Empty;
    private string[] _permissionsArray = EmptyArray;
    private string _rolesString = string.Empty;
    private string[] _rolesArray = EmptyArray;
    private string _usersString = string.Empty;
    private string[] _usersArray = EmptyArray;

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

    /// <summary>Gets or sets the user roles that are authorized to access the controller or action method.</summary>
    /// <returns>The user roles that are authorized to access the controller or action method.</returns>
    public new string Roles
    {
      get
      {
        return _rolesString ?? string.Empty;
      }
      set
      {
        _rolesString = value;
        _rolesArray = SplitString(value);
      }
    }

    /// <summary>Gets or sets the user roles that are authorized to access the controller or action method.</summary>
    /// <returns>The user roles that are authorized to access the controller or action method.</returns>
    public string Users
    {
      get
      {
        return _usersString ?? string.Empty;
      }
      set
      {
        _usersString = value;
        _usersArray = SplitString(value);
      }
    }

#if NETSTANDARD2_0
    public new string Policy
    {
      get
      {
        ICollection<string> policyParts = new List<string>();
        policyParts.Add(PolicyTypes.Advanced);
        if (_permissionsArray.Any())
        {
          string permission = $"{PolicySubTypes.Permission}={_permissionsString}";
          policyParts.Add(permission);
        }
        if (_rolesArray.Any())
        {
          string role = $"{PolicySubTypes.Role}={_rolesString}";
          policyParts.Add(role);
        }
        if (_usersArray.Any())
        {
          string user = $"{PolicySubTypes.User}={_usersString}";
          policyParts.Add(user);
        }
        string policyString = string.Join(AuthorizationConstants.TypeDelimiter, policyParts);
        return policyString;
      }
    }
#else
    /// <summary>When overridden, provides an entry point for custom authorization checks.</summary>
    /// <returns>true if the user is authorized; otherwise, false.</returns>
    /// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="httpContext" /> parameter is null.</exception>
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
      return CheckPermission(_permissionsArray, claims)
        && CheckRole(_rolesArray, claims)
        && CheckUser(_usersArray, claims);
    }
#endif
  }
}
