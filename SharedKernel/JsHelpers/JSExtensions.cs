using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace SharedKernel.JsHelpers
{
    public static class JSExtensions
    {
        public static async Task SaveAs(this IJSRuntime jSRuntime, string filename, byte[] fileBytes)
        {
            await jSRuntime.InvokeAsync<object>("saveAsFile", filename, Convert.ToBase64String(fileBytes));
        }

        public static async Task ScrollToTop(this IJSRuntime jSRuntime, string selector)
        {
            await jSRuntime.InvokeVoidAsync("ScrollToTop", selector);
        }
    }
}
