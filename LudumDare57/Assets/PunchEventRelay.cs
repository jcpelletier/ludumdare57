using UnityEngine;

public class PunchEventRelay : MonoBehaviour
{
    public MeleeAttack meleeAttack;

    // This gets called by the animation event
    public void Punched()
    {
        if (meleeAttack != null)
        {
            meleeAttack.Punched();
        }
    }
}
