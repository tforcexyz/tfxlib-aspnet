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
      _usersString = string.Join(AuthorizationConstants.ListDelimiter, userSet);
    }

    /// <summary>Gets or sets the authorized users.</summary>
    /// <returns>The users string.</returns>
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

    /// <summary>Indicates whether the specified control is authorized.</summary>
    /// <returns>true if the control is authorized; otherwise, false.</returns>
    /// <param name="actionContext">The context.</param>
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
      return CheckUser(_usersArray, claims);
    }
  }
}
#endif
