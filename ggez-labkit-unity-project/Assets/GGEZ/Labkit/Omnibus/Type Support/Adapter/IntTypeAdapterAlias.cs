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

public void SetA (int value) { this.bus.SetObject (this.aliases[Pin.Std_A_Index], value); }
public void SignalA (int value) { this.bus.SignalObject (this.aliases[Pin.Std_A_Index], value); }
public bool GetIntA (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_A_Index], out value); }
public int GetIntA (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_A_Index], defaultValue); }

public void SetB (int value) { this.bus.SetObject (this.aliases[Pin.Std_B_Index], value); }
public void SignalB (int value) { this.bus.SignalObject (this.aliases[Pin.Std_B_Index], value); }
public bool GetIntB (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_B_Index], out value); }
public int GetIntB (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_B_Index], defaultValue); }
    
public void SetC (int value) { this.bus.SetObject (this.aliases[Pin.Std_C_Index], value); }
public void SignalC (int value) { this.bus.SignalObject (this.aliases[Pin.Std_C_Index], value); }
public bool GetIntC (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_C_Index], out value); }
public int GetIntC (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_C_Index], defaultValue); }
    
public void SetD (int value) { this.bus.SetObject (this.aliases[Pin.Std_D_Index], value); }
public void SignalD (int value) { this.bus.SignalObject (this.aliases[Pin.Std_D_Index], value); }
public bool GetIntD (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_D_Index], out value); }
public int GetIntD (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_D_Index], defaultValue); }
    
public void SetE (int value) { this.bus.SetObject (this.aliases[Pin.Std_E_Index], value); }
public void SignalE (int value) { this.bus.SignalObject (this.aliases[Pin.Std_E_Index], value); }
public bool GetIntE (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_E_Index], out value); }
public int GetIntE (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_E_Index], defaultValue); }
    
public void SetF (int value) { this.bus.SetObject (this.aliases[Pin.Std_F_Index], value); }
public void SignalF (int value) { this.bus.SignalObject (this.aliases[Pin.Std_F_Index], value); }
public bool GetIntF (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_F_Index], out value); }
public int GetIntF (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_F_Index], defaultValue); }
    
public void SetG (int value) { this.bus.SetObject (this.aliases[Pin.Std_G_Index], value); }
public void SignalG (int value) { this.bus.SignalObject (this.aliases[Pin.Std_G_Index], value); }
public bool GetIntG (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_G_Index], out value); }
public int GetIntG (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_G_Index], defaultValue); }
    
public void SetH (int value) { this.bus.SetObject (this.aliases[Pin.Std_H_Index], value); }
public void SignalH (int value) { this.bus.SignalObject (this.aliases[Pin.Std_H_Index], value); }
public bool GetIntH (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_H_Index], out value); }
public int GetIntH (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_H_Index], defaultValue); }
    
public void SetI (int value) { this.bus.SetObject (this.aliases[Pin.Std_I_Index], value); }
public void SignalI (int value) { this.bus.SignalObject (this.aliases[Pin.Std_I_Index], value); }
public bool GetIntI (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_I_Index], out value); }
public int GetIntI (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_I_Index], defaultValue); }
    
public void SetJ (int value) { this.bus.SetObject (this.aliases[Pin.Std_J_Index], value); }
public void SignalJ (int value) { this.bus.SignalObject (this.aliases[Pin.Std_J_Index], value); }
public bool GetIntJ (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_J_Index], out value); }
public int GetIntJ (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_J_Index], defaultValue); }
    
public void SetK (int value) { this.bus.SetObject (this.aliases[Pin.Std_K_Index], value); }
public void SignalK (int value) { this.bus.SignalObject (this.aliases[Pin.Std_K_Index], value); }
public bool GetIntK (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_K_Index], out value); }
public int GetIntK (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_K_Index], defaultValue); }
    
public void SetL (int value) { this.bus.SetObject (this.aliases[Pin.Std_L_Index], value); }
public void SignalL (int value) { this.bus.SignalObject (this.aliases[Pin.Std_L_Index], value); }
public bool GetIntL (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_L_Index], out value); }
public int GetIntL (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_L_Index], defaultValue); }
    
public void SetM (int value) { this.bus.SetObject (this.aliases[Pin.Std_M_Index], value); }
public void SignalM (int value) { this.bus.SignalObject (this.aliases[Pin.Std_M_Index], value); }
public bool GetIntM (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_M_Index], out value); }
public int GetIntM (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_M_Index], defaultValue); }
    
public void SetN (int value) { this.bus.SetObject (this.aliases[Pin.Std_N_Index], value); }
public void SignalN (int value) { this.bus.SignalObject (this.aliases[Pin.Std_N_Index], value); }
public bool GetIntN (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_N_Index], out value); }
public int GetIntN (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_N_Index], defaultValue); }
    
public void SetO (int value) { this.bus.SetObject (this.aliases[Pin.Std_O_Index], value); }
public void SignalO (int value) { this.bus.SignalObject (this.aliases[Pin.Std_O_Index], value); }
public bool GetIntO (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_O_Index], out value); }
public int GetIntO (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_O_Index], defaultValue); }
    
public void SetP (int value) { this.bus.SetObject (this.aliases[Pin.Std_P_Index], value); }
public void SignalP (int value) { this.bus.SignalObject (this.aliases[Pin.Std_P_Index], value); }
public bool GetIntP (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_P_Index], out value); }
public int GetIntP (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_P_Index], defaultValue); }
    
public void SetQ (int value) { this.bus.SetObject (this.aliases[Pin.Std_Q_Index], value); }
public void SignalQ (int value) { this.bus.SignalObject (this.aliases[Pin.Std_Q_Index], value); }
public bool GetIntQ (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_Q_Index], out value); }
public int GetIntQ (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_Q_Index], defaultValue); }
    
public void SetR (int value) { this.bus.SetObject (this.aliases[Pin.Std_R_Index], value); }
public void SignalR (int value) { this.bus.SignalObject (this.aliases[Pin.Std_R_Index], value); }
public bool GetIntR (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_R_Index], out value); }
public int GetIntR (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_R_Index], defaultValue); }
    
public void SetS (int value) { this.bus.SetObject (this.aliases[Pin.Std_S_Index], value); }
public void SignalS (int value) { this.bus.SignalObject (this.aliases[Pin.Std_S_Index], value); }
public bool GetIntS (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_S_Index], out value); }
public int GetIntS (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_S_Index], defaultValue); }
    
public void SetT (int value) { this.bus.SetObject (this.aliases[Pin.Std_T_Index], value); }
public void SignalT (int value) { this.bus.SignalObject (this.aliases[Pin.Std_T_Index], value); }
public bool GetIntT (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_T_Index], out value); }
public int GetIntT (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_T_Index], defaultValue); }
    
public void SetU (int value) { this.bus.SetObject (this.aliases[Pin.Std_U_Index], value); }
public void SignalU (int value) { this.bus.SignalObject (this.aliases[Pin.Std_U_Index], value); }
public bool GetIntU (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_U_Index], out value); }
public int GetIntU (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_U_Index], defaultValue); }
    
public void SetV (int value) { this.bus.SetObject (this.aliases[Pin.Std_V_Index], value); }
public void SignalV (int value) { this.bus.SignalObject (this.aliases[Pin.Std_V_Index], value); }
public bool GetIntV (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_V_Index], out value); }
public int GetIntV (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_V_Index], defaultValue); }
    
public void SetW (int value) { this.bus.SetObject (this.aliases[Pin.Std_W_Index], value); }
public void SignalW (int value) { this.bus.SignalObject (this.aliases[Pin.Std_W_Index], value); }
public bool GetIntW (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_W_Index], out value); }
public int GetIntW (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_W_Index], defaultValue); }
    
public void SetX (int value) { this.bus.SetObject (this.aliases[Pin.Std_X_Index], value); }
public void SignalX (int value) { this.bus.SignalObject (this.aliases[Pin.Std_X_Index], value); }
public bool GetIntX (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_X_Index], out value); }
public int GetIntX (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_X_Index], defaultValue); }
    
public void SetY (int value) { this.bus.SetObject (this.aliases[Pin.Std_Y_Index], value); }
public void SignalY (int value) { this.bus.SignalObject (this.aliases[Pin.Std_Y_Index], value); }
public bool GetIntY (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_Y_Index], out value); }
public int GetIntY (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_Y_Index], defaultValue); }
    
public void SetZ (int value) { this.bus.SetObject (this.aliases[Pin.Std_Z_Index], value); }
public void SignalZ (int value) { this.bus.SignalObject (this.aliases[Pin.Std_Z_Index], value); }
public bool GetIntZ (out int value) { return this.bus.GetInt (this.aliases[Pin.Std_Z_Index], out value); }
public int GetIntZ (int defaultValue) { return this.bus.GetInt (this.aliases[Pin.Std_Z_Index], defaultValue); }
    
}

}
