using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShield : MonoBehaviour
{

    public float Power;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public virtual float TakeDamage(float damageTaken)
    {
        if (Power >= damageTaken)
        {
            gameObject.SetActive(true);
            StartCoroutine(DamageTaken());

            Power -= damageTaken;
            return 0f;
        }
        else
        {
            damageTaken -= Power;
            Power = 0f;
            return damageTaken;
        }
    }

    private IEnumerator DamageTaken()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }

}
