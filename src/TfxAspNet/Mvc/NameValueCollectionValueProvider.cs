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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
#if NETSTANDARD2_0
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
#else
using System.Web.Mvc;
#endif

namespace Xyz.TForce.AspNet.Mvc
{

  public class NameValueCollectionValueProvider : IValueProvider
  {

    private PrefixContainer _prefixContainer;
    private Dictionary<string, ValueProviderResultPlaceholder> _values;

    /// <summary>Initializes a new instance of the <see cref="T:System.Web.Mvc.NameValueCollectionValueProvider" /> class.</summary>
    /// <param name="collection">A collection that contains the values that are used to initialize the provider.</param>
    /// <param name="culture">An object that contains information about the target culture.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="collection" /> parameter is null.</exception>
    public NameValueCollectionValueProvider(NameValueCollection collection, CultureInfo culture)
      : this(collection, null, culture)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Web.Mvc.NameValueCollectionValueProvider" /> class using the specified unvalidated collection.</summary>
    /// <param name="collection">A collection that contains the values that are used to initialize the provider.</param>
    /// <param name="unvalidatedCollection">A collection that contains the values that are used to initialize the provider. This collection will not be validated.</param>
    /// <param name="culture">An object that contains information about the target culture.</param>
    public NameValueCollectionValueProvider(NameValueCollection collection, NameValueCollection unvalidatedCollection, CultureInfo culture)
      : this(collection, unvalidatedCollection, culture, false)
    {
    }

    /// <summary>Initializes Name Value collection provider.</summary>
    /// <param name="collection">Key value collection from request.</param>
    /// <param name="unvalidatedCollection">Unvalidated key value collection from the request.</param>
    /// <param name="culture">Culture with which the values are to be used.</param>
    /// <param name="jQueryToMvcRequestNormalizationRequired">jQuery POST when sending complex Javascript objects to server does not encode in the way understandable by MVC. This flag should be set if the request should be normalized to MVC form - https://aspnetwebstack.codeplex.com/workitem/1564.</param>
    public NameValueCollectionValueProvider(NameValueCollection collection, NameValueCollection unvalidatedCollection, CultureInfo culture, bool jQueryToMvcRequestNormalizationRequired)
    {
      if (collection == null)
      {
        throw new ArgumentNullException(nameof(collection));
      }
      UnvalidatedCollection = unvalidatedCollection ?? collection;
      Collection = collection;
      Culture = culture;
      JQueryToMvcRequestNormalizationRequired = jQueryToMvcRequestNormalizationRequired;
    }

    protected CultureInfo Culture { get; private set; }

    protected bool JQueryToMvcRequestNormalizationRequired { get; private set; }

    protected NameValueCollection Collection { get; private set; }

    protected NameValueCollection UnvalidatedCollection { get; private set; }

    public Dictionary<string, ValueProviderResultPlaceholder> Values
    {
      get
      {
        if (_values == null)
        {
          _values = InitializeCollectionValues();
        }
        return _values;
      }
    }

    public PrefixContainer PrefixContainer
    {
      get
      {
        if (_prefixContainer == null)
        {
          _prefixContainer = new PrefixContainer(Values.Keys);
        }
        return _prefixContainer;
      }
    }

    /// <summary>Determines whether the collection contains the specified prefix.</summary>
    /// <returns>true if the collection contains the specified prefix; otherwise, false.</returns>
    /// <param name="prefix">The prefix to search for.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="prefix" /> parameter is null.</exception>
    public virtual bool ContainsPrefix(string prefix)
    {
      return PrefixContainer.ContainsPrefix(prefix);
    }

    /// <summary>Returns a value object using the specified key.</summary>
    /// <returns>The value object for the specified key.</returns>
    /// <param name="key">The key of the value object to retrieve.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="key" /> parameter is null.</exception>
    public virtual ValueProviderResult GetValue(string key)
    {
      return GetValue(key, false);
    }

    /// <summary>Returns a value object using the specified key and validation directive.</summary>
    /// <returns>The value object for the specified key.</returns>
    /// <param name="key">The key.</param>
    /// <param name="skipValidation">true if validation should be skipped; otherwise, false.</param>
    public virtual ValueProviderResult GetValue(string key, bool skipValidation)
    {
      if (key == null)
      {
        throw new ArgumentNullException(nameof(key));
      }
      ValueProviderResultPlaceholder resultPlaceholder;
      _ = Values.TryGetValue(key, out resultPlaceholder);
#pragma warning disable IDE0046 // Convert to conditional expression
      if (resultPlaceholder == null)
#pragma warning restore IDE0046 // Convert to conditional expression
      {
        return default(ValueProviderResult);
      }
      return skipValidation ? resultPlaceholder.UnvalidatedResult : resultPlaceholder.ValidatedResult;
    }

    /// <summary>Gets the keys using the specified prefix.</summary>
    /// <returns>They keys.</returns>
    /// <param name="prefix">The prefix.</param>
    public virtual IDictionary<string, string> GetKeysFromPrefix(string prefix)
    {
      return PrefixContainer.GetKeysFromPrefix(prefix);
    }

    internal virtual Dictionary<string, ValueProviderResultPlaceholder> InitializeCollectionValues()
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
        }
      }
      return dictionary;
    }

    protected static string NormalizeJQueryToMvc(string key)
    {
      if (key == null)
      {
        return string.Empty;
      }
      StringBuilder stringBuilder = null;
      int startIndex1 = 0;
      do
      {
        int startIndex2 = key.IndexOf('[', startIndex1);
        if (startIndex2 < 0)
        {
          if (startIndex1 == 0)
          {
            return key;
          }
          stringBuilder = stringBuilder ?? new StringBuilder();
          _ = stringBuilder.Append(key, startIndex1, key.Length - startIndex1);
          break;
        }
        stringBuilder = stringBuilder ?? new StringBuilder();
        _ = stringBuilder.Append(key, startIndex1, startIndex2 - startIndex1);
        int num = key.IndexOf(']', startIndex2);
        if (num == -1)
        {
          throw new ArgumentException("Key");
        }
        if (num != startIndex2 + 1)
        {
          if (char.IsDigit(key[startIndex2 + 1]))
          {
            _ = stringBuilder.Append(key, startIndex2, num - startIndex2 + 1);
          }
          else
          {
            _ = stringBuilder.Append('.');
            _ = stringBuilder.Append(key, startIndex2 + 1, num - startIndex2 - 1);
          }
        }
        startIndex1 = num + 1;
      }
      while (startIndex1 < key.Length);
      return stringBuilder.ToString();
    }

    public class ValueProviderResultPlaceholder
    {

#if NET45
      private ValueProviderResult _validatedResult;
      private ValueProviderResult _unvalidatedResult;
#endif
      private readonly string _key;
      private readonly NameValueCollection _validatedCollection;
      private readonly NameValueCollection _unvalidatedCollection;
      private readonly CultureInfo _culture;

      public ValueProviderResultPlaceholder(string key, NameValueCollection validatedCollection, NameValueCollection unvalidatedCollection, CultureInfo culture)
      {
        _key = key;
        _validatedCollection = validatedCollection;
        _unvalidatedCollection = unvalidatedCollection;
        _culture = culture;
      }

#if NETSTANDARD2_0
      public ValueProviderResult ValidatedResult
      {
        get
        {
          ValueProviderResult validatedResult = GetResultFromCollection(_key, _validatedCollection, _culture);
          return validatedResult;
        }
      }

      public ValueProviderResult UnvalidatedResult
      {
        get
        {
          ValueProviderResult unvalidatedResult = GetResultFromCollection(_key, _unvalidatedCollection, _culture);
          return unvalidatedResult;
        }
      }
#else
      public ValueProviderResult ValidatedResult
      {
        get
        {
          if (_validatedResult == null)
          {
            _validatedResult = GetResultFromCollection(_key, _validatedCollection, _culture);
          }
          return _validatedResult;
        }
      }

      public ValueProviderResult UnvalidatedResult
      {
        get
        {
          if (_unvalidatedResult == null)
          {
            _unvalidatedResult = GetResultFromCollection(_key, _unvalidatedCollection, _culture);
          }
          return _unvalidatedResult;
        }
      }
#endif

      private static ValueProviderResult GetResultFromCollection(string key, NameValueCollection collection, CultureInfo culture)
      {
#if NETSTANDARD2_0
        return new ValueProviderResult(collection.GetValues(key), culture);
#else
        return new ValueProviderResult(collection.GetValues(key), collection[key], culture);
#endif
      }
    }
  }
}
