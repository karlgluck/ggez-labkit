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

public void SetA (bool value) { this.bus.SetObject (this.aliases[Pin.Std_A_Index], value); }
public void SignalA (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_A_Index], value); }
public bool GetBooleanA (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_A_Index], out value); }
public bool GetBooleanA (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_A_Index], defaultValue); }

public void SetB (bool value) { this.bus.SetObject (this.aliases[Pin.Std_B_Index], value); }
public void SignalB (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_B_Index], value); }
public bool GetBooleanB (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_B_Index], out value); }
public bool GetBooleanB (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_B_Index], defaultValue); }
    
public void SetC (bool value) { this.bus.SetObject (this.aliases[Pin.Std_C_Index], value); }
public void SignalC (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_C_Index], value); }
public bool GetBooleanC (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_C_Index], out value); }
public bool GetBooleanC (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_C_Index], defaultValue); }
    
public void SetD (bool value) { this.bus.SetObject (this.aliases[Pin.Std_D_Index], value); }
public void SignalD (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_D_Index], value); }
public bool GetBooleanD (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_D_Index], out value); }
public bool GetBooleanD (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_D_Index], defaultValue); }
    
public void SetE (bool value) { this.bus.SetObject (this.aliases[Pin.Std_E_Index], value); }
public void SignalE (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_E_Index], value); }
public bool GetBooleanE (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_E_Index], out value); }
public bool GetBooleanE (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_E_Index], defaultValue); }
    
public void SetF (bool value) { this.bus.SetObject (this.aliases[Pin.Std_F_Index], value); }
public void SignalF (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_F_Index], value); }
public bool GetBooleanF (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_F_Index], out value); }
public bool GetBooleanF (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_F_Index], defaultValue); }
    
public void SetG (bool value) { this.bus.SetObject (this.aliases[Pin.Std_G_Index], value); }
public void SignalG (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_G_Index], value); }
public bool GetBooleanG (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_G_Index], out value); }
public bool GetBooleanG (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_G_Index], defaultValue); }
    
public void SetH (bool value) { this.bus.SetObject (this.aliases[Pin.Std_H_Index], value); }
public void SignalH (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_H_Index], value); }
public bool GetBooleanH (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_H_Index], out value); }
public bool GetBooleanH (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_H_Index], defaultValue); }
    
public void SetI (bool value) { this.bus.SetObject (this.aliases[Pin.Std_I_Index], value); }
public void SignalI (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_I_Index], value); }
public bool GetBooleanI (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_I_Index], out value); }
public bool GetBooleanI (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_I_Index], defaultValue); }
    
public void SetJ (bool value) { this.bus.SetObject (this.aliases[Pin.Std_J_Index], value); }
public void SignalJ (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_J_Index], value); }
public bool GetBooleanJ (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_J_Index], out value); }
public bool GetBooleanJ (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_J_Index], defaultValue); }
    
public void SetK (bool value) { this.bus.SetObject (this.aliases[Pin.Std_K_Index], value); }
public void SignalK (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_K_Index], value); }
public bool GetBooleanK (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_K_Index], out value); }
public bool GetBooleanK (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_K_Index], defaultValue); }
    
public void SetL (bool value) { this.bus.SetObject (this.aliases[Pin.Std_L_Index], value); }
public void SignalL (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_L_Index], value); }
public bool GetBooleanL (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_L_Index], out value); }
public bool GetBooleanL (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_L_Index], defaultValue); }
    
public void SetM (bool value) { this.bus.SetObject (this.aliases[Pin.Std_M_Index], value); }
public void SignalM (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_M_Index], value); }
public bool GetBooleanM (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_M_Index], out value); }
public bool GetBooleanM (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_M_Index], defaultValue); }
    
public void SetN (bool value) { this.bus.SetObject (this.aliases[Pin.Std_N_Index], value); }
public void SignalN (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_N_Index], value); }
public bool GetBooleanN (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_N_Index], out value); }
public bool GetBooleanN (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_N_Index], defaultValue); }
    
public void SetO (bool value) { this.bus.SetObject (this.aliases[Pin.Std_O_Index], value); }
public void SignalO (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_O_Index], value); }
public bool GetBooleanO (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_O_Index], out value); }
public bool GetBooleanO (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_O_Index], defaultValue); }
    
public void SetP (bool value) { this.bus.SetObject (this.aliases[Pin.Std_P_Index], value); }
public void SignalP (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_P_Index], value); }
public bool GetBooleanP (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_P_Index], out value); }
public bool GetBooleanP (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_P_Index], defaultValue); }
    
public void SetQ (bool value) { this.bus.SetObject (this.aliases[Pin.Std_Q_Index], value); }
public void SignalQ (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_Q_Index], value); }
public bool GetBooleanQ (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_Q_Index], out value); }
public bool GetBooleanQ (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_Q_Index], defaultValue); }
    
public void SetR (bool value) { this.bus.SetObject (this.aliases[Pin.Std_R_Index], value); }
public void SignalR (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_R_Index], value); }
public bool GetBooleanR (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_R_Index], out value); }
public bool GetBooleanR (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_R_Index], defaultValue); }
    
public void SetS (bool value) { this.bus.SetObject (this.aliases[Pin.Std_S_Index], value); }
public void SignalS (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_S_Index], value); }
public bool GetBooleanS (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_S_Index], out value); }
public bool GetBooleanS (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_S_Index], defaultValue); }
    
public void SetT (bool value) { this.bus.SetObject (this.aliases[Pin.Std_T_Index], value); }
public void SignalT (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_T_Index], value); }
public bool GetBooleanT (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_T_Index], out value); }
public bool GetBooleanT (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_T_Index], defaultValue); }
    
public void SetU (bool value) { this.bus.SetObject (this.aliases[Pin.Std_U_Index], value); }
public void SignalU (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_U_Index], value); }
public bool GetBooleanU (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_U_Index], out value); }
public bool GetBooleanU (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_U_Index], defaultValue); }
    
public void SetV (bool value) { this.bus.SetObject (this.aliases[Pin.Std_V_Index], value); }
public void SignalV (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_V_Index], value); }
public bool GetBooleanV (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_V_Index], out value); }
public bool GetBooleanV (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_V_Index], defaultValue); }
    
public void SetW (bool value) { this.bus.SetObject (this.aliases[Pin.Std_W_Index], value); }
public void SignalW (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_W_Index], value); }
public bool GetBooleanW (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_W_Index], out value); }
public bool GetBooleanW (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_W_Index], defaultValue); }
    
public void SetX (bool value) { this.bus.SetObject (this.aliases[Pin.Std_X_Index], value); }
public void SignalX (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_X_Index], value); }
public bool GetBooleanX (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_X_Index], out value); }
public bool GetBooleanX (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_X_Index], defaultValue); }
    
public void SetY (bool value) { this.bus.SetObject (this.aliases[Pin.Std_Y_Index], value); }
public void SignalY (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_Y_Index], value); }
public bool GetBooleanY (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_Y_Index], out value); }
public bool GetBooleanY (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_Y_Index], defaultValue); }
    
public void SetZ (bool value) { this.bus.SetObject (this.aliases[Pin.Std_Z_Index], value); }
public void SignalZ (bool value) { this.bus.SignalObject (this.aliases[Pin.Std_Z_Index], value); }
public bool GetBooleanZ (out bool value) { return this.bus.GetBoolean (this.aliases[Pin.Std_Z_Index], out value); }
public bool GetBooleanZ (bool defaultValue) { return this.bus.GetBoolean (this.aliases[Pin.Std_Z_Index], defaultValue); }
    
}

}
