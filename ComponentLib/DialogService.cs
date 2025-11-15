using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Reflection;

namespace ComponentLib;

public class DialogService : IDisposable
{
    public event Action<string, Type, Dictionary<string, object>, DialogOptions> OnOpen;
    public event Action<dynamic> OnClose;
    public event Action OnRefresh;
    internal DotNetObjectReference<DialogService> Reference
    {
        get
        {
            field ??= DotNetObjectReference.Create(this);
            return field;
        }
    }

    protected List<object> _dialogs = [];
    protected List<TaskCompletionSource<dynamic>> _tasks = [];

    private readonly NavigationManager _uriHelper;
    private readonly IJSRuntime _jsRuntime;

    public DialogService(NavigationManager uriHelper, IJSRuntime jsRuntime)
    {
        _uriHelper = uriHelper;
        _jsRuntime = jsRuntime;

        if (_uriHelper != null)
        {
            _uriHelper.LocationChanged += UriHelper_OnLocationChanged;
        }
    }

    private void UriHelper_OnLocationChanged(object sender, LocationChangedEventArgs e)
    {
        while (_dialogs.Count != 0)
        {
            Close();
        }
    }

    public virtual void Close(dynamic result = null)
    {
        var dialog = _dialogs.LastOrDefault();

        if (dialog != null)
        {
            OnClose?.Invoke(result);
            _dialogs.Remove(dialog);
        }

        var task = _tasks.LastOrDefault();
        if (task != null && task.Task != null && !task.Task.IsCompleted)
        {
            _tasks.Remove(task);
            task.SetResult(result);
        }
    }

    public virtual void Open<T>(string title, Dictionary<string, object> parameters = null, DialogOptions options = null) where T : ComponentBase
    {
        OpenDialog<T>(title, parameters, options);
    }

    public virtual void Open(string title, Type componentType, Dictionary<string, object> parameters = null, DialogOptions options = null)
    {
        if (!typeof(ComponentBase).IsAssignableFrom(componentType))
        {
            throw new ArgumentException("The component type must be a subclass of ComponentBase.", nameof(componentType));
        }

        var method = GetType().GetMethod(nameof(OpenDialog), BindingFlags.NonPublic | BindingFlags.Instance);

        method.MakeGenericMethod(componentType).Invoke(this, [title, parameters, options]);
    }

    public void Refresh()
    {
        OnRefresh?.Invoke();
    }

    public virtual Task<dynamic> OpenAsync<T>(string title, Dictionary<string, object> parameters = null, DialogOptions options = null) where T : ComponentBase
    {
        var task = new TaskCompletionSource<dynamic>();
        _tasks.Add(task);

        OpenDialog<T>(title, parameters, options);

        return task.Task;
    }

    public virtual Task<dynamic> OpenAsync(string title, Type componentType, Dictionary<string, object> parameters = null, DialogOptions options = null)
    {
        if (!typeof(ComponentBase).IsAssignableFrom(componentType))
        {
            throw new ArgumentException("The component type must be a subclass of ComponentBase.", nameof(componentType));
        }

        var task = new TaskCompletionSource<dynamic>();
        _tasks.Add(task);

        var method = GetType().GetMethod(nameof(OpenDialog), BindingFlags.Instance | BindingFlags.NonPublic);

        method.MakeGenericMethod(componentType).Invoke(this, new object[] { title, parameters, options });

        return task.Task;
    }

    public virtual Task<dynamic> OpenAsync(string title, RenderFragment<DialogService> childContent, DialogOptions options = null, CancellationToken? cancellationToken = null)
    {
        var task = new TaskCompletionSource<dynamic>();

        if (cancellationToken.HasValue)
            cancellationToken.Value.Register(() => task.TrySetCanceled());

        _tasks.Add(task);

        options ??= new DialogOptions();
        options.ChildContent = childContent;

        OpenDialog<object>(title, null, options);

        return task.Task;
    }

    public virtual Task<dynamic> OpenAsync(RenderFragment<DialogService> titleContent, RenderFragment<DialogService> childContent, DialogOptions options = null, CancellationToken? cancellationToken = null)
    {
        var task = new TaskCompletionSource<dynamic>();

        if (cancellationToken.HasValue)
            cancellationToken.Value.Register(() => task.TrySetCanceled());

        _tasks.Add(task);

        options ??= new DialogOptions();
        options.ChildContent = childContent;
        options.TitleContent = titleContent;

        OpenDialog<object>(null, null, options);

        return task.Task;
    }

    public virtual void Open(string title, RenderFragment<DialogService> childContent, DialogOptions options = null)
    {
        options ??= new DialogOptions();

        options.ChildContent = childContent;

        OpenDialog<object>(title, null, options);
    }

    internal void OpenDialog<T>(string title, Dictionary<string, object> parameters, DialogOptions options)
    {
        _dialogs.Add(new object());

        // Validate and set default values for the dialog options
        options ??= new();
        options.Width = !string.IsNullOrEmpty(options.Width) ? options.Width : "600px";
        options.Left = !string.IsNullOrEmpty(options.Left) ? options.Left : "";
        options.Top = !string.IsNullOrEmpty(options.Top) ? options.Top : "";
        options.Bottom = !string.IsNullOrEmpty(options.Bottom) ? options.Bottom : "";
        options.Height = !string.IsNullOrEmpty(options.Height) ? options.Height : "";
        options.Style = !string.IsNullOrEmpty(options.Style) ? options.Style : "";
        options.CssClass = !string.IsNullOrEmpty(options.CssClass) ? options.CssClass : "";
        options.WrapperCssClass = !string.IsNullOrEmpty(options.WrapperCssClass) ? options.WrapperCssClass : "";
        options.ContentCssClass = !string.IsNullOrEmpty(options.ContentCssClass) ? options.ContentCssClass : "";

        OnOpen?.Invoke(title, typeof(T), parameters, options);
    }

    public virtual async Task<bool?> Confirm(string message = "Confirm?", string title = "Confirm", ConfirmOptions options = null, CancellationToken? cancellationToken = null)
    {
        options ??= new();
        options.OkButtonText = !string.IsNullOrEmpty(options.OkButtonText) ? options.OkButtonText : "Ok";
        options.CancelButtonText = !string.IsNullOrEmpty(options.CancelButtonText) ? options.CancelButtonText : "Cancel";
        options.Width = !string.IsNullOrEmpty(options.Width) ? options.Width : ""; // Width is set to 600px by default by OpenAsync
        options.Style = !string.IsNullOrEmpty(options.Style) ? options.Style : "";
        options.CssClass = !string.IsNullOrEmpty(options.CssClass) ? $"{options.CssClass}" : "";
        options.WrapperCssClass = !string.IsNullOrEmpty(options.WrapperCssClass) ? $"{options.WrapperCssClass}" : "";

        return await OpenAsync(title, ds =>
        {
            return b =>
            {
                var i = 0;
                b.OpenElement(i++, "p");
                b.AddAttribute(i++, "class", "text-center");
                b.AddContent(i++, message);
                b.CloseElement();

                b.OpenElement(i++, "hr");
                b.AddAttribute(i++, "class", "my-3");
                b.CloseElement();

                b.OpenElement(i++, "div");

                b.AddAttribute(i++, "class", "d-flex justify-content-end gap-2");

                b.OpenElement(i++, "button");
                b.AddAttribute(i++, "class", "btn btn-primary");
                b.AddAttribute(i++, "onclick", EventCallback.Factory.Create(this, () => ds.Close(true)));
                b.AddContent(i++, options.OkButtonText);
                b.CloseElement();

                b.OpenElement(i++, "button");
                b.AddAttribute(i++, "class", "btn btn-secondary");
                b.AddAttribute(i++, "onclick", EventCallback.Factory.Create(this, () => ds.Close(false)));
                b.AddContent(i++, options.CancelButtonText);
                b.CloseElement();

                b.CloseElement();
            };
        }, options, cancellationToken);
    }

    public virtual async Task<bool?> Confirm(RenderFragment message, string title = "Confirm", ConfirmOptions options = null, CancellationToken? cancellationToken = null)
    {
        options ??= new();
        options.OkButtonText = !string.IsNullOrEmpty(options.OkButtonText) ? options.OkButtonText : "Ok";
        options.CancelButtonText = !string.IsNullOrEmpty(options.CancelButtonText) ? options.CancelButtonText : "Cancel";
        options.Width = !string.IsNullOrEmpty(options.Width) ? options.Width : ""; // Width is set to 600px by default by OpenAsync
        options.Style = !string.IsNullOrEmpty(options.Style) ? options.Style : "";
        options.CssClass = !string.IsNullOrEmpty(options.CssClass) ? $"rz-dialog-confirm {options.CssClass}" : "rz-dialog-confirm";
        options.WrapperCssClass = !string.IsNullOrEmpty(options.WrapperCssClass) ? $"rz-dialog-wrapper {options.WrapperCssClass}" : "rz-dialog-wrapper";

        return await OpenAsync(title, ds => b =>
        {
            var i = 0;
            b.OpenElement(i++, "p");
            b.AddContent(i++, message);
            b.CloseElement();

            b.OpenElement(i++, "hr");
            b.AddAttribute(i++, "class", "my-3");
            b.CloseElement();

            b.OpenElement(i++, "div");

            b.AddAttribute(i++, "class", "d-flex justify-content-end gap-2");

            b.OpenElement(i++, "button");
            b.AddAttribute(i++, "class", "btn btn-primary");
            b.AddAttribute(i++, "onclick", EventCallback.Factory.Create(this, () => ds.Close(true)));
            b.AddContent(i++, options.OkButtonText);
            b.CloseElement();

            b.OpenElement(i++, "button");
            b.AddAttribute(i++, "class", "btn btn-secondary");
            b.AddAttribute(i++, "onclick", EventCallback.Factory.Create(this, () => ds.Close(false)));
            b.AddContent(i++, options.CancelButtonText);
            b.CloseElement();

            b.CloseElement();
        }, options, cancellationToken);
    }

    public virtual async Task<bool?> Alert(string message = "", string title = "Message", AlertOptions options = null, CancellationToken? cancellationToken = null)
    {
        options ??= new();
        options.OkButtonText = !string.IsNullOrEmpty(options.OkButtonText) ? options.OkButtonText : "Ok";
        options.Width = !string.IsNullOrEmpty(options.Width) ? options.Width : "";
        options.Style = !string.IsNullOrEmpty(options.Style) ? options.Style : "";
        options.CssClass = !string.IsNullOrEmpty(options.CssClass) ? $"{options.CssClass}" : "";
        options.WrapperCssClass = !string.IsNullOrEmpty(options.WrapperCssClass) ? $"{options.WrapperCssClass}" : "";
        options.ContentCssClass = !string.IsNullOrEmpty(options.ContentCssClass) ? $"{options.ContentCssClass}" : "";

        return await OpenAsync(title, ds =>
        {
            return b =>
            {
                var i = 0;
                b.OpenElement(i++, "p");
                b.AddAttribute(i++, "class", "text-center");
                b.AddContent(i++, message);
                b.CloseElement();

                b.OpenElement(i++, "hr");
                b.AddAttribute(i++, "class", "my-3");
                b.CloseElement();

                b.OpenElement(i++, "div");

                b.AddAttribute(i++, "class", "d-flex justify-content-end");

                b.OpenElement(i++, "button");

                b.AddAttribute(i++, "class", "btn btn-primary");
                b.AddAttribute(i++, "onclick", EventCallback.Factory.Create(this, () => ds.Close(true)));
                b.AddContent(i++, options.OkButtonText);
                b.CloseElement();

                b.CloseElement();
            };
        }, options, cancellationToken);
    }

    public virtual async Task<bool?> Alert(RenderFragment message, string title = "Message", AlertOptions options = null, CancellationToken? cancellationToken = null)
    {
        options ??= new();
        options.OkButtonText = !string.IsNullOrEmpty(options.OkButtonText) ? options.OkButtonText : "Ok";
        options.Width = !string.IsNullOrEmpty(options.Width) ? options.Width : "";
        options.Style = !string.IsNullOrEmpty(options.Style) ? options.Style : "";
        options.CssClass = !string.IsNullOrEmpty(options.CssClass) ? $"{options.CssClass}" : "";
        options.WrapperCssClass = !string.IsNullOrEmpty(options.WrapperCssClass) ? $"{options.WrapperCssClass}" : "";
        options.ContentCssClass = !string.IsNullOrEmpty(options.ContentCssClass) ? $"{options.ContentCssClass}" : "";

        return await OpenAsync(title, ds =>
            {
                return b =>
                {
                    var i = 0;
                    b.OpenElement(i++, "p");
                    b.AddContent(i++, message);
                    b.CloseElement();

                    b.OpenElement(i++, "hr");
                    b.AddAttribute(i++, "class", "my-3");
                    b.CloseElement();

                    b.OpenElement(i++, "div");

                    b.AddAttribute(i++, "class", "d-flex justify-content-end");

                    b.OpenElement(i++, "button");

                    b.AddAttribute(i++, "class", "btn btn-primary");
                    b.AddAttribute(i++, "onclick", EventCallback.Factory.Create(this, () => ds.Close(true)));
                    b.AddContent(i++, options.OkButtonText);
                    b.CloseElement();

                    b.CloseElement();
                };
            }, options, cancellationToken);
    }

    public void Dispose()
    {
        Reference?.Dispose();
        _uriHelper.LocationChanged -= UriHelper_OnLocationChanged;
    }
}
