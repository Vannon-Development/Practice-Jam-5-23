using UnityEngine;

[AddComponentMenu("Escape Hammer/Detector")]
[RequireComponent(typeof(Collider2D))]
public class Detector : MonoBehaviour
{
    private bool _blocked;

    private void OnTriggerEnter2D(Collider2D other)
    {
        _blocked = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _blocked = false;
    }

    public bool IsBlocked => _blocked;
}
