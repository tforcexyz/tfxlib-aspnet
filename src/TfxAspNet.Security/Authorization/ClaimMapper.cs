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
using System.Security.Claims;

namespace Xyz.TForce.AspNet.Security.Authorization
{

  public class ClaimMapper
  {

    private static readonly Lazy<IDictionary<string, string[]>> s_claimMapping = new Lazy<IDictionary<string, string[]>>(InitDictionary);

    private static IDictionary<string, string[]> InitDictionary()
    {
      IDictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
      dictionary[ClaimNames.ClientId] = new string[] { AdditionalClaimTypes.ClientId };
      dictionary[ClaimNames.ExternalProvider] = new string[] { AdditionalClaimTypes.ExternalProvider };
      dictionary[ClaimNames.Icon] = new string[] { AdditionalClaimTypes.Icon };
      dictionary[ClaimNames.IdToken] = new string[] { AdditionalClaimTypes.IdToken };
      dictionary[ClaimNames.Role] = new string[] { ClaimTypes.Role };
      dictionary[ClaimNames.Permission] = new string[] { AdditionalClaimTypes.Permission };
      return dictionary;
    }

    public static string[] GetClaimTypes(IEnumerable<string> claimNames)
    {
      List<string> claimTypes = new List<string>();
      IDictionary<string, string[]> mapping = s_claimMapping.Value;
      foreach (string claimName in claimNames)
      {
        string[] mappedClaimTypes = mapping.SafeGetValue(claimName);
        if (mappedClaimTypes == null)
        {
          claimTypes.Add(claimName);
        }
        else
        {
          claimTypes.AddRange(mappedClaimTypes);
        }
      }
      return claimTypes.ToArray();
    }
  }
}
