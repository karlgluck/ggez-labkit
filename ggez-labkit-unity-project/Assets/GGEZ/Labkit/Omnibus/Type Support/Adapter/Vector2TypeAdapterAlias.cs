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

public void SetA (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_A_Index], value); }
public void SignalA (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_A_Index], value); }
public bool GetVector2A (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_A_Index], out value); }
public Vector2 GetVector2A (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_A_Index], defaultValue); }

public void SetB (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_B_Index], value); }
public void SignalB (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_B_Index], value); }
public bool GetVector2B (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_B_Index], out value); }
public Vector2 GetVector2B (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_B_Index], defaultValue); }
    
public void SetC (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_C_Index], value); }
public void SignalC (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_C_Index], value); }
public bool GetVector2C (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_C_Index], out value); }
public Vector2 GetVector2C (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_C_Index], defaultValue); }
    
public void SetD (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_D_Index], value); }
public void SignalD (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_D_Index], value); }
public bool GetVector2D (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_D_Index], out value); }
public Vector2 GetVector2D (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_D_Index], defaultValue); }
    
public void SetE (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_E_Index], value); }
public void SignalE (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_E_Index], value); }
public bool GetVector2E (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_E_Index], out value); }
public Vector2 GetVector2E (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_E_Index], defaultValue); }
    
public void SetF (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_F_Index], value); }
public void SignalF (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_F_Index], value); }
public bool GetVector2F (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_F_Index], out value); }
public Vector2 GetVector2F (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_F_Index], defaultValue); }
    
public void SetG (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_G_Index], value); }
public void SignalG (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_G_Index], value); }
public bool GetVector2G (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_G_Index], out value); }
public Vector2 GetVector2G (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_G_Index], defaultValue); }
    
public void SetH (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_H_Index], value); }
public void SignalH (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_H_Index], value); }
public bool GetVector2H (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_H_Index], out value); }
public Vector2 GetVector2H (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_H_Index], defaultValue); }
    
public void SetI (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_I_Index], value); }
public void SignalI (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_I_Index], value); }
public bool GetVector2I (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_I_Index], out value); }
public Vector2 GetVector2I (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_I_Index], defaultValue); }
    
public void SetJ (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_J_Index], value); }
public void SignalJ (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_J_Index], value); }
public bool GetVector2J (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_J_Index], out value); }
public Vector2 GetVector2J (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_J_Index], defaultValue); }
    
public void SetK (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_K_Index], value); }
public void SignalK (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_K_Index], value); }
public bool GetVector2K (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_K_Index], out value); }
public Vector2 GetVector2K (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_K_Index], defaultValue); }
    
public void SetL (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_L_Index], value); }
public void SignalL (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_L_Index], value); }
public bool GetVector2L (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_L_Index], out value); }
public Vector2 GetVector2L (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_L_Index], defaultValue); }
    
public void SetM (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_M_Index], value); }
public void SignalM (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_M_Index], value); }
public bool GetVector2M (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_M_Index], out value); }
public Vector2 GetVector2M (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_M_Index], defaultValue); }
    
public void SetN (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_N_Index], value); }
public void SignalN (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_N_Index], value); }
public bool GetVector2N (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_N_Index], out value); }
public Vector2 GetVector2N (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_N_Index], defaultValue); }
    
public void SetO (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_O_Index], value); }
public void SignalO (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_O_Index], value); }
public bool GetVector2O (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_O_Index], out value); }
public Vector2 GetVector2O (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_O_Index], defaultValue); }
    
public void SetP (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_P_Index], value); }
public void SignalP (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_P_Index], value); }
public bool GetVector2P (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_P_Index], out value); }
public Vector2 GetVector2P (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_P_Index], defaultValue); }
    
public void SetQ (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_Q_Index], value); }
public void SignalQ (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_Q_Index], value); }
public bool GetVector2Q (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_Q_Index], out value); }
public Vector2 GetVector2Q (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_Q_Index], defaultValue); }
    
public void SetR (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_R_Index], value); }
public void SignalR (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_R_Index], value); }
public bool GetVector2R (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_R_Index], out value); }
public Vector2 GetVector2R (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_R_Index], defaultValue); }
    
public void SetS (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_S_Index], value); }
public void SignalS (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_S_Index], value); }
public bool GetVector2S (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_S_Index], out value); }
public Vector2 GetVector2S (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_S_Index], defaultValue); }
    
public void SetT (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_T_Index], value); }
public void SignalT (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_T_Index], value); }
public bool GetVector2T (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_T_Index], out value); }
public Vector2 GetVector2T (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_T_Index], defaultValue); }
    
public void SetU (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_U_Index], value); }
public void SignalU (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_U_Index], value); }
public bool GetVector2U (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_U_Index], out value); }
public Vector2 GetVector2U (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_U_Index], defaultValue); }
    
public void SetV (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_V_Index], value); }
public void SignalV (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_V_Index], value); }
public bool GetVector2V (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_V_Index], out value); }
public Vector2 GetVector2V (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_V_Index], defaultValue); }
    
public void SetW (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_W_Index], value); }
public void SignalW (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_W_Index], value); }
public bool GetVector2W (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_W_Index], out value); }
public Vector2 GetVector2W (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_W_Index], defaultValue); }
    
public void SetX (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_X_Index], value); }
public void SignalX (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_X_Index], value); }
public bool GetVector2X (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_X_Index], out value); }
public Vector2 GetVector2X (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_X_Index], defaultValue); }
    
public void SetY (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_Y_Index], value); }
public void SignalY (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_Y_Index], value); }
public bool GetVector2Y (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_Y_Index], out value); }
public Vector2 GetVector2Y (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_Y_Index], defaultValue); }
    
public void SetZ (Vector2 value) { this.bus.SetObject (this.aliases[Pin.Std_Z_Index], value); }
public void SignalZ (Vector2 value) { this.bus.SignalObject (this.aliases[Pin.Std_Z_Index], value); }
public bool GetVector2Z (out Vector2 value) { return this.bus.GetVector2 (this.aliases[Pin.Std_Z_Index], out value); }
public Vector2 GetVector2Z (Vector2 defaultValue) { return this.bus.GetVector2 (this.aliases[Pin.Std_Z_Index], defaultValue); }
    
}

}
