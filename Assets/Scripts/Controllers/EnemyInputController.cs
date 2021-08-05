using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInputController : BaseInputController
{

    private Vector3 _movementDirection = Vector3.zero;
    private bool _hasGravity = true;

    public virtual void UpdateEnemy()
    {

        //HandleMovement();

        ApplyGravityAndForce();

    }

    protected override void HandleMovement()
    {
        Vector3 move = _movementDirection;

        _controller.Move(move * Time.deltaTime * CurrentSpeed);

        //if (move != Vector3.zero)
        //    transform.forward = move;
    }

    protected override void ApplyGravityAndForce()
    {
        if (_hasGravity)
        {
            base.ApplyGravityAndForce();
        }
    }

    public void SetMovementDirection(Vector3 direction)
    {
        _movementDirection = direction;
    }

    public void SetHasGravity(bool HasGravity)
    {
        _hasGravity = HasGravity;
    }
}
