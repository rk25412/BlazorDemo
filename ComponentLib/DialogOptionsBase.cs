using System.ComponentModel;
using Microsoft.AspNetCore.Components;
namespace ComponentLib;

public class DialogOptionsBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool ShowTitle
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(ShowTitle));
            }
        }
    } = true;

    public bool ShowClose
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(ShowClose));
            }
        }
    } = true;

    public string Width
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Width));
            }
        }
    }

    public string Height
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Height));
            }
        }
    }

    public string Style
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Style));
            }
        }
    }

    public string CssClass
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(CssClass));
            }
        }
    }

    public string WrapperCssClass
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(WrapperCssClass));
            }
        }
    }

    public string ContentCssClass
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(ContentCssClass));
            }
        }
    }

    public int CloseTabIndex
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(CloseTabIndex));
            }
        }
    }

    public RenderFragment<DialogService> TitleContent
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(TitleContent));
            }
        }
    }
}
