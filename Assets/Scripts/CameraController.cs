using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private Vector2 _xLimit = new Vector2(-20f, 20f);
    [SerializeField] private Vector2 _yLimit = new Vector2(-20f, 20f);

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x + h * _speed * Time.deltaTime, _xLimit.x, _xLimit.y);
        pos.y = Mathf.Clamp(pos.y + v * _speed * Time.deltaTime, _yLimit.x, _yLimit.y);
        transform.position = pos;
    }
}
