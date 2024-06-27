using Microsoft.JSInterop;

namespace Zufanci.Client.Helpers
{
    /// <summary>
    /// Provides extension methods for working with JavaScript interop (JSInterop) in Blazor applications.
    /// </summary>
    public static class IJSRuntimeExtension
    {
        /// <summary>
        /// Displays a success toastr notification using JavaScript interop.
        /// </summary>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/> instance.</param>
        /// <param name="title">The title of the toastr notification.</param>
        /// <param name="message">The message of the toastr notification.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        public static async ValueTask ToastrSuccess(this IJSRuntime jsRuntime, string title, string message)
        {
            await jsRuntime.InvokeVoidAsync("ShowToastr", "success", title, message);
        }

        /// <summary>
        /// Displays an error toastr notification using JavaScript interop.
        /// </summary>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/> instance.</param>
        /// <param name="title">The title of the toastr notification.</param>
        /// <param name="message">The message of the toastr notification.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        public static async ValueTask ToastrError(this IJSRuntime jsRuntime, string title, string message)
        {
            await jsRuntime.InvokeVoidAsync("ShowToastr", "error", title, message);
        }
    }
}
