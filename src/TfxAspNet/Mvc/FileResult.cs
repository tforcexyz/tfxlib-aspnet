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
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
#else
using System.Web.Mvc;
#endif

namespace Xyz.TForce.AspNet.Mvc
{

  public class FileResult : ActionResult
  {

    public string PhysicalPath { get; set; }

    public string FileName { get; set; }

    public string MimeType { get; set; }

#if NETSTANDARD2_0
    public override Task ExecuteResultAsync(ActionContext context)
    {
      Stream stream = File.OpenRead(PhysicalPath);
      FileStreamResult fileStreamResult = new FileStreamResult(stream, new MediaTypeHeaderValue(MimeType));
      if (!string.IsNullOrWhiteSpace(FileName))
      {
        fileStreamResult.FileDownloadName = FileName;
      }
      return context.HttpContext.RequestServices.GetRequiredService<FileStreamResultExecutor>().ExecuteAsync(context, fileStreamResult);
    }
#else
    public override void ExecuteResult(ControllerContext context)
    {
      if (!string.IsNullOrEmpty(FileName))
      {
        context.HttpContext.Response.AddHeader("content-disposition", $"attachment; filename=\"{FileName}\"");
      }
      context.HttpContext.Response.TransmitFile(PhysicalPath);
    }
#endif
  }
}
