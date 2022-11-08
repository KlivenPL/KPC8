using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Player._Configuration.Dtos;
using Player.Contexts;
using Player.Persistence;

namespace Player.Loaders {
    internal class KPC8ConfigurationLoader {
        private readonly static JSchema kPC8ConfigurationSaveSchema;
        private readonly ProgramContext programContext;

        static KPC8ConfigurationLoader() {
            var schemaGenerator = new JSchemaGenerator {
                DefaultRequired = Required.DisallowNull
            };

            kPC8ConfigurationSaveSchema = schemaGenerator.Generate(typeof(KPC8ConfigurationSave));
        }

        public KPC8ConfigurationLoader(ProgramContext programContext) {
            this.programContext = programContext;
        }

        public bool TryGetConfiguration(out KPC8ConfigurationDto configurationDto, out string configValidationErrors) {
            configurationDto = null;
            configValidationErrors = null;

            DirectoryInfo confSaveDirInfo;

            if (programContext.IsSourceFileSelected) {
                confSaveDirInfo = programContext.SourceFile.Directory;
            } else if (programContext.IsRomFileSelected) {
                confSaveDirInfo = programContext.RomFile.Directory;
            } else {
                return false;
            }

            if (TryGetConfigurationSaveFile(confSaveDirInfo, out var confSaveFileInfo)) {
                if (TryLoadConfiguration(confSaveFileInfo, out var configurationSave, out configValidationErrors)) {
                    configurationDto = KPC8ConfigurationDto.FromSave(configurationSave);
                    return true;
                }
            }

            return false;
        }

        private bool TryGetConfigurationSaveFile(DirectoryInfo directoryInfo, out FileInfo configurationSaveFileInfo) {
            var files = directoryInfo.GetFiles("*.kpcconfig");
            configurationSaveFileInfo = files?.FirstOrDefault();
            return configurationSaveFileInfo != null;

        }

        private bool TryLoadConfiguration(FileInfo fileInfo, out KPC8ConfigurationSave kPC8ConfigurationSave, out string configValidationErrors) {
            kPC8ConfigurationSave = null;
            configValidationErrors = null;

            using StreamReader file = fileInfo.OpenText();
            using JsonTextReader reader = new JsonTextReader(file);

            JObject o2 = (JObject)JToken.ReadFrom(reader);

            if (o2.IsValid(kPC8ConfigurationSaveSchema, out IList<string> errors)) {
                kPC8ConfigurationSave = o2.ToObject<KPC8ConfigurationSave>();
                return true;
            } else {
                configValidationErrors = string.Join(Environment.NewLine, errors);
            }

            return false;
        }
    }
}
