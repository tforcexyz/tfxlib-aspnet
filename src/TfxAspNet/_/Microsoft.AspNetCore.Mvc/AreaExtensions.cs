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
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc
{

  // Source: https://stackoverflow.com/questions/233711/add-property-to-anonymous-type-after-creation
  public static class AreaExtensions
  {

    public static IHtmlContent ActionLink(this IHtmlHelper helper, string linkText, string action, string controller, string area)
    {
      RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
      if (area != null)
      {
        routeValueDictionary["area"] = area;
      }
      return helper.ActionLink(linkText, action, controller, routeValueDictionary);
    }

    public static IHtmlContent ActionLink(this IHtmlHelper helper, string linkText, string action, string controller, string area, object routeValues)
    {
      RouteValueDictionary routeValueDictionary = new RouteValueDictionary(routeValues);
      if (area != null)
      {
        routeValueDictionary["area"] = area;
      }
      return helper.ActionLink(linkText, action, controller, routeValueDictionary);
    }

    public static IHtmlContent ActionLink(this IHtmlHelper helper, string linkText, string action, string controller, string area, object routeValues, object htmlAttributes)
    {
      RouteValueDictionary routeValueDictionary = new RouteValueDictionary(routeValues);
      if (area != null)
      {
        routeValueDictionary["area"] = area;
      }
      return helper.ActionLink(linkText, action, controller, routeValueDictionary, htmlAttributes);
    }

    public static string Action(this IUrlHelper helper, string action, string controller, string area, object routeValues = null)
    {
      RouteValueDictionary routeValueDictionary = routeValues == null ?
        new RouteValueDictionary() : new RouteValueDictionary(routeValues);
      if (area != null)
      {
        routeValueDictionary["area"] = area;
      }
      return helper.Action(action, controller, routeValueDictionary);
    }
  }
}
#endif
