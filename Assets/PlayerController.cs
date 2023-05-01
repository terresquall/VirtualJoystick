using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Terresquall;

public class PlayerController : MonoBehaviour {

    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(
            VirtualJoystick.GetAxis("Horizontal"),
            VirtualJoystick.GetAxis("Vertical")
        ) * Time.deltaTime;
    }

    void OnSwipeEnter2D(Touch touch) {
        Debug.Log("Swiped Enter");
        sr.color = Color.green;
    }

    void OnSwipeEnter(Touch touch) {
        Debug.Log("Swiped Enter");
        sr.color = Color.green;
    }

    void OnSwipeExit(Touch touch) {
        Debug.Log("Swiped Exit");
        sr.color = Color.white;
    }

    void OnSwipeExit2D(Touch touch) {
        Debug.Log("Swiped Exit");
        sr.color = Color.white;
    }

    void OnSwipeStay(Touch touch) {
        Debug.Log("Swiped Stay");
        sr.color = Color.blue;
    }

    void OnSwipeStay2D(Touch touch) {
        Debug.Log("Swiped Stay");
        sr.color = Color.blue;
    }

    void OnTouchTap(Touch touch) {
        sr.color = Color.black;
    }

    void OnTouchTap2D(Touch touch) {
        sr.color = Color.black;
    }

    void OnTouchUntap(Touch touch) {
        sr.color = Color.magenta;
    }

    void OnTouchUntap2D(Touch touch) {
        sr.color = Color.magenta;
    }

    void OnTouchHold2D(Touch touch) {
        sr.color = Color.gray;
    }

    void OnTouchHold(Touch touch) {
        sr.color = Color.gray;
    }
}
