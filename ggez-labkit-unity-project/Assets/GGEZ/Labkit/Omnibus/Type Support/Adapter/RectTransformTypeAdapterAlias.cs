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

public void SetA (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_A_Index], value); }
public void SignalA (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_A_Index], value); }
public bool GetRectTransformA (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_A_Index], out value); }
public RectTransform GetRectTransformA (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_A_Index], defaultValue); }

public void SetB (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_B_Index], value); }
public void SignalB (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_B_Index], value); }
public bool GetRectTransformB (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_B_Index], out value); }
public RectTransform GetRectTransformB (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_B_Index], defaultValue); }
    
public void SetC (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_C_Index], value); }
public void SignalC (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_C_Index], value); }
public bool GetRectTransformC (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_C_Index], out value); }
public RectTransform GetRectTransformC (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_C_Index], defaultValue); }
    
public void SetD (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_D_Index], value); }
public void SignalD (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_D_Index], value); }
public bool GetRectTransformD (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_D_Index], out value); }
public RectTransform GetRectTransformD (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_D_Index], defaultValue); }
    
public void SetE (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_E_Index], value); }
public void SignalE (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_E_Index], value); }
public bool GetRectTransformE (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_E_Index], out value); }
public RectTransform GetRectTransformE (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_E_Index], defaultValue); }
    
public void SetF (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_F_Index], value); }
public void SignalF (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_F_Index], value); }
public bool GetRectTransformF (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_F_Index], out value); }
public RectTransform GetRectTransformF (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_F_Index], defaultValue); }
    
public void SetG (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_G_Index], value); }
public void SignalG (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_G_Index], value); }
public bool GetRectTransformG (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_G_Index], out value); }
public RectTransform GetRectTransformG (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_G_Index], defaultValue); }
    
public void SetH (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_H_Index], value); }
public void SignalH (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_H_Index], value); }
public bool GetRectTransformH (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_H_Index], out value); }
public RectTransform GetRectTransformH (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_H_Index], defaultValue); }
    
public void SetI (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_I_Index], value); }
public void SignalI (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_I_Index], value); }
public bool GetRectTransformI (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_I_Index], out value); }
public RectTransform GetRectTransformI (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_I_Index], defaultValue); }
    
public void SetJ (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_J_Index], value); }
public void SignalJ (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_J_Index], value); }
public bool GetRectTransformJ (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_J_Index], out value); }
public RectTransform GetRectTransformJ (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_J_Index], defaultValue); }
    
public void SetK (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_K_Index], value); }
public void SignalK (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_K_Index], value); }
public bool GetRectTransformK (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_K_Index], out value); }
public RectTransform GetRectTransformK (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_K_Index], defaultValue); }
    
public void SetL (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_L_Index], value); }
public void SignalL (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_L_Index], value); }
public bool GetRectTransformL (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_L_Index], out value); }
public RectTransform GetRectTransformL (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_L_Index], defaultValue); }
    
public void SetM (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_M_Index], value); }
public void SignalM (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_M_Index], value); }
public bool GetRectTransformM (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_M_Index], out value); }
public RectTransform GetRectTransformM (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_M_Index], defaultValue); }
    
public void SetN (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_N_Index], value); }
public void SignalN (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_N_Index], value); }
public bool GetRectTransformN (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_N_Index], out value); }
public RectTransform GetRectTransformN (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_N_Index], defaultValue); }
    
public void SetO (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_O_Index], value); }
public void SignalO (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_O_Index], value); }
public bool GetRectTransformO (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_O_Index], out value); }
public RectTransform GetRectTransformO (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_O_Index], defaultValue); }
    
public void SetP (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_P_Index], value); }
public void SignalP (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_P_Index], value); }
public bool GetRectTransformP (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_P_Index], out value); }
public RectTransform GetRectTransformP (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_P_Index], defaultValue); }
    
public void SetQ (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_Q_Index], value); }
public void SignalQ (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_Q_Index], value); }
public bool GetRectTransformQ (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_Q_Index], out value); }
public RectTransform GetRectTransformQ (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_Q_Index], defaultValue); }
    
public void SetR (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_R_Index], value); }
public void SignalR (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_R_Index], value); }
public bool GetRectTransformR (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_R_Index], out value); }
public RectTransform GetRectTransformR (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_R_Index], defaultValue); }
    
public void SetS (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_S_Index], value); }
public void SignalS (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_S_Index], value); }
public bool GetRectTransformS (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_S_Index], out value); }
public RectTransform GetRectTransformS (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_S_Index], defaultValue); }
    
public void SetT (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_T_Index], value); }
public void SignalT (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_T_Index], value); }
public bool GetRectTransformT (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_T_Index], out value); }
public RectTransform GetRectTransformT (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_T_Index], defaultValue); }
    
public void SetU (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_U_Index], value); }
public void SignalU (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_U_Index], value); }
public bool GetRectTransformU (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_U_Index], out value); }
public RectTransform GetRectTransformU (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_U_Index], defaultValue); }
    
public void SetV (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_V_Index], value); }
public void SignalV (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_V_Index], value); }
public bool GetRectTransformV (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_V_Index], out value); }
public RectTransform GetRectTransformV (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_V_Index], defaultValue); }
    
public void SetW (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_W_Index], value); }
public void SignalW (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_W_Index], value); }
public bool GetRectTransformW (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_W_Index], out value); }
public RectTransform GetRectTransformW (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_W_Index], defaultValue); }
    
public void SetX (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_X_Index], value); }
public void SignalX (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_X_Index], value); }
public bool GetRectTransformX (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_X_Index], out value); }
public RectTransform GetRectTransformX (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_X_Index], defaultValue); }
    
public void SetY (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_Y_Index], value); }
public void SignalY (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_Y_Index], value); }
public bool GetRectTransformY (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_Y_Index], out value); }
public RectTransform GetRectTransformY (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_Y_Index], defaultValue); }
    
public void SetZ (RectTransform value) { this.bus.SetObject (this.aliases[Pin.Std_Z_Index], value); }
public void SignalZ (RectTransform value) { this.bus.SignalObject (this.aliases[Pin.Std_Z_Index], value); }
public bool GetRectTransformZ (out RectTransform value) { return this.bus.GetRectTransform (this.aliases[Pin.Std_Z_Index], out value); }
public RectTransform GetRectTransformZ (RectTransform defaultValue) { return this.bus.GetRectTransform (this.aliases[Pin.Std_Z_Index], defaultValue); }
    
}

}
