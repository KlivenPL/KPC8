using Player._Infrastructure.Events;

namespace Player.Events {
    internal class LoadedProgramChangedEvent : IEvent {
        public FileInfo RomFile { get; init; }
        public FileInfo SourceFile { get; init; }
    }
}
