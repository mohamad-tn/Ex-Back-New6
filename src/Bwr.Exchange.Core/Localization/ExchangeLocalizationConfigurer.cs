using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace Bwr.Exchange.Localization
{
    public static class ExchangeLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(ExchangeConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(ExchangeLocalizationConfigurer).GetAssembly(),
                        "Bwr.Exchange.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
