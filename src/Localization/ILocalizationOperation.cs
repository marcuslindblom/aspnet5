namespace src.Localization
{
    public interface ILocalizationOperation 
    {
        string Key { get; set; }

        object Entity { get; set; }

        Page Page { get; set; }

        T Load<T>(string id);
    }
}