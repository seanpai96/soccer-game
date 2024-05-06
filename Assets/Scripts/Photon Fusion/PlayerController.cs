using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    private new Rigidbody rigidbody;

    [SerializeField]
    private float moveSpeed = 15f;

    [SerializeField]
    private int maxHp = 100;

    [SerializeField]
    private bool alive = true;

    [SerializeField]
    private Image hpBar = null;

    [SerializeField]
    private int collideDamage = 2;

    [Networked(OnChanged = nameof(OnHpChanged))]
    public int Hp { get; set; }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
            Hp = maxHp;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data) && alive)
        {
            Vector3 moveVector = data.movementInput.normalized;
            rigidbody.MovePosition(rigidbody.position + moveVector * Runner.DeltaTime);
        }

        if (Hp <= 0)
        {
            PlayerDead();
        }
    }

    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)
            Hp -= damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Untagged")
            Debug.Log(other.tag);
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            TakeDamage(collideDamage);
            player.TakeDamage(collideDamage);
        }
    }

    private void PlayerDead()
    {
        alive = false;
    }

    private static void OnHpChanged(Changed<PlayerController> changed)
    {
        changed.Behaviour.hpBar.fillAmount = (float)changed.Behaviour.Hp / changed.Behaviour.maxHp;
    }
}