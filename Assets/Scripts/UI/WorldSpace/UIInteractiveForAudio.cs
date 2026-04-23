using UnityEngine;

public class UIInteractiveForAudio : UIInteractiveWorldSpace, IHittable
{
    public VolumeSetting volumeSetting;

    protected void Start()
    {
        base.Start();
    }

    public override void OnGetHit(PlayerController hitter, HitType hitType)
    {

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
