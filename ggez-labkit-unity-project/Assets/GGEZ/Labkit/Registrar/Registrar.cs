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


// ----[ int ]-----------------------------------------------------------------
void Set (string key, int value);
void Trigger (string key, int value);
bool Get (string key, out int value);
int GetInt (string key, int defaultValue);

}







}