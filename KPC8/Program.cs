using Autofac;
using KPC8._Infrastructure;

namespace KPC8 {
    class Program {
        static void Main(string[] args) {
            using var scope = CompositionRoot.BeginLifetimeScope();
            scope.Resolve<Application>().Run();
        }
    }
}
