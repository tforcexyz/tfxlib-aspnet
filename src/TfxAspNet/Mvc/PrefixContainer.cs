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
using System.Linq;

namespace Xyz.TForce.AspNet.Mvc
{

  public class PrefixContainer
  {

    private readonly ICollection<string> _originalValues;
    private readonly string[] _sortedValues;

    internal PrefixContainer(ICollection<string> values)
    {
      if (values == null)
      {
        throw new ArgumentNullException(nameof(values));
      }
      _originalValues = values;
      _sortedValues = _originalValues.ToArray();
      Array.Sort(_sortedValues, StringComparer.OrdinalIgnoreCase);
    }

    internal bool ContainsPrefix(string prefix)
    {
      if (prefix == null)
      {
        throw new ArgumentNullException(nameof(prefix));
      }
      if (prefix.Length == 0)
      {
        return _sortedValues.Length > 0;
      }
      PrefixComparer prefixComparer = new PrefixComparer(prefix);
      bool flag = Array.BinarySearch(_sortedValues, prefix, prefixComparer) > -1;
      if (!flag)
      {
        flag = Array.BinarySearch(_sortedValues, prefix + "[", prefixComparer) > -1;
      }
      return flag;
    }

    internal IDictionary<string, string> GetKeysFromPrefix(string prefix)
    {
      IDictionary<string, string> results = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
      foreach (string originalValue in _originalValues)
      {
        if (originalValue != null && originalValue.Length != prefix.Length)
        {
          if (prefix.Length == 0)
          {
            GetKeyFromEmptyPrefix(originalValue, results);
          }
          else if (originalValue.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
          {
            GetKeyFromNonEmptyPrefix(prefix, originalValue, results);
          }
        }
      }
      return results;
    }

    private static void GetKeyFromEmptyPrefix(string entry, IDictionary<string, string> results)
    {
      int val1 = entry.IndexOf('.');
      int val2 = entry.IndexOf('[');
      int length = -1;
      if (val1 == -1)
      {
        if (val2 != -1)
        {
          length = val2;
        }
      }
      else
      {
        length = val2 != -1 ? Math.Min(val1, val2) : val1;
      }
      string index = length == -1 ? entry : entry.Substring(0, length);
      results[index] = index;
    }

    private static void GetKeyFromNonEmptyPrefix(string prefix, string entry, IDictionary<string, string> results)
    {
      int startIndex = prefix.Length + 1;
      string key;
      string str;
      switch (entry[prefix.Length])
      {
        case '.':
          int length = entry.IndexOf('.', startIndex);
          if (length == -1)
          {
            length = entry.Length;
          }
          key = entry.Substring(startIndex, length - startIndex);
          str = entry.Substring(0, length);
          break;
        case '[':
          int num = entry.IndexOf(']', startIndex);
          if (num == -1)
          {
            return;
          }
          key = entry.Substring(startIndex, num - startIndex);
          str = entry.Substring(0, num + 1);
          break;
        default:
          return;
      }
      if (results.ContainsKey(key))
      {
        return;
      }
      results.Add(key, str);
    }

    internal static bool IsPrefixMatch(string prefix, string testString)
    {
      if (testString == null)
      {
        return false;
      }
      if (prefix.Length == 0)
      {
        return true;
      }
      if (prefix.Length > testString.Length || !testString.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
      {
        return false;
      }
      if (testString.Length == prefix.Length)
      {
        return true;
      }
      switch (testString[prefix.Length])
      {
        case '.':
        case '[':
          return true;
        default:
          return false;
      }
    }

    private class PrefixComparer : IComparer<string>
    {

      private readonly string _prefix;

      public PrefixComparer(string prefix)
      {
        _prefix = prefix;
      }

      public int Compare(string x, string y)
      {
        return IsPrefixMatch(_prefix, ReferenceEquals(x, _prefix) ? y : x) ? 0 : StringComparer.OrdinalIgnoreCase.Compare(x, y);
      }
    }
  }
}
