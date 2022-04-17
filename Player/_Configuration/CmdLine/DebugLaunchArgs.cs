using CommandLine;

namespace Player._Configuration.CmdLine {

    [Verb("debug", HelpText = "No-GUI debug mode")]
    internal class DebugLaunchArgs {

        [Option('s', "src-path", Required = true, HelpText = "Source file path to compile and load")]
        public string SourceFilePath { get; init; }

        [Option('c', "config-path", Required = false, HelpText = "KPC8 configuration file path")]
        public string KPC8ConfigFilePath { get; init; }

        [Option('p', "pause-at-entry", Required = false, HelpText = "Pause at entry (step into debug)")]
        public bool PauseAtEntry { get; init; }
    }
}
