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

public void SetA (string value) { this.bus.SetObject (this.aliases[Pin.Std_A_Index], value); }
public void SignalA (string value) { this.bus.SignalObject (this.aliases[Pin.Std_A_Index], value); }
public bool GetStringA (out string value) { return this.bus.GetString (this.aliases[Pin.Std_A_Index], out value); }
public string GetStringA (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_A_Index], defaultValue); }

public void SetB (string value) { this.bus.SetObject (this.aliases[Pin.Std_B_Index], value); }
public void SignalB (string value) { this.bus.SignalObject (this.aliases[Pin.Std_B_Index], value); }
public bool GetStringB (out string value) { return this.bus.GetString (this.aliases[Pin.Std_B_Index], out value); }
public string GetStringB (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_B_Index], defaultValue); }
    
public void SetC (string value) { this.bus.SetObject (this.aliases[Pin.Std_C_Index], value); }
public void SignalC (string value) { this.bus.SignalObject (this.aliases[Pin.Std_C_Index], value); }
public bool GetStringC (out string value) { return this.bus.GetString (this.aliases[Pin.Std_C_Index], out value); }
public string GetStringC (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_C_Index], defaultValue); }
    
public void SetD (string value) { this.bus.SetObject (this.aliases[Pin.Std_D_Index], value); }
public void SignalD (string value) { this.bus.SignalObject (this.aliases[Pin.Std_D_Index], value); }
public bool GetStringD (out string value) { return this.bus.GetString (this.aliases[Pin.Std_D_Index], out value); }
public string GetStringD (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_D_Index], defaultValue); }
    
public void SetE (string value) { this.bus.SetObject (this.aliases[Pin.Std_E_Index], value); }
public void SignalE (string value) { this.bus.SignalObject (this.aliases[Pin.Std_E_Index], value); }
public bool GetStringE (out string value) { return this.bus.GetString (this.aliases[Pin.Std_E_Index], out value); }
public string GetStringE (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_E_Index], defaultValue); }
    
public void SetF (string value) { this.bus.SetObject (this.aliases[Pin.Std_F_Index], value); }
public void SignalF (string value) { this.bus.SignalObject (this.aliases[Pin.Std_F_Index], value); }
public bool GetStringF (out string value) { return this.bus.GetString (this.aliases[Pin.Std_F_Index], out value); }
public string GetStringF (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_F_Index], defaultValue); }
    
public void SetG (string value) { this.bus.SetObject (this.aliases[Pin.Std_G_Index], value); }
public void SignalG (string value) { this.bus.SignalObject (this.aliases[Pin.Std_G_Index], value); }
public bool GetStringG (out string value) { return this.bus.GetString (this.aliases[Pin.Std_G_Index], out value); }
public string GetStringG (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_G_Index], defaultValue); }
    
public void SetH (string value) { this.bus.SetObject (this.aliases[Pin.Std_H_Index], value); }
public void SignalH (string value) { this.bus.SignalObject (this.aliases[Pin.Std_H_Index], value); }
public bool GetStringH (out string value) { return this.bus.GetString (this.aliases[Pin.Std_H_Index], out value); }
public string GetStringH (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_H_Index], defaultValue); }
    
public void SetI (string value) { this.bus.SetObject (this.aliases[Pin.Std_I_Index], value); }
public void SignalI (string value) { this.bus.SignalObject (this.aliases[Pin.Std_I_Index], value); }
public bool GetStringI (out string value) { return this.bus.GetString (this.aliases[Pin.Std_I_Index], out value); }
public string GetStringI (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_I_Index], defaultValue); }
    
public void SetJ (string value) { this.bus.SetObject (this.aliases[Pin.Std_J_Index], value); }
public void SignalJ (string value) { this.bus.SignalObject (this.aliases[Pin.Std_J_Index], value); }
public bool GetStringJ (out string value) { return this.bus.GetString (this.aliases[Pin.Std_J_Index], out value); }
public string GetStringJ (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_J_Index], defaultValue); }
    
public void SetK (string value) { this.bus.SetObject (this.aliases[Pin.Std_K_Index], value); }
public void SignalK (string value) { this.bus.SignalObject (this.aliases[Pin.Std_K_Index], value); }
public bool GetStringK (out string value) { return this.bus.GetString (this.aliases[Pin.Std_K_Index], out value); }
public string GetStringK (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_K_Index], defaultValue); }
    
public void SetL (string value) { this.bus.SetObject (this.aliases[Pin.Std_L_Index], value); }
public void SignalL (string value) { this.bus.SignalObject (this.aliases[Pin.Std_L_Index], value); }
public bool GetStringL (out string value) { return this.bus.GetString (this.aliases[Pin.Std_L_Index], out value); }
public string GetStringL (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_L_Index], defaultValue); }
    
public void SetM (string value) { this.bus.SetObject (this.aliases[Pin.Std_M_Index], value); }
public void SignalM (string value) { this.bus.SignalObject (this.aliases[Pin.Std_M_Index], value); }
public bool GetStringM (out string value) { return this.bus.GetString (this.aliases[Pin.Std_M_Index], out value); }
public string GetStringM (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_M_Index], defaultValue); }
    
public void SetN (string value) { this.bus.SetObject (this.aliases[Pin.Std_N_Index], value); }
public void SignalN (string value) { this.bus.SignalObject (this.aliases[Pin.Std_N_Index], value); }
public bool GetStringN (out string value) { return this.bus.GetString (this.aliases[Pin.Std_N_Index], out value); }
public string GetStringN (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_N_Index], defaultValue); }
    
public void SetO (string value) { this.bus.SetObject (this.aliases[Pin.Std_O_Index], value); }
public void SignalO (string value) { this.bus.SignalObject (this.aliases[Pin.Std_O_Index], value); }
public bool GetStringO (out string value) { return this.bus.GetString (this.aliases[Pin.Std_O_Index], out value); }
public string GetStringO (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_O_Index], defaultValue); }
    
public void SetP (string value) { this.bus.SetObject (this.aliases[Pin.Std_P_Index], value); }
public void SignalP (string value) { this.bus.SignalObject (this.aliases[Pin.Std_P_Index], value); }
public bool GetStringP (out string value) { return this.bus.GetString (this.aliases[Pin.Std_P_Index], out value); }
public string GetStringP (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_P_Index], defaultValue); }
    
public void SetQ (string value) { this.bus.SetObject (this.aliases[Pin.Std_Q_Index], value); }
public void SignalQ (string value) { this.bus.SignalObject (this.aliases[Pin.Std_Q_Index], value); }
public bool GetStringQ (out string value) { return this.bus.GetString (this.aliases[Pin.Std_Q_Index], out value); }
public string GetStringQ (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_Q_Index], defaultValue); }
    
public void SetR (string value) { this.bus.SetObject (this.aliases[Pin.Std_R_Index], value); }
public void SignalR (string value) { this.bus.SignalObject (this.aliases[Pin.Std_R_Index], value); }
public bool GetStringR (out string value) { return this.bus.GetString (this.aliases[Pin.Std_R_Index], out value); }
public string GetStringR (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_R_Index], defaultValue); }
    
public void SetS (string value) { this.bus.SetObject (this.aliases[Pin.Std_S_Index], value); }
public void SignalS (string value) { this.bus.SignalObject (this.aliases[Pin.Std_S_Index], value); }
public bool GetStringS (out string value) { return this.bus.GetString (this.aliases[Pin.Std_S_Index], out value); }
public string GetStringS (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_S_Index], defaultValue); }
    
public void SetT (string value) { this.bus.SetObject (this.aliases[Pin.Std_T_Index], value); }
public void SignalT (string value) { this.bus.SignalObject (this.aliases[Pin.Std_T_Index], value); }
public bool GetStringT (out string value) { return this.bus.GetString (this.aliases[Pin.Std_T_Index], out value); }
public string GetStringT (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_T_Index], defaultValue); }
    
public void SetU (string value) { this.bus.SetObject (this.aliases[Pin.Std_U_Index], value); }
public void SignalU (string value) { this.bus.SignalObject (this.aliases[Pin.Std_U_Index], value); }
public bool GetStringU (out string value) { return this.bus.GetString (this.aliases[Pin.Std_U_Index], out value); }
public string GetStringU (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_U_Index], defaultValue); }
    
public void SetV (string value) { this.bus.SetObject (this.aliases[Pin.Std_V_Index], value); }
public void SignalV (string value) { this.bus.SignalObject (this.aliases[Pin.Std_V_Index], value); }
public bool GetStringV (out string value) { return this.bus.GetString (this.aliases[Pin.Std_V_Index], out value); }
public string GetStringV (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_V_Index], defaultValue); }
    
public void SetW (string value) { this.bus.SetObject (this.aliases[Pin.Std_W_Index], value); }
public void SignalW (string value) { this.bus.SignalObject (this.aliases[Pin.Std_W_Index], value); }
public bool GetStringW (out string value) { return this.bus.GetString (this.aliases[Pin.Std_W_Index], out value); }
public string GetStringW (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_W_Index], defaultValue); }
    
public void SetX (string value) { this.bus.SetObject (this.aliases[Pin.Std_X_Index], value); }
public void SignalX (string value) { this.bus.SignalObject (this.aliases[Pin.Std_X_Index], value); }
public bool GetStringX (out string value) { return this.bus.GetString (this.aliases[Pin.Std_X_Index], out value); }
public string GetStringX (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_X_Index], defaultValue); }
    
public void SetY (string value) { this.bus.SetObject (this.aliases[Pin.Std_Y_Index], value); }
public void SignalY (string value) { this.bus.SignalObject (this.aliases[Pin.Std_Y_Index], value); }
public bool GetStringY (out string value) { return this.bus.GetString (this.aliases[Pin.Std_Y_Index], out value); }
public string GetStringY (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_Y_Index], defaultValue); }
    
public void SetZ (string value) { this.bus.SetObject (this.aliases[Pin.Std_Z_Index], value); }
public void SignalZ (string value) { this.bus.SignalObject (this.aliases[Pin.Std_Z_Index], value); }
public bool GetStringZ (out string value) { return this.bus.GetString (this.aliases[Pin.Std_Z_Index], out value); }
public string GetStringZ (string defaultValue) { return this.bus.GetString (this.aliases[Pin.Std_Z_Index], defaultValue); }
    
}

}
