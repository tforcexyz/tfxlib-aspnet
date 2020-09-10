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
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Mvc;

namespace Xyz.TForce.AspNet.Mvc
{

  public class ImageResult : ActionResult
  {

    public Bitmap Bitmap { get; set; }

    public override void ExecuteResult(ControllerContext context)
    {
      context.HttpContext.Response.ContentType = "image/jpg";
      Bitmap.Save(context.HttpContext.Response.OutputStream, ImageFormat.Jpeg);
    }
  }
}
#endif