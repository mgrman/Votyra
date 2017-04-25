using System;
using System.Threading;
using TycoonTerrain.Common.Logging;

namespace TycoonTerrain.TerrainGenerators
{
    public class AsyncGenerator<TGenerator, TOptions, TResult> : IGenerator<TOptions, TResult>, IDisposable
        where TGenerator : IGenerator<TOptions, TResult>, new()
        where TOptions: class,IOptions<TOptions>
    {
        private readonly ILogger _logger = LoggerFactory.Create<AsyncGenerator<TGenerator, TOptions, TResult>>();

        private readonly TGenerator _terrainGenerator;
        private object _threadAccessLock = new object();
        private Thread _thread;
        private bool _stop;

        private DateTime _lastResult = DateTime.Now;

        private TOptions _optionsToComputeMesh = null;
        private bool _lastJobFinished = true;
        private TResult _lastComputedTriangleMesh = default(TResult);

        private TOptions _old_options;

        public AsyncGenerator()
        {
            _terrainGenerator = new TGenerator();

            _thread = new Thread(Compute);
            _thread.Start();
        }

        public TResult Generate(TOptions options)
        {
            if (!options.IsValid)
            {
                return default(TResult);
            }
            else if (_old_options != null && !options.IsChanged(_old_options))
            {
                TResult computedTriangleMesh;
                lock (_threadAccessLock)
                {
                    if (this._lastJobFinished)
                    {
                        computedTriangleMesh = this._lastComputedTriangleMesh;
                        this._lastComputedTriangleMesh = default(TResult);
                        this._lastJobFinished = false;
                    }
                    else
                    {
                        computedTriangleMesh = default(TResult);
                    }
                }
                return computedTriangleMesh;
            }
            else
            {
                if (_old_options != null)
                {
                    _old_options.Dispose();
                }
                _old_options = options.Clone();

                TResult computedTriangleMesh;
                lock (_threadAccessLock)
                {
                    if (this._lastJobFinished)
                    {
                        computedTriangleMesh = this._lastComputedTriangleMesh;
                        this._lastComputedTriangleMesh = default(TResult);
                        this._lastJobFinished = false;

                        this._optionsToComputeMesh = options;
                    }
                    else
                    {
                        computedTriangleMesh = default(TResult);
                    }
                }
                return computedTriangleMesh;
            }
        }
        
        private void Compute()
        {
            int counter = 0;
            while (!_stop)
            {
                if (_optionsToComputeMesh != null)
                {
                    TOptions optionsToCompute;
                    lock (_threadAccessLock)
                    {
                        optionsToCompute = _optionsToComputeMesh;
                        _optionsToComputeMesh = null;
                    }

                    var computedTriangleMesh = _terrainGenerator.Generate(optionsToCompute);

                    //TODO zistit efekt ked sa mesh clonuje vs ked nie!
                    //if (computedTriangleMesh != null)
                    //{
                    //    //computedTriangleMesh = computedTriangleMesh.Select(o => o.Clone()).ToArray();
                    //    //computedTriangleMesh = computedTriangleMesh.ToArray();
                    //}

                    lock (_threadAccessLock)
                    {
                        this._lastJobFinished = true;
                        this._lastComputedTriangleMesh = computedTriangleMesh;
                    }

                    counter++;
                    if (counter % 10 == 0)
                    {
                        double freq = 1.0 / (DateTime.Now - _lastResult).TotalSeconds;
                        _logger.LogMessage(string.Format("Computation is at {0}fps", freq));
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
                _lastResult = DateTime.Now;
            }
        }

        public void Dispose()
        {
            if (_thread != null)
            {
                _stop = true;
                _thread = null;
            }
        }
    }
}