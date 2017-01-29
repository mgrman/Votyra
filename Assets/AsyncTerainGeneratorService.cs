using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;

public class AsyncTerainGeneratorService : ITerainGeneratorService
{
    private readonly TerainGeneratorService _service;
    private object _threadAccessLock = new object();
    private Thread _thread;
    private bool _stop;

    private DateTime _lastResult = DateTime.Now;

    private TerainGeneratorOptions _optionsToCompute = null;
    private bool _lastJobFinished = true;
    private IDictionary<Vector2i, ITriangleMesh> _lastComputedTriangleMesh = null;

    public AsyncTerainGeneratorService()
    {
        _service = new TerainGeneratorService();

        _thread = new Thread(Compute);
        _thread.Start();
    }

    public IDictionary<Vector2i, ITriangleMesh> Sample(TerainGeneratorOptions options)
    {
        IDictionary<Vector2i, ITriangleMesh> computedTriangleMesh;
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

    private void Compute()
    {
        int counter = 0;
        while (!_stop)
        {
            if (_optionsToCompute != null)
            {
                TerainGeneratorOptions optionsToCompute;
                lock (_threadAccessLock)
                {
                    optionsToCompute = _optionsToCompute;
                    _optionsToCompute = null;
                }

                IDictionary<Vector2i, ITriangleMesh> computedTriangleMesh = _service.Sample(optionsToCompute);

                if (computedTriangleMesh != null)
                {
                    //computedTriangleMesh = computedTriangleMesh.Select(o => o.Clone()).ToArray();
                }

                lock (_threadAccessLock)
                {
                    this._lastJobFinished = true;
                    this._lastComputedTriangleMesh = computedTriangleMesh;
                }


                counter++;
                if (counter % 100 == 0)
                {
                    double freq = 1.0 / (DateTime.Now - _lastResult).TotalSeconds;
                    Debug.Log(string.Format("Computation took {0}hz", freq));
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
