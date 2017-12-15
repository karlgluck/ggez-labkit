// This is free and unencumbered software released into the public domain.
//
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
//
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// For more information, please refer to <http://unlicense.org/>

using System;
using UnityEngine;
using Image = UnityEngine.UI.Image;


namespace GGEZ.Omnibus
{



[
Serializable,
AddComponentMenu ("GGEZ/Omnibus/Modules/UI.Image/Image Alpha Fade Timer (Module)"),
RequireComponent (typeof (Image))
]
public sealed class ImageAlphaFadeTimerModule : Cell
{

#region Programming Interface

public Bus Bus
    {
    get { return this.bus; }
    set
        {
        this.bus = value;
        this.input.Connect (this.bus, this.pin);
        }
    }

public string Pin
    {
    get { return this.pin; }
    set
        {
        this.pin = value;
        this.input.Connect (this.bus, this.pin);
        }
    }

public float StayTime
    {
    get { return this.stayTime; }
    set
        {
        this.stayTime = Mathf.Max (0.0001f, value);
        }
    }

public float FadeTime
    {
    get { return this.fadeTime; }
    set
        {
        this.fadeTime = Mathf.Max (0.0001f, value);
        }
    }

#endregion


[Header ("*:" + Omnibus.Pin.INPUT + " (void)")]
[SerializeField] private Bus bus;
[SerializeField] private string pin;

[Header ("Settings")]
[SerializeField] private float stayTime = 1f;
[SerializeField] private float fadeTime = 1f;

private Wire input = Wire.CELL_INPUT;
private Image image;

private float timer = 0.0f;

public override void OnDidSignal (string pin, object value)
    {
	Debug.Assert (pin == Omnibus.Pin.INPUT);
    this.enabled = true;
    }

public override void Route (string port, Bus bus)
    {
	this.Bus = bus;
    if (!this.input.IsAttached && Application.isPlaying)
        {
        this.enabled = true;
        }
    }
    
void Awake ()
    {
    this.image = (Image)this.GetComponent (typeof (Image));
    }

void OnEnable ()
    {
    this.timer = 0f;
    if (this.input.IsAttached)
        {
        return;
        }
    this.input.Attach (this, this.bus, this.pin);
    this.image.color = this.image.color.WithA (0f);
    this.enabled = false;
    }

void OnDestroy ()
    {
    this.input.Detach ();
    }

void OnValidate ()
    {
	this.input.Connect (this.bus, this.pin);
    this.fadeTime = Mathf.Max (0.0001f, this.fadeTime);
    }

void Update ()
    {
    Color color = this.image.color;
    this.timer += Time.smoothDeltaTime;
    if (this.timer < this.stayTime)
        {
        color.a = 1f;
        this.image.color = color;
        return;
        }
    float endTime = this.stayTime + this.fadeTime;
    float t = Mathf.InverseLerp (this.stayTime, endTime, this.timer);
    color.a = Mathf.Lerp (1f, 0f, t);
    this.image.color = color;
	this.enabled = this.timer < endTime;
    }

}

}