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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Rendering
{

  // Source: https://stackoverflow.com/questions/26916664/html-action-in-asp-net-core
  public static class HtmlHelperExtensions
  {

    public static IHtmlContent Action(this IHtmlHelper helper, string action, object parameters = null)
    {
      string controller = (string)helper.ViewContext.RouteData.Values["controller"];
      return Action(helper, action, controller, parameters);
    }

    public static IHtmlContent Action(this IHtmlHelper helper, string action, string controller, object parameters = null)
    {
      string area = (string)helper.ViewContext.RouteData.Values["area"];
      return Action(helper, action, controller, area, parameters);
    }

    public static IHtmlContent Action(this IHtmlHelper helper, string action, string controller, string area, object parameters = null)
    {
      if (action == null)
      {
        throw new ArgumentNullException("action");
      }

      if (controller == null)
      {
        throw new ArgumentNullException("controller");
      }

      Task<IHtmlContent> task = RenderActionAsync(helper, action, controller, area, parameters);
      return task.Result;
    }

    public static string CreateFieldId<TModel, TValue>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
    {
      string nameExpression = ExpressionHelper.GetExpressionText(expression);
      string fieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(nameExpression);
      string fieldId = helper.CreateSanitizedId(fieldName);
      return fieldId;
    }

    public static string CreateFieldName<TModel, TValue>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
    {
      string nameExpression = ExpressionHelper.GetExpressionText(expression);
      string fieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(nameExpression);
      return fieldName;
    }

    public static string CreateSanitizedId<TModel>(this IHtmlHelper<TModel> helper, string fieldName)
    {
      return TagBuilder.CreateSanitizedId(fieldName, helper.IdAttributeDotReplacement);
    }

    internal static HtmlString CreateMvcString<TModel>(this IHtmlHelper<TModel> helper, StringBuilder builder)
    {
      _ = helper;
      return new HtmlString(builder.ToString());
    }

    internal static string CreateAbsoluteUrl<TModel>(this IHtmlHelper<TModel> helper, string url)
    {
      return new UrlHelper(helper.ViewContext).Content(url);
    }

    private static async Task<IHtmlContent> RenderActionAsync(this IHtmlHelper helper, string action, string controller, string area, object parameters = null)
    {
      // fetching required services for invocation
      IServiceProvider serviceProvider = helper.ViewContext.HttpContext.RequestServices;
      IActionContextAccessor actionContextAccessor = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();
      IHttpContextAccessor httpContextAccessor = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();
      IActionSelector actionSelector = serviceProvider.GetRequiredService<IActionSelector>();

      // creating new action invocation context
      RouteData routeData = new RouteData();
      foreach (IRouter router in helper.ViewContext.RouteData.Routers)
      {
        _ = routeData.PushState(router, null, null);
      }
#pragma warning disable IDE0037 // Use inferred member name
      _ = routeData.PushState(null, new RouteValueDictionary(new { controller = controller, action = action, area = area }), null);
#pragma warning restore IDE0037 // Use inferred member name
      _ = routeData.PushState(null, new RouteValueDictionary(parameters ?? new { }), null);

      //get the actiondescriptor
      RouteContext routeContext = new RouteContext(helper.ViewContext.HttpContext) { RouteData = routeData };
      IReadOnlyList<ActionDescriptor> candidates = actionSelector.SelectCandidates(routeContext);
      ActionDescriptor actionDescriptor = actionSelector.SelectBestCandidate(routeContext, candidates);

      ActionContext originalActionContext = actionContextAccessor.ActionContext;
      HttpContext originalhttpContext = httpContextAccessor.HttpContext;
      try
      {
        HttpContext newHttpContext = serviceProvider.GetRequiredService<IHttpContextFactory>().Create(helper.ViewContext.HttpContext.Features);
        if (newHttpContext.Items.ContainsKey(typeof(IUrlHelper)))
        {
          _ = newHttpContext.Items.Remove(typeof(IUrlHelper));
        }
        newHttpContext.Response.Body = new MemoryStream();
        ActionContext actionContext = new ActionContext(newHttpContext, routeData, actionDescriptor);
        actionContextAccessor.ActionContext = actionContext;
        IActionInvoker invoker = serviceProvider.GetRequiredService<IActionInvokerFactory>().CreateInvoker(actionContext);
        await invoker.InvokeAsync();
        newHttpContext.Response.Body.Position = 0;
        using (StreamReader reader = new StreamReader(newHttpContext.Response.Body))
        {
          return new HtmlString(reader.ReadToEnd());
        }
      }
      catch (Exception ex)
      {
        return new HtmlString(ex.Message);
      }
      finally
      {
        actionContextAccessor.ActionContext = originalActionContext;
        httpContextAccessor.HttpContext = originalhttpContext;
        if (helper.ViewContext.HttpContext.Items.ContainsKey(typeof(IUrlHelper)))
        {
          _ = helper.ViewContext.HttpContext.Items.Remove(typeof(IUrlHelper));
        }
      }
    }
  }
}
#endif
