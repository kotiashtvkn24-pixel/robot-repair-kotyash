using System.Collections;
using UnityEngine;

public class TimedSwitch : MonoBehaviour
{
    public GameObject door;
    public float openTime = 4.0f;
    public Color pressedColor = new Color(0.4f, 1f, 0.4f);

    SpriteRenderer plate;
    Color normalColor;

    void Start()
    {
        plate = GetComponent<SpriteRenderer>();
        if (plate != null)
        {
            normalColor = plate.color;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null)
        {
            return;
        }

        StopAllCoroutines();
        StartCoroutine(OpenDoorFor(openTime));
    }

    IEnumerator OpenDoorFor(float seconds)
    {
        door.SetActive(false);
        if (plate != null)
        {
            plate.color = pressedColor;
        }

        yield return new WaitForSeconds(seconds);

        door.SetActive(true);
        if (plate != null)
        {
            plate.color = normalColor;
        }
    }
}
