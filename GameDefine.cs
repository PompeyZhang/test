using UnityEngine;
using System.Collections.Generic;

// move Layers to GameDefine.cs | add by liao

public static class LayersMask
{
    public const int none = 0;
    public const int def = 1 << Layers.def;
    public const int transparentFx = 1 << Layers.transparentFx;
    public const int igoreRaycast = 1 << Layers.igoreRaycast;
    public const int water = 1 << Layers.water;
    public const int ui = 1 << Layers.ui;
    public const int midUI = 1 << Layers.midUI;
    public const int topUI = 1 << Layers.topUI;
    public const int sceneObject = 1 << Layers.sceneObject;
    public const int assist = 1 << Layers.assist;
    public const int collision = 1 << Layers.collision;
    public const int character = 1 << Layers.character;
    public const int monsterWidget = 1 << Layers.monsterWidget;
    public const int monster = 1 << Layers.monster;
    public const int hud = 1 << Layers.hud;
    public const int hurtNumber = 1 << Layers.hurtNumber;
    public const int weapon = 1 << Layers.weapon;
    public const int doodad = 1 << Layers.doodad;
    public const int ui3d = 1 << Layers.ui3d;
    public const int skybox = 1 << Layers.skybox;
    public const int joystick = 1 << Layers.joystick;
    public const int obstacle = 1 << Layers.obstacle;
    public const int dragbox = 1 << Layers.dragbox;
    public const int creature = LayersMask.character | LayersMask.monster | LayersMask.monsterWidget;
    public const int allUI = LayersMask.ui | LayersMask.midUI | LayersMask.topUI | LayersMask.ui3d;
    public const int all = 0x0fffffff;
    public const int UGUI = 1 << Layers.UGUI;
    public const int Grenade = 1 << Layers.Grenade;
    public static int GetLayerMask(int layer)
    {
        return 1 << layer;
    }
}
public delegate Coroutine PreLoadFunc();

public static class GameObjectRenderQueue
{
    public static int entity = RenderQueue.geometry + 2;
    public static int creature = GameObjectRenderQueue.entity;
    public static int character = RenderQueue.geometry + 1;
}

public static class MtlPropId
{
    private static int _color;

    static MtlPropId()
    {
        _color = Shader.PropertyToID("_Color");
    }

}

public enum MoveSpeed
{
    stop,
    move,       //正常移动
    dash,
    backdash,
    beHitMove,
    born,
    patrol,
    beForceTarget,  //受到嘲讽
}
public static class CommonSlot
{
    public const string bodyShoot = "slotBodyShoot";
    public const string bodyAttack = "slotBodyAttack";
    public const string bodyCenter = "slotBodyCenter";
    public const string bodyMidPart = "slotMidPart";
    public const string bodyTop = "slotBodyTop";
    public const string bodyFoot = "slotBodyFoot";
    public const string bodyWeapon = "slotWeapon";
    public const string bodyDash = "slotDash";
    public const string bodyJet = "slotBodyJet";
    public const string bodyFootLeft = "slotBodyFootLeft";
    public const string bodyFootRight = "slotBodyFootRight";
    public const string bodyReboundWarning = "slotReboundWarning";
    public const string bodyNeck = "slotBodyNeck";
}

[AnimatorEventTypeAttribute]
public static class CreatureAnimatorEvent
{
    public const string runFootstep = "creature/runFootstep";
    public const string walkFootstep = "creature/walkFootstep";
    public const string bornEnd = "creature/bornEnd";
}


[AnimatorEventTypeAttribute]
public static class MonsterAnimatorEvent
{
    public const string footLeftEffect = "monster/footLeftEffect";
    public const string footrightEffect = "monster/footrightEffect";
    public const string neckEffect = "monster/neckEffect";
    public const string specialEffect = "monster/specialEffect";
}

[AnimatorEventTypeAttribute]
public static class SoundAnimatorEvent
{
    public const string bornEndSound = "sound/bornEndSound";
    public const string bodyMoveSound = "sound/bodyMoveSound";
    public const string skillSound1 = "sound/skillSound1";
    public const string skillSound2 = "sound/skillSound2";
    public const string skillSound3 = "sound/skillSound3";
    public const string skillSound4 = "sound/skillSound4";
    public const string skillSound5 = "sound/skillSound5";
    public const string skillSound6 = "sound/skillSound6";
    public const string skillSound7 = "sound/skillSound7";
    public const string skillSound8 = "sound/skillSound8";
    public const string skillSound9 = "sound/skillSound9";
}

[AnimatorEventTypeAttribute]
public static class SkillAnimatorEvent
{
    public const string trigger = "skill/skillTrigger";
    public const string trigger2 = "skill/skillTrigger2";
    public const string trigger3 = "skill/skillTrigger3";
    public const string locktrigger = "skill/skillLockTrigger";
    public const string triggerMove = "skill/skillTriggerMove";
    public const string triggerMove2 = "skill/skillTriggerMove2";
    public const string triggerEffect = "skill/skillTriggerEffect";
    public const string triggerEffect2 = "skill/skillTriggerEffect2";
    public const string triggerEffect3 = "skill/skillTriggerEffect3";
    public const string skillPlayShaderAnimation = "skill/skillPlayShaderAnimation";
    public const string triggerGuide = "skill/triggerGuide";
    public const string triggerAddDoodad = "skill/triggerAddDoodad";
    public const string triggerRemoveDoodad = "skill/triggerRemoveDoodad";
    public const string triggerShowRangeDamage = "skill/triggerShowRangeDamage";
    public const string triggerRemoveShowRangeDamage = "skill/triggerRemoveShowRangeDamage";
    public const string triggerSelfShowRangeDamage = "skill/triggerSelfShowRangeDamage"; // 走自己控制的预警事件
    public const string triggerCounterAttackStartTime = "skill/triggerCounterAttackStartTime"; //给予玩家的防反开始时机
    public const string triggerCounterAttackEndTime = "skill/triggerCounterAttackEndTime";     //给予玩家的防反结束时机

}

[AnimatorEventTypeAttribute]
public static class GunAnimatorEvent
{
    public const string gunFire = "gun/gunFire";
    public const string gunFire1 = "gun/gunFire1";
    public const string putDownGun = "gun/putDownGun";
    public const string takeOutGun = "gun/takeOutGun";
    public const string snipeEnd = "gun/snipeEnd";
}

[AnimatorEventTypeAttribute]
public static class SwordAnimatorEvent
{
    public const string swordFire = "sword/swordFire";
    public const string muzzleFlash = "sword/muzzleFlash";
}

[AnimatorEventTypeAttribute]
public static class OtherAnimatorEvent
{
    public const string shakeScreenOneWay = "other/shakeScreenOneWay";
    public const string shakeScreenRandomCircle = "other/shakeScreenRandomCircle";
    public const string hideObject = "other/hideObject";
    public const string showObject = "other/showObject";
    public const string playSound = "other/playSound";
    public const string changeColliderRadius = "other/changeColliderRadius";
    public const string resetColliderRadius = "other/resetColliderRadius";
    public const string hideShadow = "other/hideshadow";
}

[AnimatorEventTypeAttribute]
public static class BornAnimatorEvent
{
    public const string trigger = "born/bornTrigger";
    public const string trigger2 = "born/bornTrigger2";
    public const string trigger3 = "born/bornTrigger3";
    public const string trigger4 = "born/bornTrigger4";
}
