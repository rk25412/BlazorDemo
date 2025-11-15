using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ComponentLib;

public partial class PopupDialogContainer
{
    [Parameter]
    public Dialog Dialog { get; set; }

    [Parameter]
    public bool ShowMask { get; set; } = true;

    [Inject]
    public required DialogService Service { get; set; }

    private ElementReference dialog;
    private object reference;

    private RenderFragment ChildContent => new(builder =>
    {
        builder.OpenComponent<CascadingValue<Dialog>>(0); // Open CascadingValue
        builder.AddAttribute(1, "Value", Dialog);
        builder.AddAttribute(2, "IsFixed", true);

        builder.AddAttribute(3, "ChildContent", (RenderFragment)(builder2 =>
        {
            builder2.OpenComponent(0, Dialog.Type); // Open Dialog

            if (Dialog.Parameters != null)
            {
                foreach (var parameter in Dialog.Parameters)
                {
                    builder2.AddAttribute(1, parameter.Key, parameter.Value);
                }
            }

            builder2.AddComponentReferenceCapture(2, component => reference = component); // Capture reference
            builder2.CloseComponent();
        }));

        builder.CloseComponent();
    });
    private bool shouldRender = true;
    protected override bool ShouldRender()
    {
        return shouldRender;
    }
    private void OnKeyPress(KeyboardEventArgs args)
    {
        var key = args.Code != null ? args.Code : args.Key;

        if (key == "Space" || key == "Enter")
        {
            Close();
        }
    }
    private void Close()
    {
        Service.Close();
    }
}


