using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ACritter : MonoBehaviour
{
    // methods to overwrite
    public abstract void notice(ACritter other);

    public abstract void forget(ACritter other);

    public virtual string critterID()
    {
        return gameObject.name;
    }

    public abstract void takeHit(float damage, Vector2 knockbackDirection, float stun);

    public abstract void touchOther(ACritter other);
}
