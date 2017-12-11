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

public void SetA (float value) { this.bus.SetObject (this.aliases[Pin.Std_A_Index], value); }
public void SignalA (float value) { this.bus.SignalObject (this.aliases[Pin.Std_A_Index], value); }
public bool GetFloatA (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_A_Index], out value); }
public float GetFloatA (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_A_Index], defaultValue); }

public void SetB (float value) { this.bus.SetObject (this.aliases[Pin.Std_B_Index], value); }
public void SignalB (float value) { this.bus.SignalObject (this.aliases[Pin.Std_B_Index], value); }
public bool GetFloatB (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_B_Index], out value); }
public float GetFloatB (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_B_Index], defaultValue); }
    
public void SetC (float value) { this.bus.SetObject (this.aliases[Pin.Std_C_Index], value); }
public void SignalC (float value) { this.bus.SignalObject (this.aliases[Pin.Std_C_Index], value); }
public bool GetFloatC (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_C_Index], out value); }
public float GetFloatC (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_C_Index], defaultValue); }
    
public void SetD (float value) { this.bus.SetObject (this.aliases[Pin.Std_D_Index], value); }
public void SignalD (float value) { this.bus.SignalObject (this.aliases[Pin.Std_D_Index], value); }
public bool GetFloatD (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_D_Index], out value); }
public float GetFloatD (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_D_Index], defaultValue); }
    
public void SetE (float value) { this.bus.SetObject (this.aliases[Pin.Std_E_Index], value); }
public void SignalE (float value) { this.bus.SignalObject (this.aliases[Pin.Std_E_Index], value); }
public bool GetFloatE (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_E_Index], out value); }
public float GetFloatE (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_E_Index], defaultValue); }
    
public void SetF (float value) { this.bus.SetObject (this.aliases[Pin.Std_F_Index], value); }
public void SignalF (float value) { this.bus.SignalObject (this.aliases[Pin.Std_F_Index], value); }
public bool GetFloatF (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_F_Index], out value); }
public float GetFloatF (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_F_Index], defaultValue); }
    
public void SetG (float value) { this.bus.SetObject (this.aliases[Pin.Std_G_Index], value); }
public void SignalG (float value) { this.bus.SignalObject (this.aliases[Pin.Std_G_Index], value); }
public bool GetFloatG (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_G_Index], out value); }
public float GetFloatG (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_G_Index], defaultValue); }
    
public void SetH (float value) { this.bus.SetObject (this.aliases[Pin.Std_H_Index], value); }
public void SignalH (float value) { this.bus.SignalObject (this.aliases[Pin.Std_H_Index], value); }
public bool GetFloatH (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_H_Index], out value); }
public float GetFloatH (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_H_Index], defaultValue); }
    
public void SetI (float value) { this.bus.SetObject (this.aliases[Pin.Std_I_Index], value); }
public void SignalI (float value) { this.bus.SignalObject (this.aliases[Pin.Std_I_Index], value); }
public bool GetFloatI (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_I_Index], out value); }
public float GetFloatI (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_I_Index], defaultValue); }
    
public void SetJ (float value) { this.bus.SetObject (this.aliases[Pin.Std_J_Index], value); }
public void SignalJ (float value) { this.bus.SignalObject (this.aliases[Pin.Std_J_Index], value); }
public bool GetFloatJ (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_J_Index], out value); }
public float GetFloatJ (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_J_Index], defaultValue); }
    
public void SetK (float value) { this.bus.SetObject (this.aliases[Pin.Std_K_Index], value); }
public void SignalK (float value) { this.bus.SignalObject (this.aliases[Pin.Std_K_Index], value); }
public bool GetFloatK (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_K_Index], out value); }
public float GetFloatK (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_K_Index], defaultValue); }
    
public void SetL (float value) { this.bus.SetObject (this.aliases[Pin.Std_L_Index], value); }
public void SignalL (float value) { this.bus.SignalObject (this.aliases[Pin.Std_L_Index], value); }
public bool GetFloatL (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_L_Index], out value); }
public float GetFloatL (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_L_Index], defaultValue); }
    
public void SetM (float value) { this.bus.SetObject (this.aliases[Pin.Std_M_Index], value); }
public void SignalM (float value) { this.bus.SignalObject (this.aliases[Pin.Std_M_Index], value); }
public bool GetFloatM (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_M_Index], out value); }
public float GetFloatM (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_M_Index], defaultValue); }
    
public void SetN (float value) { this.bus.SetObject (this.aliases[Pin.Std_N_Index], value); }
public void SignalN (float value) { this.bus.SignalObject (this.aliases[Pin.Std_N_Index], value); }
public bool GetFloatN (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_N_Index], out value); }
public float GetFloatN (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_N_Index], defaultValue); }
    
public void SetO (float value) { this.bus.SetObject (this.aliases[Pin.Std_O_Index], value); }
public void SignalO (float value) { this.bus.SignalObject (this.aliases[Pin.Std_O_Index], value); }
public bool GetFloatO (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_O_Index], out value); }
public float GetFloatO (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_O_Index], defaultValue); }
    
public void SetP (float value) { this.bus.SetObject (this.aliases[Pin.Std_P_Index], value); }
public void SignalP (float value) { this.bus.SignalObject (this.aliases[Pin.Std_P_Index], value); }
public bool GetFloatP (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_P_Index], out value); }
public float GetFloatP (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_P_Index], defaultValue); }
    
public void SetQ (float value) { this.bus.SetObject (this.aliases[Pin.Std_Q_Index], value); }
public void SignalQ (float value) { this.bus.SignalObject (this.aliases[Pin.Std_Q_Index], value); }
public bool GetFloatQ (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_Q_Index], out value); }
public float GetFloatQ (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_Q_Index], defaultValue); }
    
public void SetR (float value) { this.bus.SetObject (this.aliases[Pin.Std_R_Index], value); }
public void SignalR (float value) { this.bus.SignalObject (this.aliases[Pin.Std_R_Index], value); }
public bool GetFloatR (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_R_Index], out value); }
public float GetFloatR (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_R_Index], defaultValue); }
    
public void SetS (float value) { this.bus.SetObject (this.aliases[Pin.Std_S_Index], value); }
public void SignalS (float value) { this.bus.SignalObject (this.aliases[Pin.Std_S_Index], value); }
public bool GetFloatS (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_S_Index], out value); }
public float GetFloatS (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_S_Index], defaultValue); }
    
public void SetT (float value) { this.bus.SetObject (this.aliases[Pin.Std_T_Index], value); }
public void SignalT (float value) { this.bus.SignalObject (this.aliases[Pin.Std_T_Index], value); }
public bool GetFloatT (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_T_Index], out value); }
public float GetFloatT (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_T_Index], defaultValue); }
    
public void SetU (float value) { this.bus.SetObject (this.aliases[Pin.Std_U_Index], value); }
public void SignalU (float value) { this.bus.SignalObject (this.aliases[Pin.Std_U_Index], value); }
public bool GetFloatU (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_U_Index], out value); }
public float GetFloatU (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_U_Index], defaultValue); }
    
public void SetV (float value) { this.bus.SetObject (this.aliases[Pin.Std_V_Index], value); }
public void SignalV (float value) { this.bus.SignalObject (this.aliases[Pin.Std_V_Index], value); }
public bool GetFloatV (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_V_Index], out value); }
public float GetFloatV (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_V_Index], defaultValue); }
    
public void SetW (float value) { this.bus.SetObject (this.aliases[Pin.Std_W_Index], value); }
public void SignalW (float value) { this.bus.SignalObject (this.aliases[Pin.Std_W_Index], value); }
public bool GetFloatW (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_W_Index], out value); }
public float GetFloatW (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_W_Index], defaultValue); }
    
public void SetX (float value) { this.bus.SetObject (this.aliases[Pin.Std_X_Index], value); }
public void SignalX (float value) { this.bus.SignalObject (this.aliases[Pin.Std_X_Index], value); }
public bool GetFloatX (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_X_Index], out value); }
public float GetFloatX (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_X_Index], defaultValue); }
    
public void SetY (float value) { this.bus.SetObject (this.aliases[Pin.Std_Y_Index], value); }
public void SignalY (float value) { this.bus.SignalObject (this.aliases[Pin.Std_Y_Index], value); }
public bool GetFloatY (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_Y_Index], out value); }
public float GetFloatY (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_Y_Index], defaultValue); }
    
public void SetZ (float value) { this.bus.SetObject (this.aliases[Pin.Std_Z_Index], value); }
public void SignalZ (float value) { this.bus.SignalObject (this.aliases[Pin.Std_Z_Index], value); }
public bool GetFloatZ (out float value) { return this.bus.GetFloat (this.aliases[Pin.Std_Z_Index], out value); }
public float GetFloatZ (float defaultValue) { return this.bus.GetFloat (this.aliases[Pin.Std_Z_Index], defaultValue); }
    
}

}
