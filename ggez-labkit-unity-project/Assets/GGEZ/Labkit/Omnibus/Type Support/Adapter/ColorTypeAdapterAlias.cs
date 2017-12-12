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

public void SetA (Color value) { this.bus.SetObject (this.aliases[Pin.Std_A_Index], value); }
public void SignalA (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_A_Index], value); }
public bool GetColorA (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_A_Index], out value); }
public Color GetColorA (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_A_Index], defaultValue); }

public void SetB (Color value) { this.bus.SetObject (this.aliases[Pin.Std_B_Index], value); }
public void SignalB (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_B_Index], value); }
public bool GetColorB (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_B_Index], out value); }
public Color GetColorB (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_B_Index], defaultValue); }
    
public void SetC (Color value) { this.bus.SetObject (this.aliases[Pin.Std_C_Index], value); }
public void SignalC (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_C_Index], value); }
public bool GetColorC (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_C_Index], out value); }
public Color GetColorC (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_C_Index], defaultValue); }
    
public void SetD (Color value) { this.bus.SetObject (this.aliases[Pin.Std_D_Index], value); }
public void SignalD (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_D_Index], value); }
public bool GetColorD (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_D_Index], out value); }
public Color GetColorD (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_D_Index], defaultValue); }
    
public void SetE (Color value) { this.bus.SetObject (this.aliases[Pin.Std_E_Index], value); }
public void SignalE (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_E_Index], value); }
public bool GetColorE (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_E_Index], out value); }
public Color GetColorE (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_E_Index], defaultValue); }
    
public void SetF (Color value) { this.bus.SetObject (this.aliases[Pin.Std_F_Index], value); }
public void SignalF (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_F_Index], value); }
public bool GetColorF (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_F_Index], out value); }
public Color GetColorF (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_F_Index], defaultValue); }
    
public void SetG (Color value) { this.bus.SetObject (this.aliases[Pin.Std_G_Index], value); }
public void SignalG (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_G_Index], value); }
public bool GetColorG (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_G_Index], out value); }
public Color GetColorG (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_G_Index], defaultValue); }
    
public void SetH (Color value) { this.bus.SetObject (this.aliases[Pin.Std_H_Index], value); }
public void SignalH (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_H_Index], value); }
public bool GetColorH (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_H_Index], out value); }
public Color GetColorH (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_H_Index], defaultValue); }
    
public void SetI (Color value) { this.bus.SetObject (this.aliases[Pin.Std_I_Index], value); }
public void SignalI (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_I_Index], value); }
public bool GetColorI (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_I_Index], out value); }
public Color GetColorI (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_I_Index], defaultValue); }
    
public void SetJ (Color value) { this.bus.SetObject (this.aliases[Pin.Std_J_Index], value); }
public void SignalJ (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_J_Index], value); }
public bool GetColorJ (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_J_Index], out value); }
public Color GetColorJ (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_J_Index], defaultValue); }
    
public void SetK (Color value) { this.bus.SetObject (this.aliases[Pin.Std_K_Index], value); }
public void SignalK (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_K_Index], value); }
public bool GetColorK (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_K_Index], out value); }
public Color GetColorK (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_K_Index], defaultValue); }
    
public void SetL (Color value) { this.bus.SetObject (this.aliases[Pin.Std_L_Index], value); }
public void SignalL (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_L_Index], value); }
public bool GetColorL (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_L_Index], out value); }
public Color GetColorL (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_L_Index], defaultValue); }
    
public void SetM (Color value) { this.bus.SetObject (this.aliases[Pin.Std_M_Index], value); }
public void SignalM (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_M_Index], value); }
public bool GetColorM (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_M_Index], out value); }
public Color GetColorM (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_M_Index], defaultValue); }
    
public void SetN (Color value) { this.bus.SetObject (this.aliases[Pin.Std_N_Index], value); }
public void SignalN (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_N_Index], value); }
public bool GetColorN (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_N_Index], out value); }
public Color GetColorN (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_N_Index], defaultValue); }
    
public void SetO (Color value) { this.bus.SetObject (this.aliases[Pin.Std_O_Index], value); }
public void SignalO (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_O_Index], value); }
public bool GetColorO (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_O_Index], out value); }
public Color GetColorO (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_O_Index], defaultValue); }
    
public void SetP (Color value) { this.bus.SetObject (this.aliases[Pin.Std_P_Index], value); }
public void SignalP (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_P_Index], value); }
public bool GetColorP (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_P_Index], out value); }
public Color GetColorP (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_P_Index], defaultValue); }
    
public void SetQ (Color value) { this.bus.SetObject (this.aliases[Pin.Std_Q_Index], value); }
public void SignalQ (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_Q_Index], value); }
public bool GetColorQ (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_Q_Index], out value); }
public Color GetColorQ (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_Q_Index], defaultValue); }
    
public void SetR (Color value) { this.bus.SetObject (this.aliases[Pin.Std_R_Index], value); }
public void SignalR (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_R_Index], value); }
public bool GetColorR (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_R_Index], out value); }
public Color GetColorR (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_R_Index], defaultValue); }
    
public void SetS (Color value) { this.bus.SetObject (this.aliases[Pin.Std_S_Index], value); }
public void SignalS (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_S_Index], value); }
public bool GetColorS (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_S_Index], out value); }
public Color GetColorS (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_S_Index], defaultValue); }
    
public void SetT (Color value) { this.bus.SetObject (this.aliases[Pin.Std_T_Index], value); }
public void SignalT (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_T_Index], value); }
public bool GetColorT (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_T_Index], out value); }
public Color GetColorT (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_T_Index], defaultValue); }
    
public void SetU (Color value) { this.bus.SetObject (this.aliases[Pin.Std_U_Index], value); }
public void SignalU (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_U_Index], value); }
public bool GetColorU (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_U_Index], out value); }
public Color GetColorU (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_U_Index], defaultValue); }
    
public void SetV (Color value) { this.bus.SetObject (this.aliases[Pin.Std_V_Index], value); }
public void SignalV (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_V_Index], value); }
public bool GetColorV (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_V_Index], out value); }
public Color GetColorV (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_V_Index], defaultValue); }
    
public void SetW (Color value) { this.bus.SetObject (this.aliases[Pin.Std_W_Index], value); }
public void SignalW (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_W_Index], value); }
public bool GetColorW (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_W_Index], out value); }
public Color GetColorW (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_W_Index], defaultValue); }
    
public void SetX (Color value) { this.bus.SetObject (this.aliases[Pin.Std_X_Index], value); }
public void SignalX (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_X_Index], value); }
public bool GetColorX (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_X_Index], out value); }
public Color GetColorX (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_X_Index], defaultValue); }
    
public void SetY (Color value) { this.bus.SetObject (this.aliases[Pin.Std_Y_Index], value); }
public void SignalY (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_Y_Index], value); }
public bool GetColorY (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_Y_Index], out value); }
public Color GetColorY (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_Y_Index], defaultValue); }
    
public void SetZ (Color value) { this.bus.SetObject (this.aliases[Pin.Std_Z_Index], value); }
public void SignalZ (Color value) { this.bus.SignalObject (this.aliases[Pin.Std_Z_Index], value); }
public bool GetColorZ (out Color value) { return this.bus.GetColor (this.aliases[Pin.Std_Z_Index], out value); }
public Color GetColorZ (Color defaultValue) { return this.bus.GetColor (this.aliases[Pin.Std_Z_Index], defaultValue); }
    
}

}
