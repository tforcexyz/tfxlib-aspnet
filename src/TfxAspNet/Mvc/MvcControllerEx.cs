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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
#else
using System.Web.Mvc;
using System.Web.Routing;
#endif

namespace Xyz.TForce.AspNet.Mvc
{

  public abstract class MvcControllerEx : Controller
  {

#if NETSTANDARD2_0
    public RedirectToActionResult RedirectToAction(string actionName, string controllerName, string area, object routeValues = null)
#else
    public RedirectToRouteResult RedirectToAction(string actionName, string controllerName, string area, object routeValues = null)
#endif
    {
      RouteValueDictionary routeValueDictionary = routeValues == null ?
        new RouteValueDictionary() : new RouteValueDictionary(routeValues);
      if (area != null)
      {
        routeValueDictionary["area"] = area;
      }
      return base.RedirectToAction(actionName, controllerName, routeValueDictionary);
    }

#if NETSTANDARD2_0
    public RedirectToActionResult RedirectToActionPermanent(string actionName, string controllerName, string area, object routeValues = null)
#else
    public RedirectToRouteResult RedirectToActionPermanent(string actionName, string controllerName, string area, object routeValues = null)
#endif
    {
      RouteValueDictionary routeValueDictionary = routeValues == null ?
        new RouteValueDictionary() : new RouteValueDictionary(routeValues);
      if (area != null)
      {
        routeValueDictionary["area"] = area;
      }
      return base.RedirectToActionPermanent(actionName, controllerName, routeValueDictionary);
    }
  }
}
