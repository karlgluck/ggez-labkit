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

namespace GGEZ.Omnibus
{

public sealed partial class Adapter
{

public void SetA (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_A_Index], value); }
public void SignalA (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_A_Index], value); }
public bool GetVector3A (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_A_Index], out value); }
public Vector3 GetVector3A (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_A_Index], defaultValue); }

public void SetB (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_B_Index], value); }
public void SignalB (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_B_Index], value); }
public bool GetVector3B (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_B_Index], out value); }
public Vector3 GetVector3B (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_B_Index], defaultValue); }
    
public void SetC (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_C_Index], value); }
public void SignalC (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_C_Index], value); }
public bool GetVector3C (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_C_Index], out value); }
public Vector3 GetVector3C (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_C_Index], defaultValue); }
    
public void SetD (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_D_Index], value); }
public void SignalD (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_D_Index], value); }
public bool GetVector3D (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_D_Index], out value); }
public Vector3 GetVector3D (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_D_Index], defaultValue); }
    
public void SetE (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_E_Index], value); }
public void SignalE (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_E_Index], value); }
public bool GetVector3E (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_E_Index], out value); }
public Vector3 GetVector3E (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_E_Index], defaultValue); }
    
public void SetF (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_F_Index], value); }
public void SignalF (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_F_Index], value); }
public bool GetVector3F (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_F_Index], out value); }
public Vector3 GetVector3F (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_F_Index], defaultValue); }
    
public void SetG (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_G_Index], value); }
public void SignalG (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_G_Index], value); }
public bool GetVector3G (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_G_Index], out value); }
public Vector3 GetVector3G (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_G_Index], defaultValue); }
    
public void SetH (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_H_Index], value); }
public void SignalH (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_H_Index], value); }
public bool GetVector3H (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_H_Index], out value); }
public Vector3 GetVector3H (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_H_Index], defaultValue); }
    
public void SetI (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_I_Index], value); }
public void SignalI (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_I_Index], value); }
public bool GetVector3I (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_I_Index], out value); }
public Vector3 GetVector3I (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_I_Index], defaultValue); }
    
public void SetJ (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_J_Index], value); }
public void SignalJ (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_J_Index], value); }
public bool GetVector3J (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_J_Index], out value); }
public Vector3 GetVector3J (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_J_Index], defaultValue); }
    
public void SetK (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_K_Index], value); }
public void SignalK (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_K_Index], value); }
public bool GetVector3K (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_K_Index], out value); }
public Vector3 GetVector3K (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_K_Index], defaultValue); }
    
public void SetL (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_L_Index], value); }
public void SignalL (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_L_Index], value); }
public bool GetVector3L (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_L_Index], out value); }
public Vector3 GetVector3L (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_L_Index], defaultValue); }
    
public void SetM (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_M_Index], value); }
public void SignalM (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_M_Index], value); }
public bool GetVector3M (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_M_Index], out value); }
public Vector3 GetVector3M (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_M_Index], defaultValue); }
    
public void SetN (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_N_Index], value); }
public void SignalN (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_N_Index], value); }
public bool GetVector3N (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_N_Index], out value); }
public Vector3 GetVector3N (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_N_Index], defaultValue); }
    
public void SetO (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_O_Index], value); }
public void SignalO (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_O_Index], value); }
public bool GetVector3O (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_O_Index], out value); }
public Vector3 GetVector3O (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_O_Index], defaultValue); }
    
public void SetP (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_P_Index], value); }
public void SignalP (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_P_Index], value); }
public bool GetVector3P (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_P_Index], out value); }
public Vector3 GetVector3P (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_P_Index], defaultValue); }
    
public void SetQ (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_Q_Index], value); }
public void SignalQ (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_Q_Index], value); }
public bool GetVector3Q (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_Q_Index], out value); }
public Vector3 GetVector3Q (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_Q_Index], defaultValue); }
    
public void SetR (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_R_Index], value); }
public void SignalR (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_R_Index], value); }
public bool GetVector3R (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_R_Index], out value); }
public Vector3 GetVector3R (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_R_Index], defaultValue); }
    
public void SetS (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_S_Index], value); }
public void SignalS (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_S_Index], value); }
public bool GetVector3S (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_S_Index], out value); }
public Vector3 GetVector3S (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_S_Index], defaultValue); }
    
public void SetT (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_T_Index], value); }
public void SignalT (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_T_Index], value); }
public bool GetVector3T (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_T_Index], out value); }
public Vector3 GetVector3T (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_T_Index], defaultValue); }
    
public void SetU (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_U_Index], value); }
public void SignalU (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_U_Index], value); }
public bool GetVector3U (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_U_Index], out value); }
public Vector3 GetVector3U (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_U_Index], defaultValue); }
    
public void SetV (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_V_Index], value); }
public void SignalV (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_V_Index], value); }
public bool GetVector3V (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_V_Index], out value); }
public Vector3 GetVector3V (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_V_Index], defaultValue); }
    
public void SetW (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_W_Index], value); }
public void SignalW (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_W_Index], value); }
public bool GetVector3W (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_W_Index], out value); }
public Vector3 GetVector3W (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_W_Index], defaultValue); }
    
public void SetX (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_X_Index], value); }
public void SignalX (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_X_Index], value); }
public bool GetVector3X (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_X_Index], out value); }
public Vector3 GetVector3X (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_X_Index], defaultValue); }
    
public void SetY (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_Y_Index], value); }
public void SignalY (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_Y_Index], value); }
public bool GetVector3Y (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_Y_Index], out value); }
public Vector3 GetVector3Y (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_Y_Index], defaultValue); }
    
public void SetZ (Vector3 value) { this.bus.SetObject (this.aliases[Pin.Std_Z_Index], value); }
public void SignalZ (Vector3 value) { this.bus.SignalObject (this.aliases[Pin.Std_Z_Index], value); }
public bool GetVector3Z (out Vector3 value) { return this.bus.GetVector3 (this.aliases[Pin.Std_Z_Index], out value); }
public Vector3 GetVector3Z (Vector3 defaultValue) { return this.bus.GetVector3 (this.aliases[Pin.Std_Z_Index], defaultValue); }
    
}

}
