using System;
using System.Data.Common;
using System.Linq;
using static System.AppContext;
using static System.IO.File;
using static System.IO.Path;

namespace StringAsSql.Tests {
    public abstract class BaseFixture : IDisposable {
        public readonly string dbFilePath;
        private readonly string[] pathsToDelete;
        public DbConnection Connection { get; protected set; }
        public Func<DbConnection> ConnectionFactory { get; protected set; }

        protected BaseFixture(string filename, params string[] filenamesToDelete) {
            dbFilePath = Combine(BaseDirectory, filename);
            pathsToDelete = filenamesToDelete.Select(x => Combine(BaseDirectory, x)).ToArray();
            Cleanup();
        }

        public void Dispose() {
            Connection?.Dispose();
            Cleanup();
        }

        private void Cleanup() {
            if (Exists(dbFilePath)) { Delete(dbFilePath); }
            foreach (var path in pathsToDelete) {
                Delete(path);
            }
        }
    }
}

// TODO add connection field to BaseFixture; set in inheriting constructors; inheriting constructors need to take a bool sharedConnection argument