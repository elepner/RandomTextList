using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using RandomTextList.Models;

namespace RandomTextList.Code
{
    public class DatabaseWriter<T> where T : class
    {
        private const int PERIOD = 1000;
        private readonly IDatagenerator<T> _datagenerator;
        private readonly IDBContextFactory _dbContextFactory;
        private Timer _timer;

        public DatabaseWriter(IDatagenerator<T> datagenerator, IDBContextFactory dbContextFactory)
        {
            _datagenerator = datagenerator;
            _dbContextFactory = dbContextFactory;
            RecordsPerSecond = 150;
            using (var dbContext = _dbContextFactory.Create())
            {
                _recordsCount = dbContext.Set<T>().Count();
            }
        }

        public int RecordsPerSecond { get; set; }
        private int _recordsCount;
        private float _throttling;


        public int RecordsCount
        {
            get { return _recordsCount; }
            set
            {
                if(_timer!=null) return;
                _recordsCount = value;
            }
        }

        public WriterStatus WriterStatus
        {
            get
            {
                bool running = _timer != null;
                return new WriterStatus()
                {
                    RecordsCount = _recordsCount,
                    Throttling = running ? _throttling : 0,
                    Running = running,
                    RecordsPerSecond = RecordsPerSecond
                };
            }
        }
        

        public void StartDataGeneration()
        {
            if (_timer != null) return;

            bool writingData = false;
            _timer = new Timer(state =>
            {
                if(writingData) return;
                var sw = new Stopwatch();
                try
                {
                    sw.Reset();
                    sw.Start();
                    using (var dbContext = _dbContextFactory.Create())
                    {
                        var set = dbContext.Set<T>();
                        set.AddRange(_datagenerator.GetRecords(RecordsPerSecond));
                        _recordsCount = set.Count();
                        dbContext.SaveChanges();
                    }    
                    sw.Stop();
                    _throttling = (float)sw.ElapsedMilliseconds / PERIOD;
                } finally
                {
                    writingData = false;
                }
            }, null, 0, PERIOD);
        }

        public void StopDataGeneration()
        {
            if(_timer == null) return;
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timer.Dispose();
            _timer = null;
        }
    }

    public interface IDBContextFactory
    {
        DbContext Create();
    }

    /// <summary>
    /// The interface which <see cref="DatabaseWriter"/> uses when generates necessary data.
    /// </summary>
    public interface IDatagenerator<T>
    {
        T[] GetRecords(int count);
    }

    public class WriterStatus
    {
        public bool Running { get; set; }
        public float Throttling { get; set; }
        public int RecordsCount { get; set; }
        public int RecordsPerSecond { get; set; }
    }

    public class RandomRecordsGenerator : IDatagenerator<Record>
    {
        private static readonly Random Random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789    ";

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public Record[] GetRecords(int count)
        {

            return Enumerable.Range(0, count)
                .Select(idx => new Record
                {
                    Header = RandomString(Random.Next(4, 8)),
                    Text = RandomString(Random.Next(50, 1000))
                }).ToArray();

        }
    }
}