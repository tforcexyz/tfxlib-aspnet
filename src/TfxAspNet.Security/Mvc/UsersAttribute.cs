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

  public class UsersAttribute : AuthorizeExAttribute
  {

    private string _usersString = string.Empty;
    private string[] _usersArray = EmptyArray;

    public UsersAttribute()
    {
    }

    public UsersAttribute(params string[] users)
    {
      ISet<string> userSet = new HashSet<string>();
      foreach (string user in users)
      {
        string[] splitedUsers = SplitString(user);
        foreach (string splitedUser in splitedUsers)
        {
          _ = userSet.Add(splitedUser);
        }
      }
      _usersArray = userSet.ToArray();
      _usersString = string.Join(AuthorizationConstants.ListDelimiter, _usersArray);
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
        return $"{PolicyTypes.User}{AuthorizationConstants.TypeDelimiter}{_usersString}";
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
      return CheckUser(_usersArray, claims);
    }
#endif
  }
}
