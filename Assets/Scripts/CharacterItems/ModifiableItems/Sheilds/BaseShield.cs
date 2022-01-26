using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShield : ModifiableItem
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public virtual void ActivateShield()
    {
        gameObject.SetActive(true);

        StartCoroutine(DamageTaken());
    }

    private IEnumerator DamageTaken()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }
}
