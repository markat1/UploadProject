using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using UploadProject.Web.Models;
using File = UploadProject.Web.Models.File;

namespace UploadProject.Web.Components.Pages
{
    public partial class UploadFiles
    {
        private List<IBrowserFile> loadedFiles = new();
        private long maxFileSize = 1_000_024 * 15;
        private int maxAllowedFiles = 3;
        private bool isLoading;
        private bool shouldRender;
        private List<UploadResult> uploadResults = new();
        private List<File> files = new();

        [Inject] IHttpClientFactory ClientFactory { get; set; }


        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            //shouldRender = false;
            int maxFileSize = 10000024 * 15;
            var upload = false;

            using var content = new MultipartFormDataContent();

            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                if (uploadResults.SingleOrDefault(
                    f => f.FileName == file.Name) is null)
                {
                    try
                    {
                        files.Add(new() { Name = file.Name });

                        var stream = new LazyBrowserFileStream(file, maxFileSize);
                        var fileContent = new StreamContent(stream);

                        fileContent.Headers.ContentType =
                            new MediaTypeHeaderValue(file.ContentType);

                        content.Add(
                            content: fileContent,
                            name: "\"files\"",
                            fileName: file.Name);

                        upload = true;
                    }
                    catch (Exception ex)
                    {

                        uploadResults.Add(
                            new()
                            {
                                FileName = file.Name,
                                ErrorCode = 6,
                                Uploaded = false
                            });
                    }
                }
            }

            if (upload)
            {
                var client = ClientFactory.CreateClient();
                var reqtoken = await client.GetFromJsonAsync<ReceiverToken>("http://apiservice/api/antiforgery-token");

                client.DefaultRequestHeaders.Add("RequestVerificationToken", reqtoken!.Token);

                var response =
                   await client.PostAsync("http://apiservice/api/Filesave", content);
                //await client.PostAsync("http://apiservice/api/filesave", content);


            }


        }

        private static bool FileUpload(IList<UploadResult> uploadResults, string? fileName, out UploadResult result)
        {
            result = uploadResults.SingleOrDefault(f => f.FileName == fileName) ?? new();

            if (!result.Uploaded)
            {
                result.ErrorCode = 5;
            }

            return result.Uploaded;
        }

    }
}
