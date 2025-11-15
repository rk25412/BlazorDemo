namespace ComponentLib;

public class AlertOptions : DialogOptions
{
    public string OkButtonText
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(OkButtonText));
            }
        }
    }
}
