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
  public class AdvancedQueryStringValueProvider : NameValueCollectionValueProvider
  {

    /// <summary>
    /// Creates a value provider for <see cref="T:Microsoft.AspNetCore.Http.IQueryCollection" />.
    /// </summary>
    /// <param name="values">The key value pairs to wrap.</param>
    /// <param name="culture">The culture to return with ValueProviderResult instances.</param>
    public AdvancedQueryStringValueProvider(IQueryCollection values, CultureInfo culture)
      : base(Initialize(values), culture)
    {
    }

    /// <summary>
    /// Creates a value provider for <see cref="T:Microsoft.AspNetCore.Http.IQueryCollection" />.
    /// </summary>
    /// <param name="values">The key value pairs to wrap.</param>
    /// <param name="unvalidatedvalues">The key value pairs to wrap.</param>
    /// <param name="culture">The culture to return with ValueProviderResult instances.</param>
    public AdvancedQueryStringValueProvider(IQueryCollection values, IQueryCollection unvalidatedvalues, CultureInfo culture)
        : base(Initialize(values), Initialize(unvalidatedvalues), culture)
    {
    }

    /// <inheritdoc />
    public override bool ContainsPrefix(string prefix)
    {
      return PrefixContainer.ContainsPrefix(prefix);
    }

    private static NameValueCollection Initialize(IQueryCollection values)
    {
      NameValueCollection collection = new NameValueCollection();
      foreach (KeyValuePair<string, StringValues> keyValuePair in values)
      {
        if (keyValuePair.Key.Contains("-"))
        {
          collection.Add(keyValuePair.Key.Replace("-", ""), keyValuePair.Value);
        }
        collection.Add(keyValuePair.Key, keyValuePair.Value);
      }
      return collection;
    }
  }
#else
  public class AdvancedQueryStringValueProvider : NameValueCollectionValueProvider
  {

    public AdvancedQueryStringValueProvider(HttpContextBase context)
        : base(context.Request.QueryString, context.Request.QueryString, CultureInfo.InvariantCulture)
    {
    }

    public AdvancedQueryStringValueProvider(HttpContextBase context, CultureInfo culture)
        : base(context.Request.QueryString, context.Request.QueryString, culture)
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
