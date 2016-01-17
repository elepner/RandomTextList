using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace RandomTextList.Code
{
    /// <summary>
    /// Class which writes chunks of data provided by <see cref="IDatagenerator{T}"/>to the database. 
    /// </summary>
    /// <typeparam name="T">The type of objects to be filled to the database.</typeparam>
    public class DatabaseWriter<T> where T : class
    {
        private const int PERIOD = 1000;
        private readonly IDatagenerator<T> _datagenerator;
        private readonly IDBContextFactory _dbContextFactory;
        private Timer _timer;

        /// <summary>
        /// Constructor which creates new DatabaseWriter instance.
        /// </summary>
        /// <param name="datagenerator">Instance of generator.</param>
        /// <param name="dbContextFactory">The factory object which creates <see cref="DbContext"/></param>
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

        /// <summary>
        /// Target number of records for writing to the database.
        /// </summary>
        public int RecordsPerSecond { get; set; }
        private int _recordsCount;
        private float _throttling;

        /// <summary>
        /// Number of records in the database. Set value will not take affect if DatabaseWriter is running.
        /// </summary>
        public int RecordsCount
        {
            get { return _recordsCount; }
            set
            {
                if(_timer!=null) return;
                _recordsCount = value;
            }
        }

        /// <summary>
        /// Returns status object for the DatabaseWriter instance.
        /// </summary>
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
        
        /// <summary>
        /// Starts writing data to the database.
        /// </summary>
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
                        dbContext.SaveChanges();
                        _recordsCount = set.Count();
                    }    
                    sw.Stop();
                    _throttling = (float)sw.ElapsedMilliseconds / PERIOD;
                } finally
                {
                    writingData = false;
                }
            }, null, 0, PERIOD);
        }

        /// <summary>
        /// Stops writing data to the database.
        /// </summary>
        public void StopDataGeneration()
        {
            if(_timer == null) return;
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timer.Dispose();
            _timer = null;
        }
    }

    /// <summary>
    /// Factory interface for creating DbContext.
    /// </summary>
    public interface IDBContextFactory
    {
        DbContext Create();
    }

    /// <summary>
    /// The interface which <see cref="DatabaseWriter{T}"/> uses when generates necessary data.
    /// </summary>
    public interface IDatagenerator<T>
    {
        T[] GetRecords(int count);
    }

    /// <summary>
    /// Composite object describing the status of data writer.
    /// </summary>
    public class WriterStatus
    {
        /// <summary>
        /// Indicates whether the DatabaseWriter is running.
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// Indicates the loading of database. Indicates the time taken by a tick of dabase data filling.
        /// </summary>
        public float Throttling { get; set; }

        /// <summary>
        /// Number of records in the database.
        /// </summary>
        public int RecordsCount { get; set; }

        /// <summary>
        /// Desried records writing speed.
        /// </summary>
        public int RecordsPerSecond { get; set; }
    }
}