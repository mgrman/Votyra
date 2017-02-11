using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;

public class AsyncTerrainGenerator<T> : ITerrainGenerator,IDisposable
    where T:TerrainGenerator,new()
{
    private readonly T _terrainGenerator;
    private object _threadAccessLock = new object();
    private Thread _thread;
    private bool _stop;

    private DateTime _lastResult = DateTime.Now;

    private TerrainOptions _optionsToCompute = null;
    private bool _lastJobFinished = true;
    private IList<ITriangleMesh> _lastComputedTriangleMesh = null;

    private TerrainOptions _old_options;

    public AsyncTerrainGenerator()
    {
        _terrainGenerator = new T();

        _thread = new Thread(Compute);
        _thread.Start();
    }

    public IList<ITriangleMesh> Generate(TerrainOptions options)
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

                    this._optionsToCompute = options;
                }
                else
                {
                    computedTriangleMesh = null;
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
            if (_optionsToCompute != null)
            {
                TerrainOptions optionsToCompute;
                lock (_threadAccessLock)
                {
                    optionsToCompute = _optionsToCompute;
                    _optionsToCompute = null;
                }

                var computedTriangleMesh = _terrainGenerator.Generate(optionsToCompute);

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
                    Debug.Log(string.Format("Computation is at {0}fps", freq));
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
