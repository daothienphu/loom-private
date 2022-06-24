using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform _player;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Vector3.Slerp(this.transform.position, new Vector3(_player.transform.position.x, _player.transform.position.y, this.transform.position.z), 0.3f);
    }
}
