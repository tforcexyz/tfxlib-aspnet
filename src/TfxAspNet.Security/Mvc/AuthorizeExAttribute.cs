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

using System;
using System.Linq;
#if NETSTANDARD2_0
using Microsoft.AspNetCore.Authorization;
#else
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using AuthorizationContext = System.Web.Mvc.AuthorizationContext;
#endif

namespace Xyz.TForce.AspNet.Security.Mvc
{
#if NETSTANDARD2_0
  public abstract class AuthorizeExAttribute : AuthorizeAttribute
  {
    protected static readonly string[] EmptyArray = new string[0];

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
  }
#else
  public abstract class AuthorizeExAttribute : FilterAttribute, IAuthorizationFilter
  {

    protected static readonly string[] EmptyArray = new string[0];
    private readonly object _typeId = new object();

#if NETSTANDARD2_0
#else
    public string Roles { get; set; }
#endif

    /// <summary>Gets the unique identifier for this attribute.</summary>
    /// <returns>The unique identifier for this attribute.</returns>
    public override object TypeId
    {
      get
      {
        return _typeId;
      }
    }

    /// <summary>When overridden, provides an entry point for custom authorization checks.</summary>
    /// <returns>true if the user is authorized; otherwise, false.</returns>
    /// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="httpContext" /> parameter is null.</exception>
    protected abstract bool AuthorizeCore(HttpContextBase httpContext);

    private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
    {
      validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
    }

    public virtual void OnAuthorization(AuthorizationContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException(nameof(context));
      }
      if (OutputCacheAttribute.IsChildActionCacheActive(context))
      {
        throw new InvalidOperationException("AuthorizeAttribute_CannotUseWithinChildActionCache");
      }
      if (context.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) ||
          context.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
      {
        return;
      }
      if (AuthorizeCore(context.HttpContext))
      {
        HttpCachePolicyBase cache = context.HttpContext.Response.Cache;
        cache.SetProxyMaxAge(new TimeSpan(0L));
        cache.AddValidationCallback(CacheValidateHandler, null);
      }
      else
      {
        HandleUnauthorizedRequest(context);
      }
    }

    /// <summary>Processes HTTP requests that fail authorization.</summary>
    /// <param name="context">Encapsulates the information for using <see cref="T:System.Web.Mvc.AuthorizeExAttribute" />. The <paramref name="context" /> object contains the controller, HTTP context, request context, action result, and route data.</param>
    protected virtual void HandleUnauthorizedRequest(AuthorizationContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException(nameof(context));
      }
      if (context.HttpContext.User.Identity.IsAuthenticated)
      {
        // 403 we know who you are, but you haven't been granted access
        context.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
      }
      else
      {
        // 401 who are you? go login and then try again
        context.Result = new HttpUnauthorizedResult();
      }
    }
    /// <summary>Called when the caching module requests authorization.</summary>
    /// <returns>A reference to the validation status.</returns>
    /// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="httpContext" /> parameter is null.</exception>
    protected virtual HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
    {
      if (httpContext == null)
      {
        throw new ArgumentNullException(nameof(httpContext));
      }
      return AuthorizeCore(httpContext) ? HttpValidationStatus.Valid : HttpValidationStatus.IgnoreThisRequest;
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
#endif
}
