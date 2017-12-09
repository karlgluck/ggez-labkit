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

public void SetA (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_A_Index], value); }
public void SignalA (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_A_Index], value); }
public bool GetRectA (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_A_Index], out value); }
public Rect GetRectA (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_A_Index], defaultValue); }

public void SetB (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_B_Index], value); }
public void SignalB (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_B_Index], value); }
public bool GetRectB (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_B_Index], out value); }
public Rect GetRectB (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_B_Index], defaultValue); }
    
public void SetC (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_C_Index], value); }
public void SignalC (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_C_Index], value); }
public bool GetRectC (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_C_Index], out value); }
public Rect GetRectC (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_C_Index], defaultValue); }
    
public void SetD (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_D_Index], value); }
public void SignalD (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_D_Index], value); }
public bool GetRectD (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_D_Index], out value); }
public Rect GetRectD (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_D_Index], defaultValue); }
    
public void SetE (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_E_Index], value); }
public void SignalE (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_E_Index], value); }
public bool GetRectE (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_E_Index], out value); }
public Rect GetRectE (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_E_Index], defaultValue); }
    
public void SetF (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_F_Index], value); }
public void SignalF (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_F_Index], value); }
public bool GetRectF (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_F_Index], out value); }
public Rect GetRectF (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_F_Index], defaultValue); }
    
public void SetG (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_G_Index], value); }
public void SignalG (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_G_Index], value); }
public bool GetRectG (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_G_Index], out value); }
public Rect GetRectG (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_G_Index], defaultValue); }
    
public void SetH (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_H_Index], value); }
public void SignalH (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_H_Index], value); }
public bool GetRectH (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_H_Index], out value); }
public Rect GetRectH (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_H_Index], defaultValue); }
    
public void SetI (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_I_Index], value); }
public void SignalI (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_I_Index], value); }
public bool GetRectI (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_I_Index], out value); }
public Rect GetRectI (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_I_Index], defaultValue); }
    
public void SetJ (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_J_Index], value); }
public void SignalJ (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_J_Index], value); }
public bool GetRectJ (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_J_Index], out value); }
public Rect GetRectJ (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_J_Index], defaultValue); }
    
public void SetK (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_K_Index], value); }
public void SignalK (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_K_Index], value); }
public bool GetRectK (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_K_Index], out value); }
public Rect GetRectK (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_K_Index], defaultValue); }
    
public void SetL (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_L_Index], value); }
public void SignalL (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_L_Index], value); }
public bool GetRectL (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_L_Index], out value); }
public Rect GetRectL (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_L_Index], defaultValue); }
    
public void SetM (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_M_Index], value); }
public void SignalM (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_M_Index], value); }
public bool GetRectM (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_M_Index], out value); }
public Rect GetRectM (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_M_Index], defaultValue); }
    
public void SetN (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_N_Index], value); }
public void SignalN (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_N_Index], value); }
public bool GetRectN (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_N_Index], out value); }
public Rect GetRectN (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_N_Index], defaultValue); }
    
public void SetO (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_O_Index], value); }
public void SignalO (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_O_Index], value); }
public bool GetRectO (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_O_Index], out value); }
public Rect GetRectO (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_O_Index], defaultValue); }
    
public void SetP (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_P_Index], value); }
public void SignalP (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_P_Index], value); }
public bool GetRectP (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_P_Index], out value); }
public Rect GetRectP (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_P_Index], defaultValue); }
    
public void SetQ (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_Q_Index], value); }
public void SignalQ (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_Q_Index], value); }
public bool GetRectQ (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_Q_Index], out value); }
public Rect GetRectQ (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_Q_Index], defaultValue); }
    
public void SetR (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_R_Index], value); }
public void SignalR (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_R_Index], value); }
public bool GetRectR (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_R_Index], out value); }
public Rect GetRectR (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_R_Index], defaultValue); }
    
public void SetS (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_S_Index], value); }
public void SignalS (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_S_Index], value); }
public bool GetRectS (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_S_Index], out value); }
public Rect GetRectS (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_S_Index], defaultValue); }
    
public void SetT (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_T_Index], value); }
public void SignalT (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_T_Index], value); }
public bool GetRectT (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_T_Index], out value); }
public Rect GetRectT (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_T_Index], defaultValue); }
    
public void SetU (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_U_Index], value); }
public void SignalU (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_U_Index], value); }
public bool GetRectU (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_U_Index], out value); }
public Rect GetRectU (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_U_Index], defaultValue); }
    
public void SetV (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_V_Index], value); }
public void SignalV (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_V_Index], value); }
public bool GetRectV (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_V_Index], out value); }
public Rect GetRectV (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_V_Index], defaultValue); }
    
public void SetW (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_W_Index], value); }
public void SignalW (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_W_Index], value); }
public bool GetRectW (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_W_Index], out value); }
public Rect GetRectW (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_W_Index], defaultValue); }
    
public void SetX (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_X_Index], value); }
public void SignalX (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_X_Index], value); }
public bool GetRectX (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_X_Index], out value); }
public Rect GetRectX (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_X_Index], defaultValue); }
    
public void SetY (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_Y_Index], value); }
public void SignalY (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_Y_Index], value); }
public bool GetRectY (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_Y_Index], out value); }
public Rect GetRectY (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_Y_Index], defaultValue); }
    
public void SetZ (Rect value) { this.bus.SetObject (this.aliases[Pin.Std_Z_Index], value); }
public void SignalZ (Rect value) { this.bus.SignalObject (this.aliases[Pin.Std_Z_Index], value); }
public bool GetRectZ (out Rect value) { return this.bus.GetRect (this.aliases[Pin.Std_Z_Index], out value); }
public Rect GetRectZ (Rect defaultValue) { return this.bus.GetRect (this.aliases[Pin.Std_Z_Index], defaultValue); }
    
}

}
