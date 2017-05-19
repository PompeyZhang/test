using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResManager : Singleton<ResManager>
{
    private Dictionary<string, AssetInfo> dicAssetInfo = null;
    public override void Init()
    {
        dicAssetInfo = new Dictionary<string, AssetInfo>();
    }

    #region Load Resources & Instantiate Object  
    //加载并生成对象 同步 协程 异步
    public Object LoadInstance(string _path)
    {
        Object _obj = Load(_path);
        return Instantiate(_obj);
    }

    public void LoadCoroutineInstance(string _path, Action<Object> _loaded = null)
    {
        LoadCoroutine(_path, (_obj) =>
        {
            Instantiate(_obj, _loaded);
        });
    }

    public void LoadAsyncInstance(string _path, Action<Object> _loaded = null, Action<float> _progress = null)
    {
        LoadAsync(_path, _obj =>
        {
            Instantiate(_obj, _loaded);
        }, _progress);
    }
    #endregion

    #region Load Resources
    //加载不生成对象
    public UnityEngine.Object Load(string _path)
    {
        AssetInfo _assetInfo = GetAssetInfo(_path);
        if (null != _assetInfo)
        {
            return _assetInfo.AssetObject;
        }
        return null;
    }
    #endregion

    #region Load Coroutine Resources

    public void LoadCoroutine(string _path, Action<Object> _loaded = null)
    {
        AssetInfo _assetInfo = GetAssetInfo(_path, _loaded);
        if (null != _assetInfo)
        {
            GlobalCoroutine.Start(_assetInfo.GetCoroutineObject(_loaded));
        }
    }
    #endregion

    #region Load Async Resources

    public void LoadAsync(string _path, Action<Object> _loaded = null, Action<float> _progress = null)
    {
        AssetInfo _assetInfo = GetAssetInfo(_path, _loaded);
        if (null != _assetInfo)
        {
            GlobalCoroutine.Start(_assetInfo.GetAsyncObject(_loaded, _progress));
        }
    }
    #endregion

    #region GetAssetInfo & Instantiate Object

    private AssetInfo GetAssetInfo(string _path, Action<Object> _loaded = null)
    {
        if (string.IsNullOrEmpty(_path))
        {
            Debug.LogError("null _path");
            if (null != _loaded)
                _loaded(null);
        }

        AssetInfo _assetInfo = null;
        if (!dicAssetInfo.TryGetValue(_path, out _assetInfo))
        {
            _assetInfo = new AssetInfo();
            _assetInfo.path = _path;
            dicAssetInfo.Add(_path, _assetInfo);
        }
        _assetInfo.refCount++;
        return _assetInfo;
    }

    private Object Instantiate(Object _obj, Action<Object> _loaded = null)
    {
        Object _retObj = null;
        if (null != _obj)
        {
            _retObj = MonoBehaviour.Instantiate(_obj);
            if (null != _retObj)
            {
                if (null != _loaded)
                {
                    _loaded(_retObj);
                }
                return _retObj;
            }
            else
            {
                Debug.LogError("Error: null Instantiate _retObj");
            }
        }
        else
        {
            Debug.LogError("Error: null Resources Load return _obj.");
        }
        return null;
    }
    #endregion

    void Destroy()
    {
        Resources.UnloadUnusedAssets();//删除所有未引用的obj
        GC.Collect();
    }
}

public class AssetInfo
{
    private Object _object;

    public Type assetType { get; set; }
    public string path { get; set; }
    public int refCount { get; set; }

    public bool IsLoaded
    {
        get
        {
            return null != _object;
        }
    }

    public Object AssetObject
    {
        get
        {
            if (null == _object)
            {
                _ResourcesLoad();
            }

            return _object;
        }
    }

    private void _ResourcesLoad()
    {
        try
        {
            _object = Resources.Load(path);
            if (!_object)
                Debug.Log("Resources load failue:" + path);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    public IEnumerator GetCoroutineObject(Action<Object> _loaded)
    {
        while (true)
        {
            yield return null;
            if (!_object)
            {
                _ResourcesLoad();
                yield return null;
            }
            else
            {
                if (_loaded != null)
                    _loaded(_object);
            }

            yield break;
        }
    }

    public IEnumerator GetAsyncObject(Action<Object> _loaded, Action<float> _progress = null)
    {
        if (null != _object)
        {
            _loaded(_object);
            yield break;
        }

        ResourceRequest _resRequest = Resources.LoadAsync(path);

        while (_resRequest.isDone)
        {
            if (_progress != null)
            {
                _progress(_resRequest.progress);
                yield return null;
            }
        }

        _object = _resRequest.asset;
        if (null != _loaded)
        {
            _loaded(_object);
        }

        yield return _resRequest;
    }
}

