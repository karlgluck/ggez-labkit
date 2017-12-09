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

public void SetA (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_A_Index], value); }
public void SignalA (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_A_Index], value); }
public bool GetTransformA (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_A_Index], out value); }
public Transform GetTransformA (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_A_Index], defaultValue); }

public void SetB (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_B_Index], value); }
public void SignalB (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_B_Index], value); }
public bool GetTransformB (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_B_Index], out value); }
public Transform GetTransformB (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_B_Index], defaultValue); }
    
public void SetC (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_C_Index], value); }
public void SignalC (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_C_Index], value); }
public bool GetTransformC (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_C_Index], out value); }
public Transform GetTransformC (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_C_Index], defaultValue); }
    
public void SetD (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_D_Index], value); }
public void SignalD (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_D_Index], value); }
public bool GetTransformD (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_D_Index], out value); }
public Transform GetTransformD (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_D_Index], defaultValue); }
    
public void SetE (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_E_Index], value); }
public void SignalE (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_E_Index], value); }
public bool GetTransformE (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_E_Index], out value); }
public Transform GetTransformE (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_E_Index], defaultValue); }
    
public void SetF (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_F_Index], value); }
public void SignalF (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_F_Index], value); }
public bool GetTransformF (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_F_Index], out value); }
public Transform GetTransformF (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_F_Index], defaultValue); }
    
public void SetG (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_G_Index], value); }
public void SignalG (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_G_Index], value); }
public bool GetTransformG (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_G_Index], out value); }
public Transform GetTransformG (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_G_Index], defaultValue); }
    
public void SetH (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_H_Index], value); }
public void SignalH (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_H_Index], value); }
public bool GetTransformH (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_H_Index], out value); }
public Transform GetTransformH (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_H_Index], defaultValue); }
    
public void SetI (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_I_Index], value); }
public void SignalI (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_I_Index], value); }
public bool GetTransformI (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_I_Index], out value); }
public Transform GetTransformI (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_I_Index], defaultValue); }
    
public void SetJ (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_J_Index], value); }
public void SignalJ (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_J_Index], value); }
public bool GetTransformJ (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_J_Index], out value); }
public Transform GetTransformJ (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_J_Index], defaultValue); }
    
public void SetK (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_K_Index], value); }
public void SignalK (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_K_Index], value); }
public bool GetTransformK (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_K_Index], out value); }
public Transform GetTransformK (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_K_Index], defaultValue); }
    
public void SetL (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_L_Index], value); }
public void SignalL (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_L_Index], value); }
public bool GetTransformL (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_L_Index], out value); }
public Transform GetTransformL (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_L_Index], defaultValue); }
    
public void SetM (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_M_Index], value); }
public void SignalM (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_M_Index], value); }
public bool GetTransformM (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_M_Index], out value); }
public Transform GetTransformM (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_M_Index], defaultValue); }
    
public void SetN (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_N_Index], value); }
public void SignalN (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_N_Index], value); }
public bool GetTransformN (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_N_Index], out value); }
public Transform GetTransformN (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_N_Index], defaultValue); }
    
public void SetO (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_O_Index], value); }
public void SignalO (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_O_Index], value); }
public bool GetTransformO (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_O_Index], out value); }
public Transform GetTransformO (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_O_Index], defaultValue); }
    
public void SetP (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_P_Index], value); }
public void SignalP (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_P_Index], value); }
public bool GetTransformP (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_P_Index], out value); }
public Transform GetTransformP (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_P_Index], defaultValue); }
    
public void SetQ (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_Q_Index], value); }
public void SignalQ (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_Q_Index], value); }
public bool GetTransformQ (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_Q_Index], out value); }
public Transform GetTransformQ (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_Q_Index], defaultValue); }
    
public void SetR (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_R_Index], value); }
public void SignalR (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_R_Index], value); }
public bool GetTransformR (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_R_Index], out value); }
public Transform GetTransformR (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_R_Index], defaultValue); }
    
public void SetS (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_S_Index], value); }
public void SignalS (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_S_Index], value); }
public bool GetTransformS (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_S_Index], out value); }
public Transform GetTransformS (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_S_Index], defaultValue); }
    
public void SetT (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_T_Index], value); }
public void SignalT (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_T_Index], value); }
public bool GetTransformT (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_T_Index], out value); }
public Transform GetTransformT (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_T_Index], defaultValue); }
    
public void SetU (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_U_Index], value); }
public void SignalU (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_U_Index], value); }
public bool GetTransformU (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_U_Index], out value); }
public Transform GetTransformU (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_U_Index], defaultValue); }
    
public void SetV (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_V_Index], value); }
public void SignalV (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_V_Index], value); }
public bool GetTransformV (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_V_Index], out value); }
public Transform GetTransformV (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_V_Index], defaultValue); }
    
public void SetW (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_W_Index], value); }
public void SignalW (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_W_Index], value); }
public bool GetTransformW (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_W_Index], out value); }
public Transform GetTransformW (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_W_Index], defaultValue); }
    
public void SetX (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_X_Index], value); }
public void SignalX (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_X_Index], value); }
public bool GetTransformX (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_X_Index], out value); }
public Transform GetTransformX (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_X_Index], defaultValue); }
    
public void SetY (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_Y_Index], value); }
public void SignalY (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_Y_Index], value); }
public bool GetTransformY (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_Y_Index], out value); }
public Transform GetTransformY (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_Y_Index], defaultValue); }
    
public void SetZ (Transform value) { this.bus.SetObject (this.aliases[Pin.Std_Z_Index], value); }
public void SignalZ (Transform value) { this.bus.SignalObject (this.aliases[Pin.Std_Z_Index], value); }
public bool GetTransformZ (out Transform value) { return this.bus.GetTransform (this.aliases[Pin.Std_Z_Index], out value); }
public Transform GetTransformZ (Transform defaultValue) { return this.bus.GetTransform (this.aliases[Pin.Std_Z_Index], defaultValue); }
    
}

}
