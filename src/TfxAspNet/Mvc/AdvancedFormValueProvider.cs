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
using System.Globalization;
#if NETSTANDARD2_0
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
#else
using System;
using System.Web;
#endif

namespace Xyz.TForce.AspNet.Mvc
{

#if NETSTANDARD2_0
  public class AdvancedFormValueProvider : NameValueCollectionValueProvider
  {

    /// <summary>
    /// Creates a value provider for <see cref="T:Microsoft.AspNetCore.Http.IFormCollection" />.
    /// </summary>
    /// <param name="values">The key value pairs to wrap.</param>
    /// <param name="culture">The culture to return with ValueProviderResult instances.</param>
    public AdvancedFormValueProvider(IFormCollection values, CultureInfo culture)
      : base(Initialize(values), culture)
    {
    }

    /// <summary>
    /// Creates a value provider for <see cref="T:Microsoft.AspNetCore.Http.IFormCollection" />.
    /// </summary>
    /// <param name="values">The key value pairs to wrap.</param>
    /// <param name="unvalidatedValues">The key value pairs to wrap.</param>
    /// <param name="culture">The culture to return with ValueProviderResult instances.</param>
    public AdvancedFormValueProvider(IFormCollection values, IFormCollection unvalidatedValues, CultureInfo culture)
        : base(Initialize(values), Initialize(unvalidatedValues), culture)
    {
    }

    private static NameValueCollection Initialize(IFormCollection values)
    {
      NameValueCollection collection = new NameValueCollection();
      foreach (KeyValuePair<string, StringValues> x in values)
      {
        if (x.Key.Contains("-"))
        {
          collection.Add(x.Key.Replace("-", ""), x.Value);
        }
        collection.Add(x.Key, x.Value);
      }
      return collection;
    }
  }
#else
  public class AdvancedFormValueProvider : NameValueCollectionValueProvider
  {

    public AdvancedFormValueProvider(HttpContext context)
        : base(context.Request.Form, context.Request.Form, CultureInfo.InvariantCulture)
    {
    }

    public AdvancedFormValueProvider(HttpContext context, CultureInfo culture)
        : base(context.Request.Form, context.Request.Form, culture)
    {
    }
    public AdvancedFormValueProvider(HttpContextBase context)
        : base(context.Request.Form, context.Request.Form, CultureInfo.InvariantCulture)
    {
    }

    public AdvancedFormValueProvider(HttpContextBase context, CultureInfo culture)
        : base(context.Request.Form, context.Request.Form, culture)
    {
    }
    internal override Dictionary<string, ValueProviderResultPlaceholder> InitializeCollectionValues()
    {
      Dictionary<string, ValueProviderResultPlaceholder> dictionary = new Dictionary<string, ValueProviderResultPlaceholder>(StringComparer.OrdinalIgnoreCase);
      foreach (string unvalidated in UnvalidatedCollection)
      {
        if (unvalidated != null)
        {
          string index = unvalidated;
          if (JQueryToMvcRequestNormalizationRequired)
          {
            index = NormalizeJQueryToMvc(unvalidated);
          }
          dictionary[index] = new ValueProviderResultPlaceholder(unvalidated, Collection, UnvalidatedCollection, Culture);
          if (index.Contains("-"))
          {
            string newIndex = index.Replace("-", string.Empty);
            dictionary[newIndex] = new ValueProviderResultPlaceholder(unvalidated, Collection, UnvalidatedCollection, Culture);
          }
        }
      }
      return dictionary;
    }
  }
#endif
}
