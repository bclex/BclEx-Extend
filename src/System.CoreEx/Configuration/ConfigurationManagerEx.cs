#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System.Collections.Specialized;
namespace System.Configuration
{
    /// <summary>
    /// ConfigurationManagerEx
    /// </summary>
    public static class ConfigurationManagerEx
    {
        // Summary:
        //     Gets the System.Configuration.AppSettingsSection data for the current application's
        //     default configuration.
        //
        // Returns:
        //     Returns a System.Collections.Specialized.NameValueCollection object that
        //     contains the contents of the System.Configuration.AppSettingsSection object
        //     for the current application's default configuration.
        //
        // Exceptions:
        //   System.Configuration.ConfigurationErrorsException:
        //     Could not retrieve a System.Collections.Specialized.NameValueCollection object
        //     with the application settings data.
        /// <summary>
        /// Gets the app settings.
        /// </summary>
        public static NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }

        // Summary:
        //     Gets the System.Configuration.ConnectionStringsSection data for the current
        //     application's default configuration.
        //
        // Returns:
        //     Returns a System.Configuration.ConnectionStringSettingsCollection object
        //     that contains the contents of the System.Configuration.ConnectionStringsSection
        //     object for the current application's default configuration.
        //
        // Exceptions:
        //   System.Configuration.ConfigurationErrorsException:
        //     Could not retrieve a System.Configuration.ConnectionStringSettingsCollection
        //     object.
        /// <summary>
        /// Gets the connection strings.
        /// </summary>
        public static ConnectionStringSettingsCollection ConnectionStrings
        {
            get { return ConfigurationManager.ConnectionStrings; }
        }

        // Summary:
        //     Retrieves a specified configuration section for the current application's
        //     default configuration.
        //
        // Parameters:
        //   sectionName:
        //     The configuration section path and name.
        //
        // Returns:
        //     The specified System.Configuration.ConfigurationSection object, or null if
        //     the section does not exist.
        //
        // Exceptions:
        //   System.Configuration.ConfigurationErrorsException:
        //     A configuration file could not be loaded.
        /// <summary>
        /// Gets the section.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns></returns>
        public static T GetSection<T>(string sectionName)
        {
            var section = (T)ConfigurationManager.GetSection(sectionName);
            if (section == null)
                throw new InvalidOperationException(string.Format(Local.UndefinedItemAB, "Configuration::Section", sectionName));
            return section;
        }
    }
}
