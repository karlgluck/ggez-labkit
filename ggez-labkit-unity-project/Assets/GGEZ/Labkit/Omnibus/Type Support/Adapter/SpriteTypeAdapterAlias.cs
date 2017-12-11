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

public void SetA (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_A_Index], value); }
public void SignalA (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_A_Index], value); }
public bool GetSpriteA (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_A_Index], out value); }
public Sprite GetSpriteA (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_A_Index], defaultValue); }

public void SetB (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_B_Index], value); }
public void SignalB (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_B_Index], value); }
public bool GetSpriteB (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_B_Index], out value); }
public Sprite GetSpriteB (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_B_Index], defaultValue); }
    
public void SetC (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_C_Index], value); }
public void SignalC (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_C_Index], value); }
public bool GetSpriteC (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_C_Index], out value); }
public Sprite GetSpriteC (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_C_Index], defaultValue); }
    
public void SetD (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_D_Index], value); }
public void SignalD (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_D_Index], value); }
public bool GetSpriteD (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_D_Index], out value); }
public Sprite GetSpriteD (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_D_Index], defaultValue); }
    
public void SetE (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_E_Index], value); }
public void SignalE (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_E_Index], value); }
public bool GetSpriteE (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_E_Index], out value); }
public Sprite GetSpriteE (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_E_Index], defaultValue); }
    
public void SetF (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_F_Index], value); }
public void SignalF (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_F_Index], value); }
public bool GetSpriteF (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_F_Index], out value); }
public Sprite GetSpriteF (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_F_Index], defaultValue); }
    
public void SetG (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_G_Index], value); }
public void SignalG (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_G_Index], value); }
public bool GetSpriteG (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_G_Index], out value); }
public Sprite GetSpriteG (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_G_Index], defaultValue); }
    
public void SetH (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_H_Index], value); }
public void SignalH (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_H_Index], value); }
public bool GetSpriteH (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_H_Index], out value); }
public Sprite GetSpriteH (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_H_Index], defaultValue); }
    
public void SetI (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_I_Index], value); }
public void SignalI (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_I_Index], value); }
public bool GetSpriteI (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_I_Index], out value); }
public Sprite GetSpriteI (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_I_Index], defaultValue); }
    
public void SetJ (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_J_Index], value); }
public void SignalJ (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_J_Index], value); }
public bool GetSpriteJ (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_J_Index], out value); }
public Sprite GetSpriteJ (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_J_Index], defaultValue); }
    
public void SetK (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_K_Index], value); }
public void SignalK (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_K_Index], value); }
public bool GetSpriteK (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_K_Index], out value); }
public Sprite GetSpriteK (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_K_Index], defaultValue); }
    
public void SetL (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_L_Index], value); }
public void SignalL (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_L_Index], value); }
public bool GetSpriteL (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_L_Index], out value); }
public Sprite GetSpriteL (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_L_Index], defaultValue); }
    
public void SetM (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_M_Index], value); }
public void SignalM (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_M_Index], value); }
public bool GetSpriteM (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_M_Index], out value); }
public Sprite GetSpriteM (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_M_Index], defaultValue); }
    
public void SetN (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_N_Index], value); }
public void SignalN (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_N_Index], value); }
public bool GetSpriteN (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_N_Index], out value); }
public Sprite GetSpriteN (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_N_Index], defaultValue); }
    
public void SetO (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_O_Index], value); }
public void SignalO (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_O_Index], value); }
public bool GetSpriteO (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_O_Index], out value); }
public Sprite GetSpriteO (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_O_Index], defaultValue); }
    
public void SetP (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_P_Index], value); }
public void SignalP (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_P_Index], value); }
public bool GetSpriteP (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_P_Index], out value); }
public Sprite GetSpriteP (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_P_Index], defaultValue); }
    
public void SetQ (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_Q_Index], value); }
public void SignalQ (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_Q_Index], value); }
public bool GetSpriteQ (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_Q_Index], out value); }
public Sprite GetSpriteQ (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_Q_Index], defaultValue); }
    
public void SetR (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_R_Index], value); }
public void SignalR (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_R_Index], value); }
public bool GetSpriteR (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_R_Index], out value); }
public Sprite GetSpriteR (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_R_Index], defaultValue); }
    
public void SetS (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_S_Index], value); }
public void SignalS (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_S_Index], value); }
public bool GetSpriteS (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_S_Index], out value); }
public Sprite GetSpriteS (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_S_Index], defaultValue); }
    
public void SetT (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_T_Index], value); }
public void SignalT (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_T_Index], value); }
public bool GetSpriteT (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_T_Index], out value); }
public Sprite GetSpriteT (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_T_Index], defaultValue); }
    
public void SetU (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_U_Index], value); }
public void SignalU (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_U_Index], value); }
public bool GetSpriteU (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_U_Index], out value); }
public Sprite GetSpriteU (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_U_Index], defaultValue); }
    
public void SetV (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_V_Index], value); }
public void SignalV (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_V_Index], value); }
public bool GetSpriteV (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_V_Index], out value); }
public Sprite GetSpriteV (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_V_Index], defaultValue); }
    
public void SetW (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_W_Index], value); }
public void SignalW (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_W_Index], value); }
public bool GetSpriteW (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_W_Index], out value); }
public Sprite GetSpriteW (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_W_Index], defaultValue); }
    
public void SetX (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_X_Index], value); }
public void SignalX (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_X_Index], value); }
public bool GetSpriteX (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_X_Index], out value); }
public Sprite GetSpriteX (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_X_Index], defaultValue); }
    
public void SetY (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_Y_Index], value); }
public void SignalY (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_Y_Index], value); }
public bool GetSpriteY (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_Y_Index], out value); }
public Sprite GetSpriteY (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_Y_Index], defaultValue); }
    
public void SetZ (Sprite value) { this.bus.SetObject (this.aliases[Pin.Std_Z_Index], value); }
public void SignalZ (Sprite value) { this.bus.SignalObject (this.aliases[Pin.Std_Z_Index], value); }
public bool GetSpriteZ (out Sprite value) { return this.bus.GetSprite (this.aliases[Pin.Std_Z_Index], out value); }
public Sprite GetSpriteZ (Sprite defaultValue) { return this.bus.GetSprite (this.aliases[Pin.Std_Z_Index], defaultValue); }
    
}

}
