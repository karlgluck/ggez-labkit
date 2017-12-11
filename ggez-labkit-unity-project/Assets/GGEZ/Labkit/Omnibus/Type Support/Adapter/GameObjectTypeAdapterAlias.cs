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

public void SetA (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_A_Index], value); }
public void SignalA (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_A_Index], value); }
public bool GetGameObjectA (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_A_Index], out value); }
public GameObject GetGameObjectA (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_A_Index], defaultValue); }

public void SetB (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_B_Index], value); }
public void SignalB (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_B_Index], value); }
public bool GetGameObjectB (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_B_Index], out value); }
public GameObject GetGameObjectB (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_B_Index], defaultValue); }
    
public void SetC (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_C_Index], value); }
public void SignalC (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_C_Index], value); }
public bool GetGameObjectC (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_C_Index], out value); }
public GameObject GetGameObjectC (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_C_Index], defaultValue); }
    
public void SetD (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_D_Index], value); }
public void SignalD (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_D_Index], value); }
public bool GetGameObjectD (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_D_Index], out value); }
public GameObject GetGameObjectD (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_D_Index], defaultValue); }
    
public void SetE (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_E_Index], value); }
public void SignalE (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_E_Index], value); }
public bool GetGameObjectE (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_E_Index], out value); }
public GameObject GetGameObjectE (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_E_Index], defaultValue); }
    
public void SetF (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_F_Index], value); }
public void SignalF (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_F_Index], value); }
public bool GetGameObjectF (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_F_Index], out value); }
public GameObject GetGameObjectF (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_F_Index], defaultValue); }
    
public void SetG (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_G_Index], value); }
public void SignalG (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_G_Index], value); }
public bool GetGameObjectG (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_G_Index], out value); }
public GameObject GetGameObjectG (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_G_Index], defaultValue); }
    
public void SetH (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_H_Index], value); }
public void SignalH (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_H_Index], value); }
public bool GetGameObjectH (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_H_Index], out value); }
public GameObject GetGameObjectH (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_H_Index], defaultValue); }
    
public void SetI (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_I_Index], value); }
public void SignalI (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_I_Index], value); }
public bool GetGameObjectI (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_I_Index], out value); }
public GameObject GetGameObjectI (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_I_Index], defaultValue); }
    
public void SetJ (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_J_Index], value); }
public void SignalJ (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_J_Index], value); }
public bool GetGameObjectJ (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_J_Index], out value); }
public GameObject GetGameObjectJ (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_J_Index], defaultValue); }
    
public void SetK (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_K_Index], value); }
public void SignalK (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_K_Index], value); }
public bool GetGameObjectK (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_K_Index], out value); }
public GameObject GetGameObjectK (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_K_Index], defaultValue); }
    
public void SetL (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_L_Index], value); }
public void SignalL (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_L_Index], value); }
public bool GetGameObjectL (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_L_Index], out value); }
public GameObject GetGameObjectL (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_L_Index], defaultValue); }
    
public void SetM (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_M_Index], value); }
public void SignalM (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_M_Index], value); }
public bool GetGameObjectM (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_M_Index], out value); }
public GameObject GetGameObjectM (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_M_Index], defaultValue); }
    
public void SetN (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_N_Index], value); }
public void SignalN (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_N_Index], value); }
public bool GetGameObjectN (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_N_Index], out value); }
public GameObject GetGameObjectN (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_N_Index], defaultValue); }
    
public void SetO (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_O_Index], value); }
public void SignalO (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_O_Index], value); }
public bool GetGameObjectO (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_O_Index], out value); }
public GameObject GetGameObjectO (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_O_Index], defaultValue); }
    
public void SetP (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_P_Index], value); }
public void SignalP (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_P_Index], value); }
public bool GetGameObjectP (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_P_Index], out value); }
public GameObject GetGameObjectP (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_P_Index], defaultValue); }
    
public void SetQ (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_Q_Index], value); }
public void SignalQ (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_Q_Index], value); }
public bool GetGameObjectQ (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_Q_Index], out value); }
public GameObject GetGameObjectQ (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_Q_Index], defaultValue); }
    
public void SetR (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_R_Index], value); }
public void SignalR (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_R_Index], value); }
public bool GetGameObjectR (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_R_Index], out value); }
public GameObject GetGameObjectR (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_R_Index], defaultValue); }
    
public void SetS (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_S_Index], value); }
public void SignalS (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_S_Index], value); }
public bool GetGameObjectS (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_S_Index], out value); }
public GameObject GetGameObjectS (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_S_Index], defaultValue); }
    
public void SetT (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_T_Index], value); }
public void SignalT (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_T_Index], value); }
public bool GetGameObjectT (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_T_Index], out value); }
public GameObject GetGameObjectT (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_T_Index], defaultValue); }
    
public void SetU (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_U_Index], value); }
public void SignalU (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_U_Index], value); }
public bool GetGameObjectU (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_U_Index], out value); }
public GameObject GetGameObjectU (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_U_Index], defaultValue); }
    
public void SetV (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_V_Index], value); }
public void SignalV (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_V_Index], value); }
public bool GetGameObjectV (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_V_Index], out value); }
public GameObject GetGameObjectV (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_V_Index], defaultValue); }
    
public void SetW (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_W_Index], value); }
public void SignalW (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_W_Index], value); }
public bool GetGameObjectW (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_W_Index], out value); }
public GameObject GetGameObjectW (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_W_Index], defaultValue); }
    
public void SetX (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_X_Index], value); }
public void SignalX (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_X_Index], value); }
public bool GetGameObjectX (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_X_Index], out value); }
public GameObject GetGameObjectX (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_X_Index], defaultValue); }
    
public void SetY (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_Y_Index], value); }
public void SignalY (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_Y_Index], value); }
public bool GetGameObjectY (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_Y_Index], out value); }
public GameObject GetGameObjectY (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_Y_Index], defaultValue); }
    
public void SetZ (GameObject value) { this.bus.SetObject (this.aliases[Pin.Std_Z_Index], value); }
public void SignalZ (GameObject value) { this.bus.SignalObject (this.aliases[Pin.Std_Z_Index], value); }
public bool GetGameObjectZ (out GameObject value) { return this.bus.GetGameObject (this.aliases[Pin.Std_Z_Index], out value); }
public GameObject GetGameObjectZ (GameObject defaultValue) { return this.bus.GetGameObject (this.aliases[Pin.Std_Z_Index], defaultValue); }
    
}

}
