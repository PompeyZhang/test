
//////////////////////////////////////////////////////////////////////////
//
//   FileName : GameDefault.cs
//     Author : Chiyer
// CreateTime : 2014-07-17
//       Desc :
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class GameDefault : MonoBehaviour
{
    public Object creature;
    public Object gun;
    public string emptyScenePath;
    public string resourceServerURL;
    public bool enableLog;
    public bool enableLogWarning;
    public bool enableLogError;
    public bool enableLogRes;
	public bool enableLogOverhead;
	public bool enableRecordGateway;
	public bool enableRecordBattle;
    public string resourceBreakLoad;
    public Object UguiCanvas;

    GameDefault()
    {
        emptyScenePath = "@empty.unity";
        resourceServerURL = "http://192.168.1.210/game-x";
        enableLog = true;
        enableLogWarning = true;
        enableLogError = true;
        enableLogRes = false;
    }

    void Awake()
    {
        XDataGameDefault baseType = GetComponent<XDataGameDefault>();
        if (baseType != null)
        {
            this.enabled = baseType.enabled;
            creature = baseType.creature;
            gun = baseType.gun;
            emptyScenePath = baseType.emptyScenePath;
            resourceServerURL = baseType.resourceServerURL;

            enableLog = baseType.enableLog;
            enableLogWarning = baseType.enableLogWarning;
            enableLogError = baseType.enableLogError;
            enableLogRes = baseType.enableLogRes;
            enableLogOverhead = baseType.enableLogOverhead;
            resourceBreakLoad = baseType.resourceBreakLoad;

			enableRecordGateway = baseType.enableRecordGateway;
			enableRecordBattle = baseType.enableRecordBattle;
            UguiCanvas = baseType.UguiCanvas;
        }
    }
}
