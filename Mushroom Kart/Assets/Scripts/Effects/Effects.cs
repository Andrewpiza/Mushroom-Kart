using UnityEngine;

public enum Effect
{
    Star
}

public class Effects
{
    private Effect effect;
    private float time;
    public Effects(Effect e, float t)
    {
        effect = e;
        time = t;
    }

    public void EffectTick()
    {
        
    }
}
