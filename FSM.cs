using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    #region callback delegate

    public delegate void FSMTranslationCallFunc();
    public delegate void OnEnterState();
    public delegate void OnExitState();

    #endregion

    public class FSMState
    {
        public string stateName;
        public OnEnterState onEnter;
        public OnExitState onExit;

        public FSMState(string _stateName, OnEnterState _enterCallBackFunc = null, OnExitState _exitCallBackFunc = null)
        {
            stateName = _stateName;
            onEnter = _enterCallBackFunc;
            onExit = _exitCallBackFunc;
        }

        public Dictionary<string, FSMTranslation> TranslationDict = new Dictionary<string, FSMTranslation>();
    }

    public class FSMTranslation
    {
        public FSMState fromState;
        public string name;

        public FSMState toState;
        public FSMTranslationCallFunc callFunc;

        public FSMTranslation(FSMState _fromState, string _name, FSMState _toState, FSMTranslationCallFunc _callFun)
        {
            name = _name;
            fromState = _fromState;
            toState = _toState;
            callFunc = _callFun;
        }
    }

    public FSMState curState;

    protected Dictionary<string, FSMState> stateDict = new Dictionary<string, FSMState>();

    public bool CheckStateByName(string stateName)
    {
        return stateDict.ContainsKey(stateName);
    }
    public void AddState(FSMState _state)
    {
        if (!stateDict.ContainsKey(_state.stateName))
        {
            stateDict.Add(_state.stateName, _state);
        }
        else
        {
            stateDict[_state.stateName] = _state;
        }
    }

    public void AddTranslation(FSMTranslation translation)
    {
        if (CheckStateByName(translation.fromState.stateName))
        {
            stateDict[translation.fromState.stateName].TranslationDict[translation.name] = translation;
        }
    }

    public void Start(FSMState _state)
    {
        curState = _state;
        curState.onEnter();
    }

    public void HandlerEvent(string name)
    {
        if (curState != null && curState.TranslationDict.ContainsKey(name))
        {
            if (curState.onEnter != null)
            {
                curState.onExit();
            }

            curState.TranslationDict[name].callFunc();
            curState = curState.TranslationDict[name].toState;

            if (curState.onEnter != null)
            {
                curState.onEnter();
            }
        }
    }
}
