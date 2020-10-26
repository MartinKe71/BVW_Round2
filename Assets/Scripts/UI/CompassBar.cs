using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassBar : MonoBehaviour
{
    public float _maxDistance;
    public GameObject _iconPrefab;
    List<HumanMarker> humanMarkers = new List<HumanMarker>();

    public RawImage _compassImage;
    public GameObject _player;

    float _compassUnit;
    void Start()
    {
        
        _compassUnit = _compassImage.rectTransform.rect.width/360f;
    }

    void Awake()
    {
        _compassImage = gameObject.GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        _compassImage.uvRect = new Rect(_player.transform.localEulerAngles.y / 360f, 0f, 1f, 1f);
        foreach(HumanMarker marker in humanMarkers)
        {

            marker.image.rectTransform.anchoredPosition = GetPosOnCompass(marker);
            float dst = Vector2.Distance(
                new Vector2(_player.transform.position.x, _player.transform.position.z),
                marker.position);
            float scale = 0f;

            if (dst < _maxDistance)
            {
                scale = 1f - (dst / _maxDistance) * 0.8f;
                // SoundManager.instance.Narration(1);
            }
            marker.image.rectTransform.localScale = Vector3.one * scale;
        }
    }

    public void AddHumanMarker(HumanMarker marker)
    {
        Debug.Log("Adding marker");
        Debug.Log("iconPrefab: " + _iconPrefab.transform.name);
        Debug.Log("compass image: " + _compassImage.name);
        Debug.Log("compass image rect: " + _compassImage.rectTransform);

        GameObject newMarker = Instantiate(_iconPrefab, _compassImage.rectTransform);
        marker.image = newMarker.GetComponent<Image>();
        marker.image.sprite = marker.icon;

        humanMarkers.Add(marker);
    }

    public void DeleteHumanMarker(HumanMarker marker)
    {
        humanMarkers.Remove(marker);
        Destroy(marker.image.gameObject);
        marker.enabled = false;
    }

    Vector2 GetPosOnCompass(HumanMarker marker)
    {
        Vector2 playerPos = new Vector2(_player.transform.position.x, _player.transform.position.z);
        Vector2 playerForward = new Vector2(_player.transform.forward.x, _player.transform.forward.z);

        float angle = Vector2.SignedAngle(marker.position - playerPos, playerForward);

        return new Vector2(angle * _compassUnit, 0);
    }
}
