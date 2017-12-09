using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGEZ.Omnibus;

[
RequireComponent (typeof (Transform))
]
public class RegisterTransformOnce : MonoBehaviour
{
[SerializeField] private UnityEngine.Object bus;
[SerializeField] private string key;

public Bus Bus
    {
    set
        {
        this.bus = value;
        this.enabled = value != null;
        }
    get { return this.bus as Bus; }
    }

void OnEnable ()
	{
	if (this.Bus == null)
		{
        this.enabled = false;
		}
	else
		{
		this.Bus.Set (this.key, this.GetComponent <Transform> ());
        GameObject.Destroy (this);
		}
	}

void OnValidate ()
	{
    var bus = this.bus as Bus;
    if (bus == null)
        {
        var gameObject = this.bus as GameObject;
        if (gameObject != null)
            {
            this.bus = gameObject.GetComponent <Bus> ();
            bus = this.bus as Bus;
            }
        else
            {
            this.bus = null;
            }
        }
	}
}
