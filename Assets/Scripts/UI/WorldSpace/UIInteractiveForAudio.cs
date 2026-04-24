using UnityEngine;

public class UIInteractiveForAudio : UIInteractiveWorldSpace, IHittable
{
    public VolumeSetting volumeSetting;

    public void Start()
    {
        base.Start();
    }

    public override void OnGetHit(PlayerController hitter, HitType hitType)
    {
        SoundManager.instance.PlaySound(SoundType.Object_Bounce);
        if (buttonType == ButtonWoldSpaceType.MusicSetting)
        {
            volumeSetting.SetMusicVolumeByPlayer(hitter.side);
        }
        else if (buttonType == ButtonWoldSpaceType.SoundEffectSetting)
        {
            volumeSetting.SetSFXVolumeByPlayer(hitter.side);
        }

    }

}
