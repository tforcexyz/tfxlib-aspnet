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
#if NETSTANDARD2_0
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace Xyz.TForce.AspNet.Mvc
{

#if NETSTANDARD2_0
  public class AdvancedFormValueProviderFactory : IValueProviderFactory
  {

    public async Task CreateValueProviderAsync(ValueProviderFactoryContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException(nameof(context));
      }

      if (context.ActionContext.HttpContext.Request.HasFormContentType)
      {
        IFormCollection values = await context.ActionContext.HttpContext.Request.ReadFormAsync();
        context.ValueProviders.Add(new AdvancedFormValueProvider(values, CultureInfo.CurrentCulture));
      }
    }
  }
#else
  public class AdvancedFormValueProviderFactory : ValueProviderFactory
  {

    /// <summary>Returns a value-provider object for the specified controller context.</summary>
    /// <returns>A query-string value-provider object.</returns>
    /// <param name="controllerContext">An object that encapsulates information about the current HTTP request.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="controllerContext" /> parameter is null.</exception>
    public override IValueProvider GetValueProvider(ControllerContext controllerContext)
    {
      if (controllerContext == null)
      {
        throw new ArgumentNullException(nameof(controllerContext));
      }
      HttpContextBase context = controllerContext.HttpContext;
      return new AdvancedFormValueProvider(context);
    }
  }
#endif
}
