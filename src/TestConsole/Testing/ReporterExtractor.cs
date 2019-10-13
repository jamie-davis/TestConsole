using System.Linq;

namespace TestConsoleLib.Testing
{
    internal static class ReporterExtractor
    {
        public const string ReporterSetting = "reporter";
        public const string CustomSetting = "custom";
        public const string CustomArgs = "template";

        public static ExtractedReporterInfo Extract(string[] settingsFile)
        {
            ExtractedReporterInfo found = null;
            string customPath = null;
            string customArgs = null;
            foreach (var setting in settingsFile.Select(l => l.Trim()))
            {
                var eqPos = setting.IndexOf('=');
                if (eqPos > 0) //note that starting with = makes no sense (i.e. eqPos == 0 is not useful)
                {
                    var settingName = setting.Substring(0, eqPos);
                    var settingValue = setting.Substring(eqPos + 1).Trim();
                    if (settingName == ReporterSetting && !string.IsNullOrWhiteSpace(settingValue))
                    {
                        found = found ?? new ExtractedReporterInfo(settingValue);
                        if (customPath == null)
                            break;
                    } else if (settingName == CustomSetting && !string.IsNullOrWhiteSpace(settingValue))
                    {
                        customPath = customPath ?? settingValue;
                    } else if (settingName == CustomArgs && !string.IsNullOrWhiteSpace(settingValue))
                    {
                        customArgs = customArgs ?? settingValue;
                    }

                    if (customPath != null && customArgs != null)
                    {
                        break;
                    }

                }
            }

            if (customPath != null)
            {
                return new ExtractedReporterInfo(customPath, customArgs);
            }
            return found ?? new ExtractedReporterInfo();
        }
    }
}