using BenchmarkDotNet.Attributes;
using Susanoo;
using Susanoo.Processing;
using System.Data;
using System.Linq;

namespace Dapper.Tests.Performance
{
    public class SusanooBenchmarks : BenchmarkBase
    {
        private DatabaseManager _db;
        private static readonly ISingleResultSetCommandProcessor<dynamic, Post> _cmd =
                CommandManager.Instance.DefineCommand("SELECT * FROM Posts WHERE Id = @Id", CommandType.Text)
                    .DefineResults<Post>()
                    .Realize();
        private static readonly ISingleResultSetCommandProcessor<dynamic, dynamic> _cmdDynamic =
                CommandManager.Instance.DefineCommand("SELECT * FROM Posts WHERE Id = @Id", CommandType.Text)
                    .DefineResults<dynamic>()
                    .Realize();

        [Setup]
        public void Setup()
        {
            BaseSetup();
            _db = new DatabaseManager(_connection);
        }

        [Benchmark(Description = "Mapping Cache", OperationsPerInvoke = Iterations)]
        public Post MappingCache()
        {
            Step();
            return CommandManager.Instance.DefineCommand("SELECT * FROM Posts WHERE Id = @Id", CommandType.Text)
                    .DefineResults<Post>()
                    .Realize()
                    .Execute(_db, new { Id = i }).First();
        }

        [Benchmark(Description = "Mapping Cache (dynamic)", OperationsPerInvoke = Iterations)]
        public dynamic MappingCacheDynamic()
        {
            Step();
            return CommandManager.Instance.DefineCommand("SELECT * FROM Posts WHERE Id = @Id", CommandType.Text)
                    .DefineResults<dynamic>()
                    .Realize()
                    .Execute(_db, new { Id = i }).First();
        }

        [Benchmark(Description = "Mapping Static", OperationsPerInvoke = Iterations)]
        public Post MappingStatic()
        {
            Step();
            return _cmd.Execute(_db, new { Id = i }).First();
        }

        [Benchmark(Description = "Mapping Static (dynamic)", OperationsPerInvoke = Iterations)]
        public dynamic MappingStaticDynamic()
        {
            Step();
            return _cmdDynamic.Execute(_db, new { Id = i }).First();
        }
    }
}