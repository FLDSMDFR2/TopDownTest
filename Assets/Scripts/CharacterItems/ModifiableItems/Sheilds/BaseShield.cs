using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShield : ModifiableItem
{
    #region Item Init
    protected override void PerformStart()
    {
        base.PerformStart();

        gameObject.SetActive(false);
    }
    #endregion

    #region Class Logic
    public virtual void ActivateShield()
    {
        gameObject.SetActive(true);

        StartCoroutine(VisualizeSheild());
    }

    private IEnumerator VisualizeSheild()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }
    #endregion
}
