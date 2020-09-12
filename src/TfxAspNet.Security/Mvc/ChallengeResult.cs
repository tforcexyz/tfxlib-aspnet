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
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace Xyz.TForce.AspNet.Security.Mvc
{

  public class ChallengeResult : HttpUnauthorizedResult
  {

    private const string XsrfKey = "XsrfId";

    public string LoginProvider { get; set; }
    public string RedirectUri { get; set; }
    public string UserId { get; set; }

    public override void ExecuteResult(ControllerContext context)
    {
      AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = RedirectUri };
      if (UserId != null)
      {
        properties.Dictionary[XsrfKey] = UserId;
      }
      context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
    }
  }
}
#endif
