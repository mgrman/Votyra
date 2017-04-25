using System;
using System.Collections.Generic;
using System.Threading;
using TycoonTerrain.Common.Logging;
using TycoonTerrain.TerrainMeshers.TriangleMesh;

namespace TycoonTerrain.TerrainGenerators
{
    public class AsyncTerrainGenerator<T> : ITerrainGenerator, IDisposable
        where T : TerrainGenerator, new()
    {
        private readonly ILogger _logger = LoggerFactory.Create<AsyncTerrainGenerator<T>>();

        private readonly T _terrainGenerator;
        private object _threadAccessLock = new object();
        private Thread _thread;
        private bool _stop;

        private DateTime _lastResult = DateTime.Now;

        private TerrainOptions _optionsToComputeMesh = null;
        private TerrainOptions _optionsToComputeTiles = null;
        private bool _lastJobFinished = true;
        private IList<ITriangleMesh> _lastComputedTriangleMesh = null;
        private IList<ITerrainGroup> _lastComputedTiles = null;

        private TerrainOptions _old_options;

        public AsyncTerrainGenerator()
        {
            _terrainGenerator = new T();

            _thread = new Thread(Compute);
            _thread.Start();
        }

        public IList<ITriangleMesh> GenerateMesh(TerrainOptions options)
        {
            if (!options.IsValid)
            {
                return null;
            }
            else if (_old_options != null && !options.IsChanged(_old_options))
            {
                IList<ITriangleMesh> computedTriangleMesh;
                lock (_threadAccessLock)
                {
                    if (this._lastJobFinished)
                    {
                        computedTriangleMesh = this._lastComputedTriangleMesh;
                        this._lastComputedTriangleMesh = null;
                        this._lastJobFinished = false;
                    }
                    else
                    {
                        computedTriangleMesh = null;
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

                IList<ITriangleMesh> computedTriangleMesh;
                lock (_threadAccessLock)
                {
                    if (this._lastJobFinished)
                    {
                        computedTriangleMesh = this._lastComputedTriangleMesh;
                        this._lastComputedTriangleMesh = null;
                        this._lastJobFinished = false;

                        this._optionsToComputeMesh = options;
                    }
                    else
                    {
                        computedTriangleMesh = null;
                    }
                }
                return computedTriangleMesh;
            }
        }

        public IList<ITerrainGroup> GenerateTiles(TerrainOptions options)
        {
            if (!options.IsValid)
            {
                return null;
            }
            else if (_old_options != null && !options.IsChanged(_old_options))
            {
                IList<ITerrainGroup> computedTiles;
                lock (_threadAccessLock)
                {
                    if (this._lastJobFinished)
                    {
                        computedTiles = this._lastComputedTiles;
                        this._lastComputedTiles = null;
                        this._lastJobFinished = false;
                    }
                    else
                    {
                        computedTiles = null;
                    }
                }
                return computedTiles;
            }
            else
            {
                if (_old_options != null)
                {
                    _old_options.Dispose();
                }
                _old_options = options.Clone();

                IList<ITerrainGroup> computedTiles;
                lock (_threadAccessLock)
                {
                    if (this._lastJobFinished)
                    {
                        computedTiles = this._lastComputedTiles;
                        this._lastComputedTiles = null;
                        this._lastJobFinished = false;

                        this._optionsToComputeTiles = options;
                    }
                    else
                    {
                        computedTiles = null;
                    }
                }
                return computedTiles;
            }
        }

        private void Compute()
        {
            int counter = 0;
            while (!_stop)
            {
                if (_optionsToComputeMesh != null)
                {
                    TerrainOptions optionsToCompute;
                    lock (_threadAccessLock)
                    {
                        optionsToCompute = _optionsToComputeMesh;
                        _optionsToComputeMesh = null;
                    }

                    var computedTriangleMesh = _terrainGenerator.GenerateMesh(optionsToCompute);

                    if (computedTriangleMesh != null)
                    {
                        //TODO zistit efekt ked sa mesh clonuje vs ked nie!
                        //computedTriangleMesh = computedTriangleMesh.Select(o => o.Clone()).ToArray();
                        //computedTriangleMesh = computedTriangleMesh.ToArray();
                    }

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
                else if (_optionsToComputeTiles != null)
                {
                    TerrainOptions optionsToCompute;
                    lock (_threadAccessLock)
                    {
                        optionsToCompute = _optionsToComputeTiles;
                        _optionsToComputeTiles = null;
                    }

                    var computedTiles = _terrainGenerator.GenerateTiles(optionsToCompute);

                    if (computedTiles != null)
                    {
                        //TODO zistit efekt ked sa mesh clonuje vs ked nie!
                        //computedTriangleMesh = computedTriangleMesh.Select(o => o.Clone()).ToArray();
                        //computedTriangleMesh = computedTriangleMesh.ToArray();
                    }

                    lock (_threadAccessLock)
                    {
                        this._lastJobFinished = true;
                        this._lastComputedTiles = computedTiles;
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