using Microsoft.AspNetCore.Components;
namespace ComponentLib;

public class DialogOptions : DialogOptionsBase
{
    public string Left
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Left));
            }
        }
    }

    public string Top
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Top));
            }
        }
    }

    public string Bottom
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Bottom));
            }
        }
    }

    public RenderFragment<DialogService> ChildContent
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(ChildContent));
            }
        }
    }

    public bool AutoFocusFirstElement
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(AutoFocusFirstElement));
            }
        }
    }

    public bool CloseDialogOnEsc
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(CloseDialogOnEsc));
            }
        }
    }
}
