
using UnityEngine;
using System.Collections;

public class GameAgent : MonoBehaviour
{
    void Awake()
    {
        GlobalCoroutine.Start = StartCoroutine;
        GlobalCoroutine.StartMethod = StartCoroutine;
        GlobalCoroutine.StartAuto = StartCoroutine;
        GlobalCoroutine.Stop = StopCoroutine;
        GlobalCoroutine.StopMethod = StopCoroutine;
        GlobalCoroutine.StopAll = StopAllCoroutines;

        // avoid null reference | add by liao
        InternalResource tempInternalResource = GetComponent<InternalResource>();
        if (tempInternalResource == null) 
        {
            tempInternalResource = this.gameObject.AddComponent<InternalResource>();
        }
        ResourcesDatabase.Internal = tempInternalResource;

        // avoid null reference | add by liao
        GameDefault tempGameDefault = GetComponent<GameDefault>();
        if (tempGameDefault == null) 
        {
            tempGameDefault = this.gameObject.AddComponent<GameDefault>();
        }
        GameWorld.Default = tempGameDefault;

        GameWorld.gameWorld = gameObject;
        GameWorld.Awake();

    }

    void Start()
    {
        GameWorld.Start();
    }

    void OnDestroy()
    {
        GameWorld.Exit();
    }

    void Update()
    {
        GameWorld.Update();
    }

    void LateUpdate()
    {
        GameWorld.LateUpdate();
    }

    void FixedUpdate()
    {
        GameWorld.FixedUpdate();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        GameWorld.OnApplicationPause(pauseStatus);
    }

    void OnApplicationFocus(bool focusStatus)
    {
        GameWorld.OnApplicationFocus(focusStatus);
    }

    public void SaySendMessage(string str)
    {
		DebugUtility.Log("==================SaySendMessage:" + str);
        GameEventCenter.Send(GameEvent.say_send_message, str);
    }
}
