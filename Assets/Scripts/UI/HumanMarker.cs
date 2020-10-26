using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Gender
{
    Male,
    Femaile
}
public class HumanMarker : MonoBehaviour
{
    public Gender _gender;
    public CompassBar compassBar;
    public Sprite icon;
    public Image image;
    public Transform head;
    public CharacterJoint joint;
    public GameObject heart;

    Animator animator;
    Rigidbody[] rbs;

    public Vector2 position
    {
        get { return new Vector2(transform.position.x, transform.position.z); }
    }

    void Start()
    {
        TryGetComponent<Animator>(out animator);
        rbs = GetComponentsInChildren<Rigidbody>();
        compassBar.AddHumanMarker(this);
    }

    public void OnDeath()
    {
        compassBar.DeleteHumanMarker(this);
        StartCoroutine(Ragdoll(true, 0f));
        ToBoat.instance.dead.Add(gameObject);
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().enabled = false;
        if (ToBoat.instance.humanProtected.Find(x => x == gameObject))
        {
            ToBoat.instance.humanProtected.Remove(gameObject);
        }
    }

    IEnumerator Ragdoll(bool state, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        if (animator != null) animator.enabled = false;
        gameObject.transform.rotation *= Quaternion.Euler(90, 0, 0);
        if (joint!=null)
        {
            Destroy(joint.gameObject);
        }
        Destroy(heart);
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
            rb.mass = 0f;
        }
        // if (!state) head.GetComponent<Rigidbody>().AddForce(transform.forward * 10f, ForceMode.Impulse);
    }

}
