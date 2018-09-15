using System.Linq;

namespace TestConsoleLib.Testing
{
    public static class ReporterExtractor
    {
        public const string ReporterSetting = "reporter";

        public static bool Extract(string[] settingsFile, out string reporterName)
        {
            foreach (var setting in settingsFile.Select(l => l.Trim()))
            {
                var eqPos = setting.IndexOf('=');
                if (eqPos > 0) //note that starting with = makes no sense (i.e. eqPos == 0 is not useful)
                {
                    var settingName = setting.Substring(0, eqPos);
                    var settingValue = setting.Substring(eqPos + 1).Trim();
                    if (settingName == ReporterSetting && !string.IsNullOrWhiteSpace(settingValue))
                    {
                        reporterName = settingValue;
                        return true;
                    }
                }
            }

            reporterName = null;
            return false;
        }
    }
}