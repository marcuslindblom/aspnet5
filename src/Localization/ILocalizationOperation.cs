using System.Threading.Tasks;

namespace src.Localization
{
    public interface ILocalizationOperation 
    {
        string Key { get; set; }

        object Entity { get; set; }

        Page Page { get; set; }

        Page Load(string id);
    }
}