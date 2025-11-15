namespace ComponentLib;

public class ConfirmOptions : AlertOptions
{
    public string CancelButtonText
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(CancelButtonText));
            }
        }
    }
}
