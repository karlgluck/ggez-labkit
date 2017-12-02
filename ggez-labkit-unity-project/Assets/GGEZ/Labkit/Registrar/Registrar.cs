using System;
using UnityEngine;
using System.Collections.Generic;
using StringCollection = System.Collections.Generic.ICollection<string>;

namespace GGEZ
{



public interface Registrar
{
bool HasListeners (string key);
bool HasValue (string key);
StringCollection GetEventKeys ();
StringCollection GetRegisterKeys ();

void RegisterListener (string key, Listener listener);
void UnregisterListener (string key, Listener listener);


// ----[ void ]----------------------------------------------------------------
void Trigger (string key);

// ----[ object ]--------------------------------------------------------------
object Get (string key);


// ----[ int ]-----------------------------------------------------------------
void Set (string key, int value);
void Trigger (string key, int value);
bool Get (string key, out int value);
int GetInt (string key, int defaultValue);


// ----[ bool ]----------------------------------------------------------------
void Set (string key, bool value);
void Trigger (string key, bool value);
bool Get (string key, out bool value);
bool GetBool (string key, bool defaultValue);


// ----[ string ]--------------------------------------------------------------
void Set (string key, string value);
void Trigger (string key, string value);
bool Get (string key, out string value);
string GetString (string key, string defaultValue);


// ----[ float ]---------------------------------------------------------------
void Set (string key, float value);
void Trigger (string key, float value);
bool Get (string key, out float value);
float GetFloat (string key, float defaultValue);

}







}