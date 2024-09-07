using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using UploadProject.ApiService.Models;

namespace UploadProject.ApiService.Api
{
    public static class UploadAssistantApi
    {
        public static void MapUploadApiEndpoints(this WebApplication app)
        {
            app.MapPost("api/filesave", PostFile);
            app.MapPost("api/filesave-two", PostFileTwo);

            app.MapGet("/api/antiforgery-token", async (HttpContext context, IAntiforgery antiforgery) =>
            {
                var tokens = antiforgery.GetAndStoreTokens(context);
                return Results.Ok(new { token = tokens.RequestToken });
            });

        }

     
        public static ActionResult<IList<UploadResult>> PostFile(IFormCollection files, IHostEnvironment env)
        {
            var maxAllowedFiles = 3;
            long maxFileSize = 1024 * 15;
            var filesProcessed = 0;
            //var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
            List<UploadResult> uploadResults = new();
            var resourcePath = new Uri("{Request.Scheme}://{Request.Host}/");

            foreach (var file in files)
            {
                var uploadResult = new UploadResult();
                string trustedFileNameForFileStorage;
                //var untrustedFileName = file.FileName;
                //uploadResult.FileName = untrustedFileName;
                //var trustedFileNameForDisplay =
                //    WebUtility.HtmlEncode(untrustedFileName);

                if (filesProcessed < maxAllowedFiles)
                {
                    //if (file == 0)
                    //{
                    //    uploadResult.ErrorCode = 1;
                    //}
                    //else if (file.Length > maxFileSize)
                    //{
                    //    uploadResult.ErrorCode = 2;
                    //}
                    try
                    {
                        trustedFileNameForFileStorage = Path.GetRandomFileName();
                        var path = Path.Combine(env.ContentRootPath,
                            env.EnvironmentName, "unsafe_uploads",
                            trustedFileNameForFileStorage);


                        uploadResult.Uploaded = true;
                        uploadResult.StoredFileName = trustedFileNameForFileStorage;
                    }
                    catch (IOException ex)
                    {

                        uploadResult.ErrorCode = 3;
                    }

                    filesProcessed++;
                }
                else
                {
                    uploadResult.ErrorCode = 4;
                }

                uploadResults.Add(uploadResult);
            }

            return new CreatedResult(resourcePath, uploadResults);
        }

        public static void PostFileTwo([FromForm] IEnumerable<IFormFile> files)
        {
            var t = files;
        }


    }
}
