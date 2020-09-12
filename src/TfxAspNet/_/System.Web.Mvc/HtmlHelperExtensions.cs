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
using System.Text;
using System.Linq.Expressions;

namespace System.Web.Mvc
{

  public static class HtmlHelperExtensions
  {

    public static string CreateFieldId<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
    {
      string nameExpression = ExpressionHelper.GetExpressionText(expression);
      string fieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(nameExpression);
      string fieldId = helper.CreateSanitizedId(fieldName);
      return fieldId;
    }

    public static string CreateFieldName<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
    {
      string nameExpression = ExpressionHelper.GetExpressionText(expression);
      string fieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(nameExpression);
      return fieldName;
    }

    public static string CreateSanitizedId<TModel>(this HtmlHelper<TModel> helper, string fieldName)
    {
      return TagBuilder.CreateSanitizedId(fieldName, HtmlHelper.IdAttributeDotReplacement);
    }

    internal static MvcHtmlString CreateMvcString<TModel>(this HtmlHelper<TModel> helper, StringBuilder builder)
    {
      _ = helper;
      return MvcHtmlString.Create(builder.ToString());
    }

    internal static string CreateAbsoluteUrl<TModel>(this HtmlHelper<TModel> helper, string url)
    {
      return UrlHelper.GenerateContentUrl(url, helper.ViewContext.HttpContext);
    }
  }
}
#endif
