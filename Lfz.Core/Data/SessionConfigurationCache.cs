using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Cfg;
using PMSoft.Logging;
using PMSoft.Security;
using PMSoft.Utitlies;

namespace PMSoft.Data
{
    /// <summary>
    /// Session≈‰÷√ª∫¥Ê
    /// </summary>
    public class SessionConfigurationCache : ISessionConfigurationCache
    {
        private readonly ShellSettings _shellSettings;

        private ConfigurationCache _currentConfig;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shellSettings"></param>
        public SessionConfigurationCache(
            ShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
            _currentConfig = null;

            Logger = LoggerFactory.GetLog();
        }

        ILogger Logger { get; set; }

        /// <summary>
        /// ¥”ª∫¥Ê÷–ªÒ»°≈‰÷√
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public Configuration GetConfiguration(Func<Configuration> builder)
        {
            var hash = ComputeHash().Value;

            // if the current configuration is unchanged, return it
            if (_currentConfig != null && _currentConfig.Hash == hash)
            {
                return _currentConfig.Configuration;
            }

            // Return previous configuration if it exists and has the same hash as
            // the current blueprint.
            var previousConfig = ReadConfiguration(hash);
            if (previousConfig != null)
            {
                _currentConfig = previousConfig;
                return previousConfig.Configuration;
            }

            // Create cache and persist it
            _currentConfig = new ConfigurationCache
            {
                Hash = hash,
                Configuration = builder()
            };

            StoreConfiguration(_currentConfig);
            return _currentConfig.Configuration;
        }

        private class ConfigurationCache
        {
            public string Hash { get; set; }
            public Configuration Configuration { get; set; }
        }

        private void StoreConfiguration(ConfigurationCache cache)
        {
            if (!Utils.IsFullTrust)
                return;

            var pathName = GetMappingPathName();

            try
            {
                Utils.CheckDirectoryExists("Config");
                var formatter = new BinaryFormatter();
                using (var stream = File.Create(pathName))
                {
                    formatter.Serialize(stream, cache.Hash);
                    formatter.Serialize(stream, cache.Configuration);
                }
            }
            catch (SerializationException e)
            {
                //Note: This can happen when multiple processes/AppDomains try to save
                //      the cached configuration at the same time. Only one concurrent
                //      writer will win, and it's harmless for the other ones to fail.
                for (Exception scan = e; scan != null; scan = scan.InnerException)
                    Logger.Warning("Error storing new NHibernate cache configuration: {0}", scan.Message);
            }
        }

        private ConfigurationCache ReadConfiguration(string hash)
        {
            if (!Utils.IsFullTrust)
                return null;

            var pathName = GetMappingPathName();

            if (!File.Exists(pathName))
                return null;

            try
            {
                var formatter = new BinaryFormatter();
                using (var stream = File.OpenRead(pathName))
                {

                    // if the stream is empty, stop here
                    if (stream.Length == 0)
                    {
                        return null;
                    }

                    var oldHash = (string)formatter.Deserialize(stream);
                    if (hash != oldHash)
                    {
                        Logger.Information("The cached NHibernate configuration is out of date. A new one will be re-generated.");
                        return null;
                    }

                    var oldConfig = (Configuration)formatter.Deserialize(stream);

                    return new ConfigurationCache
                    {
                        Hash = oldHash,
                        Configuration = oldConfig
                    };
                }
            }
            catch (Exception e)
            {
                for (var scan = e; scan != null; scan = scan.InnerException)
                    Logger.Warning("Error reading the cached NHibernate configuration: {0}", scan.Message);
                Logger.Information("A new one will be re-generated.");
                return null;
            }
        }

        private Hash ComputeHash()
        {
            var hash = new Hash();

            // Shell settings physical location
            //   The nhibernate configuration stores the physical path to the SqlCe database
            //   so we need to include the physical location as part of the hash key, so that
            //   xcopy migrations work as expected.
            var pathName = GetMappingPathName();
            hash.AddString(pathName.ToLowerInvariant());

            // Shell settings data
            hash.AddString(_shellSettings.Settings.DataProvider);
            hash.AddString(_shellSettings.Settings.TablePrefix);
            hash.AddString(_shellSettings.Settings.DataConnectionString);
            hash.AddString(_shellSettings.Settings.DataFolder);
            hash.AddString(_shellSettings.Settings.Version.ToString());

            // Assembly names, record names and property names
            foreach (var tableName in _shellSettings.RecordBlueprints.Select(x => x.TableName))
            {
                hash.AddString(tableName);
            }

            foreach (var recordType in _shellSettings.RecordBlueprints.Select(x => x.Type))
            {
                hash.AddTypeReference(recordType);

                if (recordType.BaseType != null)
                    hash.AddTypeReference(recordType.BaseType);

                foreach (var property in recordType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public))
                {
                    hash.AddString(property.Name);
                    hash.AddTypeReference(property.PropertyType);

                    foreach (var attr in property.GetCustomAttributesData())
                    {
                        hash.AddTypeReference(attr.Constructor.DeclaringType);
                    }
                }
            }

            return hash;
        }

        private string GetMappingPathName()
        {
            string path = Utils.MapPath("Config");
            return Path.Combine(path, "mapping.bin");
        }

    }
}
