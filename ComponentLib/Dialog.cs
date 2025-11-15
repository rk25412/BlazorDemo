using System.ComponentModel;

namespace ComponentLib;

public class Dialog : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public string Title
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Title));
            }
        }
    }

    public Type Type
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Type));
            }
        }
    }

    public Dictionary<string, object> Parameters
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Parameters));
            }
        }
    }

    public DialogOptions Options
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Options));
            }
        }
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
