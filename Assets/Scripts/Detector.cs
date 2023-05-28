using System;
using UnityEngine;

[AddComponentMenu("Escape Hammer/Detector")]
[RequireComponent(typeof(Collider2D))]
public class Detector : MonoBehaviour
{
    private bool _blocked;
    private Collider2D _col;

    private void Start()
    {
        _col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _blocked = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(_col.OverlapCollider(new ContactFilter2D(), new Collider2D[] { null }) == 0)
            _blocked = false;
    }

    public bool IsBlocked => _blocked;
    public void ForceReset() => _blocked = false;
    public void ForceBlock() => _blocked = true;
}
