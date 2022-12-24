using System.Diagnostics;

using Microsoft.Extensions.Options;

namespace WebSocketMockServer.Configuration.Validation
{
    /// <summary>
    /// Validator for <see cref="FileLoaderConfiguration"/>.
    /// </summary>
    public class FileLoaderConfigurationValidator : IValidateOptions<FileLoaderConfiguration>
    {
        /// <summary>
        /// Validates <see cref="FileLoaderConfiguration"/>.
        /// </summary>
        public ValidateOptionsResult Validate(string? name, FileLoaderConfiguration options)
        {
            Debug.Assert(name is not null);
            Debug.Assert(options is not null);

            if (string.IsNullOrEmpty(options.Folder))
            {
                return ValidateOptionsResult.Fail("Root folder is not set.");
            }

            if (options.Mapping == null)
            {
                return ValidateOptionsResult.Fail("Templates of the config are not set.");
            }

            if (!options.Mapping.Any())
            {
                return ValidateOptionsResult.Fail("There is no any template in configuration.");
            }

            foreach (var template in options.Mapping!)
            {
                if (string.IsNullOrWhiteSpace(template.File))
                {
                    return ValidateOptionsResult.Fail("Template file path not set.");
                }

                if (template.Reactions == null)
                {
                    return ValidateOptionsResult.Fail($"Template {template.File} reactions not set.");
                }

                if (!template.Reactions.Any())
                {
                    return ValidateOptionsResult.Fail("There is no any reactions in configuration.");
                }

                foreach (var res in template.Reactions)
                {
                    if (string.IsNullOrWhiteSpace(res.File))
                    {
                        return ValidateOptionsResult.Fail($"Template {template.File} reactions file path not set.");
                    }
                }
            }

            return ValidateOptionsResult.Success;
        }
    }
}
