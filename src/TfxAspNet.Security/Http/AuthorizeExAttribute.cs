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
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Xyz.TForce.AspNet.Security.Http
{

  public abstract class AuthorizeExAttribute : AuthorizationFilterAttribute
  {

    protected static readonly string[] EmptyArray = new string[0];
    private readonly object _typeId = new object();

    /// <summary>Gets a unique identifier for this attribute.</summary>
    /// <returns>A unique identifier for this attribute.</returns>
    public override object TypeId
    {
      get
      {
        return _typeId;
      }
    }

    /// <summary>Indicates whether the specified control is authorized.</summary>
    /// <returns>true if the control is authorized; otherwise, false.</returns>
    /// <param name="actionContext">The context.</param>
    protected abstract bool IsAuthorized(HttpActionContext actionContext);

    public override void OnAuthorization(HttpActionContext actionContext)
    {
      if (actionContext == null)
      {
        throw new ArgumentNullException(nameof(actionContext));
      }
      if (SkipAuthorization(actionContext) || IsAuthorized(actionContext))
      {
        return;
      }
      HandleUnauthorizedRequest(actionContext);
    }

    /// <summary>Processes requests that fail authorization.</summary>
    /// <param name="actionContext">The context.</param>
    protected virtual void HandleUnauthorizedRequest(HttpActionContext actionContext)
    {
      if (actionContext == null)
      {
        throw new ArgumentNullException(nameof(actionContext));
      }
      if (actionContext.RequestContext.Principal.Identity.IsAuthenticated)
      {
        // 403 we know who you are, but you haven't been granted access
        actionContext.Response.StatusCode = System.Net.HttpStatusCode.Forbidden;
      }
      else
      {
        // 401 who are you? go login and then try again
        actionContext.Response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
      }
    }

    private static bool SkipAuthorization(HttpActionContext actionContext)
    {
      if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
      {
        return true;
      }
      bool isControllerHasAttribute = actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
      return isControllerHasAttribute;
    }

    internal static string[] SplitString(string original)
    {
      if (string.IsNullOrWhiteSpace(original))
      {
        return EmptyArray;
      }
      string[] splitted = original.Split(new[] { AuthorizationConstants.ListDelimiter }, StringSplitOptions.RemoveEmptyEntries)
        .ToArray();
      return splitted;
    }

    protected virtual bool CheckPermission(string[] array, List<Claim> claims)
    {
      if (array.Length <= 0)
      {
        return true;
      }
      foreach (string permission in array)
      {
        if (!claims.Any(x => { return x.Type == AdditionalClaimTypes.Permission && x.Value == permission; }))
        {
          return false;
        }
      }
      return true;
    }

    protected virtual bool CheckRole(string[] array, List<Claim> claims)
    {
      if (array.Length <= 0)
      {
        return true;
      }
      foreach (string role in array)
      {
        if (claims.Any(x => { return x.Type == ClaimTypes.Role && x.Value == role; }))
        {
          return true;
        }
      }
      return false;
    }

    protected virtual bool CheckUser(string[] array, List<Claim> claims)
    {
      if (array.Length <= 0)
      {
        return true;
      }
      foreach (string user in array)
      {
        if (claims.Any(x => { return x.Type == ClaimTypes.NameIdentifier && x.Value == user; }))
        {
          return true;
        }
      }
      return false;
    }
  }
}
#endif
